using System;
using NLog;

class Program
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    static void Main(string[] args)
    {
        // Configure NLog from the nlog.config file
        var config = new NLog.Config.XmlLoggingConfiguration("nlog.config", true);
        LogManager.Configuration = config;

        // Example log messages
        Logger.Info("This is an info message");
        Logger.Error("This is an error message");

        // Ensure to flush and close the log before application exit
        LogManager.Shutdown();
    }
}
