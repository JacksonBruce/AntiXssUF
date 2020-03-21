using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ufangx.Xss
{
    /// <summary>
    /// json格式过滤策略配置
    /// </summary>
    public partial class JsonFilterPolicy : IFilterPolicy
    {
        Dictionary<string, string> commonRegularExpressions, directives;
        Dictionary<string, PolicyHtmlAttribute> commonAttributes, globalAttributes;
        Dictionary<string, PolicyHtmlTag> tagRules;
        Dictionary<string, PolicyCssProperty> cssRules;
        private string name;
        private bool initialized;
        /// <summary>
        /// 策略名称
        /// </summary>
        public string Name => name;
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        public bool Initialized => initialized;
        /// <summary>
        /// 公用正则表达式
        /// </summary>
        public Dictionary<string, string> CommonRegularExpressions => commonRegularExpressions;
        /// <summary>
        /// 控制设置
        /// </summary>
        public Dictionary<string, string> Directives => directives;
        /// <summary>
        /// 公用属性
        /// </summary>
        public Dictionary<string, PolicyHtmlAttribute> CommonAttributes => commonAttributes;
        /// <summary>
        /// 全局属性白名单
        /// </summary>
        public Dictionary<string, PolicyHtmlAttribute> GlobalAttributes => globalAttributes;
        /// <summary>
        /// html标签白名单
        /// </summary>
        public Dictionary<string, PolicyHtmlTag> TagRules => tagRules;
        /// <summary>
        /// 样式表规则白名单
        /// </summary>
        public Dictionary<string, PolicyCssProperty> CssRules => cssRules;
         
    
  




    }
}
