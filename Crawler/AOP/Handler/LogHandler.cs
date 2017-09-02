﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Crawler.AOP.Handler
{
    public class LogHandler : ICallHandler

    {
        public int Order
        {
            get;
            set;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            StringBuilder sb=new StringBuilder();
            foreach (var para in input.Inputs)
            {
                sb.AppendFormat("Para={0} ", para == null ? "null" : para.ToString());
            }
            Console.WriteLine("日志已记录，开始请求{0}", sb.ToString());
            return getNext()(input, getNext);
        }
    }
}
