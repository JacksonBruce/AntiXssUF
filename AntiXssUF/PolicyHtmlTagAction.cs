using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    /// <summary>
    /// 过滤标签的动作
    /// </summary>
    public enum PolicyHtmlTagAction
    {
        /// <summary>
        /// 删除节点，包括其属性和所有子节点
        /// </summary>
        Remove
            ,
        /// <summary>
        /// 删除所有的属性和子元素，但保留文本和备注节点
        /// </summary>
        Truncate
            ,
        /// <summary>
        /// 验证属性和过滤子节点
        /// </summary>
        Validate
          ,
        /// <summary>
        /// 删除标签，但保留其有效的子节点和文本
        /// </summary>
        Filter
    }
}
