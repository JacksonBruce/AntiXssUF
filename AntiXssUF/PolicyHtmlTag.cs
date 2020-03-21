using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    /// <summary>
    ///html标签策略
    /// </summary>
    [Serializable]
    public class PolicyHtmlTag
    {
        private readonly Dictionary<string, PolicyHtmlAttribute> allowedAttributes;
        /// <summary>
        /// 创建html标签策略
        /// </summary>
        /// <param name="attributes"></param>
        public PolicyHtmlTag(Dictionary<string, PolicyHtmlAttribute> attributes)
        {
            if (attributes != null)
            {
                allowedAttributes = attributes;
            }
        }
        /// <summary>
        /// 标签白名单
        /// </summary>
        public Dictionary<string, PolicyHtmlAttribute> AllowedAttributes => allowedAttributes;
        /// <summary>
        /// 标签的过滤动作
        /// </summary>
        public PolicyHtmlTagAction Action
        {
            get;
            set;
        }
        /// <summary>
        /// 标签名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }
    
        

    }
}
