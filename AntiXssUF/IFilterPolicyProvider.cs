using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    /// <summary>
    /// 策略提供者接口
    /// </summary>
    public interface IFilterPolicyProvider
    {
        /// <summary>
        /// 添加策略方案
        /// </summary>
        /// <param name="scheme"></param>
        void AddScheme(FilterPolicyBuilder scheme);
        /// <summary>
        /// 获取默认策略方案
        /// </summary>
        /// <returns></returns>
        Task<FilterPolicyBuilder> GetDefaultSchemeAsync();
        /// <summary>
        /// 获取策略方案
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<FilterPolicyBuilder> GetSchemeAsync(string name);
        /// <summary>
        /// 返回所有策略方案
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<FilterPolicyBuilder>> GetSchemesAsync();
        /// <summary>
        /// 移除策略方案
        /// </summary>
        /// <param name="name"></param>
        void RemoveScheme(string name);
        /// <summary>
        /// 尝试添加策略
        /// </summary>
        /// <param name="scheme"></param>
        /// <returns></returns>
        bool TryAddScheme(FilterPolicyBuilder scheme);
    }
}