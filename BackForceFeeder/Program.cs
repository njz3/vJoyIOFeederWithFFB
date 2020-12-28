using BackForceFeeder.Managers;
using BackForceFeeder.Utils;
using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace BackForceFeeder
{
    public static class Program
    {
        #region Globals
        public static BFFManager Manager;
        public static string AppDataPath;
        public static string LogFilename;

        public static string AppCfgFilename;
        public static string HwdCfgFilename;
        public static string CtlSetsCfgFilename;

        public static StreamWriter Logfile;
        #endregion

        public static void ConsoleLog(string text)
        {
            Console.WriteLine(text);
        }

        public static void LogToFile(string text)
        {
            Logfile.WriteLine(text);
        }

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        static int Main(string[] args)
        {
            AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BackForceFeeder", "Config");
            AppCfgFilename = AppDataPath + @"/ApplicationCfg.xml";
            HwdCfgFilename = AppDataPath + @"/HardwareCfg.xml";
            CtlSetsCfgFilename = AppDataPath + @"/ControlSetsBackup.xml";

            if (!Directory.Exists(AppDataPath)) {
                Directory.CreateDirectory(AppDataPath);
            }
            
            Manager = new BFFManager();
            Manager.LoadConfigurationFiles(AppCfgFilename, HwdCfgFilename);
            Manager.LoadControlSetFiles();

            CommandLine.ParseCommandLine(args, out var outputArgs);
            CommandLine.ProcessOptions(outputArgs);

            if (BFFManager.Config.Application.DumpLogToFile) {
                if (!Directory.Exists(BFFManager.Config.Application.LogsDirectory)) {
                    Directory.CreateDirectory(BFFManager.Config.Application.LogsDirectory);
                }
                LogFilename = Path.Combine(BFFManager.Config.Application.LogsDirectory, "_Log-" +
                    DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace("/", "-").Replace(":", "-") + ".txt");
                Logfile = File.CreateText(LogFilename);
                Logger.Loggers += LogToFile;
            }

            Console.Title = "BackForceFeeder v" +typeof(BFFManager).Assembly.GetName().Version.ToString() + " Made for Gamoover by njz3";
            Logger.Loggers += ConsoleLog;

            Logger.Start();
            Manager.Start();

            while (!OSUtilities.IsKeyPressed(ConsoleKey.Escape)) {
                Thread.Sleep(500);
            }

            Manager.Stop();
            Manager.SaveConfigurationFiles(AppCfgFilename, HwdCfgFilename);
            // Make a backup of the control set, just in case
            Manager.SaveControlSetFiles(true, CtlSetsCfgFilename);
            Logger.Stop();

            if (BFFManager.Config.Application.DumpLogToFile && Logfile!=null) {
                Logfile.Close();
            }

            return 0;
        } // Main


    } // class Program
} // namespace FeederDemoCS
