using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDll
{
    public interface IRepository<T> where T:class ,new()
    {
       
        void Save(T entity);
        void SaveList(List<T> tList);
        void Saveasc(T entity);
        void SaveListasc(List<T> tList);

    }
}
