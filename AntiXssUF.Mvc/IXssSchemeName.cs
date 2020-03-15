using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    public interface IXssSchemeName
    {
        Task<string> GetSchemeName(HttpContext httpContext);
    }
}