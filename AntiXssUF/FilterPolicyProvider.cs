using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    public class FilterPolicyProvider : IFilterPolicyProvider
    {
        public FilterPolicyProvider(IOptions<FilterPolicyOptions> options)
           : this(options, new Dictionary<string, FilterPolicyBuilder>(StringComparer.OrdinalIgnoreCase))
        {
        }

        protected FilterPolicyProvider(IOptions<FilterPolicyOptions> options, IDictionary<string, FilterPolicyBuilder> schemes)
        {
            _options = options.Value;

            _schemes = schemes ?? throw new ArgumentNullException(nameof(schemes));
            _requestHandlers = new List<FilterPolicyBuilder>();

            foreach (var builder in _options.Schemes)
            {
                ///var scheme = builder.Build();
                AddScheme(builder);
            }
        }


        private readonly FilterPolicyOptions _options;
        private readonly IDictionary<string, FilterPolicyBuilder> _schemes;
        private readonly List<FilterPolicyBuilder> _requestHandlers;
        private readonly object _lock = new object();
        private FilterPolicyBuilder[] _requestHandlersCopy;
        private FilterPolicyBuilder[] _schemesCopy;

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
        public virtual Task<FilterPolicyBuilder> GetSchemeAsync(string name)
        => Task.FromResult(_schemes.ContainsKey(name) ? _schemes[name] : null);
        public virtual Task<FilterPolicyBuilder> GetDefaultSchemeAsync()
         => _options.DefaultSchemeName != null
         ? GetSchemeAsync(_options.DefaultSchemeName)
         : Task.FromResult<FilterPolicyBuilder>(null);
        public virtual async Task<IEnumerable<FilterPolicyBuilder>> GetSchemesAsync()
         => await Task.FromResult(_requestHandlersCopy);
    }
}
