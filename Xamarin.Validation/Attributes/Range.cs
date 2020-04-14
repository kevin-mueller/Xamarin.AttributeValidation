using System;

namespace Xamarin.AttributeValidation.Attributes
{
    public sealed class Range : Attribute, IValidationAttribute
    {
        public string ErrorMessage { get; }

        private readonly double From;
        private readonly double To;

        public Range(string ErrorMessage, double From, double To)
        {
            this.ErrorMessage = ErrorMessage;
            this.From = From;
            this.To = To;
        }

        public Range(Type StringResourcesType, string key, double From, double To)
        {
            ErrorMessage = Array.Find(StringResourcesType.GetProperties(), x => x.Name.Equals(key)).GetValue(null, null).ToString();
            this.From = From;
            this.To = To;
        }

        public string ValidateValue(string value)
        {
            double numericValue;
            try
            {
                numericValue = Convert.ToDouble(value);
            }
            catch
            {
                return ErrorMessage;
            }
            return (numericValue > From && numericValue < To) ? string.Empty : ErrorMessage;
        }
    }
}