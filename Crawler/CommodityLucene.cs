using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Bll;
using IBLL;
using Model;
using Tool;

namespace Crawler
{
    public class CommodityLucene
    {
        private static Logger logger = new Logger(typeof(CommodityLucene));

        #region QueryCommodity

        public static List<Commodity> QueryCommodity(int pageIndex, int pageSize,
            out int totalCount, string keyword,
            List<int> categoryIdList, string priceFilter,
            string priceOrderBy)
        {
            totalCount = 0;
            try
            {
                if (string.IsNullOrWhiteSpace(keyword) && (categoryIdList == null || categoryIdList.Count == 0))
                {
                    return null;
                }
                ILuceneQuery luceneQuery = new LuceneQuery();
                string queryString = string.Format("{0}{1}", string.IsNullOrWhiteSpace(keyword) ? "" : string.Format("+{0}",
                    AnalyzerKeyword(keyword)), categoryIdList == null || categoryIdList.Count == 0 ? "" : string.Format("+{0}", AnalyzeerCategory(categoryIdList)));
                return luceneQuery.QueryIndexPage(queryString, pageIndex, pageSize, out totalCount, priceFilter, priceOrderBy);
            }
            catch (Exception e)
            {
                logger.Error("Error:", e);
                return null;
            }
        }


        /// <summary>
        /// 为keywordd做盘古分词
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        private static string AnalyzerKeyword(string keyword)
        {
            StringBuilder queryStringBuilder = new StringBuilder();
            ILuceneAnalyze analyzer = new LuceneAnalyze();
            string[] words = analyzer.AnalyzerKey(keyword);
            foreach (var word in words)
            {
                queryStringBuilder.AppendFormat("{0}:{1}", "title", word);
            }
            string result = queryStringBuilder.ToString().TrimEnd();
            return result;
        }


        private static string AnalyzeerCategory(List<int> categoryIdList)
        {
            return string.Join("", categoryIdList.Select(c => string.Format("{0}:{1}", "categoryid", c)));
        }

        #endregion
        


    }
}
