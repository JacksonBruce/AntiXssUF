using System;
using System.Collections.Generic;
using System.Text;

namespace Ufangx.Xss
{
    /// <summary>
    /// 过滤正则表达式
    /// </summary>
    [Serializable]
    public class FilterRegExp
    {
        /// <summary>
        /// 表达式名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 正则表达式
        /// </summary>
        public string Value { get; set; }
    }
    /// <summary>
    /// 正则表达式比较器
    /// </summary>
    public class FilterRegExpComparer : IEqualityComparer<FilterRegExp>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(FilterRegExp x, FilterRegExp y)
        {
            if (ReferenceEquals(x, y)||x==null&&y==null) return true;
            if (x == null || y == null || x.GetType() != y.GetType()) return false;
            return
                string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Value, y.Value, StringComparison.OrdinalIgnoreCase);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(FilterRegExp obj)
        {
            return (obj.Name ?? string.Empty).GetHashCode() ^ (obj.Value ?? string.Empty).GetHashCode();
        }
    }
}
