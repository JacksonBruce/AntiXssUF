using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class XssSchemeNameAttribute : Attribute, IXssSchemeName
    {
        private readonly string scheme;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheme"></param>
        public XssSchemeNameAttribute(string scheme)
        {
            this.scheme = scheme;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public Task<string> GetSchemeName(HttpContext httpContext)
            => Task.FromResult(scheme);
    }
}
