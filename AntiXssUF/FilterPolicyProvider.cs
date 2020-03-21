using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    /// <summary>
    /// 策略管理提供者
    /// </summary>
    public partial class FilterPolicyProvider : IFilterPolicyProvider
    {

        private readonly FilterPolicyOptions _options;
        private readonly IDictionary<string, FilterPolicyBuilder> _schemes;
        private readonly List<FilterPolicyBuilder> _requestHandlers;
        private readonly object _lock = new object();
        private FilterPolicyBuilder[] _requestHandlersCopy;
        private FilterPolicyBuilder[] _schemesCopy;
        /// <summary>
        /// 尝试添加策略方案
        /// </summary>
        /// <param name="scheme"></param>
        /// <returns></returns>
        public virtual bool TryAddScheme(FilterPolicyBuilder scheme)
        {
            if (_schemes.ContainsKey(scheme.Name))
            {
                return false;
            }
            lock (_lock)
            {
                if (_schemes.ContainsKey(scheme.Name))
                {
                    return false;
                }
                if (typeof(IFilterPolicy).IsAssignableFrom(scheme.PolicyType))
                {
                    _requestHandlers.Add(scheme);
                    _requestHandlersCopy = _requestHandlers.ToArray();
                }
                _schemes[scheme.Name] = scheme;
                _schemesCopy = _schemes.Values.ToArray();
                return true;
            }
        }
        /// <summary>
        /// 添加策略方案
        /// </summary>
        /// <param name="scheme"></param>
        public virtual void AddScheme(FilterPolicyBuilder scheme)
        {
            if (_schemes.ContainsKey(scheme.Name))
            {
                throw new InvalidOperationException("策略方案名称已经存在: " + scheme.Name);
            }
            lock (_lock)
            {
                if (!TryAddScheme(scheme))
                {
                    throw new InvalidOperationException("策略方案名称已经存在: " + scheme.Name);
                }
            }
        }
        /// <summary>
        /// 移除策略方案
        /// </summary>
        /// <param name="name"></param>
        public virtual void RemoveScheme(string name)
        {
            if (!_schemes.ContainsKey(name))
            {
                return;
            }
            lock (_lock)
            {
                if (_schemes.ContainsKey(name))
                {
                    var scheme = _schemes[name];
                    if (_requestHandlers.Remove(scheme))
                    {
                        _requestHandlersCopy = _requestHandlers.ToArray();
                    }
                    _schemes.Remove(name);
                    _schemesCopy = _schemes.Values.ToArray();
                }
            }
        }
        /// <summary>
        /// 获取策略
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual Task<FilterPolicyBuilder> GetSchemeAsync(string name)
        => Task.FromResult(_schemes.ContainsKey(name) ? _schemes[name] : null);
        /// <summary>
        /// 获取默认策略
        /// </summary>
        /// <returns></returns>
        public virtual Task<FilterPolicyBuilder> GetDefaultSchemeAsync()
         => _options.DefaultSchemeName != null
         ? GetSchemeAsync(_options.DefaultSchemeName)
         : Task.FromResult<FilterPolicyBuilder>(null);
        /// <summary>
        /// 返回所有策略方案
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<FilterPolicyBuilder>> GetSchemesAsync()
         => await Task.FromResult(_requestHandlersCopy);
    }
}
