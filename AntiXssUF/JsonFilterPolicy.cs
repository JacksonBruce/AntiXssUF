using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ufangx.Xss
{
    public class JsonFilterPolicy : IFilterPolicy
    {
        Dictionary<string, string> commonRegularExpressions, directives;
        Dictionary<string, PolicyHtmlAttribute> commonAttributes, globalAttributes;
        Dictionary<string, PolicyHtmlTag> tagRules;
        Dictionary<string, PolicyCssProperty> cssRules;
        private string name;
        private bool initialized;

        public string Name => name;

        public bool Initialized => initialized;

        public Dictionary<string, string> CommonRegularExpressions => commonRegularExpressions;

        public Dictionary<string, string> Directives => directives;

        public Dictionary<string, PolicyHtmlAttribute> CommonAttributes => commonAttributes;

        public Dictionary<string, PolicyHtmlAttribute> GlobalAttributes => globalAttributes;

        public Dictionary<string, PolicyHtmlTag> TagRules => tagRules;

        public Dictionary<string, PolicyCssProperty> CssRules => cssRules;
        Dictionary<string, PolicyHtmlAttribute> GetPolicyHtmlAttributes(IEnumerable<IConfigurationSection> sections)
            => sections.Select(e => new PolicyHtmlAttribute(e.GetValue<string>("Name"))
            {
                AllowedRegExp = e.GetSection("AllowedRegExp").Get<FilterRegExp[]>(),
                AllowedValues = e.GetSection("AllowedValues").Get<string[]>(),
                Description = e.GetValue<string>("Description"),
                OnInvalid = e.GetValue<PolicyHtmlAttributeOnInvalid>("OnInvalid")
            }).ToDictionary(e => e.Name, e => e, StringComparer.OrdinalIgnoreCase);
        public void Init(IConfigurationRoot configuration, string name) {
            if (Initialized) return;      
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            initialized = true;
            this.name = name;
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
        }
        public void Init(string config, string name) 
            => Init(Initialized?null:new MemoryStream(Encoding.UTF8.GetBytes(config), false), name);
        public void Init(Stream config, string name)
            => Init(Initialized ? null : new ConfigurationBuilder().AddJsonStream(config ?? throw new ArgumentNullException(nameof(config))).Build(), null);




    }
}
