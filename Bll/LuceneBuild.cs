using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dll;
using IBLL;
using Lucene.Net.Index;
using Model;
using Tool;
using Lucene.Net.Analysis;

using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Util;
using LuceneIO = Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace Bll
{
    public class LuceneBuild : ILuceneBuild
    {
        #region Identity 
        private Logger logger = new Logger(typeof(LuceneBuild));
        #endregion

        public void BuildIndex(List<Commodity> ciList, string pathSuffix = "", bool isCreate = false)
        {
            IndexWriter writer = null;
            try
            {
                if (ciList == null || ciList.Count == 0)
                {
                    return;
                }
                string rootIndexPath = Constant.IndexPath;
                string indexPath = string.IsNullOrWhiteSpace(pathSuffix) ? rootIndexPath : string.Format("{0}\\{1}", rootIndexPath, pathSuffix);

                DirectoryInfo dirInfo = Directory.CreateDirectory(indexPath);
                LuceneIO.Directory directory = LuceneIO.FSDirectory.Open(dirInfo);
                writer = new IndexWriter(directory, new PanGuAnalyzer(), isCreate, IndexWriter.MaxFieldLength.LIMITED);
                writer.SetMaxBufferedDocs(100);
                writer.MergeFactor = 100;
                writer.UseCompoundFile = true;
                ciList.ForEach(c => CreateCIIndex(writer, c));

            }
            catch (Exception e)
            {
                logger.Error("Error:", e);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        private void CreateCIIndex(IndexWriter writer, Commodity ci)
        {
            try
            {
                writer.AddDocument(ParseCItoDoc(ci));
            }
            catch (Exception e)
            {
                logger.Error("Error:", e);
            }
        }

        private Document ParseCItoDoc(Commodity ci)
        {
            Document doc = new Document();
            doc.Add(new Field("id", ci.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("title", ci.Title, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("productid", ci.ProductId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("categoryid", ci.CategoryId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("categoryname", ci.CategoryName, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("imageurl", ci.ImageUrl, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("url", ci.Url, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new NumericField("price", Field.Store.YES, true).SetFloatValue((float)ci.Price));
            return doc;
        }

        private PerFieldAnalyzerWrapper CreateAnalyzerWrapper()
        {
            Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            PerFieldAnalyzerWrapper analyzerWrapper = new PerFieldAnalyzerWrapper(analyzer);
            analyzerWrapper.AddAnalyzer("title", new PanGuAnalyzer());
            analyzerWrapper.AddAnalyzer("categoryname", new PanGuAnalyzer());
            return analyzerWrapper;
        }
        /// <summary>
        /// 将索引合并到上级目录
        /// </summary>
        /// <param name="childDirs">子文件夹名</param>
        public void MergeIndex(string[] childDirs)
        {
            IndexWriter writer = null;
            try
            {
                if (childDirs == null || childDirs.Length == 0)
                {
                    return;
                }
                Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);
                string rootPath = Constant.IndexPath;
                DirectoryInfo dirInfo = Directory.CreateDirectory(rootPath);
                LuceneIO.Directory directory = LuceneIO.FSDirectory.Open(dirInfo);
                writer = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.LIMITED);
                LuceneIO.Directory[] dirNo = childDirs
                    .Select(dir => LuceneIO.FSDirectory.Open(
                        Directory.CreateDirectory(string.Format("{0}\\{1}", rootPath, dir)))).ToArray();

                writer.MergeFactor = 100;
                writer.UseCompoundFile = true;//创建符合文件 减少索引文件数量
                writer.AddIndexesNoOptimize(dirNo);
                Console.WriteLine("Over");
            }
            catch (Exception e)
            {
                logger.Error("Error:", e);
            }
            finally
            {
                try
                {
                    if (writer != null)
                    {
                        writer.Optimize();
                        writer.Close();
                    }
                }
                finally
                {
                    Console.WriteLine("Over1");
                }
               
               
            }
        }

        public void InsertIndex(Commodity ci)
        {
            IndexWriter writer = null;
            try
            {
                if (ci == null)
                {
                    return;
                }
                string rootIndexPath = Constant.IndexPath;
                DirectoryInfo dirInfo = Directory.CreateDirectory(rootIndexPath);

                bool isCreate = dirInfo.GetFiles().Count() == 0;
                LuceneIO.Directory directory = LuceneIO.FSDirectory.Open(dirInfo);
                writer = new IndexWriter(directory, CreateAnalyzerWrapper(), isCreate, IndexWriter.MaxFieldLength.UNLIMITED);
                writer.MergeFactor = 100;
                writer.UseCompoundFile = true;
                CreateCIIndex(writer, ci);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void InsertIndexMuti(List<Commodity> ciList)
        {
            BuildIndex(ciList, "", false);
        }

        public void DeleteIndex(Commodity ci)
        {
            IndexReader reader = null;
            try
            {
                if (ci == null)
                {
                    return;
                }
                Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);
                string rootIndexPath = Constant.IndexPath;
                DirectoryInfo dirInfo = Directory.CreateDirectory(rootIndexPath);
                LuceneIO.Directory directory = LuceneIO.FSDirectory.Open(dirInfo);
                reader = IndexReader.Open(directory, false);
                reader.DeleteDocuments(new Term("productid", ci.ProductId.ToString()));
            }
            catch (Exception e)
            {
                logger.Error("DeleteIndex异常",e);

            }
            finally
            {
                if (reader!=null)
                {
                   reader.Dispose();
                }

            }
        }

        public void DeleteIndexMuti(List<Commodity> ciList)
        { 
            try
            {
                foreach (var tem in ciList)
                {
                    DeleteIndex(tem);
                } 
            }
            catch (Exception e)
            {
               logger.Error("Error:",e);
            } 
        }

        #region 更新索引                                                                                                                                                                                                                                  
        public void UpdateIndex(Commodity ci)
        {
            IndexWriter writer = null;
            try
            {
                if (ci == null)
                    return;
                string rootIndexPath = Constant.IndexPath;
                DirectoryInfo dirInfo = Directory.CreateDirectory(rootIndexPath); 
                bool isCreate = dirInfo.GetFiles().Count() == 0;
                LuceneIO.Directory directory = LuceneIO.FSDirectory.Open(dirInfo);
                writer = new IndexWriter(directory, CreateAnalyzerWrapper(), isCreate,
                    IndexWriter.MaxFieldLength.LIMITED);
                writer.MergeFactor = 100;
                writer.UseCompoundFile = true;
                writer.UpdateDocument(new Term("productid", ci.ProductId.ToString()), ParseCItoDoc(ci)); 
            }
            catch (Exception e)
            { 
                logger.Error("Error:", e);
            }
            finally
            {
                if (writer!=null)
                {
                   writer.Close();
                }
            }
        }

        public void UpdateIndexMuti(List<Commodity> ciList)
        {
            try
            {
                foreach (var tem in ciList)
                {
                    UpdateIndex(tem);
                }
            }
            catch (Exception e)
            {
                 logger.Error("Error:",e);
            }
        }
        #endregion
    }
}
