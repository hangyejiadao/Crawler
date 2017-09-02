using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dll;
using HtmlAgilityPack;
using IBLL;
using Model;
using Tool;

namespace Bll
{
  public  class CategoryCrawl:ICrawler
    {
        private static Logger logger = new Logger(typeof(CategoryCrawl));
        private int _Count = 1;//每次都得new一个 重新初始化类别
        private CategoryDll dll=new CategoryDll();
        public void Crawler()
        {
            List<Category> categoryList = new List<Category>();
            try
            {
                string html = Tool.Crawler.CrawlHtml(Constant.JDCategoryUrl,Constant.encoding);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                string fristPath = "//*[@class='category-item m']";
                HtmlNodeCollection nodeList = doc.DocumentNode.SelectNodes(fristPath);
                int k = 1;
                foreach (HtmlNode node in nodeList)
                {
                    categoryList.AddRange(this.First(node.InnerHtml, k++.ToString("00") + "f", "root"));
                }

                CategoryDll categoryRepository = new CategoryDll();
                categoryRepository.SaveList(categoryList);
            }
            catch (Exception ex)
            {
                logger.Error("CrawlerMuti出现异常", ex);
            }
            finally
            {
                Console.WriteLine($"类型数据初始化完成，共抓取类别{ categoryList?.Count}个");
            }
        }

        /// <summary>
        /// 对每一个一级类进行查找
        /// </summary>
        /// <param name="html"></param>
        /// <param name="code"></param>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        private List<Category> First(string html, string code, string parentCode)
        {
            List<Category> categoryList = new List<Category>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            string path = "//*[@class='mt']/h2/span";
            HtmlNodeCollection nodeList = doc.DocumentNode.SelectNodes(path);
            foreach (HtmlNode node in nodeList)
            {
                Category category = new Category()
                {
                    Id = _Count++,
                    State = 0,
                    CategoryLevel = 1,
                    Code = code,
                    ParentCode = parentCode
                };
                category.Name = node.InnerText;
                category.Url = "";// node.Attributes["href"].Value;
                categoryList.Add(category);
            }
            categoryList.AddRange(this.Second(html, code));
            return categoryList;
        }

        /// <summary>
        /// 在一个一级类下面的全部二级类进行查找
        /// </summary>
        /// <param name="html"></param>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        private List<Category> Second(string html, string parentCode)
        {
            List<Category> categoryList = new List<Category>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            string path = "//*[@class='items']/dl";
            HtmlNodeCollection nodeList = doc.DocumentNode.SelectNodes(path);
            int k = 1;
            foreach (HtmlNode node in nodeList)
            {
                string code = string.Format("{0}{1}s", parentCode, k.ToString("00"));
                string secondHtml = node.InnerHtml;
                if (string.IsNullOrWhiteSpace(secondHtml)) continue;
                HtmlDocument secondDoc = new HtmlDocument();
                secondDoc.LoadHtml(secondHtml);
                Category category = new Category()
                {
                    Id = _Count++,
                    State = 0,
                    CategoryLevel = 2,
                    Code = code,
                    ParentCode = parentCode
                };


                HtmlNode secondNode = secondDoc.DocumentNode.SelectSingleNode("//dt/a");
                if (secondNode == null)//图书音像
                {
                    secondNode = secondDoc.DocumentNode.SelectSingleNode("//dt");
                }
                category.Name = secondNode.InnerText;
                if (secondNode.Attributes["href"] != null)
                {
                    category.Url = secondNode.Attributes["href"].Value;
                    if (!category.Url.StartsWith("http:"))
                    {
                        category.Url = string.Concat("http:", category.Url);
                    }
                }
                categoryList.Add(category);
                HtmlNode thirdNode = secondDoc.DocumentNode.SelectSingleNode("//dd");
                if (thirdNode == null) continue;
                categoryList.AddRange(this.Third(thirdNode.InnerHtml, code));
                k++;
            }
            return categoryList;
        }

        /// <summary>
        /// 在一个二级类下的全部三级类里面进行查找
        /// </summary>
        /// <param name="html"></param>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        private List<Category> Third(string html, string parentCode)
        {
            List<Category> categoryList = new List<Category>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            string path = "//a";
            HtmlNodeCollection nodeList = doc.DocumentNode.SelectNodes(path);
            if (nodeList == null || nodeList.Count == 0) return categoryList;
            int k = 1;
            foreach (HtmlNode node in nodeList)
            {
                string code = string.Format("{0}{1}t", parentCode, k.ToString("00"));
                Category category = new Category()
                {
                    Id = _Count++,
                    State = 0,
                    CategoryLevel = 3,
                    Code = code,
                    ParentCode = parentCode
                };
                category.Name = node.InnerText;
                category.Url = node.Attributes["href"].Value;
                if (!category.Url.StartsWith("http:"))
                {
                    category.Url = string.Concat("http:", category.Url);
                }
                categoryList.Add(category);
                k++;
            }
            return categoryList;
        }

        public List<Category> GetLevel(int Level)
        {
           return dll.QueryListByLevel(Level);
        }
     
    }
}
