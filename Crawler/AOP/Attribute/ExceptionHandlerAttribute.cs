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
    public class ExceptionHandlerAttribute:HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            
            return new ExceptionHandler()
            {
                Order = this.Order
            };
        }
    }
}
