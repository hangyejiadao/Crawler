using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDll
{
    public interface IQuery<T> where T:new()

    {
    List<T> QueryList(int tableNum, int pageIndex, int pageSize);
    }
}
