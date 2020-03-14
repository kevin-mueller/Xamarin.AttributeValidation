using System;
using System.Linq;

namespace Xamarin.Validation.Attributes
{
    public sealed class Password : Attribute, IValidationAttribute
    {
        public string ErrorMessage { get; }

        public int MinimumLenght { get; set; } = 8;
        public bool MustContainSpecial { get; set; } = true;
        public bool MustContainNumber { get; set; } = true;

        public Password(string ErrorMessage)
        {
            this.ErrorMessage = ErrorMessage;
        }

        public Password(Type StringResourcesType, string key)
        {
            var property = StringResourcesType.GetProperties().FirstOrDefault(x => x.Name.Equals(key));
            ErrorMessage = property.GetValue(null, null).ToString();
        }

        /// <summary>
        /// Tests the input value for the specified conditions
        /// </summary>
        /// <param name="value"></param>
        /// <returns>
        /// string.Empty if the password is valid.
        /// Otherwise the errormessage.
        /// </returns>
        public string Test(string value)
        {
            bool isValid = false;
            if (value.Length >= 8 && HasSpecialChars(value) && HasNumbers(value))
                isValid = true;

            return isValid ? string.Empty : ErrorMessage;
        }

        private bool HasSpecialChars(string value)
        {
            return value.Any(ch => !char.IsLetterOrDigit(ch));
        }

        private bool HasNumbers(string value)
        {
            return value.Any(char.IsDigit);
        }
    }
}