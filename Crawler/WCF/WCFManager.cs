using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.WCF
{
   public  class WCFManager
    {
        private static List<ServiceHost> hostList = null;

        public static void Start()
        {
            hostList = new List<ServiceHost>()
            {
                new ServiceHost(typeof(Searcher))
            };
            foreach (ServiceHost host in hostList)
            {
               
                host.Opened += (sender, e) => Console.WriteLine("{0}已经启动！ 地址为:{1}", host.Description,host.BaseAddresses.ToString());
                host.Open();

            }
        }

        public static void Stop()
        {
            if (hostList != null)
                foreach (ServiceHost host in hostList)
                {
                    Console.WriteLine("{0}已经停止！", host.Description);
                    host.Abort();
                }
        }
    }
}
