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
        private readonly IServiceCollection services;

        public Configures(IServiceCollection services)
        {
            this.services = services;
        }
        public void Options<TOptions>(Action<TOptions> configure,string name=null) where TOptions : class, new() {

            _Options(configure,name);
        }
        partial void _Options<TOptions>(Action<TOptions> configure,string name) where TOptions : class, new();
    }
}
