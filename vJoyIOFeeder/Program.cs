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
        static string ConfigPath;

        public static void ConsoleLog(string text)
        {
            Console.WriteLine(text);
        }

        static int Main(string[] args)
        {
            ConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/vJoyIOFeeder/config.xml";

            Logger.Start();
            Logger.Loggers += ConsoleLog;
            Manager = new vJoyManager();

            Manager.LoadConfigurationFiles(ConfigPath);

            Manager.Start();

            while (!vJoyManager.IsKeyPressed(ConsoleKey.Escape)) {
                Thread.Sleep(100);
            }

            Manager.Stop();
            Logger.Stop();
            Manager.SaveConfigurationFiles(ConfigPath);

            return 0;
        } // Main


    } // class Program
} // namespace FeederDemoCS
