using BackForceFeeder.Configuration;
using BackForceFeeder.Managers;
using BackForceFeeder.Utils;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BackForceFeeder.Outputs
{
    /// <summary>
    /// Game Output manager
    /// </summary>
    public class OutputsManager
    {
        public Outputs CurrentOutputsAgent = null;
        protected List<Outputs> AllAgents = new List<Outputs>();
        protected RawMemoryM2OutputsAgent RawMemory;
        protected MAMEOutputsWinAgent MAMEWin;
        protected MAMEOutputsNetAgent MAMENet;


        public OutputsManager()
        {
            RawMemory = new RawMemoryM2OutputsAgent();
            MAMEWin = new MAMEOutputsWinAgent();
            MAMENet = new MAMEOutputsNetAgent();
            AllAgents.Add(RawMemory);
            AllAgents.Add(MAMEWin);
            AllAgents.Add(MAMENet);
            CurrentOutputsAgent = MAMEWin;
        }


        protected void Log(string text, LogLevels level = LogLevels.DEBUG)
        {
            Logger.Log("[OUTPUTS] " + text, level);
        }

        protected void LogFormat(LogLevels level, string text, params object[] args)
        {
            Logger.LogFormat(level, "[OUTPUTS] " + text, args);
        }

        protected bool Running = false;
        protected Thread ManagerThread = null;

        public void Start()
        {
            if (Running) return;
            if (ManagerThread != null) Stop();
            

            for (int i = 0; i<AllAgents.Count; i++) {
                //AllAgents[i].Start();
            }

            ManagerThread = new Thread(ManagerThreadMethod);
            Running = true;
            ManagerThread.Name = "Outputs Manager";
            ManagerThread.Priority = ThreadPriority.Normal;
                
            ManagerThread.Start();
        }
        public void Stop()
        {
            if (!Running) return;

            Running = false;
            if (ManagerThread == null)
                return;
            Thread.Sleep(100);
            ManagerThread.Join(1000);
            ManagerThread = null;

            for (int i = 0; i<AllAgents.Count; i++) {
                AllAgents[i].Stop();
            }
        }

        protected void ManagerThreadMethod()
        {
            OutputTypes prevOutputType = OutputTypes.NONE;
            while (Running) {
                try {
                    // Depending on current control set, pick right agent
                    var newOutputType = BFFManager.Config.CurrentControlSet.ProcessDescriptor.OutputType;
                    if (newOutputType!=prevOutputType) {
                        prevOutputType = newOutputType;
                        switch (newOutputType) {
                            case OutputTypes.RAW_MEMORY_READ:
                                // Start Rawmemory
                                RawMemory.Start();
                                CurrentOutputsAgent = RawMemory;
                                // Stop others
                                MAMEWin.Stop();
                                MAMENet.Stop();
                                break;
                            case OutputTypes.MAME_WIN:
                                // Start MAME Win
                                MAMEWin.Start();
                                CurrentOutputsAgent = MAMEWin;
                                // Stop others
                                RawMemory.Stop();
                                MAMENet.Stop();
                                break;
                            case OutputTypes.MAME_NET:
                                // Start MAME Net
                                MAMENet.Start();
                                CurrentOutputsAgent = MAMENet;
                                // Stop others
                                MAMEWin.Stop();
                                RawMemory.Stop();
                                break;
                        }
                    }
                } catch (Exception ex) {
                    Log("Outputs got exception " + ex.Message, LogLevels.IMPORTANT);
                }
                Thread.Sleep(64);
            }
            Log("Outputs manager terminated", LogLevels.IMPORTANT);
        }

        /// <summary>
        /// Get last lamps output from game
        /// </summary>
        /// <returns>-1 if error</returns>
        public Int32 GetLampsOutputs()
        {
            if (this.CurrentOutputsAgent!=null) {
                return this.CurrentOutputsAgent.LampsValue;
            } else {
                return -1;
            }
        }
        /// <summary>
        /// Get last drive output from game
        /// </summary>
        /// <returns>-1 if error</returns>
        public Int32 GetDriveboardOutputs()
        {
            if (this.CurrentOutputsAgent!=null) {
                return this.CurrentOutputsAgent.DriveValue;
            } else {
                return -1;
            }
        }
    }
}
