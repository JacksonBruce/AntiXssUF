using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using Ufangx.Xss;

namespace AntiXssUF.TestSite.Binders
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
                //return new BinderTypeModelBinder(typeof(RichTextBinder));
                return new RichTextBinder();

            }

            return null;
        }
    }
}
