using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Ufangx.Xss
{
    public class FilterPolicyException:Exception
    {      
        public FilterPolicyException() { }
        public FilterPolicyException(string message) : base(message) { }
        public FilterPolicyException(string message, Exception innerException) : base(message, innerException) { }
        public FilterPolicyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
