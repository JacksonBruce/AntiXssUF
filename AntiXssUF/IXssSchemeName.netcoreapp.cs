using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

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
        Task<string> GetSchemeName(HttpContext httpContext);
    }
}