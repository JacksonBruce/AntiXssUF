using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using Ufangx.Xss;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Ufangx.Xss
{
    /// <summary>
    /// 
    /// </summary>
    public class RichTextBinderProvider : IModelBinderProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(RichText))
            {
                return new BinderTypeModelBinder(typeof(RichTextBinder));

            }

            return null;
        }
    }
}
