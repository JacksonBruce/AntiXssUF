using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;

namespace Ufangx.Xss
{
    [Serializable]
    public class AntisamyPolicy: IFilterPolicy
    {
        Dictionary<string, string> commonRegularExpressions, directives;
        Dictionary<string, PolicyHtmlAttribute> commonAttributes, globalAttributes;
        Dictionary<string, PolicyHtmlTag> tagRules;
        Dictionary<string, PolicyCssProperty> cssRules;
        private string name;

        public string Name => name;

        public Dictionary<string, string> CommonRegularExpressions =>commonRegularExpressions;

        public Dictionary<string, string> Directives => directives;

        public Dictionary<string, PolicyHtmlAttribute> CommonAttributes => commonAttributes;

        public Dictionary<string, PolicyHtmlAttribute> GlobalAttributes =>globalAttributes;

        public Dictionary<string, PolicyHtmlTag> TagRules => tagRules;

        public Dictionary<string, PolicyCssProperty> CssRules =>cssRules;

        public bool Initialized { get; private set; }

        public void Init(string config,string name)
        {
            if (Initialized) return;
            if (string.IsNullOrWhiteSpace(config))
            {
                throw new ArgumentException("message", nameof(config));
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("message", nameof(name));
            }    
            this.name = name;
            XDocument doc;
            try
            {
                doc = XDocument.Parse(config);
            }
            catch (Exception x) { throw new FilterPolicyException("无效的XSSAttacks过滤策略。", x); }
            try
            {
                Init(doc);
            }
            catch (Exception x) { throw new FilterPolicyException("XSSAttacks策略文档不是一个有效的架构。", x); }
            Initialized = true;
        
        }
    

        #region 帮助方法
        void Init(XDocument doc)
        {
            XElement root = doc.Root;
            var commonRegularExpressionListNode = root.Element("common-regexps");
            commonRegularExpressions = ParseNamesValues(commonRegularExpressionListNode.Elements("regexp"));

            var directiveListNode = root.Element("directives");
            this.directives = ParseNamesValues(directiveListNode.Elements("directive"));

            var commonAttributeListNode = root.Element("common-attributes");
            this.commonAttributes = ParseHtmlAttributes(commonAttributeListNode);

            var globalAttributesListNode = root.Element("global-tag-attributes");
            this.globalAttributes = ParseHtmlAttributes(globalAttributesListNode);

            var tagListNode = root.Element("tag-rules");
            this.tagRules = ParseHtmlTags(tagListNode);

            var cssListNode = root.Element("css-rules");
            this.cssRules = ParseCssProperties(cssListNode);

        }

        FilterRegExp[] GetRegexList(XElement e)
        {
            IEnumerable<XElement> nodes = e?.Element("regexp-list")?.Elements("regexp");
            if (nodes==null || nodes.Count() == 0)
            {
                return null;
            }
            return (from n in nodes
                    let name = Attr(n, "name")
                    let value = Attr(n, "value")
                    where !string.IsNullOrWhiteSpace(name) || !string.IsNullOrWhiteSpace(value)
                    select new FilterRegExp() { Name = name, Value = value }).ToArray();
        }
        string[] GetLiteralList(XElement e)
        { 
            IEnumerable<XElement> nodes= e?.Element("literal-list")?.Elements("literal");
            if (nodes == null || nodes.Count() == 0)
            {
                return null;
            }
            return (from n in nodes
                    let vale = Attr(n, "value") ?? Value(n)
                    where (vale != null && vale.Length > 0)
                    select vale).ToArray();
        }
        Dictionary<string, string> ParseNamesValues(IEnumerable<XElement> elements)
        {
            if (elements == null) return null;
            var list = from e in elements select new { name = Attr(e, "name"), value = Attr(e, "value") };
            Dictionary<string, string> dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var n in list)
            {
                if (n != null && !string.IsNullOrWhiteSpace(n.name) && !string.IsNullOrWhiteSpace(n.value))
                {
                    string key = n.name.ToLower();
                    if (!dic.ContainsKey(key))
                    { dic.Add(key, n.value); }

                }
            }
            return dic;
        }
        Dictionary<string, PolicyHtmlAttribute> ParseHtmlAttributes(XElement e)
        {
            IEnumerable<XElement> elements = e?.Elements("attribute");
            if (elements == null) return null;
            Dictionary<string, PolicyHtmlAttribute> attrs = new Dictionary<string, PolicyHtmlAttribute>(StringComparer.OrdinalIgnoreCase);
            foreach (var node in elements)
            {
                string key;
                var name = Attr(node, "name");
                if (string.IsNullOrWhiteSpace(name) || attrs.ContainsKey(key=name.Trim().ToLower())) continue;
                attrs.Add(key, new PolicyHtmlAttribute(name)
                {
                    OnInvalid = Enum.TryParse(Attr(node, "onInvalid") ?? string.Empty, true, out PolicyHtmlAttributeOnInvalid onInvalid) ? onInvalid : PolicyHtmlAttributeOnInvalid.RemoveAttribute,
                    Description = Attr(node, "description"),
                    AllowedRegExp = GetRegexList(node),
                    AllowedValues = GetLiteralList(node)
                });
            }
            return attrs;
        }
        Dictionary<string, PolicyHtmlTag> ParseHtmlTags(XElement e)
        {
            IEnumerable<XElement> elements = e?.Elements("tag");
            if (elements == null) return null;
            Dictionary<string, PolicyHtmlTag> tags = new Dictionary<string, PolicyHtmlTag>(StringComparer.OrdinalIgnoreCase);
           
            foreach (var tagNode in elements)
            {
                string name = Attr(tagNode, "name"), key;
                if (string.IsNullOrWhiteSpace(name) || tags.ContainsKey(key = name.Trim().ToLower())) continue;
                tags.Add(key
                    , new PolicyHtmlTag(ParseHtmlAttributes(tagNode))
                    {
                        Name = name,
                        Action = Enum.TryParse(Attr(tagNode, "action")??string.Empty, true, out PolicyHtmlTagAction action) ? action : PolicyHtmlTagAction.Remove
                    });

            }
            return tags;

        }
        private Dictionary<string, PolicyCssProperty> ParseCssProperties(XElement e)
        {
            IEnumerable<XElement> elements = e.Elements("property");
            if (elements == null) return null;
            Dictionary<string, PolicyCssProperty> properties = new Dictionary<string, PolicyCssProperty>(StringComparer.OrdinalIgnoreCase);

            foreach (var node in elements)
            {
                string name = Attr(node, "name"), key;
                if (string.IsNullOrWhiteSpace(name) || properties.ContainsKey(key = name.Trim().ToLower())) continue;
                var shorthands = node?.Element("shorthand-list")?.Elements("shorthand");
                properties.Add(key, new PolicyCssProperty(name)
                {
                    Description = Attr(node, "description"),
                    AllowedRegExp = GetRegexList(node),
                    AllowedValues = GetLiteralList(node),
                    Shorthands = shorthands == null ? null : (from n in shorthands
                                                              let shn = Attr(n, "name")
                                                              where shn != null && shn.Length > 0
                                                              select shn).ToArray()
                });
            }

            return properties;
        }
        string Attr(XElement e, string name)
        {
            XAttribute attr = string.IsNullOrWhiteSpace(name) ? null : e?.Attribute(name);
            return attr == null ? null : HttpUtility.HtmlDecode(attr.Value);
        }
        string Value(XElement e)
        {
            return e?.Value;
        }

        #endregion

    }
}
