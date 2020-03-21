using System;
using System.Collections.Generic;
using System.Text;
using Ufangx.Xss;

namespace Microsoft.Extensions.DependencyInjection
{
   /// <summary>
   /// 服务扩展方法
   /// </summary>
    public static class AntiXssUFServiceCollectionExtensions
    {
      /// <summary>
      /// 添加Xss过滤器
      /// </summary>
      /// <param name="services"></param>
      /// <param name="configureOptions"></param>
      /// <returns></returns>
        public static XssFilterBuilder AddXssFilter(this IServiceCollection services, Action<FilterPolicyOptions> configureOptions=null) {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            new Configures(services).Options(configureOptions);
            services.AddSingleton<IFilterPolicyProvider, FilterPolicyProvider>();
            services.AddTransient<IFilterPolicyFactory, FilterPolicyFactory>();
            return XssFilterBuilder.Builder ?? new XssFilterBuilder(services);
        }
    }
}
