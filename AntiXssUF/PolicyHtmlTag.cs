using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    [Serializable]
    public class PolicyHtmlTag
    {
        private readonly Dictionary<string, PolicyHtmlAttribute> allowedAttributes;

        public PolicyHtmlTag(Dictionary<string, PolicyHtmlAttribute> attributes)
        {
            if (attributes != null)
            {
                allowedAttributes = attributes;
            }
        }
        public Dictionary<string, PolicyHtmlAttribute> AllowedAttributes => allowedAttributes;
        public PolicyHtmlTagAction Action
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
    
        

    }
}
