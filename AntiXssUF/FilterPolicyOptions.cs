using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    /// <summary>
    /// 
    /// </summary>
    public class FilterPolicyOptions
    {
        private readonly IList<FilterPolicyBuilder> _schemes = new List<FilterPolicyBuilder>();
         /// <summary>
         /// 策略集合
         /// </summary>
        public IEnumerable<FilterPolicyBuilder> Schemes => _schemes;
        /// <summary>
        /// 策略字典集合
        /// </summary>
        public IDictionary<string, FilterPolicyBuilder> SchemeMap { get; } = new Dictionary<string, FilterPolicyBuilder>(StringComparer.Ordinal);
        /// <summary>
        /// 添加策略
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configureBuilder"></param>
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
        /// <summary>
        /// 添加策略
        /// </summary>
        /// <typeparam name="TPolicyType"></typeparam>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public void AddScheme<TPolicyType>(string name, string config) where TPolicyType : IFilterPolicy
          => AddScheme(name, b =>
          {
              b.GetConfig = () => Task.FromResult(config);
              b.PolicyType = typeof(TPolicyType);
          });
        /// <summary>
        /// 添加策略
        /// </summary>
        /// <typeparam name="TPolicyType"></typeparam>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public void AddScheme<TPolicyType>(string name, Func<Task<string>> config) where TPolicyType : IFilterPolicy
          => AddScheme(name, b =>
          {
              b.GetConfig = config;
              b.PolicyType = typeof(TPolicyType);
          });
        /// <summary>
        /// 默认过滤策略
        /// </summary>
        public string DefaultSchemeName { get; set; }
    }
}
