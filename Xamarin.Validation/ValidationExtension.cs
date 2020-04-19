using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.AttributeValidation.Attributes;
using Xamarin.AttributeValidation.Helpers;
using Xamarin.Forms;

namespace Xamarin.AttributeValidation
{
    public static class ValidationExtension
    {
        public static bool Validate(this Page page)
        {
            return Validation.GetInstance(page).Validate(page);
        }
    }
}