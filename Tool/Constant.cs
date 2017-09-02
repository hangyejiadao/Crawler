using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Tool
{
    public class Constant
    {
        public static string DataPath = ConfigurationManager.AppSettings["DataPath"];
        public static string JDCategoryUrl = ConfigurationManager.AppSettings["JDCategoryUrl"];
        public static Encoding encoding = Encoding.UTF8;
        public static string IndexPath =  ConfigurationManager.AppSettings["IndexPath"];
    }
}
