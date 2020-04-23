using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.AttributeValidation.Attributes;
using Xamarin.AttributeValidation.Controls;
using Xamarin.AttributeValidation.Helpers;
using Xamarin.AttributeValidation.Models;
using Xamarin.Forms;

namespace Xamarin.AttributeValidation
{
    internal class Validation
    {
        private static Validation _instance;

        private readonly object viewModel;

        private readonly List<ValidationModel> UiPropertyMapping;

        private bool HasBeenValidatedBefore;

        private readonly List<View> renderedElements;

        internal static async Task<Validation> GetInstance(Page page)
        {
            //Wrap the constructor in an async method. Allows all the Reflection stuff to be executed async.
            var task = new Task(delegate
            {
                if (_instance == null)
                    _instance = new Validation(page);
            });
            task.Start();
            await task;
            return await Task.FromResult(_instance);
        }

        private Validation(Page page)
        {
            //This maps the elements in the UI to their bound property in the viewmodel.
            UiPropertyMapping = GetUiPropertyDictionary(page);
            renderedElements = new List<View>();
            viewModel = page.BindingContext;
        }

        public bool Validate(Page page)
        {
            if (UiPropertyMapping?.Count == 0)
                throw new Exception("No Binding between ViewModel and UI Elements found.");

            Grid grid;
            if (!HasBeenValidatedBefore)
            {
                //Get a dummy element, for context reasons
                var dummyElement = UiPropertyMapping[0].UiElement;

                //Wrap everything in a grid, on the same row, in order to overlay elements.
                grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition());
                var layout = (Layout)dummyElement.Parent;
                grid.Children.Add(layout, 0, 0);

                //Tap event
                var gr = new TapGestureRecognizer();
                grid.GestureRecognizers.Add(gr);
                gr.Tapped += delegate
                {
                    MessagingCenter.Send<View>(grid, "Clear_FloatingText");
                };

                page.SizeChanged += delegate
                {
                    MessagingCenter.Send<View>(grid, "Clear_FloatingText");
                    //Only re-render if there are already rendered elements.
                    if (renderedElements.Count > 0)
                        RenderElements(grid);
                };

                ((ContentPage)page).Content = grid;
            }
            else
            {
                grid = (Grid)((ContentPage)page).Content;
            }
            MessagingCenter.Send<View>(grid, "Clear_FloatingText");

            UpdateValidationResults();
            RenderElements(grid);

            HasBeenValidatedBefore = true;

            return !UiPropertyMapping.Any(x => x.ValidationResult?.Count > 0);
        }

        private void UpdateValidationResults()
        {
            foreach (var validationModel in UiPropertyMapping)
            {
                var property = validationModel.ViewModelProperty;
                List<string> result = new List<string>();
                string propertyValue = string.Empty;
                try
                {
                    propertyValue = (string)property.GetValue(viewModel);
                }
                catch (InvalidCastException)
                {
                    throw new Exception("Validation Exception. Only string properties can be validated.");
                }
                if (propertyValue == null)
                    propertyValue = string.Empty;

                foreach (var att in property.GetCustomAttributes(typeof(IValidationAttribute), false))
                {
                    IValidationAttribute validationAttribute;
                    try
                    {
                        validationAttribute = (IValidationAttribute)att;
                    }
                    catch
                    {
                        //This is another attribute. Doesn't concern us.
                        continue;
                    }
                    var validationResult = validationAttribute.ValidateValue(propertyValue);
                    if (!string.IsNullOrEmpty(validationResult))
                    {
                        result.Add(validationResult);
                    }
                }
                validationModel.ValidationResult = result;
            }
        }

        private void RenderElements(Grid grid)
        {
            //First clear all existing rendered elements.
            if (renderedElements.Count > 0)
            {
                foreach (var renderedElement in renderedElements)
                {
                    grid.Children.Remove(renderedElement);
                }
            }

            foreach (var valModel in UiPropertyMapping)
            {
                if (valModel.ValidationResult?.Count == 0)
                    continue;

                var view = valModel.UiElement;
                Entry uiElement;
                try
                {
                    uiElement = (Entry)view;
                }
                catch (InvalidCastException)
                {
                    throw new Exception("Only Entry is supported for input validation.");
                }

                uiElement.Focused += delegate
                {
                    MessagingCenter.Send<View>(grid, "Clear_FloatingText");

                    if (valModel.ValidationResult?.Count == 0)
                        return;

                    var bubble = new FloatingText(grid)
                    {
                        TranslationX = uiElement.X + uiElement.Width,
                        TranslationY = uiElement.Y + uiElement.Height - 4
                    };
                    bubble.FindByName<Label>("InfoText").Text = valModel.ValidationResult[0];

                    grid.Children.Add(bubble, 0, 0);
                };

                var label = new Label()
                {
                    Text = "❗",
                    TextColor = Color.Red,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Start,
                    TranslationX = (uiElement.X + uiElement.Width) - 20,
                    TranslationY = uiElement.Y + 10
                };
                grid.Children.Add(label, 0, 0);

                renderedElements.Add(label);
            }
        }

        private List<ValidationModel> GetUiPropertyDictionary(Page page)
        {
            var dummyPage = new ContentPage();
            var dummyLayout = new StackLayout();
            var viewModel = page.BindingContext;

            var pageContentProperty = page.GetType().GetProperty(nameof(dummyPage.Content));
            if (pageContentProperty == null)
                throw new Exception("This Page can not be validated. Please use a Page that contains a Content property.");

            var pageContent = pageContentProperty.GetValue(page);

            //pageContent actually has two Children properties. (One of them is of type IReadonlyList)
            //I think this is an internal type and can be ignored. For now the first property is picked.
            var childrenProperty = Array.Find(pageContent.GetType().GetProperties(), x => x.Name.Equals(nameof(dummyLayout.Children)));
            if (childrenProperty == null)
                throw new Exception("This Page can not be validated. There is no valid Layout. Please choose a Layout that contains a Children property.");

            var pageElementsRaw = (IEnumerable<View>)childrenProperty.GetValue(pageContent);
            if (pageElementsRaw?.Any() != true)
                throw new Exception("This Page can not be validated. There are no elements.");

            //Map the UI Element to the property in the viewModel
            var viewModelUiDict = new List<ValidationModel>();

            //Get all properties from the viewModel, that implement our interface
            var viewModelRelevantProperties = viewModel.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(IValidationAttribute), false) != null);
            foreach (var el in pageElementsRaw)
            {
                var bindingPath = el.GetBinding(Entry.TextProperty);
                if (bindingPath == null)
                    continue; //Could be any element. Skip.

                //is it in the viewmodel? This assumes that the binding path only consists of the property name.
                var viewModelProperty = viewModelRelevantProperties.FirstOrDefault(x => x.Name.Equals(bindingPath.Path));
                if (viewModelProperty == null)
                    continue; //could be any property in the viewModel. Skip.

                viewModelUiDict.Add(new ValidationModel() { UiElement = el, ViewModelProperty = viewModelProperty });
            }
            return viewModelUiDict;
        }
    }
}