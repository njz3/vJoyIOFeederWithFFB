using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using BackForceFeederGUI.GUI;
using BackForceFeeder;
using BackForceFeeder.Utils;
using System.Globalization;
using System.Collections.Generic;
using BackForceFeeder.BackForceFeeder;

namespace BackForceFeederGUI
{
    public static class Program
    {
        static MainForm MainForm;
     
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
            SharedData.Manager.Stop();
            SharedData.Manager.Start();
        }

        private static void OnAbout(object sender, EventArgs e)
        {
            MessageBox.Show(OSUtilities.AboutString(),
                "About BackForceFeeder by njz3",
                MessageBoxButtons.OK);
        }
        private static void OnExit(object sender, EventArgs e)
        {
            Logger.Log("Exiting app from tray icon", LogLevels.IMPORTANT);
            SharedData.Manager.Stop();
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
            TrayIcon.Text = "BackForceFeeder For Gamoover\r\n";
            TrayIcon.Text += "(c) 2020 B.Maurin (njz3)\r\n";
            // List resources with:
            // var ressources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            var icon = Assembly.GetExecutingAssembly().GetManifestResourceStream("BackForceFeederGUI.Resources.Gamoo.ico");
            TrayIcon.Icon = new Icon(icon, 40, 40);

            // Add menu to tray icon and show it.
            TrayIcon.ContextMenu = TrayMenu;
            TrayIcon.Visible = true;
            /*
            // This is done when minimizing the main window
            TrayIcon.ShowBalloonTip(3000,
                "BackForceFeeder by njz3",
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



        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            SharedData.Initialize(args);
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SharedData.Start();

            MainForm = new MainForm();
            StartTray();

            if (BFFManager.Config.Application.StartMinimized) {
                MainForm.WindowState = FormWindowState.Minimized;
            }

            Application.Run(MainForm);
            CloseTray();

            SharedData.Stop();

            return 0;
        }
    }
}
