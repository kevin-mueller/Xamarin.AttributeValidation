using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace Xamarin.AttributeValidation.Models
{
    internal class ValidationModel
    {
        internal View UiElement { get; set; }
        internal PropertyInfo ViewModelProperty { get; set; }
        internal List<string> ValidationResult { get; set; }
    }
}
