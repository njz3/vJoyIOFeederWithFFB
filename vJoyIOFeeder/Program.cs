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
        static string ConfigFilename;
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
            ConfigFilename = AppDataPath + @"/config.xml";
            Logfile = File.CreateText(LogFilename);

            Logger.Loggers += FileLog;
            Logger.Loggers += ConsoleLog;
            Logger.Start();
            
            Manager = new vJoyManager();
            Manager.LoadConfigurationFiles(ConfigFilename);
            Manager.Start();

            while (!vJoyManager.IsKeyPressed(ConsoleKey.Escape)) {
                Thread.Sleep(500);
                Logfile.Flush();
            }

            Manager.Stop();
            Manager.SaveConfigurationFiles(ConfigFilename);
            Logger.Stop();

            Logfile.Close();
            return 0;
        } // Main


    } // class Program
} // namespace FeederDemoCS
