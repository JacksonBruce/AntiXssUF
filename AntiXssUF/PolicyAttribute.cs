using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    /// <summary>
    /// 属性过滤策略
    /// </summary>
    [Serializable]
    public class PolicyAttribute
    {
        /// <summary>
        /// 创建属性过滤策略
        /// </summary>
        /// <param name="name"></param>
        public PolicyAttribute(string name)
        {
            Name = name; 
        }
        /// <summary>
        /// 正则表达式白名单
        /// </summary>
        public FilterRegExp[] AllowedRegExp
        {
            get;
            set;
        }
        /// <summary>
        /// 属性值白名单
        /// </summary>
        public string[] AllowedValues
        {
            get;
            set;
        }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name
        {
            get;
            protected set;
        }
        /// <summary>
        /// 策略描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }
      
        
    }
}
