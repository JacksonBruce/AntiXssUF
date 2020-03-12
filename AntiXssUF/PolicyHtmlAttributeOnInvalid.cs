using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    /// <summary>
    /// 属性值无效时处理方案
    /// </summary>
    public enum PolicyHtmlAttributeOnInvalid
    {

        /// <summary>
        /// 删除属性
        /// </summary>
        RemoveAttribute
            ,
        /// <summary>
        /// 删除当前标签
        /// </summary>
        RemoveTag
            ,
        /// <summary>
        ///删除标签，但保留其有效的子节点和文本
        /// </summary>
        FilterTag
        
    }
}
