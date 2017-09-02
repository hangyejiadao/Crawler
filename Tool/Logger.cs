using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using log4net;
using log4net.Config;

namespace Tool
{
    public class Logger
    {
        static Logger()
        {
            XmlConfigurator.Configure(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CfgFiles\\log4net.cfg.xml")));
            ILog Log = LogManager.GetLogger(typeof(Logger));
            Log.Info("系统初始化Logger模块");
        }

        private ILog logger = null;

        public Logger(Type type)
        {
            logger = LogManager.GetLogger(type);
        }

        public void Error(string msg, Exception ex)
        {
            Console.WriteLine(msg);
            logger.Error(msg, ex);
        }

        public void Info(string v)
        {
            logger.Info(v);
        }
    }
}
