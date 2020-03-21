using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    /// <summary>
    /// 过滤策略工厂
    /// </summary>
    public class FilterPolicyFactory : IFilterPolicyFactory
    {
        private readonly IFilterPolicyProvider provider;
        /// <summary>
        /// 创建过滤策略工厂
        /// </summary>
        /// <param name="provider"></param>
        public FilterPolicyFactory(IFilterPolicyProvider provider)
        {
            this.provider = provider;
        }
        /// <summary>
        /// 创建过滤策略
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<IFilterPolicy> CreatePolicy(string name = null)
        {
            FilterPolicyBuilder builder;
            if (string.IsNullOrWhiteSpace(name))
            {
                builder = await provider.GetDefaultSchemeAsync();
                if (builder == null) return null;
            }
            else
            {
                builder = await provider.GetSchemeAsync(name);
                if (builder == null)
                {
                    throw new ArgumentException($"无效的策略名称：{name}");
                }
            }

            var policy = (XssFilterBuilder.Builder.ServiceProvider.GetService(builder.PolicyType) ??
               ActivatorUtilities.CreateInstance(XssFilterBuilder.Builder.ServiceProvider, builder.PolicyType))
               as IFilterPolicy;
            if (!policy.Initialized)
            {
                policy.Init(await builder.GetConfig(), builder.Name);
            }
            return policy;
        }
        /// <summary>
        /// 创建html过滤器
        /// </summary>
        /// <param name="policyName"></param>
        /// <returns></returns>
        public async Task<IHtmlFilter> CreateHtmlFilter(string policyName = null)
            => await CreateHtmlFilter(await CreatePolicy(policyName));
        /// <summary>
        /// 创建Css过滤器
        /// </summary>
        /// <param name="policyName"></param>
        /// <returns></returns>
        public async Task<ICssFilter> CreateCssFilter(string policyName=null)
            => await CreateCssFilter(await CreatePolicy(policyName));
        /// <summary>
        /// 创建html过滤器
        /// </summary>
        /// <param name="policy"></param>
        /// <returns></returns>
        public Task<IHtmlFilter> CreateHtmlFilter(IFilterPolicy policy)
            => Task.FromResult<IHtmlFilter>(new HtmlFilter(policy));
        /// <summary>
        /// 创建Css过滤器
        /// </summary>
        /// <param name="policy"></param>
        /// <returns></returns>
        public Task<ICssFilter> CreateCssFilter(IFilterPolicy policy)
            => Task.FromResult<ICssFilter>(new CssFilter(policy));
    }
}
