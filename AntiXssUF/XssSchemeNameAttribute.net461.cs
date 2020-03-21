using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Ufangx.Xss
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
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
        public Task<string> GetSchemeName(HttpContextBase httpContext)
            => Task.FromResult(scheme);
    }

 
}