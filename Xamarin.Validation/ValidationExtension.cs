using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.AttributeValidation.Attributes;
using Xamarin.AttributeValidation.Helpers;
using Xamarin.Forms;

namespace Xamarin.AttributeValidation
{
    public static class ValidationExtension
    {
        public static async Task<bool> ValidateAsync(this Page page)
        {
            return await Validation.GetInstance(page).ValidateAsync(page);
        }
    }
}