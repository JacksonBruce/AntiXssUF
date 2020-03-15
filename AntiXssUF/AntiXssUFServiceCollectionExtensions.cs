using System;
using System.Collections.Generic;
using System.Text;
using Ufangx.Xss;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AntiXssUFServiceCollectionExtensions
    {
        public static XssFilterBuilder AddXssFilter(this IServiceCollection services, Action<FilterPolicyOptions> configureOptions=null) {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }
            services.AddSingleton<IFilterPolicyProvider, FilterPolicyProvider>();
            services.AddTransient<IFilterPolicyFactory, FilterPolicyFactory>();
            return XssFilterBuilder.Builder ?? new XssFilterBuilder(services);
        }
    }
}
