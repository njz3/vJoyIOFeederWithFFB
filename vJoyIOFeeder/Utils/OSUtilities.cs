using IWshRuntimeLibrary;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace vJoyIOFeeder.Utils
{
    public class OSUtilities
    {
        public static Version AssemblyVersion()
        {
            var assembly = typeof(vJoyManager).Assembly;
            var version = assembly.GetName().Version;
            return version;
        }

        public static string AssemblyCopyright()
        {
            AssemblyCopyrightAttribute copyright =
                Assembly.GetExecutingAssembly().
                GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0]
                as AssemblyCopyrightAttribute;
            return copyright.Copyright;
        }

        public static string AboutString()
        {
            var copyright = OSUtilities.AssemblyCopyright();
            var version = OSUtilities.AssemblyVersion();
            string text = "vJoyIOFeeder for Gamoover by B. Maurin (njz3)\n";
            text += copyright;
            text += "\nVersion " + version.ToString();
            text += "\nRunning mode is " + vJoyManager.Config.Hardware.TranslatingModes.ToString();
            return text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appname">Name of application, like "app". .lnk and .exe will be added automatically</param>
        /// <param name="description"></param>
        public static void CreateStartupShortcut(string appname, string description)
        {
            var shell = new WshShell();
            var startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            var shortCutLinkFilePath = Path.Combine(startupFolderPath, appname + ".lnk");
            var windowsApplicationShortcut = (IWshShortcut)shell.CreateShortcut(shortCutLinkFilePath);
            windowsApplicationShortcut.Description = description;
            windowsApplicationShortcut.WorkingDirectory = Application.StartupPath;
            windowsApplicationShortcut.TargetPath = Application.ExecutablePath;
            windowsApplicationShortcut.IconLocation = appname + ".exe, 0";
            windowsApplicationShortcut.Save();
        }

        public static void DeleteStartupShortcut(string appname)
        {
            var startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            var shortCutLinkFilePath = Path.Combine(startupFolderPath, appname + ".lnk");

            // Remove shortcut
            if (System.IO.File.Exists(shortCutLinkFilePath)) {
                System.IO.File.Delete(shortCutLinkFilePath);
            }
        }
    }
}
