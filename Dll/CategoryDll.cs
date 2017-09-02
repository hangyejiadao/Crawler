using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDll;
using Tool;
using Model;

namespace Dll
{
    public  class CategoryDll:IRepository<Category>
    {
        private const string tableName = "Category";
        private Logger logger=new Logger(typeof(CategoryDll));

        public string Query(string Id)
        { 
            return SqlHelper.QueryList<Category>("SELECT * FROM Category where Id='"+Id+"'").First().Name;
        }



        public   void Save(Category entity)
        {
           SqlHelper.Insert(entity,tableName);
                 
        }

        public void SaveList(List<Category> tList)
        {
             SqlHelper.InsertList(tList,tableName);

        }

        public async void Saveasc(Category entity)
        {
           
        }

   
        public async void SaveListasc(List<Category> tList)
        {
           
        }

        public List<Category> QueryListByLevel(int level)
        {
            string sql = string.Format("SELECT * FROM CATEGORY where categorylevel={0}", level);
            return  SqlHelper.QueryList<Category>(sql);
        }
    }
}
