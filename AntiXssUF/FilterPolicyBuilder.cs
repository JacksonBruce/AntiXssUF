using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    /// <summary>
    /// 策略生成器
    /// </summary>
    public class FilterPolicyBuilder
    {
        /// <summary>
        /// 创建策略生成器
        /// </summary>
        /// <param name="Name"></param>
        public FilterPolicyBuilder(string Name) {
            this.Name = Name;
        }
        /// <summary>
        /// 策略名称
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 策略类型
        /// </summary>
        public Type PolicyType { get; set; }
        /// <summary>
        /// 获取策略配置方法
        /// </summary>
        public Func<Task<string>> GetConfig { get; set; }

    }
}
