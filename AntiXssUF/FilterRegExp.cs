using System;
using System.Collections.Generic;
using System.Text;

namespace Ufangx.Xss
{

    [Serializable]
    public class FilterRegExp
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class FilterRegExpComparer : IEqualityComparer<FilterRegExp>
    {
        public bool Equals(FilterRegExp x, FilterRegExp y)
        {
            if (ReferenceEquals(x, y)||x==null&&y==null) return true;
            if (x == null || y == null || x.GetType() != y.GetType()) return false;
            return
                string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Value, y.Value, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(FilterRegExp obj)
        {
            return (obj.Name ?? string.Empty).GetHashCode() ^ (obj.Value ?? string.Empty).GetHashCode();
        }
    }
}
