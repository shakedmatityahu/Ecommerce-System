using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;

namespace MarketBackend.Services
{
    

    public static class MyLogger
    {
     private static readonly Logger Log = LogManager.GetCurrentClassLogger();
     private static bool init = false;
     private static readonly object lockObject = new object();


     private static void InitLogger(){
        lock(lockObject){
        string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            var config = new NLog.Config.XmlLoggingConfiguration("nlog.config", true);
            LogManager.Configuration = config;
            init = true;
        }
     }

     public static Logger GetLogger(){
        lock(lockObject){
            if (!init){
                InitLogger();
            }
        }
        return Log;
     }



     public static void CloseLogger(){
        lock(lockObject){
            LogManager.Shutdown();
        }
     }
    }
}