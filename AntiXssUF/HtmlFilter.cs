using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.DependencyInjection;
namespace Ufangx.Xss
{
    /// <summary>
    /// html过滤器
    /// </summary>
    public class HtmlFilter : IHtmlFilter
    {
        #region 构造
        /// <summary>
        /// 创建html过滤器
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="cssFilter"></param>
        public HtmlFilter(IFilterPolicy policy, ICssFilter cssFilter)
        {
            Policy = policy ?? throw new ArgumentNullException(nameof(policy));
            _cssFilter = cssFilter;
        }
        /// <summary>
        /// 创建html过滤器
        /// </summary>
        /// <param name="policy"></param>
        public HtmlFilter(IFilterPolicy policy):this(policy,null)
        { }
        #endregion

        #region 属性
        /// <summary>
        /// 当前过滤策略
        /// </summary>
        protected virtual IFilterPolicy Policy { get; }
        ICssFilter _cssFilter;
        /// <summary>
        /// 当前css过滤器
        /// </summary>
        protected virtual ICssFilter CssFilter {
            get {
                if (_cssFilter == null) {
                    var factory = XssFilterBuilder.Builder?.ServiceProvider?.GetService<IFilterPolicyFactory>();
                    _cssFilter = factory?.CreateCssFilter(Policy)?.Result ?? new CssFilter(Policy);
                }
                return _cssFilter;
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 过滤html
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public virtual string Filters(string html)
        {
            if (html == null || html.Length == 0)
            {
                return string.Empty;
            }
            html = StripNonValidXMLCharacters(html.Trim());
            int maxInputSize = Policy.Directive<int>("maxInputSize", int.MaxValue);

            //ensure our input is less than the max
            if (maxInputSize > 0 && maxInputSize < html.Length)
            {
                return string.Empty;
            }
            var htmlParser = new HtmlParser(new HtmlParserOptions()
            {
                IsScripting = false,
                IsNotSupportingFrames = true,
                IsKeepingSourceReferences = true,
                IsSupportingProcessingInstructions = true,
                IsEmbedded = true
            });
            Regex regex = new Regex($"^({Regex.Escape("<!doctype")})|({Regex.Escape("<html")})|({Regex.Escape("<body")})", RegexOptions.IgnoreCase);
            var match = regex.Match(html);
            var doc = match.Success ? htmlParser.ParseDocument(html) : htmlParser.ParseDocument("<html><head></head><body></body></html>");
            var container = match.Success ? doc.DocumentElement : doc.Body;
            if (!match.Success) { container.InnerHtml = html; }
            FiltersTags(container.ChildNodes);
            return container.HasChildNodes ? (match.Success ? (match.Groups[3].Success ? doc.Body?.OuterHtml : container.OuterHtml) : container.InnerHtml) : string.Empty;
        }
        #endregion

        #region 帮助方法
        /// <summary>
        /// 过滤标签集合
        /// </summary>
        /// <param name="nodes"></param>
        protected virtual void FiltersTags(INodeList nodes)
        {
            if (nodes == null) return;
            for (int i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];
                if (node is IElement element)
                {
                    FiltesTag(element);
                }
                else {
                    if (node.NodeType == NodeType.Comment) {
                        node.RemoveFromParent();
                    }
                }
                if (node.Parent == null)
                {
                    i--;
                }
            }
        }
        /// <summary>
        /// 过滤指定编辑器的属性和子元素
        /// </summary>
        /// <param name="node"></param>
        protected virtual void FiltesTag(IElement node)
        {
            string nodeName = node.LocalName.ToLower();
            //if (nodeName.Equals("#text")) return;
            //IElement element = node as IElement;
            var tag = Policy.Tag(nodeName);
            PolicyHtmlTagAction actoin = tag == null ? PolicyHtmlTagAction.Filter : tag.Action;
            switch (actoin)
            {
                case PolicyHtmlTagAction.Filter:
                    //删除当前节点，但保留其有效的子节点
                    PromoteChildren(node);
                    return;
                case PolicyHtmlTagAction.Validate:
                    //过滤当前元素的属性与及子节点
                    ValidateAction(node, nodeName, tag);
                    return;
                case PolicyHtmlTagAction.Truncate:
                    //删除当前节点的所有属性以及子节点，但保留文本和备注节点。
                    TruncateAction(node);
                    return;
                default:
                    //将当前节点从父节点中删除。
                    var parentNode = node.Parent;
                    parentNode.RemoveChild(node);
                    break;
            }

        }
        /// <summary>
        /// 验证html标签
        /// </summary>
        /// <param name="node"></param>
        /// <param name="tagName"></param>
        /// <param name="tag"></param>
        protected virtual void ValidateAction(IElement node, string tagName, PolicyHtmlTag tag)
        {
            var parentNode = node.Parent;
            #region 过滤样式
            if ("style".Equals(tagName, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    node.InnerHtml = CssFilter.Filters(node.InnerHtml);
                }
                catch
                {
                    parentNode.RemoveChild(node);
                }
            }
            #endregion

            #region 过滤属性
            for (int currentAttributeIndex = 0; currentAttributeIndex < node.Attributes.Length; currentAttributeIndex++)
            {
                var attribute = node.Attributes[currentAttributeIndex];
                string name = attribute.Name, _value = attribute.Value;
                var attr = Policy.AllowedAttribute(name, tag);

                #region 如果是白名单之外的属性移除掉
                if (attr == null)
                {
                    node.RemoveAttribute(name);
                    currentAttributeIndex--;
                    continue;
                }
                #endregion
                #region 元素内嵌样式
                if ("style".Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        attribute.Value = CssFilter.Filters(_value);
                        if (string.IsNullOrWhiteSpace(attribute.Value)) {
                            node.RemoveAttribute(name);
                            currentAttributeIndex--;
                        }
                    }
                    catch
                    {
                        node.RemoveAttribute(name);
                        currentAttributeIndex--;
                    }
                    continue;
                }
                #endregion
                //如果未能通过验证，将执行指定的操作
                if (!Policy.ValidateAttribute(attr, _value))
                {
                    switch (attr.OnInvalid)
                    {
                        case PolicyHtmlAttributeOnInvalid.RemoveTag:
                            //删除当前的元素并退出函数
                            parentNode.RemoveChild(node);
                            return;
                        case PolicyHtmlAttributeOnInvalid.FilterTag:
                            //删除当前节点，但保留其有效的子节点
                            PromoteChildren(node);
                            return;
                        default:
                            //删除当前的属性，指针往回调
                            node.RemoveAttribute(attr.Name);
                            currentAttributeIndex--;
                            break;
                    }

                }
            }
            #endregion
            //过滤当前元素的子节点
            FiltersTags(node.ChildNodes);

        }
        //void FilterAction(INode node)
        //{
        //    FiltersTags(node.ChildNodes);
        //}

