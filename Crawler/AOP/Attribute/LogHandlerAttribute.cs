using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crawler.AOP.Handler;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Crawler.AOP.Attribute
{
    public class LogHandlerAttribute:HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        { 
            return  new LogHandler()
            {
                Order = this.Order
            };
        }
    }
}
