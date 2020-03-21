using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ufangx.Xss
{
    /// <summary>
    /// Xss过滤器生成者
    /// </summary>
    public partial class XssFilterBuilder
    {  
        internal static XssFilterBuilder Builder { get; private set; }

        private readonly Configures configures;

        internal XssFilterBuilder(IServiceCollection services)
        {
            Services = services;
            Builder = this;
            configures = new Configures(services);
        }
        /// <summary>
        /// 服务注册集合
        /// </summary>
        public virtual IServiceCollection Services { get; }
        IServiceProvider _serviceProvider;
        internal virtual IServiceProvider ServiceProvider => _serviceProvider ?? (_serviceProvider = Services.BuildServiceProvider());

        //partial void configureFilterPolicyOptions(string name, Type PolicyType, Func<Task<string>> configure);
        private XssFilterBuilder AddSchemeHelper<TPolicy>(string name, Func<Task<string>> configure)
           where TPolicy : class, IFilterPolicy
        {
            configures.Options<FilterPolicyOptions>(o => o.AddScheme(name, scheme =>
            {
                scheme.PolicyType = typeof(TPolicy);
                scheme.GetConfig = configure;
            }));
            Services.AddSingleton<TPolicy>();
            return this;
        }
        private XssFilterBuilder AddSchemeHelper<TOptions, TPolicy>(string name,  Func<Task<string>> configure, Action<TOptions> configureOptions)
           where TOptions : class, new()
           where TPolicy : class, IFilterPolicy
        {
            if (configureOptions != null)
            {
                configures.Options(configureOptions, name);
            }
            return AddSchemeHelper<TPolicy>(name, configure);
            
        }
        /// <summary>
        /// 添加过滤策略方案
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <typeparam name="TPolicy"></typeparam>
        /// <param name="name"></param>
        /// <param name="configure"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public XssFilterBuilder AddScheme<TOptions, TPolicy>(string name, Func<Task<string>> configure, Action<TOptions> configureOptions)
                    where TOptions : class, new()
           where TPolicy : class, IFilterPolicy
            => AddSchemeHelper<TOptions, TPolicy>(name, configure, configureOptions);
        /// <summary>
        /// 添加过滤策略方案
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <typeparam name="TPolicy"></typeparam>
        /// <param name="name"></param>
        /// <param name="configure"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public XssFilterBuilder AddScheme<TOptions, TPolicy>(string name, string configure, Action<TOptions> configureOptions)
                where TOptions : class, new()
       where TPolicy : class, IFilterPolicy
        => AddSchemeHelper<TOptions, TPolicy>(name, () => Task.FromResult(configure), configureOptions);
        /// <summary>
        /// 添加过滤策略方案
        /// </summary>
        /// <typeparam name="TPolicy"></typeparam>
        /// <param name="name"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public XssFilterBuilder AddScheme<TPolicy>(string name, Func<Task<string>> configure)
            where TPolicy : class, IFilterPolicy
            => AddSchemeHelper<TPolicy>(name, configure);
        /// <summary>
        /// 添加过滤策略方案
        /// </summary>
        /// <typeparam name="TPolicy"></typeparam>
        /// <param name="name"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public XssFilterBuilder AddScheme<TPolicy>(string name, string configure)
          where TPolicy : class, IFilterPolicy
          => AddSchemeHelper<TPolicy>(name, () => Task.FromResult(configure));

        

    }
}
