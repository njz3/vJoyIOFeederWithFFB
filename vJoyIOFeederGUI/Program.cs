using System;
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
        public static string ConfigPath;
        #endregion

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/vJoyIOFeeder/config.xml";

            Logger.Start();
            Manager = new vJoyManager();
            Manager.LoadConfigurationFiles(ConfigPath);
            Manager.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            Manager.Stop();
            Manager.SaveConfigurationFiles(ConfigPath);
            Logger.Stop();
        }
    }
}
