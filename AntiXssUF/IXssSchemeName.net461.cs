using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Ufangx.Xss
{
    /// <summary>
    /// 
    /// </summary>
    public interface IXssSchemeName
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        Task<string> GetSchemeName(HttpContextBase httpContext);
    }
}
