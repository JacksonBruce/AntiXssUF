namespace Ufangx.Xss
{
    /// <summary>
    /// html过滤器
    /// </summary>
    public interface IHtmlFilter
    {
        /// <summary>
        /// 过滤html
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        string Filters(string html);
    }
}