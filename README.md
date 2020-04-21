Get it on NuGet:
https://www.nuget.org/packages/Xamarin.AttributeValidation/

![Nuget](https://img.shields.io/nuget/v/Xamarin.AttributeValidation)


# Xamarin.AttributeValidation
This library allows to use attributes in the ViewModel to validate input fields.
The usage is very similar to ASP.NET Core Form validation.

Take a look at the examples in this repository or:

Step by Step:
1. Add a reference to Xamarin.AttributeValidation via NuGet.
2. Assuming your XAML looks something like this:
```xml
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" 
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" 
    xmlns:d="http://xamarin.com/schemas/2014/forms/design" 
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
    mc:Ignorable="d" x:Class="Example.Views.RegisterPage">
    <ContentPage.Content>
        <StackLayout HorizontalOptions="FillAndExpand">
            <Entry x:Name="Input_Name" Text="{Binding Name}" Keyboard="Default" Placeholder="Name" Margin="0,0,0,15" />
            <Entry x:Name="Input_Email" Text="{Binding Email}" Keyboard="Email" Placeholder="Email" Margin="0,0,0,15" />
            <Entry x:Name="Input_Password" Text="{Binding Password}" IsPassword="True" Placeholder="Password" Margin="0,0,0,15" />
            <Button x:Name="Button_Register" Text="Create Account" Clicked="Register_Clicked" Margin="0,0,0,15" />
        </StackLayout>
    </ContentPage.Content>
</ContentPage>
```
3. The ViewModel looks like this:
```csharp
using Example.Localization;
using Xamarin.AttributeValidation.Attributes;

namespace Example.Mobile.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        //The EmailAddress attribute makes sure that the value is a valid Email Address.
        [EmailAddress(typeof(StringResources), nameof(StringResources.Error_invalidEmailAddress))]
        public string Email { get; set; }

        //The Password Attribute makes sure the value is a strong enough password. You can customize this via properties. (See below)
        [Password(typeof(StringResources), nameof(StringResources.Error_weakPassword))]
        public string Password { get; set; }

        //The StringLength Attribute makes sure the value is of the specified length.
        [StringLength(2, typeof(StringResources), nameof(StringResources.Error_invalidName))]
        public string Name { get; set; }
    }
}
```
4. The corresponding CS file looks like this:
```csharp
using ...
using Xamarin.AttributeValidation;

namespace Example.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        private RegisterViewModel viewModel;

        public RegisterPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new RegisterViewModel();
        }

        private async void Register_Clicked(object sender, EventArgs e)
        {
            //Pass the viewModel as parameter
            var isValid = await this.ValidateAsync();
            if (isValid)
                await DisplayAlert("Validation", "Model is valid", "OK");
            //isValid holds a boolean, representing if the ViewModel is valid or not.
            //The Validator will automatically display the errormessages in the UI.
        }
    }
}
```

# Supported Attributes:
Every Attribute must be given an error message in its constructor.
Since Attributes are hardcoded into the assembly, you can only pass hardcoded strings.
But don't worry!
Every Attribute has an overload, where you can pass the type and the key of a resource file.

**IMPORTANT!** The Resource File must have code generation enabled and set to "external tool" (public properties, NOT internal!).
I highly recommend the ResX Manager: https://marketplace.visualstudio.com/items?itemName=TomEnglert.ResXManager

### Password
```csharp
//Using the overload to use the localized string value.
[Password(typeof(StringResources), nameof(StringResources.Error_weakPassword))]
```
Properties:
* MinimumLenght (Default = 8)
* MustContainSpecial (Default = true)
* MustContainNumber (Default = true)
* MustContainAlpha (Default = true)
* MustContainUpperCase (Default = true)

### Email Address
```csharp
//Using the overload to use the localized string value.
[EmailAddress(typeof(StringResources), nameof(StringResources.Error_invalidEmailAddress))]
```

### StringLength
```csharp
//Using a hardcoded value.
[StringLength(2, "Field must be at least two characters long.")]
```
Constructor:
* Minimum Length
