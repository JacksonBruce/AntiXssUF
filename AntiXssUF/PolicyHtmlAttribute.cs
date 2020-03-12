using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    [Serializable]
    public class PolicyHtmlAttribute : PolicyAttribute
    {
        public PolicyHtmlAttribute(string name)
            : base(name)
        {

        }
        public PolicyHtmlAttributeOnInvalid OnInvalid
        {
            get;
            set;
        }
      
    }
}
