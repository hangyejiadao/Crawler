using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tool;

namespace Dll
{
    public  class DBInit
    {
        private static  Logger logger=new Logger(typeof(DBInit));

        /// <summary>
        /// 谨慎使用 会全部删除数据库并重新创建
        /// </summary>
        public static void InitCommodityTable()
        {
            #region Delete

            try
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 1; i <= 30; i++)
                {
                    sb.AppendFormat(@" if exists (select * from sys.objects where Name='JD_Commodity_{0}') DROP TABLE JD_Commodity_{0};", i.ToString("000"));
                }
                SqlHelper.ExecuteNonQuery(sb.ToString());
            }
            catch (Exception ex)
            {
                logger.Error("Error:", ex);
            }
            #endregion

            #region Create
            try
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 1; i <= 30; i++)
                {
                    sb.AppendFormat(@"CREATE TABLE  JD_Commodity_{0}(
	                                    [Id] [int] IDENTITY(1,1) NOT NULL,
	                                    [ProductId] [bigint] NULL,
	                                    [CategoryId] [int] NULL,
	                                    [Title] [nvarchar](500) NULL,
	                                    [Price] [decimal](18, 2) NULL,
	                                    [Url] [varchar](1000) NULL,
	                                    [ImageUrl] [varchar](1000) NULL,
                             CONSTRAINT [PK_JD_Commodity_{0}] PRIMARY KEY CLUSTERED 
                            (
                            	[Id] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                            ) ON [PRIMARY];", i.ToString("000"));
                }
                SqlHelper.ExecuteNonQuery(sb.ToString());
            }
            catch (Exception ex)
            {
                logger.Error("InitCommodityTable创建异常", ex);
                throw ex;
            } 
            #endregion
        } 
        /// <summary>
        /// 谨慎使用  会全部删除数据库并重新创建!
        /// </summary>
        public static void InitCategoryTable()
        {
            #region Delete 
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(@"if exists(select * from sys.objects where Name='Category')  drop table Category");
                SqlHelper.ExecuteNonQuery(sb.ToString());
            }
            catch (Exception ex)
            {
                logger.Error("Error:", ex);
            }
            #endregion

            #region Create

            try
            {
                StringBuilder sb=new StringBuilder();
                sb.AppendFormat(@"CREATE TABLE Category(
	                                    [Id] [int] IDENTITY(1,1) NOT NULL,
	                                    [Code] [varchar](100) NULL,
	                                    [ParentCode] [varchar](100) NULL,
	                                    [CategoryLevel] [int] NULL,
	                                    [Name] [nvarchar](50) NULL,
	                                    [Url] [varchar](1000) NULL,
	                                    [State] [int] NULL,
                                      CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
                                     (
                                     	[Id] ASC
                                     )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                     ) ON [PRIMARY];");
                SqlHelper.ExecuteNonQuery(sb.ToString());
            }
            catch (Exception ex)
            {
              logger.Error("Error:",ex);
            }
            

            #endregion
        }

        public static string Test()
        {
            StringBuilder sb=new StringBuilder();
            sb.AppendFormat( "print 'test'"   );
            SqlHelper.GetInfo(sb.ToString());
            return SqlHelper.sqlInfo;
        }
    }
}
