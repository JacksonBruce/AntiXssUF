using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Ufangx.Xss
{
    public class XssFilterBuilder
    {  
        internal static XssFilterBuilder Builder { get; private set; }
    
        internal XssFilterBuilder(IServiceCollection services)
        {
            Services = services;
            Builder = this;
        }
        public virtual IServiceCollection Services { get; }
        IServiceProvider _serviceProvider;
        internal virtual IServiceProvider ServiceProvider => _serviceProvider ??= Services.BuildServiceProvider();

        private XssFilterBuilder AddSchemeHelper<TPolicy>(string name, Func<Task<string>> configure)
           where TPolicy : class, IFilterPolicy
        {
            Services.Configure<FilterPolicyOptions>(o =>
                o.AddScheme(name, scheme => {
                    scheme.PolicyType = typeof(TPolicy);
                    scheme.GetConfig = configure;
                })
            );
            Services.AddSingleton<TPolicy>();
            return this;
        }
        private XssFilterBuilder AddSchemeHelper<TOptions, TPolicy>(string name,  Func<Task<string>> configure, Action<TOptions> configureOptions)
           where TOptions : class, new()
           where TPolicy : class, IFilterPolicy
        {
            if (configureOptions != null)
            {
                Services.Configure(name, configureOptions);
            }
            return AddSchemeHelper<TPolicy>(name, configure);
            
        }
        public XssFilterBuilder AddScheme<TOptions, TPolicy>(string name, Func<Task<string>> configure, Action<TOptions> configureOptions)
                    where TOptions : class, new()
           where TPolicy : class, IFilterPolicy
            => AddSchemeHelper<TOptions, TPolicy>(name, configure, configureOptions);
        public XssFilterBuilder AddScheme<TOptions, TPolicy>(string name, string configure, Action<TOptions> configureOptions)
                where TOptions : class, new()
       where TPolicy : class, IFilterPolicy
        => AddSchemeHelper<TOptions, TPolicy>(name, () => Task.FromResult(configure), configureOptions);
        public XssFilterBuilder AddScheme<TPolicy>(string name, Func<Task<string>> configure)
            where TPolicy : class, IFilterPolicy
            => AddSchemeHelper<TPolicy>(name, configure);
        public XssFilterBuilder AddScheme<TPolicy>(string name, string configure)
          where TPolicy : class, IFilterPolicy
          => AddSchemeHelper<TPolicy>(name, () => Task.FromResult(configure));

        static IFilterPolicy presupposedPolicy;
        public static IFilterPolicy GetPresupposedPolicy() {
            if (presupposedPolicy == null)
            {
                var policy = new JsonFilterPolicy();
                policy.Init(Assembly.GetExecutingAssembly().GetManifestResourceStream("Ufangx.Xss.resources.DefaultPolicy.json"), "Presupposed");
                presupposedPolicy = policy;
            }
          
            return presupposedPolicy;
        
        }
    }
}
