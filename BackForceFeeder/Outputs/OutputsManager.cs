using BackForceFeeder.Configuration;
using BackForceFeeder.Inputs;
using BackForceFeeder.BackForceFeeder;
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
        public OutputsAgent CurrentOutputsAgent = null;
        protected List<OutputsAgent> AllAgents = new List<OutputsAgent>();
        protected RawMemoryM2OutputsAgent RawMemory;
        protected MAMEOutputsWinAgent MAMEWin;
        protected MAMEOutputsNetAgent MAMENet;

        public List<RawOutput> RawOutputs = new List<RawOutput>();

        public const int MAXOUTPUTS = 16;

        /// <summary>
        /// Lamps from game
        /// </summary>
        public UInt32 GameLamps { get; protected set; } = 0;
        /// <summary>
        /// Drive from game
        /// </summary>
        public UInt32 GameDriveBoard { get; protected set; } = 0;
        /// <summary>
        /// Last updated game outputs
        /// </summary>
        protected UInt64 _LastUpdatedGameOutputsValues;
        /// <summary>
        /// Combined value (lamps+driveboard) for all game outputs
        /// </summary>
        public UInt64 GameOutputsValues { get; protected set; } = 0;

        /// <summary>
        /// Combined value for all output states (send to IO board)
        /// </summary>
        public UInt64 RawOutputsStates { get; protected set; } = 0;


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

        public void Initialize(int outputs)
        {
            if (outputs>MAXOUTPUTS) {
                outputs = MAXOUTPUTS;
            }
            RawOutputs.Clear();
            for (int i = 0; i<outputs; i++) {
                var rawinput = new RawOutput();
                rawinput.RawOutputIndex = i;
                RawOutputs.Add(rawinput);
            }
            ClearAll();
        }

        /// <summary>
        /// In case control set is changed, clear all mapped buttons and
        /// shifters states
        /// </summary>
        public void ClearAll()
        {
            if (this.CurrentOutputsAgent!=null) {
                this.CurrentOutputsAgent.ClearAll();
            }
            for (int i = 0; i<RawOutputs.Count; i++) {
                RawOutputs[i].UpdateValue(false);
            }
            this.GameOutputsValues = 0;
            this.RawOutputsStates = 0;
            this.GameLamps = 0;
            this.GameDriveBoard = 0;
            this._LastUpdatedGameOutputsValues = (ulong)0xcafebabe;
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
                    var newOutputType = BFFManager.CurrentControlSet.ProcessDescriptor.OutputType;
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
        protected Int32 GetLampsValue()
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
        protected Int32 GetDriveboardValue()
        {
            if (this.CurrentOutputsAgent!=null) {
                return this.CurrentOutputsAgent.DriveValue;
            } else {
                return -1;
            }
        }


        protected UInt32 MapGameLampsToRawOutputs(UInt32 gameLampOutputs, List<RawOutputDB> rawoutputbitmap)
        {
            UInt32 rawOutputsStates = 0;
            // Decode lamps: use mapping to set raw bits accordingly
            for (int idxbit = 0; idxbit<rawoutputbitmap.Count; idxbit++) {
                // Single bit value of the lamp : on/off state
                var rawLampBitValue = (gameLampOutputs & (1<<idxbit))!=0;
                // List of final bit position(s) in digital output word
                var bitsToChange = rawoutputbitmap[idxbit].MappedRawOutputBit;
                for (int idxout = 0; idxout<bitsToChange.Count; idxout++) {
                    // Get single final bit position
                    int finalbitpos = bitsToChange[idxout];
                    // Raw state value with inverted logic
                    bool state;
                    if (rawoutputbitmap[idxbit].IsInvertedLogic)
                        state = !rawLampBitValue;
                    else {
                        state = rawLampBitValue;
                    }


                    var bitmask = (uint)(1<<finalbitpos);
                    // Set or clear bit depending on logic
                    if (state) {
                        rawOutputsStates |= bitmask;
                    } else {
                        rawOutputsStates &= (uint)~bitmask;
                    }

                }
            }
            return rawOutputsStates;
        }
        protected UInt32 MapGameDriveboardToRawOutputs(UInt32 lampOutputs, List<RawOutputDB> rawoutputbitmap)
        {
            UInt32 rawOutputsStates;
            // No mapping for driveboard data, just copy it straight from game
            rawOutputsStates = lampOutputs;
            return rawOutputsStates;
        }
        public bool UpdateOutput()
        {
            bool stt = false;
            UInt64 gameOutputsValues = 0;
            int lamps = this.GetLampsValue();
            if (lamps>=0) {
                // Detect change
                if (lamps!=this.GameLamps) {
                    // Save new value
                    this.GameLamps = (UInt32)lamps;
                    Log("Lamps=" + GameLamps.ToString("X"), LogLevels.INFORMATIVE);
                    // Map lamps to raw outputs
                    gameOutputsValues = (UInt64)lamps;

                }
            }

            // Driveboard outputs (if game provides such data)
            int drivebd = this.GetDriveboardValue();
            if (drivebd>=0) {
                // Detect change
                if (drivebd!=GameDriveBoard) {
                    // Save new value
                    GameDriveBoard = (UInt32)drivebd;
                    Log("Drive=" + GameDriveBoard.ToString("X"), LogLevels.INFORMATIVE);

                    // Always use M2PAC mode: save value from game in bits 8-15, but it will be overwritten
                    // depending on the FFB mode selected.

                    // Set new bits 8-15 for driveboard
                    gameOutputsValues |= (UInt64)(drivebd&0xFF)<<8;
                }
            }

            // Now map outputs if a change is detected
            if (gameOutputsValues!=_LastUpdatedGameOutputsValues) {
                _LastUpdatedGameOutputsValues = gameOutputsValues;
                GameOutputsValues = gameOutputsValues;
                stt = true;
                // Clear all first
                RawOutputsStates = 0;
                // Now set individual bits according to output state after filtering
                for (int i = 0; i<this.RawOutputs.Count; i++) {
                    bool val = (gameOutputsValues&(UInt64)(1<<i)) != 0;
                    var output = this.RawOutputs[i];
                    // Refresh output (perform filtering)
                    output.UpdateValue(val);
                    // Read state and set mapped bits
                    if (output.State) {
                        for (int j = 0; j<output.Config.MappedRawOutputBit.Count; j++) {
                            var rawidx = output.Config.MappedRawOutputBit[j];
                            RawOutputsStates |= ((UInt64)1<<rawidx);
                        }
                    }
                }
            }
            return stt;
        }

        public void Enforce(int idx, bool newvalue)
        {
            if (idx<0 || idx>=this.RawOutputs.Count)
                return;
            if (newvalue)
                GameOutputsValues |= ((UInt64)1<<idx);
            else
                GameOutputsValues &= ~((UInt64)1<<idx);

            // Force game output
            this.RawOutputs[idx].UpdateValue(newvalue);

            // Rebuilt current state
            RawOutputsStates = 0;
            for (int i = 0; i<this.RawOutputs.Count; i++) {
                var output = this.RawOutputs[i];
                // Read state and set mapped bits
                if (output.State) {
                    for (int j = 0; j<output.Config.MappedRawOutputBit.Count; j++) {
                        var rawidx = output.Config.MappedRawOutputBit[j];
                        RawOutputsStates |= ((UInt64)1<<rawidx);
                    }
                }
            }
        }
    }
}
