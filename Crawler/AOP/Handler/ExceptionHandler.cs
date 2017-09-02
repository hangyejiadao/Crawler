using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Crawler.AOP.Handler
{
    public class ExceptionHandler:ICallHandler
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            IMethodReturn methodReturn = getNext()(input, getNext);
            if (methodReturn.Exception == null)
            {
                Console.WriteLine("无异常");
            }
            else
            {
                Console.WriteLine("异常:{0}", methodReturn.Exception.Message);
            }
            return methodReturn;
        }

        public int Order { get; set; }
    }
}
