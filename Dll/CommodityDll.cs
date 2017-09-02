using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDll;
using Model;
using Tool;

namespace Dll
{
    public class CommodityDll : IRepository<Commodity>, IQuery<Commodity>
    {
        private Logger logger = new Logger(typeof(CommodityDll));

        private string GetTableName(Commodity commodity)
        {
            return string.Format("JD_Commodity_{0}", (commodity.ProductId % 30 + 1).ToString("000"));
        }
        public void Save(Commodity entity)
        {
            SqlHelper.Insert(entity, GetTableName(entity));
        }

        public void SaveList(List<Commodity> tList)
        {
            if (tList == null || tList.Count == 0)
            {
                return;
            }
            IEnumerable<IGrouping<string, Commodity>> group = tList.GroupBy<Commodity, string>(c => GetTableName(c));
            foreach (var data in group)
            {
                SqlHelper.InsertList<Commodity>(data.ToList(), data.Key);
            }
        }

        public void Saveasc(Commodity entity)
        {

        }

        public void SaveListasc(List<Commodity> tList)
        {

        }


        public List<Commodity> QueryList(int tableNum, int pageIndex, int pageSize)
        {
            string sql = string.Format("SELECT top {2} * FROM JD_Commodity_{0} WHERE id>{1};", tableNum.ToString("000"), pageSize * Math.Max(0, pageIndex - 1), pageSize);
            return SqlHelper.QueryList<Commodity>(sql);
        }
    }
}
