using Microsoft.Extensions.Configuration;
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
        Dictionary<string, PolicyHtmlAttribute> GetPolicyHtmlAttributes(IEnumerable<IConfigurationSection> sections)
            => sections.Select(e => new PolicyHtmlAttribute(e.GetValue<string>("Name"))
            {
                AllowedRegExp = e.GetSection("AllowedRegExp").Get<FilterRegExp[]>(),
                AllowedValues = e.GetSection("AllowedValues").Get<string[]>(),
                Description = e.GetValue<string>("Description"),
                OnInvalid = e.GetValue<PolicyHtmlAttributeOnInvalid>("OnInvalid")
            }).ToDictionary(e => e.Name, e => e, StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 初始化策略
        /// </summary>
        /// <param name="configuration">配置文档</param>
        /// <param name="name">策略名称</param>
        public void Init(IConfigurationRoot configuration, string name) {
            if (Initialized) return;      
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            commonRegularExpressions = new Dictionary<string, string>(configuration.GetSection("CommonRegularExpressions").Get<Dictionary<string, string>>(),StringComparer.OrdinalIgnoreCase);
            directives =new Dictionary<string, string>(configuration.GetSection("Directives").Get< Dictionary<string, string>>(), StringComparer.OrdinalIgnoreCase);
            commonAttributes = GetPolicyHtmlAttributes(configuration.GetSection("CommonAttributes").GetChildren());
            globalAttributes = GetPolicyHtmlAttributes(configuration.GetSection("GlobalAttributes").GetChildren());
            cssRules = configuration.GetSection("CssRules").GetChildren().Select(e =>
            new PolicyCssProperty(e.GetValue<string>("Name"))
            { 
                AllowedRegExp = e.GetSection("AllowedRegExp").Get<FilterRegExp[]>(),
                AllowedValues = e.GetSection("AllowedValues").Get<string[]>(),
                Description = e.GetValue<string>("Description"),
                Shorthands = e.GetSection("Shorthands").Get<string[]>()
            }).ToDictionary(e => e.Name, e => e, StringComparer.OrdinalIgnoreCase);
            tagRules = configuration.GetSection("TagRules").GetChildren().Select(e =>
                  new PolicyHtmlTag(GetPolicyHtmlAttributes(e.GetSection("AllowedAttributes").GetChildren()))
                  {
                      Name = e.GetValue<string>("Name"),
                      Action = e.GetValue<PolicyHtmlTagAction>("Action")
                  }
            ).ToDictionary(e => e.Name, e => e, StringComparer.OrdinalIgnoreCase);

            initialized = true;
            this.name = name;
        }

        /// <summary>
        /// 初始化策略
        /// </summary>
        /// <param name="config">json配置文档</param>
        /// <param name="name">策略名称</param>
        public void Init(string config, string name) 
            => Init(Initialized?null:new MemoryStream(Encoding.UTF8.GetBytes(config), false), name);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config">json配置文档</param>
        /// <param name="name">策略名称</param>
        public void Init(Stream config, string name)
            => Init(Initialized ? null : new ConfigurationBuilder().AddJsonStream(config ?? throw new ArgumentNullException(nameof(config))).Build(), null);




    }
}
