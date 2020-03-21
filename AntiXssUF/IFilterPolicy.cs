using System.Collections.Generic;

namespace Ufangx.Xss
{
    /// <summary>
    /// 过滤策略
    /// </summary>
    public interface IFilterPolicy
    {
        /// <summary>
        /// 策略名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 初始化策略
        /// </summary>
        /// <param name="config">配置文档</param>
        /// <param name="name">策略名称</param>
        void Init(string config, string name);
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        bool Initialized { get; }
        /// <summary>
        /// 公用正则表达式
        /// </summary>
        Dictionary<string, string> CommonRegularExpressions { get; }
        /// <summary>
        /// 控制设置
        /// </summary>
        Dictionary<string, string> Directives { get; }
        /// <summary>
        /// 公用属性
        /// </summary>
        Dictionary<string, PolicyHtmlAttribute> CommonAttributes { get; }
        /// <summary>
        /// 全局属性白名单
        /// </summary>
        Dictionary<string, PolicyHtmlAttribute> GlobalAttributes { get; }
        /// <summary>
        /// html标签白名单
        /// </summary>
        Dictionary<string, PolicyHtmlTag> TagRules { get; }
        /// <summary>
        /// 样式表规则白名单
        /// </summary>
        Dictionary<string, PolicyCssProperty> CssRules { get; }
    }
}