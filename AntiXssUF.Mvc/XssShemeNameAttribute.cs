using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ufangx.Xss
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class XssSchemeNameAttribute : Attribute, IXssSchemeName
    {
        private readonly string scheme;

        public XssSchemeNameAttribute(string scheme)
        {
            this.scheme = scheme;
        }

        public Task<string> GetSchemeName(HttpContext httpContext)
            => Task.FromResult(scheme);
    }
}
