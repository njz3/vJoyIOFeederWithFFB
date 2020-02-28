using System;
using System.IO;
using System.Windows.Forms;
using IOFeederGUI.GUI;
using vJoyIOFeeder;
using vJoyIOFeeder.Utils;

namespace IOFeederGUI
{
    public static class Program
    {
        #region Globals
        public static vJoyManager Manager;
        public static string AppDataPath;
        public static string LogFilename;
        public static string ConfigFilename;
        static StreamWriter Logfile;

        #endregion

        public static void FileLog(string text)
        {
            Logfile.WriteLine(text);
        }

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/vJoyIOFeeder";
            LogFilename = AppDataPath + @"/log.txt";
            ConfigFilename = AppDataPath + @"/config.xml";

            if (!Directory.Exists(AppDataPath)) {
                Directory.CreateDirectory(AppDataPath);
            }

            Manager = new vJoyManager();
            Manager.LoadConfigurationFiles(ConfigFilename);
            if (vJoyManager.Config.DumpToLogFile) {
                Logfile = File.CreateText(LogFilename);
                Logger.Loggers += FileLog;
            }
            Logger.Start();
            Manager.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            Manager.Stop();
            Manager.SaveConfigurationFiles(ConfigFilename);
            Logger.Stop();

            if (vJoyManager.Config.DumpToLogFile && Logfile!=null) {
                Logfile.Close();
            }
        }
    }
}
