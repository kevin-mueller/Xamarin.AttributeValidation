using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Validation.Attributes;

namespace Xamarin.Validation
{
    public static class Validator
    {
        /// <summary>
        /// Validates the ViewModel.
        /// For every property that is invalid, the specified error message will be added to the errormessage property.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns>
        /// True if all properties of the model are valid
        /// False if any property the model is invalid.
        /// </returns>
        public static async Task<bool> Validate(object viewModel)
        {
            bool isValid = true;

            var properties = viewModel.GetType().GetProperties();

            var errorProperties = GetAllErrorProperties(properties);
            ResetAllErrorMessages(errorProperties, viewModel);

            foreach (var property in properties)
            {
                foreach (var att in property.GetCustomAttributes(typeof(IValidationAttribute), false))
                {
                    IValidationAttribute validationAttribute;
                    try
                    {
                        validationAttribute = (IValidationAttribute)att;
                    }
                    catch
                    {
                        //This is another attribute. Doesn't concern us.
                        continue;
                    }
                    var propertyValue = property.GetValue(viewModel)?.ToString() ?? string.Empty;

                    var validationResult = validationAttribute.ValidateValue(propertyValue);
                    if (!string.IsNullOrEmpty(validationResult))
                    {
                        isValid = false;

                        foreach (var errorProp in errorProperties)
                        {
                            var value = errorProp.GetValue(viewModel)?.ToString();
                            if (string.IsNullOrEmpty(value))
                            {
                                errorProp.SetValue(viewModel, validationResult);
                            }
                            else
                            {
                                errorProp.SetValue(viewModel, value + "\n" + validationResult);
                            }
                        }
                    }
                }
            }
            return await Task.FromResult(isValid);
        }

        private static IEnumerable<PropertyInfo> GetAllErrorProperties(PropertyInfo[] properties)
        {
            var errorProperties = properties.Where(x => x.GetCustomAttributes(typeof(ErrorDisplay), false).Length > 0);
            if (!errorProperties.Any())
                throw new Exception("Your ViewModel does not contain a property with the ErrorDisplay attribute.");
            return errorProperties;
        }

        private static void ResetAllErrorMessages(IEnumerable<PropertyInfo> errorProperties, object viewModel)
        {
            foreach (var errorProperty in errorProperties)
            {
                errorProperty.SetValue(viewModel, string.Empty);
            }
        }
    }
}