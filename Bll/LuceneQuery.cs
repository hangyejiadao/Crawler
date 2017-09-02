using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBLL;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Model;
using Tool;
using Version = Lucene.Net.Util.Version;

namespace Bll
{
    public class LuceneQuery : ILuceneQuery
    {
        private Logger logger = new Logger(typeof(LuceneQuery));
        public List<Commodity> QueryIndex(string queryString)
        {
            IndexSearcher searcher = null;
            List<Commodity> ciList = null;
            try
            {
                ciList = new List<Commodity>();
                Directory dir = FSDirectory.Open(Constant.IndexPath);
                searcher = new IndexSearcher(dir);
                Analyzer analyzer = new PanGuAnalyzer();
                //这里配置搜索条件
                QueryParser parser = new QueryParser(Version.LUCENE_30, "title", analyzer);
                Query query = parser.Parse(queryString);
                TopDocs docs = searcher.Search(query, (Filter)null, 10000);
                foreach (ScoreDoc sd in docs.ScoreDocs)
                {
                    Document doc = searcher.Doc(sd.Doc);
                    ciList.Add(DocumentToCommodityInfo(doc));
                }
                return ciList;
            }
            catch (Exception e)
            {
                logger.Error("Error:", e);
            }
            finally
            {
                if (searcher != null)
                {
                    searcher.Close();
                }
            }
            return ciList;
        }

        private Commodity DocumentToCommodityInfo(Document doc)
        {
            return new Commodity()
            {
                Id = int.Parse(doc.Get("id")),
                Title = doc.Get("title"),
                ProductId = long.Parse(doc.Get("productid")),
                CategoryId = int.Parse(doc.Get("categoryid")),
                CategoryName = doc.Get("categoryname"),
                ImageUrl = doc.Get("imageurl"),
                Price = decimal.Parse(doc.Get("price")),
                Url = doc.Get("url")
            };
        }
 
 


        public List<Commodity> QueryIndexPage(string queryString, int pageIndex, int pageSize,
            out int totalCount, string priceFilter,
            string priceOrderBy)
        {
            totalCount = 0;
            IndexSearcher searcher = null;
            List<Commodity> ciList = null;
            try
            {
                ciList = new List<Commodity>();
                FSDirectory dir = FSDirectory.Open(Constant.IndexPath);
                searcher = new IndexSearcher(dir);



                Analyzer analyzer = new PanGuAnalyzer();
                //这里配置搜索条i按
                QueryParser parser = new QueryParser(Version.LUCENE_30, "title", analyzer); 
                Query query = parser.Parse(queryString); 



                pageIndex = Math.Max(1, pageIndex);//索引从1开始
                int startIndex = (pageIndex - 1) * pageSize;
                int endIndex = pageIndex * pageSize; 


                NumericRangeFilter<float> numPriceFilter = null;
                if (!string.IsNullOrWhiteSpace(priceFilter))
                {
                    bool isContainStart = priceFilter.StartsWith("[");
                    bool isContainEnd = priceFilter.EndsWith("]");
                    string[] floatArray = priceFilter.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "").Split(',');
                    float start = 0;
                    float end = 0;
                    if (!float.TryParse(floatArray[0], out start) || float.TryParse(floatArray[1], out end))
                    {
                        return null;
                    }
                    numPriceFilter =
                        NumericRangeFilter.NewFloatRange("price", start, end, isContainStart, isContainEnd);
                }



                Sort sort = new Sort();
                if (!string.IsNullOrWhiteSpace(priceOrderBy))
                {
                    SortField sortField = new SortField("price", SortField.FLOAT, priceOrderBy.EndsWith("asc", StringComparison.CurrentCultureIgnoreCase));
                    sort.SetSort(sortField);
                }



                TopDocs docs = searcher.Search(query, numPriceFilter, 10000, sort);
                totalCount = docs.TotalHits;
                for (int i = startIndex; i < endIndex && i < totalCount; i++)
                {
                    Document doc = searcher.Doc(docs.ScoreDocs[i].Doc);
                    ciList.Add(DocumentToCommodityInfo(doc));
                }
            }
            catch (Exception e)
            {
                logger.Error("Error:", e);
            }
            return ciList;
        }
    }
}
