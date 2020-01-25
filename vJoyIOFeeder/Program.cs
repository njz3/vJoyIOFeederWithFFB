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

        public static void ConsoleLog(string text)
        {
            Console.WriteLine(text);
        }

        static int Main(string[] args)
        {
            Logger.Start();
            Logger.Loggers += ConsoleLog;
            Manager = new vJoyManager();
            Manager.Start();

            while (!vJoyManager.IsKeyPressed(ConsoleKey.Escape)) {
                Thread.Sleep(100);
            }

            Manager.Stop();
            Logger.Stop();

            return 0;
        } // Main


    } // class Program
} // namespace FeederDemoCS
