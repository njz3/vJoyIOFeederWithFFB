using BackForceFeeder.BackForceFeeder;
using System;
using System.Collections.Generic;

namespace BackForceFeeder.Utils
{
    public static class CommandLine
    {
        #region Command line arguments

        public static bool ParseCommandLine(string[] commandLine, out Dictionary<string, object> outputArgs)
        {
            outputArgs = new Dictionary<string, object>();
            // Parse command line arguments and fill application settings
            for (int i = 0; i < commandLine.Length; i++) {
                switch (commandLine[i]) {
                    case "-c":
                    case "--controlset":
                        if ((i + 1) < commandLine.Length) {
                            outputArgs.Add("controlset", commandLine[i + 1]);
                            i++;
                        }
                        break;
                    case "-l":
                    case "--logfile":
                        if ((i + 1) < commandLine.Length) {
                            outputArgs.Add("logfile", true);
                            i++;
                        }
                        break;
                    case "-h":
                    case "--help":
                    case "/?":
                    default:
                        // Display help message
                        Console.WriteLine(OSUtilities.AboutString());
                        Console.WriteLine("Usage:");
                        Console.WriteLine(" -c --controlset <uniquename>: name to a " +
                            "unique configuration file");
                        Console.WriteLine(" -l --logfile: force log to DEBUG and output to file");
                        Console.WriteLine(" -h --help: display this message");

                        // Leave
                        return false;
                }
            }
            return true;
        }


        public static void ProcessOptions(Dictionary<string, object> args)
        {
            if (args.ContainsKey("logfile")) {
                // Enforce DEBUG, almost all verbose and dump to file
                BFFManager.Config.Application.LogLevel = LogLevels.DEBUG;
                BFFManager.Config.Application.DumpLogToFile = true;
                BFFManager.Config.Application.VerboseFFBManager = true;
                BFFManager.Config.Application.VerboseFFBManagerTorqueValues = true;
                BFFManager.Config.Application.VerboseSerialIO = true;
                BFFManager.Config.Application.VerbosevJoyFFBReceiver = true;
                BFFManager.Config.Application.VerbosevJoyManager = true;
            }

            if (args.ContainsKey("controlset")) {
                // Enforce control set (if it exists in the loaded files)
                var uniquename = args["controlset"] as string;
                if (uniquename==null) {
                    Logger.Log("Wrong parameter for control set unique name: " + args["controlset"].ToString());
                }
                var cs = BFFManager.Config.AllControlSets.ControlSets.Find(x => (x.UniqueName == uniquename));
                if (cs!=null) {
                    SharedData.Manager.ChangeCurrentControlSet(cs);
                    BFFManager.Config.Application.AutodetectControlSetAtRuntime = false;
                    Logger.Log("Force control set from command line " + uniquename);
                } else {
                    Logger.Log("Control set unique name not found: " + uniquename);
                }
            }
        }
        #endregion

    }
}
