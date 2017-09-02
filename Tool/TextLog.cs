using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool
{
    public class TextLog
    {
        public void Log(string msg,string txtpath)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + txtpath + ".txt";
            FileInfo file=new FileInfo(txtpath);
            if (!file.Exists)
            {
                file.Create();
            }
            StreamWriter writer=new StreamWriter(path,true);
            writer.WriteLine(msg);
            writer.Close();
        }

    }
}
