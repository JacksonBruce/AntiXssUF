using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    [Serializable]
    public class PolicyCssProperty : PolicyAttribute
    {
        public PolicyCssProperty(string name) : base(name) { }
        public string[] Shorthands { get; set; }
    }
}
