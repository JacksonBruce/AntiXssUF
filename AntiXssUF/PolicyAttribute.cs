using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    [Serializable]
    public class PolicyAttribute
    {
        public PolicyAttribute(string name)
        {
            Name = name; 
        }
 
        public FilterRegExp[] AllowedRegExp
        {
            get;
            set;
        }
        public string[] AllowedValues
        {
            get;
            set;
        }
        public string Name
        {
            get;
            protected set;
        }
        public string Description
        {
            get;
            set;
        }
      
        
    }
}
