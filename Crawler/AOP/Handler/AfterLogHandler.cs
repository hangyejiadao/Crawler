using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Crawler.AOP.Handler
{
    public class AfterLogHandler:ICallHandler
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            IMethodReturn methodReturn = getNext()(input, getNext);
            StringBuilder sb=new StringBuilder();
            foreach (var para in input.Inputs)
            {
                sb.AppendFormat("Param={0}", para == null ? "null" : para.ToString());
            }
            Console.WriteLine("日志已记录,结束请求{0}",sb.ToString());
            return methodReturn;
        }

        public int Order { get; set; }
    }
}
