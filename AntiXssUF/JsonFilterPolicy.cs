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
        Dictionary<string, PolicyHtmlAttribute> GetPolicyHtmlAttributes(IEnumerable<IConfigurationSection> sections) {
            return sections.Select(e =>
            {
                var attr = new PolicyHtmlAttribute(e.GetValue<string>("Name"));
                attr.AllowedRegExp = e.GetSection("AllowedRegExp").Get<FilterRegExp[]>();
                attr.AllowedValues = e.GetSection("AllowedValues").Get<string[]>();
                attr.Description = e.GetValue<string>("Description");
                attr.OnInvalid = e.GetValue<PolicyHtmlAttributeOnInvalid>("OnInvalid");
                return attr;
            }).ToDictionary(e => e.Name, e => e,StringComparer.OrdinalIgnoreCase);
        }
        public void Init(IConfigurationRoot configuration, string name) {
            if (Initialized) return;
            initialized = true;
            this.name = name;
            commonRegularExpressions = new Dictionary<string, string>(configuration.GetSection("CommonRegularExpressions")
                .Get<Dictionary<string, string>>(),StringComparer.OrdinalIgnoreCase);
            directives =new Dictionary<string, string>(configuration.GetSection("Directives")
                .Get< Dictionary<string, string>>(), StringComparer.OrdinalIgnoreCase);
            commonAttributes = GetPolicyHtmlAttributes(configuration.GetSection("CommonAttributes").GetChildren());
            globalAttributes = GetPolicyHtmlAttributes(configuration.GetSection("GlobalAttributes").GetChildren());
            cssRules = configuration.GetSection("CssRules").GetChildren().Select(e =>
            {
                var attr = new PolicyCssProperty(e.GetValue<string>("Name"));
                attr.AllowedRegExp = e.GetSection("AllowedRegExp").Get<FilterRegExp[]>();
                attr.AllowedValues = e.GetSection("AllowedValues").Get<string[]>();
                attr.Description = e.GetValue<string>("Description");
                attr.Shorthands = e.GetSection("Shorthands").Get<string[]>();
                return attr;
            }).ToDictionary(e => e.Name, e => e, StringComparer.OrdinalIgnoreCase);
            tagRules = configuration.GetSection("TagRules").GetChildren().Select(e =>
                  new PolicyHtmlTag(GetPolicyHtmlAttributes(e.GetSection("AllowedAttributes").GetChildren()))
                  {
                      Name = e.GetValue<string>("Name")
                      ,
                      Action = e.GetValue<PolicyHtmlTagAction>("Action")
                  }
            ).ToDictionary(e => e.Name, e => e,StringComparer.OrdinalIgnoreCase);
        }
        public void Init(string config, string name)
        {
            if (Initialized) return;
            IConfigurationRoot root;
            var builder = new ConfigurationBuilder();
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(config),false))
            {  
                builder.AddJsonStream(memoryStream);
                root= builder.Build();
            }
            Init(root, name);
        }
        public void Init(Stream config, string name)
        {
            if (Initialized) return;
            IConfigurationRoot root;
            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(config);
            root = builder.Build();
            Init(root, name);
        }



    }
}
