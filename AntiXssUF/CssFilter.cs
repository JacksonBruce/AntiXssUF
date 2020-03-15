using AngleSharp;
using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Ufangx.Xss
{
    public class CssFilter : ICssFilter
    {

        #region 构造
        public CssFilter(IFilterPolicy policy)
        {
            Policy = policy ?? throw new ArgumentNullException(nameof(policy));
            EmbedStyleSheets = policy.Directive<bool>("embedStyleSheets");
        }
        #endregion
        protected virtual IFilterPolicy Policy { get; }
        protected virtual bool EmbedStyleSheets { get; set; }
        protected virtual bool Validate(ICssProperty attr)
        {
            if (attr == null) return false;
            var property = Policy.CssProperty(attr.Name);
            if (property == null) return false;

            return Policy.ValidateAttribute(property, attr.Value) || (property.Shorthands?.Any(shorthandPropertyName => Policy.ValidateAttribute(Policy.CssProperty(shorthandPropertyName), attr.Value)) ?? false);
        }

        protected virtual string Filters(ICssStyleSheet cssStyleSheet)
        {
            if (cssStyleSheet == null || cssStyleSheet.Rules.Length == 0) return string.Empty;
            for (var i = 0; i < cssStyleSheet.Rules.Length;)
            {
                ICssRule rule = cssStyleSheet.Rules[i];
                if (!Validate(rule))
                    cssStyleSheet.RemoveAt(i);
                else
                    i++;
            }
            return cssStyleSheet.ToCss();
        }

        protected virtual bool Validate(ICssRule rule)
        {
            if (rule is ICssStyleRule styleRule)
            {
                return Validate(styleRule.Style);
            }
            else if (rule is ICssGroupingRule groupingRule)
            {
                return groupingRule.Rules.All(child => Validate(child));
            }
            else if (rule is ICssPageRule pageRule)
            {
                return Validate(pageRule.Style);
            }
            else if (rule is ICssKeyframesRule keyFramesRule)
            {
                return keyFramesRule.Rules.OfType<ICssKeyframeRule>().All(child => Validate(child));
            }
            else if (rule is ICssKeyframeRule keyFrameRule)
            {
                return Validate(keyFrameRule.Style);
            }
            else if (rule is ICssImportRule importRule)
            {
                return EmbedStyleSheets;
            }

            return false;
        }

        protected virtual bool Validate(ICssStyleDeclaration styles)
        {
            if (styles == null) return false;
            var removings = new List<ICssProperty>();
            foreach (ICssProperty item in styles)
            {
                if (!Validate(item)) removings.Add(item);
            }
            foreach (var item in removings)
            {
                styles.RemoveProperty(item.Name);
            }

            return styles.Length > 0;
        }
        public string Filters(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return string.Empty;
            var parser = new CssParser(new CssParserOptions
            {
                IsIncludingUnknownDeclarations = true,
                IsIncludingUnknownRules = true,
                IsToleratingInvalidSelectors = true
            });
            ICssStyleSheet styleSheet;
            try
            {
                styleSheet = parser.ParseStyleSheet(code);
                if (styleSheet == null || styleSheet.Rules.Length == 0)
                {
                    var d = parser.ParseDeclaration(code);
                    if (Validate(d)) { return d.ToCss(); }
                    else { return string.Empty; }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return Filters(styleSheet);
        }
    }
}
