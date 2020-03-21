using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ufangx.Xss;

namespace Ufangx.Xss
{
    /// <summary>
    /// 模型绑定器
    /// </summary>
    public class RichTextBinder : IModelBinder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
  
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;
            IUnvalidatedValueProvider valueProvider = bindingContext.ValueProvider as IUnvalidatedValueProvider;
            var valueProviderResult = valueProvider == null ? bindingContext.ValueProvider.GetValue(modelName) : valueProvider.GetValue(modelName,true);
            if (valueProviderResult == null)
            {
                return null;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);
            string value = valueProviderResult.AttemptedValue;
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            IXssSchemeName scheme = bindingContext.ModelMetadata?.ContainerType?.GetProperty(bindingContext.ModelMetadata.PropertyName).GetCustomAttributes(true)?.Cast<IXssSchemeName>()?.FirstOrDefault() ?? null;

            if (scheme == null) { 
                return (RichText)value;
            }

            RichText richText = new RichText(value, DependencyResolver.Current.GetService<IFilterPolicyFactory>().CreateHtmlFilter(scheme.GetSchemeName(controllerContext.HttpContext).Result).Result);
            return richText;
        }
    }
   
}