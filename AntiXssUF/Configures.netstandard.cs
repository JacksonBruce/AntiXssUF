using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ufangx.Xss
{
    internal partial class Configures
    {
        partial void _Options<TOptions>(Action<TOptions> configure, string name) where TOptions : class, new()
        {
            if (configure == null) return;
            if (string.IsNullOrEmpty(name)) services.Configure(configure);
            else services.Configure(name, configure);
        }
    }
}
