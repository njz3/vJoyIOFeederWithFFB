using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using SharpDX.DirectInput;
using SharpDX.XInput;
using BackForceFeeder.FFBAgents;
using BackForceFeeder.IOCommAgents;
using BackForceFeeder.Utils;
using BackForceFeeder.vJoyIOFeederAPI;

namespace BackForceFeeder
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
            AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BackForceFeeder", "Config");
            AppCfgFilename = AppDataPath + @"/ApplicationCfg.xml";
            HwdCfgFilename = AppDataPath + @"/HardwareCfg.xml";
            CtlSetsCfgFilename = AppDataPath + @"/ControlSetsBackup.xml";

            if (!Directory.Exists(AppDataPath)) {
                Directory.CreateDirectory(AppDataPath);
            }
            
            Manager = new vJoyManager();
            Manager.LoadConfigurationFiles(AppCfgFilename, HwdCfgFilename);
            Manager.LoadControlSetFiles();

            CommandLine.ParseCommandLine(args, out var outputArgs);
            CommandLine.ProcessOptions(outputArgs);

            if (vJoyManager.Config.Application.DumpLogToFile) {
                if (!Directory.Exists(vJoyManager.Config.Application.LogsDirectory)) {
                    Directory.CreateDirectory(vJoyManager.Config.Application.LogsDirectory);
                }
                LogFilename = Path.Combine(vJoyManager.Config.Application.LogsDirectory, "_Log-" +
                    DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace("/", "-").Replace(":", "-") + ".txt");
                Logfile = File.CreateText(LogFilename);
                Logger.Loggers += LogToFile;
            }

            Console.Title = "BackForceFeeder v" +typeof(vJoyManager).Assembly.GetName().Version.ToString() + " Made for Gamoover by njz3";
            Logger.Loggers += ConsoleLog;

            Logger.Start();
            Manager.Start();

            while (!vJoyManager.IsKeyPressed(ConsoleKey.Escape)) {
                Thread.Sleep(500);
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
