using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Ufangx.Xss
{
    internal partial class Configures
    {
        static Dictionary<Type, List<object>> _options = new Dictionary<Type, List<object>>();
        partial void _Options<TOptions>(Action<TOptions> configure, string name) where TOptions : class, new()
        {
          
               var key = typeof(TOptions);
            if (!_options.ContainsKey(key)) _options.Add(key, new List<object>());
            if (configure != null) { _options[key].Add(configure); }


            services.AddTransient(provider => {
                
                var opt = new TOptions();
                var list = _options[key];
                foreach (var item in list)
                {
                    if (item is Action<TOptions> conf) {
                        conf(opt);
                    }
                }
                return opt;
            });
        }
    }
}