        /// <summary>
        /// 删除所有的属性和子元素，但保留文本和备注节点
        /// </summary>
        /// <param name="node"></param>
        protected virtual void TruncateAction(IElement node)
        {
            var attrs = node.Attributes;
            while (attrs.Length > 0)
            {
                node.RemoveAttribute(attrs[0].Name);
            }
            var nodes = node.ChildNodes;
            int position = 0;
            while (nodes.Length > position)
            {
                var nodeToRemove = nodes[position];
                var type = nodeToRemove.NodeType;
                if (type == NodeType.Text || type == NodeType.Comment || type == NodeType.Entity) { position++; continue; }
                node.RemoveChild(nodeToRemove);
            }
        }


        /// <summary>
        /// 去除无效的XML字符
        /// </summary>
        /// <param name="in_Renamed"></param>
        /// <returns></returns>
        protected virtual string StripNonValidXMLCharacters(string in_Renamed)
        {
            StringBuilder out_Renamed = new StringBuilder();
            char current;
            if (in_Renamed == null || ("".Equals(in_Renamed)))
                return "";
            for (int i = 0; i < in_Renamed.Length; i++)
            {
                current = in_Renamed[i];
                if ((current == 0x9) || (current == 0xA) || (current == 0xD) || ((current >= 0x20) && (current <= 0xD7FF)) || ((current >= 0xE000) && (current <= 0xFFFD)))
                    out_Renamed.Append(current);
            }
            return out_Renamed.ToString();
        }
        /// <summary>
        /// 将指定节点从父节点中移除，但其子节点保留
        /// </summary>
        /// <param name="node"></param>
        protected virtual void PromoteChildren(IElement node)
        {
            //过滤子节点
            FiltersTags(node.ChildNodes);
            var nodeList = node.ChildNodes;
            var parent = node.Parent;
            //将它的所有子节点往上移到父节点的前面
            while (nodeList.Length > 0)
            {
                var removeNode = node.RemoveChild(nodeList[0]);
                parent.InsertBefore(removeNode, node);
            }
            //然后将节点删除
            parent.RemoveChild(node);
        }
        #endregion

    }
}
