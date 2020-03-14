namespace Xamarin.Validation.Attributes
{
    internal interface IValidationAttribute
    {
        string Test(string value);

        string ErrorMessage { get; }
    }
}