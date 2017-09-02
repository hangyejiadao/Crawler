using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crawler.AOP.Attribute;

namespace Crawler.AOP
{
    public interface  ISearcherAOP
    {
        /// <summary>
        /// 分页信息查询
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <param name="categoryIdList"></param>
        /// <param name="priceFilter"></param>
        /// <param name="priceOrderBy"></param>
        /// <returns></returns>
        [LogHandler(Order = 2)]
        [ExceptionHandlerAttribute(Order = 3)]
        [AfterLogHandlerAttribute(Order = 5)]
        string QueryCommodityPage(int pageIndex, int pageSize, string keyword, List<int> categoryIdList, string priceFilter, string priceOrderBy);
    }
}
