using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    public class FilterPolicyBuilder
    {
        public FilterPolicyBuilder(string Name) {
            this.Name = Name;
        }

        public string Name { get; }
        public Type PolicyType { get; set; }
        public Func<Task<string>> GetConfig { get; set; }

    }
}
