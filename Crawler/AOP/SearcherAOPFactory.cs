using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Crawler.AOP
{
    public class SearcherAOPFactory
    {
        private static IUnityContainer container = null;

        static SearcherAOPFactory()
        {
            container=new UnityContainer();
            container.RegisterType<ISearcherAOP, SearcherAOP>();//声明UnityContainer并注册IUserProcessor
            container.AddNewExtension<Interception>().Configure<Interception>()
                .SetInterceptorFor<ISearcherAOP>(new InterfaceInterceptor());
        }

        public static ISearcherAOP CreateInstance()
        {
            return container.Resolve<ISearcherAOP>();
        }
    }
}
