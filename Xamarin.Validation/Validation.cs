using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.AttributeValidation.Attributes;
using Xamarin.AttributeValidation.Helpers;
using Xamarin.Forms;

namespace Xamarin.AttributeValidation
{
    internal class Validation
    {
        private static Validation _instance;

        private readonly Dictionary<View, PropertyInfo> UIPropertyDictionary;

        private bool HasBeenValidatedBefore;
        internal static Validation GetInstance(Page page)
        {
            return _instance ?? (_instance = new Validation(page));
        }

        private Validation(Page page)
        {
            UIPropertyDictionary = GetUiPropertyDictionary(page);
        }

        internal bool Validate(Page page)
        {
            var viewModel = page.BindingContext;
            //This maps the elements in the UI to their bound property in the viewmodel.

            if (UIPropertyDictionary?.Count == 0)
                throw new Exception("No Binding between ViewModel and UI Elements found.");

            Grid grid;
            if (!HasBeenValidatedBefore)
            {
                //Get a dummy element, for context reasons
                var dummyElement = UIPropertyDictionary.Keys.First();

                //Wrap everything in a grid, on the same row, in order to overlay elements.
                grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition());
                var layout = (Layout)dummyElement.Parent;
                grid.Children.Add(layout, 0, 0);

                grid.Children.Add(new Label() { HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, TextColor = Color.Red });
                ((ContentPage)page).Content = grid;
            }
            else
            {
                grid = (Grid)((ContentPage)page).Content;
            }

            foreach (var property in UIPropertyDictionary.Values)
            {
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
                        foreach (var view in UIPropertyDictionary.Where(pair => pair.Value.Equals(property)).Select(pair => pair.Key))
                        {
                            Entry uiElement;
                            try
                            {
                                uiElement = (Entry)view;
                            }
                            catch (InvalidCastException)
                            {
                                throw new Exception("Only Entry is supported for input validation.");
                            }
                            var label = new Label()
                            {
                                Text = "X",
                                TextColor = Color.Red,
                                //HorizontalTextAlignment = TextAlignment.Center,
                                //VerticalTextAlignment = TextAlignment.Center,
                                //HorizontalOptions = LayoutOptions.CenterAndExpand,
                                //VerticalOptions = LayoutOptions.CenterAndExpand
                                TranslationX = uiElement.X + uiElement.Width,
                                TranslationY = uiElement.Y
                            };
                            //label.TranslateTo(uiElement.X + uiElement.Width, uiElement.Y);
                            grid.Children.Add(label, 0, 0);
                            //TODO find a way to display the error messages within the entry.
                            //Example: If error, display littel red icon on the end of the entry. If this icon is tapped, a small "bubble" pops up with the error message.

                            //var index = layout.Children.IndexOf(uiElement);
                            //if (index >= layout.Children.Count - 1)
                            //{
                            //    layout.Children.Add(new Label() { Text = validationAttribute.ErrorMessage, TextColor = Color.Red });
                            //}
                            //else
                            //{
                            //    layout.Children.Insert(index + 1, new Label() { Text = validationAttribute.ErrorMessage, TextColor = Color.Red });
                            //}
                            //uiElement.Placeholder = validationAttribute.ErrorMessage;
                            //uiElement.PlaceholderColor = Color.Red;
                        }
                    }
                }
            }
            HasBeenValidatedBefore = true;
            return true;
        }

        private Dictionary<View, PropertyInfo> GetUiPropertyDictionary(Page page)
        {
            var dummyPage = new ContentPage();
            var dummyLayout = new StackLayout();
            var viewModel = page.BindingContext;

            var pageContentProperty = page.GetType().GetProperty(nameof(dummyPage.Content));
            if (pageContentProperty == null)
                throw new Exception("This Page can not be validated. Please use a Page that contains a Content property.");

            var pageContent = pageContentProperty.GetValue(page);

            //pageContent actually has two Children properties. (One of them is of type IReadonlyList.
            //I think this is an internal type and can be ignored. For now the first property is picked.
            var childrenProperty = Array.Find(pageContent.GetType().GetProperties(), x => x.Name.Equals(nameof(dummyLayout.Children)));
            if (childrenProperty == null)
                throw new Exception("This Page can not be validated. There is no valid Layout. Please choose a Layout that contains a Children property.");

            var pageElementsRaw = (IEnumerable<View>)childrenProperty.GetValue(pageContent);
            if (pageElementsRaw?.Any() != true)
                throw new Exception("This Page can not be validated. There are no elements.");

            //Map the UI Element to the property in the viewModel
            var viewModelUiDict = new Dictionary<View, PropertyInfo>();

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

                viewModelUiDict.Add(el, viewModelProperty);
            }
            return viewModelUiDict;
        }
    }
}