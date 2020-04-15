using System;
using System.ComponentModel;
using Xamarin.AttributeValidation.Example.ViewModels;
using Xamarin.Forms;

namespace Xamarin.AttributeValidation.Example.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private ExampleViewModel viewModel;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new ExampleViewModel();
            
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            //var isValid = await Validator.Validate(viewModel);
            var isValid = this.Validate();
            if (isValid)
                await DisplayAlert("Validation", "Model is valid", "OK");
        }
    }
}