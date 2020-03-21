using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    /// <summary>
    /// 样式属性过滤策略
    /// </summary>
    [Serializable]
    public class PolicyCssProperty : PolicyAttribute
    {
        /// <summary>
        /// 创建样式属性过滤策略
        /// </summary>
        /// <param name="name"></param>
        public PolicyCssProperty(string name) : base(name) { }
        /// <summary>
        /// 样式简短名称列表
        /// </summary>
        public string[] Shorthands { get; set; }
    }
}
