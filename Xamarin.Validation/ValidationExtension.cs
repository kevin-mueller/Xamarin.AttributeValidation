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
            return await Validation.GetInstance(page).ValidateAsync(page);
        }
    }
}