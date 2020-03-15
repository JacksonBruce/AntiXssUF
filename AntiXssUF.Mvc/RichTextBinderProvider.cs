using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using Ufangx.Xss;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Ufangx.Xss
{
    public class RichTextBinderProvider : IModelBinderProvider
    {
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
