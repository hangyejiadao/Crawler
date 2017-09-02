using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tool
{
    public class Crawler
    {
       private static Logger logger=new Logger(typeof(Crawler));
        #region 同步

        public static string CrawlHtml(string url, Encoding encode)
        {
            string html = string.Empty;
            try
            {
                HttpWebRequest request=HttpWebRequest.Create(url)as HttpWebRequest;
                ;
                request.Timeout = 30 * 1000;//设置30秒的超时
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
                request.ContentType = "text/html; charset=utf-8";// "text/html;charset=gbk";// 
                using (HttpWebResponse response=request.GetResponse()as HttpWebResponse)
                {
                    if (response.StatusCode!=HttpStatusCode.OK)
                    {
                        logger.Error(string.Format("抓取{0}地址返回失败,response.StatusCode为{1}",url,response.StatusCode),null );
                    }
                    else
                    {
                        try
                        {
                            StreamReader sr=new StreamReader(response.GetResponseStream());
                            html = sr.ReadToEnd();
                            sr.Close();
                        }
                        catch (Exception e)
                        {
                           logger.Error(string.Format($"DownloadHtml抓取{url}地址失败"),e);
                            html = null;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                logger.Error("Error",e);
            }
            return html;
        }


        #endregion

        #region 异步

        

        #endregion
    }
}
