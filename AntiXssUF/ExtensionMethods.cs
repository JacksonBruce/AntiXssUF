//using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Ufangx.Xss
{
    /// <summary>
    /// 过滤策略扩展方法
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// 验证属性的值是否有效
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="attr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ValidateAttribute(this IFilterPolicy policy, PolicyAttribute attr, string value)
        {
            if (attr == null) return false;
            //value = HtmlEntity.DeEntitize(value);

            ////验证是否在限定的值之内
            if (attr.AllowedValues != null)
            {
                foreach (string allowedValue in attr.AllowedValues)
                {
                    if (allowedValue != null && allowedValue.Equals(value, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            if (attr.AllowedRegExp != null)
            {
                //验证是否符合指定的正则表达式
                foreach (var regx in attr.AllowedRegExp)
                {
                    if (string.IsNullOrWhiteSpace(regx.Name) && string.IsNullOrWhiteSpace(regx.Value)) continue;
                    string[] arr = new string[] { regx.Value, string.IsNullOrWhiteSpace(regx.Name) ? null : policy.RegularExpression(regx.Name) };
                    foreach (var item in arr)
                    {
                        if (string.IsNullOrWhiteSpace(item)) continue;
                        string pattern = item.Trim();
                        if (!pattern.StartsWith("^")) { pattern = "^" + pattern; }
                        if (!pattern.EndsWith("$")) { pattern = pattern + "$"; }
                        if (Regex.IsMatch(value, pattern))
                        {
                            return true;
                        }

                    }
                   
                }
            }
            return false;
        }
        static T Get<T>(IDictionary<string, T> collection, string key)where T:class {
            return key == null || collection == null || !collection.ContainsKey(key = key.ToLower()) ? null : collection[key];
        }
        /// <summary>
        /// 获取标签策略
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static PolicyHtmlTag Tag(this IFilterPolicy policy, string tagName)
        {
            return Get(policy?.TagRules, tagName);
        }
        /// <summary>
        /// 获取样式属性过滤策略
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PolicyCssProperty CssProperty(this IFilterPolicy policy, string name)
        {
            return Get(policy?.CssRules, name);
        }
        /// <summary>
        /// 获取通用属性过滤策略
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PolicyHtmlAttribute CommonHtmlAttribute(this IFilterPolicy policy, string name)
        {
            return Get(policy?.CommonAttributes, name);
        }
        /// <summary>
        /// 获取全局属性过滤策略
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PolicyHtmlAttribute GlobalHtmlAttribute(this IFilterPolicy policy, string name)
        {
            return Get(policy?.GlobalAttributes, name); 
        }
        /// <summary>
        /// 获取通用正则表达式
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string RegularExpression(this IFilterPolicy policy, string name)
        {
            return Get(policy?.CommonRegularExpressions, name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Directive(this IFilterPolicy policy, string name)
        {
            return Get(policy?.Directives, name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="policy"></param>
        /// <param name="name"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        public static T Directive<T>(this IFilterPolicy policy, string name,T @default=default(T)) where T : struct
        {
            string v = Directive(policy,name);
            if (string.IsNullOrWhiteSpace(v)) return @default;
            Type t = typeof(T);
            try
            {
                if (t.IsEnum)
                {
                    return (T)Enum.Parse(t, v);
                }
                else
                {
                    return (T)Convert.ChangeType(v, t);
                }
            }
            catch
            {
                if (t == typeof(Guid))
                {
                    object o = new Guid(v);
                    return (T)o;
                }
            }
            return @default;
        }
        /// <summary>
        /// 获取属性过滤策略
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="name"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static PolicyHtmlAttribute AllowedAttribute(this IFilterPolicy policy,string name, PolicyHtmlTag tag)
        {
            var tagAttr = tag.AllowedAttributes.ContainsKey(name) ? tag.AllowedAttributes[name] : null;
            var globalAttr = policy.GlobalHtmlAttribute(name);
            if (tagAttr == null && globalAttr == null) return null;
            var commonAttr = CommonHtmlAttribute(policy, name);
            return new PolicyHtmlAttribute(name)
            {
                OnInvalid = tagAttr?.OnInvalid ?? globalAttr.OnInvalid,
                AllowedValues = tagAttr?.AllowedValues ?? globalAttr?.AllowedValues ?? commonAttr?.AllowedValues,
                AllowedRegExp = tagAttr?.AllowedRegExp ?? globalAttr?.AllowedRegExp ?? commonAttr?.AllowedRegExp
            };
        }
    }
}
