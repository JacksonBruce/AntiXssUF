using System.Threading.Tasks;

namespace Ufangx.Xss
{
    public interface IFilterPolicyFactory
    {
        Task<IFilterPolicy> CreatePolicy(string name = null);
        Task<IHtmlFilter> CreateHtmlFilter(string policyName = null);
        Task<ICssFilter> CreateCssFilter(string policyName = null);
        Task<IHtmlFilter> CreateHtmlFilter(IFilterPolicy policy);
        Task<ICssFilter> CreateCssFilter(IFilterPolicy policy);
    }
}