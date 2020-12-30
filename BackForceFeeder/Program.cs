using BackForceFeeder.BackForceFeeder;
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
        
        #endregion

        public static void ConsoleLog(string text)
        {
            Console.WriteLine(text);
        }

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        static int Main(string[] args)
        {
            SharedData.Initialize(args);

            Console.Title = "BackForceFeeder v" +typeof(BFFManager).Assembly.GetName().Version.ToString() + " Made for Gamoover by njz3";

            Logger.Loggers += ConsoleLog;

            SharedData.Start();
            
            while (!OSUtilities.IsKeyPressed(ConsoleKey.Escape)) {
                Thread.Sleep(500);
            }

            SharedData.Stop();

            return 0;
        } // Main


    } // class Program
} // namespace FeederDemoCS
