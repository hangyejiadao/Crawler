using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dll;
using HtmlAgilityPack;
using IBLL;
using Model;
using Newtonsoft.Json;
using Tool;
using System.Configuration;

namespace Bll
{
    public class CommodityCrawl : ICrawler
    {
        private string pagenumxpath = ConfigurationManager.AppSettings["pagenumxpath"].ToString();
        private string lipath = ConfigurationManager.AppSettings["lipath"].ToString();
        private string urlPath = ConfigurationManager.AppSettings["urlpath"].ToString();
        private string titlePath = ConfigurationManager.AppSettings["titlePath"].ToString();
        private string imgpath = ConfigurationManager.AppSettings["imgpath"].ToString();
        private string pricepath = ConfigurationManager.AppSettings["pricepath"].ToString();
        private Logger logger = new Logger(typeof(CommodityCrawl));
        private CommodityDll dll = new CommodityDll();
        private Category category = null;
        private  TextLog txtLog=new TextLog();
        public CommodityCrawl(Category _category)
        {
            category = _category;
        }

        public void Crawler()
        {
            try
            {
                if (string.IsNullOrEmpty(category.Url))
                {
                    return;
                }
                string html = Tool.Crawler.CrawlHtml(category.Url, Constant.encoding);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);//加载html 

                HtmlNode pageNumberNode = doc.DocumentNode.SelectSingleNode(pagenumxpath);
                if (pageNumberNode != null)
                {
                    int pageNum = Convert.ToInt32(pageNumberNode.InnerText);
                    Parallel.For(1, pageNum, t =>
                    {
                        string pageUrl = string.Format("{0}&page={1}", category.Url, t);
                        List<Commodity> commodityList = GetCommodityList(category, pageUrl);
                        if (commodityList != null||commodityList.Count>0)
                        {
                            dll.SaveList(commodityList);
                            Console.WriteLine("插入了{0}条商品",commodityList.Count);
                        }

                    });
                }
                else
                {
                    Console.WriteLine("Url {0}为空", category.Url);
                    txtLog.Log(category.Id.ToString(),"Category");
                }

            }
            catch (Exception e)
            {
                logger.Error("Crawler Error:", e);
            }
        }

        private List<Commodity> GetCommodityList(Category category, string pageUrl)
        {
            string html = Tool.Crawler.CrawlHtml(pageUrl, Constant.encoding);
            List<Commodity> commodityList = new List<Commodity>();
            if (html != null)
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                HtmlNodeCollection nonenodeList = doc.DocumentNode.SelectNodes(lipath);
                if (nonenodeList == null||nonenodeList.Count==0)
                {
                    return commodityList;
                }
                foreach (var node in nonenodeList)
                {
                    HtmlDocument docChild=new HtmlDocument();
                    docChild.LoadHtml(node.OuterHtml);

                    Commodity commodity = new Commodity()
                    {
                        CategoryId = category.Id
                    };
                    HtmlNode urlNode = docChild.DocumentNode.SelectSingleNode(urlPath);
                    if (urlNode==null)
                    {
                        continue; 
                    }

                    commodity.Url = urlNode.Attributes["href"].Value;
                    if (!commodity.Url.StartsWith("http:"))
                    {
                        commodity.Url = "http:" + commodity.Url;
                    }
                    string sId = Path.GetFileName(commodity.Url).Replace(".html", "");
                    commodity.ProductId = long.Parse(sId);

                    HtmlNode titleNode = docChild.DocumentNode.SelectSingleNode(titlePath);
                    if (titleNode==null)
                    {
                        continue;
                    }

                    commodity.Title = titleNode.InnerText;

                    HtmlNode imageNode = docChild.DocumentNode.SelectSingleNode(imgpath);
                    if (imageNode==null)
                    {
                       continue;
                    }
                    //前后不一
                    if (imageNode.Attributes.Contains("src"))
                    {
                        commodity.ImageUrl = imageNode.Attributes["src"].ToString();
                    }
                    else if (imageNode.Attributes.Contains("original"))
                        commodity.ImageUrl = imageNode.Attributes["original"].Value;
                    else if (imageNode.Attributes.Contains("data-lazy-img"))
                        commodity.ImageUrl = imageNode.Attributes["data-lazy-img"].Value;
                    else
                    {
                        continue;
                    }
                    if (!commodity.ImageUrl.StartsWith("http:"))
                    {
                        commodity.ImageUrl = "http:" + commodity.ImageUrl;
                    }
                    HtmlNode priceNode = docChild.DocumentNode.SelectSingleNode(pricepath);
                    if (priceNode==null)
                    {
                        continue;    
                    } 
                    commodityList.Add(commodity); 
                }
              
            }
            return GetCommodityPrice(category, commodityList);
        }

        private List<Commodity> GetCommodityPrice(Category category, List<Commodity> commodityList)
        {
            try
            {
                if (commodityList == null || commodityList.Count() == 0)
                    return commodityList; 
                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("http://p.3.cn/prices/mgets?callback=jQuery1069298&type=1&area=1_72_4137_0&skuIds={0}&pdbp=0&pdtk=&pdpin=&pduid=1945966343&_=1469022843655", string.Join("%2C", commodityList.Select(c => string.Format("J_{0}", c.ProductId))));
                string html = Tool.Crawler.CrawlHtml(sb.ToString(),Constant.encoding); 
                if (html!=null&&!html.Contains("error"))
                {
                    html = html.Substring(html.IndexOf("(") + 1);
                    html = html.Substring(0, html.LastIndexOf(")"));
                    List<CommodityPrice> priceList = JsonConvert.DeserializeObject<List<CommodityPrice>>(html);
                    commodityList.ForEach(c => c.Price = priceList.FirstOrDefault(p => p.Id.Equals(string.Format("J_{0}", c.ProductId))).p);
                } 
            }
            catch (Exception ex)
            {
                logger.Error("GetCommodityPrice出现异常", ex);
            }
            return commodityList;
        }
    }
}