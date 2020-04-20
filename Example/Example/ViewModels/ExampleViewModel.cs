using Example;
using Xamarin.AttributeValidation.Attributes;

namespace Xamarin.AttributeValidation.Example.ViewModels
{
    public class ExampleViewModel : BaseViewModel
    {
        [EmailAddress(typeof(Strings), nameof(Strings.Error_Required))]
        [Required(typeof(Strings), nameof(Strings.Error_EmailAddress))]
        public string EmailAddress { get; set; }

        [Password(typeof(Strings), nameof(Strings.Error_Password), MustContainSpecial = false)]
        [Required(typeof(Strings), nameof(Strings.Error_Required))]
        public string Password { get; set; }

        [Range(typeof(Strings), nameof(Strings.Error_Range), 18, 99)]
        public string Age { get; set; }

        [StringLength(2, typeof(Strings), nameof(Strings.Error_StringLength))]
        public string Name { get; set; }

        [RegularExpression(typeof(Strings), nameof(Strings.Error_Regex), @"\d")]
        public string Address { get; set; }

        [Url(typeof(Strings), nameof(Strings.Error_Url))]
        public string Url { get; set; }
    }
}