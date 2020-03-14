using System;
using System.Linq;
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
            bool result = false;

            var properties = viewModel.GetType().GetProperties();
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

                    var validationResult = validationAttribute.Test(propertyValue);
                    if (!string.IsNullOrEmpty(validationResult))
                    {
                        result = false;

                        var errorProperties = properties.Where(x => x.GetCustomAttributes(typeof(ErrorDisplay), false).Length > 0);
                        if (!errorProperties.Any())
                            throw new Exception("Your ViewModel does not contain a property with the ErrorDisplay attribute.");

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
                    else
                    {
                        result = true;
                    }
                }
            }
            return await Task.FromResult(result);
        }
    }
}