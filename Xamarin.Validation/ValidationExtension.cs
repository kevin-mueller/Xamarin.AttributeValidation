using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xamarin.AttributeValidation
{
    public static class ValidationExtension
    {
        /// <summary>
        /// Validates the ViewModel and display the error messages in the view.
        /// </summary>
        /// <param name="page"></param>
        /// <returns>
        /// True: ViewModel is valid.
        /// False: ViewModel is invalid.
        /// </returns>
        public static async Task<bool> ValidateAsync(this Page page)
        {
            var instance = await Validation.GetInstance(page);
            return instance.Validate(page);
        }
    }
}