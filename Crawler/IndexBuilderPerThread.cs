using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dll;
using IBLL;
using Model;
using Tool;

namespace Bll
{
    public class IndexBuilderPerThread
    {
        private Logger logger = new Logger(typeof(IndexBuilderPerThread));
        private int CurrentThreadNum = 0;
        private string PathSuffix = "";
        private CancellationTokenSource CTS = null;

        public IndexBuilderPerThread(int threadNum, string pathSuffix, CancellationTokenSource tokenSource)
        {
            CurrentThreadNum = threadNum;
            PathSuffix = pathSuffix;
            CTS = tokenSource;
        }

        public void Process()
        {
            try
            {
                CommodityDll commodityDll = new CommodityDll();
                ILuceneBuild builder = new LuceneBuild();
                bool isFirst = true;
                int pageIndex = 1;
                while (!CTS.IsCancellationRequested)
                {
                    List<Commodity> commodityList = commodityDll.QueryList(CurrentThreadNum, pageIndex, 10000);
                    if (commodityList == null || commodityList.Count == 0)
                    {
                        break;
                    }
                    else
                    {
                        builder.BuildIndex(commodityList, PathSuffix, isFirst);
                        Console.WriteLine (string.Format("ThreadNum={0}完成{1}条的创建", CurrentThreadNum, 10000 * pageIndex++));
                        isFirst = false;
                    }
                }
            }
            catch (Exception e)
            {
                CTS.Cancel();
                logger.Error("Error", e);
            }
        }
    }
}
