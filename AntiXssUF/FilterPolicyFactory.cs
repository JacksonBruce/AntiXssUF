using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    public class FilterPolicyFactory : IFilterPolicyFactory
    {
        private readonly IFilterPolicyProvider provider;

        public FilterPolicyFactory(IFilterPolicyProvider provider)
        {
            this.provider = provider;
        }
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
        public async Task<IHtmlFilter> CreateHtmlFilter(string policyName = null)
            => await CreateHtmlFilter(await CreatePolicy(policyName));
        public async Task<ICssFilter> CreateCssFilter(string policyName=null)
            => await CreateCssFilter(await CreatePolicy(policyName));

        public Task<IHtmlFilter> CreateHtmlFilter(IFilterPolicy policy)
            => Task.FromResult<IHtmlFilter>(new HtmlFilter(policy));

        public Task<ICssFilter> CreateCssFilter(IFilterPolicy policy)
            => Task.FromResult<ICssFilter>(new CssFilter(policy));
    }
}
