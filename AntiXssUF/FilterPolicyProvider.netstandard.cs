using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    public partial class FilterPolicyProvider : IFilterPolicyProvider
    {
        /// <summary>
        /// 创建策略方案提供者
        /// </summary>
        /// <param name="options"></param>
        public FilterPolicyProvider(IOptions<FilterPolicyOptions> options)
           : this(options, new Dictionary<string, FilterPolicyBuilder>(StringComparer.OrdinalIgnoreCase))
        {
        }
        /// <summary>
        /// 创建策略方案提供者
        /// </summary>
        /// <param name="options"></param>
        /// <param name="schemes"></param>
        protected FilterPolicyProvider(IOptions<FilterPolicyOptions> options, IDictionary<string, FilterPolicyBuilder> schemes)
        {
            _options = options.Value;

            _schemes = schemes ?? throw new ArgumentNullException(nameof(schemes));
            _requestHandlers = new List<FilterPolicyBuilder>();

            foreach (var builder in _options.Schemes)
            {
                AddScheme(builder);
            }
        }

    }
}
