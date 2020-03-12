using System.Collections.Generic;

namespace Ufangx.Xss
{
    public interface IFilterPolicy
    {
        
        string Name { get; }
        void Init(string config, string name);
        bool Initialized { get; }
        Dictionary<string, string> CommonRegularExpressions { get; }
        Dictionary<string, string> Directives { get; }
        Dictionary<string, PolicyHtmlAttribute> CommonAttributes { get; }
        Dictionary<string, PolicyHtmlAttribute> GlobalAttributes { get; }
        Dictionary<string, PolicyHtmlTag> TagRules { get; }
        Dictionary<string, PolicyCssProperty> CssRules { get; }
    }
}