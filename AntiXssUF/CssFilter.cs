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
    /// <summary>
    /// 样式过滤器
    /// </summary>
    public class CssFilter : ICssFilter
    {

        #region 构造
        /// <summary>
        /// 创建样式过滤器
        /// </summary>
        /// <param name="policy"></param>
        public CssFilter(IFilterPolicy policy)
        {
            Policy = policy ?? throw new ArgumentNullException(nameof(policy));
            EmbedStyleSheets = policy.Directive<bool>("embedStyleSheets");
        }
        #endregion
        /// <summary>
        /// 过滤策略
        /// </summary>
        protected virtual IFilterPolicy Policy { get; }
        /// <summary>
        /// 验证规则
        /// </summary>
        protected virtual bool EmbedStyleSheets { get; set; }
        /// <summary>
        /// 验证规则
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        protected virtual bool Validate(ICssProperty attr)
        {
            if (attr == null) return false;
            var property = Policy.CssProperty(attr.Name);
            if (property == null) return false;

            return Policy.ValidateAttribute(property, attr.Value) || (property.Shorthands?.Any(shorthandPropertyName => Policy.ValidateAttribute(Policy.CssProperty(shorthandPropertyName), attr.Value)) ?? false);
        }
        /// <summary>
        /// 过滤规则
        /// </summary>
        /// <param name="cssStyleSheet"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 验证规则
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 验证规则
        /// </summary>
        /// <param name="styles"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 过滤样式
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
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
                throw ex;
            }

            return Filters(styleSheet);
        }
    }
}
