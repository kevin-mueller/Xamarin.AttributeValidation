using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.AttributeValidation.Attributes
{
    public sealed class StringLength : Attribute, IValidationAttribute
    {
        public string ErrorMessage { get; }

        private readonly int length;

        public StringLength(int length, string ErrorMessage)
        {
            this.length = length;
            this.ErrorMessage = ErrorMessage;
        }

        public StringLength(int length, Type StringResourcesType, string key)
        {
            this.length = length;

            var property = Array.Find(StringResourcesType.GetProperties(), x => x.Name.Equals(key));
            ErrorMessage = property.GetValue(null, null).ToString();
        }

        /// <summary>
        /// Check if value is longer or equal to the specified length.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>
        /// string.Empty if everything is allright.
        /// Otherwise the error message.
        /// </returns>
        public string ValidateValue(string value)
        {
            return (value.Length >= length) ? string.Empty : ErrorMessage;
        }
    }
}
