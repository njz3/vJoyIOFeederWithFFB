using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using SharpDX.DirectInput;
using SharpDX.XInput;
using vJoyIOFeeder.FFBAgents;
using vJoyIOFeeder.IOCommAgents;
using vJoyIOFeeder.Utils;
using vJoyIOFeeder.vJoyIOFeederAPI;

namespace vJoyIOFeeder
{
    public static class Program
    {
        #region Globals
        public static vJoyManager Manager;
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
            AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "vJoyIOFeeder");
            AppCfgFilename = AppDataPath + @"/ApplicationCfg.xml";
            HwdCfgFilename = AppDataPath + @"/HardwareCfg.xml";
            CtlSetsCfgFilename = AppDataPath + @"/ControlSetsCfg.xml";

            if (!Directory.Exists(AppDataPath)) {
                Directory.CreateDirectory(AppDataPath);
            }
            
            Manager = new vJoyManager();
            Manager.LoadConfigurationFiles(AppCfgFilename, HwdCfgFilename);
            Manager.LoadControlSetFiles();

            if (vJoyManager.Config.Application.DumpLogToFile) {
                LogFilename = Path.Combine(vJoyManager.Config.Application.ControlSetsDirectory, "_Log-" +
                    DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace("/", "-").Replace(":", "-") + ".txt");
                Logfile = File.CreateText(LogFilename);
                Logger.Loggers += LogToFile;
            }

            Console.Title = "vJoyIOFeeder v" +typeof(vJoyManager).Assembly.GetName().Version.ToString() + " Made for Gamoover by njz3";
            Logger.Loggers += ConsoleLog;

            Logger.Start();
            Manager.Start();

            while (!vJoyManager.IsKeyPressed(ConsoleKey.Escape)) {
                Thread.Sleep(500);
                if (vJoyManager.Config.Application.DumpLogToFile && Logfile!=null) {
                    Logfile.Flush();
                }
            }

            Manager.Stop();
            Manager.SaveConfigurationFiles(AppCfgFilename, HwdCfgFilename);
            // Make a backup of the control set, just in case
            Manager.SaveControlSetFiles(true, CtlSetsCfgFilename);
            Logger.Stop();

            if (vJoyManager.Config.Application.DumpLogToFile && Logfile!=null) {
                Logfile.Close();
            }

            return 0;
        } // Main


    } // class Program
} // namespace FeederDemoCS
