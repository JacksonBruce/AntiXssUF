using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    public class FilterPolicyOptions
    {
        private readonly IList<FilterPolicyBuilder> _schemes = new List<FilterPolicyBuilder>();
         
        public IEnumerable<FilterPolicyBuilder> Schemes => _schemes;
        public IDictionary<string, FilterPolicyBuilder> SchemeMap { get; } = new Dictionary<string, FilterPolicyBuilder>(StringComparer.Ordinal);
        public void AddScheme(string name, Action<FilterPolicyBuilder> configureBuilder)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (configureBuilder == null)
            {
                throw new ArgumentNullException(nameof(configureBuilder));
            }
            if (SchemeMap.ContainsKey(name))
            {
                throw new InvalidOperationException("策略名称已经存在：" + name);
            }
            var builder = new FilterPolicyBuilder(name);
            configureBuilder(builder);
            _schemes.Add(builder);
            SchemeMap[name] = builder;
        }
        public void AddScheme<TPolicyType>(string name, string config) where TPolicyType : IFilterPolicy
          => AddScheme(name, b =>
          {
              b.GetConfig = () => Task.FromResult(config);
              b.PolicyType = typeof(TPolicyType);
          });
        public void AddScheme<TPolicyType>(string name, Func<Task<string>> config) where TPolicyType : IFilterPolicy
          => AddScheme(name, b =>
          {
              b.GetConfig = config;
              b.PolicyType = typeof(TPolicyType);
          });
        public string DefaultSchemeName { get; set; }
    }
}
