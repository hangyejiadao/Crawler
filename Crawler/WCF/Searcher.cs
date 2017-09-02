using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crawler.AOP;

namespace Crawler.WCF
{
  public  class Searcher:ISearcher
    {
        public string QueryCommodityPage(int pageIndex, int pageSize, string keyword, List<int> categoryIdList, string priceFilter,
            string priceOrderBy)
        {
            ISearcherAOP searcher = SearcherAOPFactory.CreateInstance();
            return searcher.QueryCommodityPage(pageIndex, pageSize, keyword, categoryIdList, priceFilter, priceOrderBy);
        }
    }
}
