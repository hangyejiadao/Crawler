using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace IBLL
{
    public interface ILuceneQuery
    {
        /// <summary>
        /// 获取商品信息数据
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        List<Commodity> QueryIndex(string queryString);

       
        /// <summary>
        /// 分页获取商品信息数据
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="priceFilter"></param>
        /// <param name="priceOrderBy"></param>
        /// <returns></returns>
        List<Commodity> QueryIndexPage(string queryString, int pageIndex, int pageSize, out int totalCount, string priceFilter, string priceOrderBy);
    }
}
