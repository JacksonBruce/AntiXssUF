using System.Threading.Tasks;

namespace Ufangx.Xss
{
    /// <summary>
    /// 过来策略工厂
    /// </summary>
    public interface IFilterPolicyFactory
    {
        /// <summary>
        /// 创建过滤策略
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<IFilterPolicy> CreatePolicy(string name = null);
        /// <summary>
        /// 创建html过滤器
        /// </summary>
        /// <param name="policyName"></param>
        /// <returns></returns>
        Task<IHtmlFilter> CreateHtmlFilter(string policyName = null);
        /// <summary>
        /// 创建css过滤器
        /// </summary>
        /// <param name="policyName"></param>
        /// <returns></returns>
        Task<ICssFilter> CreateCssFilter(string policyName = null);
        /// <summary>
        /// 创建html过滤器
        /// </summary>
        /// <param name="policy"></param>
        /// <returns></returns>
        Task<IHtmlFilter> CreateHtmlFilter(IFilterPolicy policy);
        /// <summary>
        /// 创建css过滤器
        /// </summary>
        /// <param name="policy"></param>
        /// <returns></returns>
        Task<ICssFilter> CreateCssFilter(IFilterPolicy policy);
    }
}