using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ufangx.Xss;

namespace AntiXssUF.TestSite.Controllers
{
    public class TestModel
    {
       public string Name { get; set; }
        [XssSchemeName("ebay")]
        public RichText RichText { get; set; }
    }
}
