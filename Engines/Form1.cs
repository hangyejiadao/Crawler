using Dll;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bll;
using IBLL;
using Model;
using Tool;

namespace Engines
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private static int ThreadCount = 0;
        private static Logger logger = new Logger(typeof(Form1));
        private static AutoResetEvent are = new AutoResetEvent(false);

        static CancellationTokenSource tokenSource = new CancellationTokenSource();
        private static CancellationToken token = tokenSource.Token;
        private bool isPause = false;
        private ICrawler categorycraw = new CategoryCrawl();
        private ICrawler commCrawler;
        private void button1_Click(object sender, EventArgs e)
        {
            DBInit.InitCategoryTable();
            DBInit.InitCommodityTable();
            button1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Task task = new Task(() =>
                {
                    CrawlerCategory();

                });
                Task task2 = new Task(() =>
                {
                    CrawlerCommodity();
                });
                task.ContinueWith(new Action<Task>(t =>
                {
                    task2.Start();
                }));
                task.Start();

            }
            catch (Exception exception)
            {
                logger.Error("Error:", exception);
            }
            button2.Enabled = false;
        }

        private void CrawlerCategory()
        {

            categorycraw.Crawler();

        }

        private void CrawlerCommodity()
        {
            Console.WriteLine($"{ DateTime.Now} jd商品开始抓取 - -");
            CategoryDll dll = new CategoryDll();
            List<Category> categoryList = dll.QueryListByLevel(3);

            List<Task> taskList = new List<Task>();
            TaskFactory taskFactory = new TaskFactory();
            foreach (var cate in categoryList)
            {
                if (isPause)
                {
                    are.WaitOne();
                }
                commCrawler = new CommodityCrawl(cate);
                taskList.Add(taskFactory.StartNew(commCrawler.Crawler));
                if (taskList.Count > 50)
                {
                    taskList = taskList.Where(t => !t.IsCompleted && !t.IsCanceled && !t.IsFaulted).ToList();
                    Task.WaitAny(taskList.ToArray());
                }
            }
            Task.WaitAll(taskList.ToArray());
            Console.WriteLine($"{ DateTime.Now} jd商品抓取全部完成 - -");

            CleanAll();
            button1.Enabled = true;
        }

        private static void CleanAll()
        {
            try
            {
                Console.WriteLine($"{ DateTime.Now} 开始清理重复数据 - -");
                StringBuilder sb = new StringBuilder();
                for (int i = 1; i < 31; i++)
                {
                    sb.AppendFormat(@"DELETE FROM [dbo].[JD_Commodity_{0}] where productid IN(select productid from [dbo].[JD_Commodity_{0}] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_{0}] group by productid,CategoryId having count(0)>1);", i.ToString("000"));
                }
                #region
                /*
                 DELETE FROM [dbo].[JD_Commodity_001] where productid IN(select productid from [dbo].[JD_Commodity_001] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_001] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_002] where productid IN(select productid from [dbo].[JD_Commodity_002] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_002] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_003] where productid IN(select productid from [dbo].[JD_Commodity_003] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_003] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_004] where productid IN(select productid from [dbo].[JD_Commodity_004] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_004] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_005] where productid IN(select productid from [dbo].[JD_Commodity_005] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_005] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_006] where productid IN(select productid from [dbo].[JD_Commodity_006] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_006] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_007] where productid IN(select productid from [dbo].[JD_Commodity_007] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as IDv from [dbo].[JD_Commodity_007] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_008] where productid IN(select productid from [dbo].[JD_Commodity_008] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_008] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_009] where productid IN(select productid from [dbo].[JD_Commodity_009] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_009] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_010] where productid IN(select productid from [dbo].[JD_Commodity_010] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_010] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_011] where productid IN(select productid from [dbo].[JD_Commodity_011] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_011] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_012] where productid IN(select productid from [dbo].[JD_Commodity_012] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_012] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_013] where productid IN(select productid from [dbo].[JD_Commodity_013] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_013] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_014] where productid IN(select productid from [dbo].[JD_Commodity_014] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_014] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_015] where productid IN(select productid from [dbo].[JD_Commodity_015] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_015] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_016] where productid IN(select productid from [dbo].[JD_Commodity_016] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_016] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_017] where productid IN(select productid from [dbo].[JD_Commodity_017] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_017] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_018] where productid IN(select productid from [dbo].[JD_Commodity_018] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_018] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_019] where productid IN(select productid from [dbo].[JD_Commodity_019] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_019] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_020] where productid IN(select productid from [dbo].[JD_Commodity_020] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_020] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_021] where productid IN(select productid from [dbo].[JD_Commodity_021] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_021] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_022] where productid IN(select productid from [dbo].[JD_Commodity_022] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_022] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_023] where productid IN(select productid from [dbo].[JD_Commodity_023] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_023] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_024] where productid IN(select productid from [dbo].[JD_Commodity_024] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_024] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_025] where productid IN(select productid from [dbo].[JD_Commodity_025] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_025] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_026] where productid IN(select productid from [dbo].[JD_Commodity_026] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_026] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_027] where productid IN(select productid from [dbo].[JD_Commodity_027] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_027] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_028] where productid IN(select productid from [dbo].[JD_Commodity_028] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_028] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_029] where productid IN(select productid from [dbo].[JD_Commodity_029] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_029] group by productid,CategoryId having count(0)>1);DELETE FROM [dbo].[JD_Commodity_030] where productid IN(select productid from [dbo].[JD_Commodity_030] group by productid,CategoryId having count(0)>1)
                                AND ID NOT IN(select max(ID) as ID from [dbo].[JD_Commodity_030] group by productid,CategoryId having count(0)>1);
                 */
                #endregion
                Console.WriteLine("执行清理sql:{0}", sb.ToString());
                SqlHelper.ExecuteNonQuery(sb.ToString());
                Console.WriteLine("{0} 完成清理重复数据 - -", DateTime.Now);
            }
            catch (Exception ex)
            {
                logger.Error("CleanAll出现异常", ex);
            }
            finally
            {
                Console.WriteLine("{0} 结束清理重复数据 - -", DateTime.Now);
            }
        }
    }
}
