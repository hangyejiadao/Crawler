using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bll;
using IBLL;
using Tool;

namespace Crawler
{
    public   class IndexBuilder
    {
        private static Logger logger=new Logger(typeof(IndexBuilder));
        private static List<string> PathSuffixList=new List<string>();
        private static CancellationTokenSource CTS = null;

        public static void BuildIndex(CancellationTokenSource cst = null)
        {
            try
            {
                Console.WriteLine("建立索引开始");
                List<Task> taskList=new List<Task>();
                TaskFactory taskFactory=new TaskFactory();
                CTS = cst == null ? new CancellationTokenSource() : cst;
                //直接循环线程数  每个线程数都是从第一张表开始  1000条每次
                //然后每次间隔n*1000来获取 如果获取的数据小于1000 直接去下一张表
                for (int i = 1; i < 31; i++)
                {
                    IndexBuilderPerThread thread=new IndexBuilderPerThread(i,i.ToString("000"),CTS);
                    PathSuffixList.Add(i.ToString("000"));
                    taskList.Add(taskFactory.StartNew(thread.Process));
                }
                taskList.Add(taskFactory.ContinueWhenAll(taskList.ToArray(),MergeIndex));
                Task.WaitAll(taskList.ToArray());
                Console.WriteLine("BuildIndex{0}", CTS.IsCancellationRequested ? "失败" : "成功");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void MergeIndex(Task[] taskList)
        {
            try
            {
                if (CTS.IsCancellationRequested)
                {
                    return;
                }
                ILuceneBuild builder=new LuceneBuild();
                builder.MergeIndex(PathSuffixList.ToArray());
            }
            catch (Exception e)
            {
                CTS.Cancel();
                logger.Error("MergeIndex出现异常",e);
            }
        }
    }
}
