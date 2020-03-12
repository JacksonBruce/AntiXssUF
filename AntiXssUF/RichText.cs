using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
namespace Ufangx.Xss
{
    public class RichText
    {
        #region 构造
        /// <summary>
        /// 实例化一个富文本对象
        /// </summary>
        /// <param name="text">未被过滤的源文本</param>
        /// <param name="policy">过滤的安全策略，如果不提供将启用默认的安全策略</param>
        public RichText(string source, IHtmlFilter htmlFilter)
        {
            this.source = source;
            this.htmlFilter = htmlFilter ?? throw new ArgumentNullException(nameof(htmlFilter));
        }
        #endregion

        string source;
        private readonly IHtmlFilter htmlFilter;

        string html;

        public string Source => source;

        public override string ToString()
        {
            if (html == null)
            {
                html = htmlFilter.Filters(source) ?? string.Empty;
            }
            return html;
        }

        #region 重载操作符
        /// <summary>
        /// 字符串隐式转换为富文本对象
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static implicit operator RichText(string text)
        {
            var factory = XssFilterBuilder.Builder?.ServiceProvider?.GetService<IFilterPolicyFactory>();
            return new RichText(text, factory == null ? new HtmlFilter(XssFilterBuilder.GetPresupposedPolicy()) : factory.CreateHtmlFilter().Result);
        }
        /// <summary>
        /// 富文本对象隐式转换为字符串
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static implicit operator string(RichText text)
        {
            return text == null ? null : text.ToString();
        }
        /// <summary>
        /// 重载富文本对象与字符串相加，并返回富文本对象
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static RichText operator +(RichText a,string b)
        {
            if (!string.IsNullOrWhiteSpace(b))
            {
                return new RichText(string.Concat(a.source, b), a.htmlFilter)  ;
            }
            return new RichText(a.source, a.htmlFilter) { html = a.html };
        }
        /// <summary>
        /// 重载字符串与富文本对象相加，并返回富文本对象
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static RichText operator +(string a, RichText b)
        {
            if (!string.IsNullOrWhiteSpace(a))
            {
                return new RichText(string.Concat(a, b.source), b.htmlFilter);
            }
            return new RichText(b.source, b.htmlFilter) { html = b.html };
        }
        /// <summary>
        /// 重载富文本对象与富文本对象相加，并返回富文本对象
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static RichText operator +(RichText a, RichText b)
        {
            if (a == null && b == null) return null;
            if (a == null || b == null) {
                var s = a ?? b;
                return new RichText(s.source, s.htmlFilter) { html = s.html };
            }
            return new RichText(string.Concat(a.source, b.source), a.htmlFilter);
        }
        ///这个是强制转换操作符的重载，在这里不需要了，因为有隐式转换了
        //public static explicit operator string(RichText text)
        //{
        //    return text == null ? null : text.ToString();
        //}
        #endregion
    }
}
