namespace Xamarin.Validation.Attributes
{
    internal interface IValidationAttribute
    {
        string ValidateValue(string value);

        string ErrorMessage { get; }
    }
}