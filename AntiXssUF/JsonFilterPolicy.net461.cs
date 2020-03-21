using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        Dictionary<string, PolicyHtmlAttribute> GetPolicyHtmlAttributes(IEnumerable<JToken> sections)
        => sections?.Select(e => new PolicyHtmlAttribute(e.Value<string>("Name"))
        {
            AllowedRegExp = e.SelectToken("AllowedRegExp")?.ToObject<FilterRegExp[]>(),
            AllowedValues = e.SelectToken("AllowedValues")?.ToObject<string[]>(),
            Description = e.Value<string>("Description"),
            OnInvalid = GetValue<PolicyHtmlAttributeOnInvalid>(e.SelectToken("OnInvalid"))
        }).ToDictionary(e => e.Name, e => e, StringComparer.OrdinalIgnoreCase);

        TEnum GetValue<TEnum>(JToken token,TEnum @default=default(TEnum)) where TEnum : struct 
            => token is JValue jValue && Enum.TryParse(jValue.ToString(), true, out TEnum value) ? value : @default;
        /// <summary>
        /// 初始化策略
        /// </summary>
        /// <param name="configuration">配置文档</param>
        /// <param name="name">策略名称</param>
        public void Init(JToken configuration, string name) {

            if (Initialized) return;      
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            commonRegularExpressions = new Dictionary<string, string>(configuration.SelectToken("CommonRegularExpressions").ToObject<Dictionary<string, string>>(),StringComparer.OrdinalIgnoreCase);
            directives =new Dictionary<string, string>(configuration.SelectToken("Directives").ToObject<Dictionary<string, string>>(), StringComparer.OrdinalIgnoreCase);
            commonAttributes = GetPolicyHtmlAttributes(configuration.SelectToken("CommonAttributes").Children());
            globalAttributes = GetPolicyHtmlAttributes(configuration.SelectToken("GlobalAttributes").Children());
            cssRules = configuration.SelectToken("CssRules").Children().Select(e =>
            new PolicyCssProperty(e.Value<string>("Name"))
            { 
                AllowedRegExp = e.SelectToken("AllowedRegExp")?.ToObject<FilterRegExp[]>(),
                AllowedValues = e.SelectToken("AllowedValues")?.ToObject<string[]>(),
                Description = e.Value<string>("Description"),
                Shorthands = e.SelectToken("Shorthands")?.ToObject<string[]>()
            }).ToDictionary(e => e.Name, e => e, StringComparer.OrdinalIgnoreCase);
            tagRules = configuration.SelectToken("TagRules").Children().Select(e =>
                  new PolicyHtmlTag(GetPolicyHtmlAttributes(e.SelectToken("AllowedAttributes")?.Children()??Enumerable.Empty<JToken>()))
                  {
                      Name = e.Value<string>("Name"),
                      Action =GetValue<PolicyHtmlTagAction>(e.SelectToken("Action"))
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
            => Init(Initialized ? null : JsonConvert.DeserializeObject(config) as JToken, name);
        /// <summary>
        /// 初始化策略
        /// </summary>
        /// <param name="config">json配置文档</param>
        /// <param name="name">策略名称</param>
        public void Init(Stream config, string name)
            => Init(Initialized ? null : JsonConvert.DeserializeObject(new StreamReader(config).ReadToEnd()) as JToken, null);




    }
}
