using BackForceFeeder.Configuration;
using BackForceFeeder.FFBManagers;
using BackForceFeeder.Inputs;
using BackForceFeeder.IOCommAgents;
using BackForceFeeder.Outputs;
using BackForceFeeder.Utils;
using BackForceFeeder.vJoyIOFeederAPI;
using SharpDX.DirectInput;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace BackForceFeeder.BackForceFeeder
{
    /// <summary>
    /// This is the main thread of the BackForceFeeder.
    /// It handles reading from the IOboard(s) and process data to expose them
    /// to either the Input or vJoy interfaces.
    /// The force feedback is managed by a separate periodic thread (realtime)
    /// to ensure correct effect processing. Then this thread will send the
    /// latest computed torque command to the IOboard connected to the motor 
    /// driver.
    /// </summary>
    public static class SharedData
    {

        public static BFFManager Manager;
        public static string AppDataPath;
        public static string LogFilename;

        public static string AppCfgFilename;
        public static string HwdCfgFilename;
        public static string CtlSetsCfgFilename;

        public static StreamWriter Logfile;
        public static void LogToFile(string text)
        {
            SharedData.Logfile.WriteLine(text);
        }

        public static void Initialize(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;

            SharedData.AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BackForceFeeder", "Config");
            SharedData.AppCfgFilename = SharedData.AppDataPath + @"/ApplicationCfg.xml";
            SharedData.HwdCfgFilename = SharedData.AppDataPath + @"/HardwareCfg.xml";
            SharedData.CtlSetsCfgFilename = SharedData.AppDataPath + @"/ControlSetsBackup.xml";

            if (!Directory.Exists(SharedData.AppDataPath)) {
                Directory.CreateDirectory(SharedData.AppDataPath);
            }

            SharedData.Manager = new BFFManager();
            SharedData.Manager.LoadConfigurationFiles(SharedData.AppCfgFilename, SharedData.HwdCfgFilename);
            SharedData.Manager.LoadControlSetFiles();

            CommandLine.ParseCommandLine(args, out var outputArgs);
            CommandLine.ProcessOptions(outputArgs);

            if (BFFManager.Config.Application.DumpLogToFile) {
                if (!Directory.Exists(BFFManager.Config.Application.LogsDirectory)) {
                    Directory.CreateDirectory(BFFManager.Config.Application.LogsDirectory);
                }

                SharedData.LogFilename = Path.Combine(BFFManager.Config.Application.LogsDirectory, "_Log-" +
                    DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace("/", "-").Replace(":", "-") + ".txt");
                SharedData.Logfile = File.CreateText(SharedData.LogFilename);
                Logger.Loggers += LogToFile;
            }

            
        }

        /// <summary>
        /// Start threads and managers
        /// </summary>
        public static void Start()
        {
            Logger.Start();
            Manager.Start();
        }

        public static void Stop()
        {
            SharedData.Manager.Stop();
            SharedData.Manager.SaveConfigurationFiles(SharedData.AppCfgFilename, SharedData.HwdCfgFilename);
            // Make a backup of the control set, just in case
            SharedData.Manager.SaveControlSetFiles(true, SharedData.CtlSetsCfgFilename);
            Logger.Stop();

            if (BFFManager.Config.Application.DumpLogToFile && SharedData.Logfile!=null) {
                SharedData.Logfile.Close();
            }
        }
    }
}
