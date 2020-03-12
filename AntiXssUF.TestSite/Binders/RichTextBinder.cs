using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ufangx.Xss;

namespace AntiXssUF.TestSite.Binders
{
    public class RichTextBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var modelName = bindingContext.ModelName;

            // Try to fetch the value of the argument by name
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            string value = (string)valueProviderResult;
            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }
            RichText richText = value;
            bindingContext.Result = ModelBindingResult.Success(richText);
            return Task.CompletedTask;

        }
    }
}
