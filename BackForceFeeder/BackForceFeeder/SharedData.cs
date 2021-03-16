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
        public static string LoadCfgExtension;
        public static string SaveCfgExtension;

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
            SharedData.AppCfgFilename = SharedData.AppDataPath + @"/ApplicationCfg";
            SharedData.HwdCfgFilename = SharedData.AppDataPath + @"/HardwareCfg";
            SharedData.CtlSetsCfgFilename = SharedData.AppDataPath + @"/ControlSetsBackup";
            SharedData.LoadCfgExtension = ".json";
            SharedData.SaveCfgExtension = ".json";

            CommandLine.ParseCommandLine(args, out var outputArgs);

            if (!Directory.Exists(SharedData.AppDataPath)) {
                Directory.CreateDirectory(SharedData.AppDataPath);
            }

            SharedData.Manager = new BFFManager();
            SharedData.Manager.LoadConfigurationFiles(SharedData.AppCfgFilename + SharedData.LoadCfgExtension, SharedData.HwdCfgFilename + SharedData.LoadCfgExtension);
            SharedData.Manager.LoadControlSetFiles();

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
            
            if (OSUtilities.IsUserAdministrator()) {
                Logger.Log("[MAIN] Running as administrator, trying realtime priority", LogLevels.IMPORTANT);
            } else {
                Logger.Log("[MAIN] Running as standard user, trying realtime priority", LogLevels.IMPORTANT);
            }
            try {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
                if (Process.GetCurrentProcess().PriorityClass != ProcessPriorityClass.RealTime) {
                    Logger.Log("[MAIN] Setting realtime priority failed, got: " + Process.GetCurrentProcess().PriorityClass, LogLevels.IMPORTANT);
                }
            } catch(Exception ex) {
                Logger.Log("[MAIN] Setting realtime priority failed, falling back to high", LogLevels.IMPORTANT);
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            }
        }

        public static void Stop()
        {
            SharedData.Manager.Stop();
            SharedData.Manager.SaveConfigurationFiles(SharedData.AppCfgFilename + SharedData.SaveCfgExtension, SharedData.HwdCfgFilename + SharedData.SaveCfgExtension);
            // Make a backup of the control set, just in case
            SharedData.Manager.SaveControlSetFiles(true, SharedData.CtlSetsCfgFilename + SharedData.SaveCfgExtension);
            Logger.Stop();

            if (BFFManager.Config.Application.DumpLogToFile && SharedData.Logfile!=null) {
                SharedData.Logfile.Close();
            }
        }
    }
}
