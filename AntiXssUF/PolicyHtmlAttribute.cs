using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    /// <summary>
    /// html标签属性过滤策略
    /// </summary>
    [Serializable]
    public class PolicyHtmlAttribute : PolicyAttribute
    {
        /// <summary>
        /// 创建html标签属性过滤策略
        /// </summary>
        /// <param name="name"></param>
        public PolicyHtmlAttribute(string name)
            : base(name)
        {

        }
        /// <summary>
        /// 属性值被验证失败后的处理方式，如果当前属性值是无效的，那么是移除属性，还是移除整个标签，或者过滤标签保留内容
        /// </summary>
        public PolicyHtmlAttributeOnInvalid OnInvalid
        {
            get;
            set;
        }
      
    }
}
