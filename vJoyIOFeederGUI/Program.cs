using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using vJoyIOFeederGUI.GUI;
using vJoyIOFeeder;
using vJoyIOFeeder.Utils;
using System.Globalization;
using System.Collections.Generic;

namespace vJoyIOFeederGUI
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

        public static MainForm MainForm;
        public static StreamWriter Logfile;
        #endregion

        #region Tray icon
        public static NotifyIcon TrayIcon;
        public static ContextMenu TrayMenu;

        private static void OnShowMain(object sender, EventArgs e)
        {
            Logger.Log("Show main form from tray icon", LogLevels.IMPORTANT);
            Program.MainForm.Focus();
            Program.MainForm.BringToFront();
            Program.MainForm.WindowState = FormWindowState.Normal;
            Program.MainForm.Show();
        }
        private static void OnRestart(object sender, EventArgs e)
        {
            Logger.Log("Restart manager from tray icon", LogLevels.IMPORTANT);
            Program.Manager.Stop();
            Program.Manager.Start();
        }

        private static void OnAbout(object sender, EventArgs e)
        {
            MessageBox.Show(OSUtilities.AboutString(),
                "About vJoyIOFeeder by njz3",
                MessageBoxButtons.OK);
        }
        private static void OnExit(object sender, EventArgs e)
        {
            Logger.Log("Exiting app from tray icon", LogLevels.IMPORTANT);
            Program.Manager.Stop();
            Program.MainForm.Close();
            Application.Exit();
        }


        public static void StartTray()
        {
            TrayMenu = new ContextMenu();
            TrayMenu.MenuItems.Add("Show", OnShowMain);
            TrayMenu.MenuItems.Add("Restart", OnRestart);
            TrayMenu.MenuItems.Add("About", OnAbout);
            TrayMenu.MenuItems.Add("Exit", OnExit);

            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            TrayIcon = new NotifyIcon();
            TrayIcon.Text = "vJoyIOFeeder For Gamoover\r\n";
            TrayIcon.Text += "(c) 2020 B.Maurin (njz3)\r\n";
            // List resources with:
            // var ressources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            var icon = Assembly.GetExecutingAssembly().GetManifestResourceStream("vJoyIOFeederGUI.Resources.Gamoo.ico");
            TrayIcon.Icon = new Icon(icon, 40, 40);

            // Add menu to tray icon and show it.
            TrayIcon.ContextMenu = TrayMenu;
            TrayIcon.Visible = true;
            /*
            // This is done when minimizing the main window
            TrayIcon.ShowBalloonTip(3000,
                "vJoyIOFeeder by njz3",
                "Running mode is " + vJoyManager.Config.TranslatingModes.ToString(),
                ToolTipIcon.Info);
                */
        }
        /// <summary>
        /// Function used to close the tray
        /// </summary>
        public static void CloseTray()
        {
            TrayIcon.Dispose();
            TrayMenu.Dispose();
        }
        #endregion

        public static void LogToFile(string text)
        {
            Logfile.WriteLine(text);
        }

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "vJoyIOFeeder");
            AppCfgFilename = AppDataPath + @"/ApplicationCfg.xml";
            HwdCfgFilename = AppDataPath + @"/HardwareCfg.xml";
            CtlSetsCfgFilename = AppDataPath + @"/ControlSetsCfg.xml";

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
                    DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace("/","-").Replace(":","-") + ".txt");
                Logfile = File.CreateText(LogFilename);
                Logger.Loggers += LogToFile;
            }
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Logger.Start();
            Manager.Start();
            
            MainForm = new MainForm();
            StartTray();

            if (vJoyManager.Config.Application.StartMinimized) {
                MainForm.WindowState = FormWindowState.Minimized;
            }

            Application.Run(MainForm);
            CloseTray();

            Manager.Stop();
            Manager.SaveConfigurationFiles(AppCfgFilename, HwdCfgFilename);
            // Make a backup of the control set, just in case
            Manager.SaveControlSetFiles(true, CtlSetsCfgFilename);
            Logger.Stop();

            if (vJoyManager.Config.Application.DumpLogToFile && Logfile!=null) {
                Logfile.Close();
            }

            return 0;
        }
    }
}
