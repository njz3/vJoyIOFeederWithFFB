using System;
using System.Collections.Generic;
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
        static vJoyManager Manager;
        static string AppDataPath;
        static string LogFilename;

        static string AppCfgFilename;
        static string HwdCfgFilename;
        static string CtlSetsCfgFilename;

        static StreamWriter Logfile;

        public static void ConsoleLog(string text)
        {
            Console.WriteLine(text);
        }

        public static void FileLog(string text)
        {
            Logfile.WriteLine(text);
        }

        static int Main(string[] args)
        {
            AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/vJoyIOFeeder";
            LogFilename = AppDataPath + @"/log.txt";
            AppCfgFilename = AppDataPath + @"/ApplicationCfg.xml";
            HwdCfgFilename = AppDataPath + @"/HardwareCfg.xml";
            CtlSetsCfgFilename = AppDataPath + @"/ControlSetsCfg.xml";

            if (!Directory.Exists(AppDataPath)) {
                Directory.CreateDirectory(AppDataPath);
            }
            
            Manager = new vJoyManager();
            Manager.LoadConfigurationFiles(AppCfgFilename, HwdCfgFilename, CtlSetsCfgFilename);
            if (vJoyManager.Config.Application.DumpToLogFile) {
                Logfile = File.CreateText(LogFilename);
                Logger.Loggers += FileLog;
            }

            Console.Title = "vJoyIOFeeder v" +typeof(vJoyManager).Assembly.GetName().Version.ToString() + " Made for Gamoover by njz3";
            Logger.Loggers += ConsoleLog;

            Logger.Start();
            Manager.Start();

            while (!vJoyManager.IsKeyPressed(ConsoleKey.Escape)) {
                Thread.Sleep(500);
                if (vJoyManager.Config.Application.DumpToLogFile && Logfile!=null) {
                    Logfile.Flush();
                }
            }

            Manager.Stop();
            Manager.SaveConfigurationFiles(AppCfgFilename, HwdCfgFilename, CtlSetsCfgFilename);
            Logger.Stop();

            if (vJoyManager.Config.Application.DumpToLogFile && Logfile!=null) {
                Logfile.Close();
            }

            return 0;
        } // Main


    } // class Program
} // namespace FeederDemoCS
