using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    public interface IFilterPolicyProvider
    {
        void AddScheme(FilterPolicyBuilder scheme);
        Task<FilterPolicyBuilder> GetDefaultSchemeAsync();
        Task<FilterPolicyBuilder> GetSchemeAsync(string name);
        Task<IEnumerable<FilterPolicyBuilder>> GetSchemesAsync();
        void RemoveScheme(string name);
        bool TryAddScheme(FilterPolicyBuilder scheme);
    }
}