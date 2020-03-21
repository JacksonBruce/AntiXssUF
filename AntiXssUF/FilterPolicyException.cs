using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Ufangx.Xss
{
    /// <summary>
    /// 策略异常
    /// </summary>
    public class FilterPolicyException:Exception
    {      
        /// <summary>
        /// 创建策略异常
        /// </summary>
        public FilterPolicyException() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public FilterPolicyException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public FilterPolicyException(string message, Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public FilterPolicyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
