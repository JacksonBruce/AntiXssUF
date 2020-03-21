using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ufangx.Xss;
using Microsoft.Extensions.DependencyInjection;

namespace Ufangx.Xss
{
    /// <summary>
    /// 
    /// </summary>
    public class RichTextBinder : IModelBinder
    {
     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }
            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return;
            }
            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);
            string value = (string)valueProviderResult;
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            IXssSchemeName scheme = bindingContext.ModelMetadata is DefaultModelMetadata defaultModelMetadata?
                (defaultModelMetadata.MetadataKind == ModelMetadataKind.Parameter ? defaultModelMetadata.Attributes.ParameterAttributes : defaultModelMetadata.Attributes.PropertyAttributes)?.Cast<IXssSchemeName>()?.FirstOrDefault():null;

            RichText richText = scheme == null ?(RichText)value : 
                new RichText(value,await bindingContext.HttpContext.RequestServices.GetService<IFilterPolicyFactory>().CreateHtmlFilter(await scheme.GetSchemeName(bindingContext.HttpContext)));
          


            bindingContext.Result = ModelBindingResult.Success(richText);
      

        }
    }
}
