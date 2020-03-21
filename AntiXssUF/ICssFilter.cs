namespace Ufangx.Xss
{
    /// <summary>
    /// Css过滤器
    /// </summary>
    public interface ICssFilter
    {
        /// <summary>
        /// 过滤css
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        string Filters(string code);
    }
}