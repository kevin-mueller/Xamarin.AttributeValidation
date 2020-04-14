using System;

namespace Xamarin.AttributeValidation.Attributes
{
    public sealed class Url : Attribute, IValidationAttribute
    {
        public string ErrorMessage { get; }

        public Url(string ErrorMessage)
        {
            this.ErrorMessage = ErrorMessage;
        }

        public Url(Type StringResourcesType, string key)
        {
            ErrorMessage = Array.Find(StringResourcesType.GetProperties(), x => x.Name.Equals(key)).GetValue(null, null).ToString();
        }

        public string ValidateValue(string value)
        {
            bool result = Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result ? string.Empty : ErrorMessage;
        }
    }
}