using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xamarin.AttributeValidation.Attributes
{
    public sealed class Required : Attribute, IValidationAttribute
    {
        public string ErrorMessage { get; }

        public Required(string ErrorMessage)
        {
            this.ErrorMessage = ErrorMessage;
        }

        public Required(Type StringResourcesType, string key)
        {
            var property = StringResourcesType.GetProperties().FirstOrDefault(x => x.Name.Equals(key));
            ErrorMessage = property.GetValue(null, null).ToString();
        }

        /// <summary>
        /// Check if value is not null or empty.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>
        /// string.Empty if  everything is allright.
        /// Otherwise the error Message.
        /// </returns>
        public string ValidateValue(string value)
        {
            return string.IsNullOrEmpty(value) ? ErrorMessage : string.Empty;
        }
    }
}
