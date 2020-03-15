namespace Xamarin.AttributeValidation.Attributes
{
    internal interface IValidationAttribute
    {
        string ValidateValue(string value);

        string ErrorMessage { get; }
    }
}