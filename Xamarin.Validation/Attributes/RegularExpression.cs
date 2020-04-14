using System;
using System.Text.RegularExpressions;

namespace Xamarin.AttributeValidation.Attributes
{
    public sealed class RegularExpression : Attribute, IValidationAttribute
    {
        public string ErrorMessage { get; }
        public Regex RegExpression { get; set; }

        public RegularExpression(string ErrorMessage, string RegExpression)
        {
            this.RegExpression = new Regex(RegExpression);
            this.ErrorMessage = ErrorMessage;
        }

        public RegularExpression(Type StringResourcesType, string key, string RegExpression)
        {
            ErrorMessage = Array.Find(StringResourcesType.GetProperties(), x => x.Name.Equals(key)).GetValue(null, null).ToString();
            this.RegExpression = new Regex(RegExpression);
        }

        public string ValidateValue(string value)
        {
            return RegExpression.IsMatch(value) ? string.Empty : ErrorMessage;
        }
    }
}