using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace vJoyIOFeeder.Utils
{
    /// <summary>
    /// High resolution timer on Windows
    /// </summary>
    public class MultimediaTimer
    {
        /// <summary>
        /// Reference high resolution timer
        /// </summary>
        static public readonly Stopwatch RefTimer = Stopwatch.StartNew();
        /// <summary>
        /// double to convert a tick to ms
        /// </summary>
        static public readonly double HRTick2Ms = 1.0e3 / Stopwatch.Frequency;
        /// <summary>
        /// Period of the timer
        /// </summary>
        public readonly int Period_ms;


        /// <summary>
        /// Current tick number, incremented every period.
        /// </summary>
        public UInt64 Tick { get; protected set; }

        /// <summary>
        /// The HRTimer event
        /// </summary>
        public class EventArgs : System.EventArgs
        {
            /// <summary>
            /// Start time stamp
            /// </summary>
            public long StartTimestamp_hrtick;
            /// <summary>
            /// Current time stamp
            /// </summary>
            public long CurrentTimestamp_hrtick;
            public TimeSpan CurrentTime;
            /// <summary>
            /// Tick number
            /// </summary>
            public ulong Tick;
            /// <summary>
            /// Overrun previous execution
            /// </summary>
            public bool OverrunOccured;
            /// <summary>
            /// Time of last execution
            /// </summary>
            public TimeSpan LastExecutionTime;
            /// <summary>
            /// To string
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return String.Format("{0:##0.000000} ms [@{1}, {2}]", (CurrentTimestamp_hrtick - StartTimestamp_hrtick) * 1.0e3 / Stopwatch.Frequency, Tick, OverrunOccured);
            }
        }

        public delegate void TimerDelegate(object sender, EventArgs args);
        public TimerDelegate Handler;

        #region Dll imports
        /// <summary>
        /// Name of the Windows Multimedia library that manage tick period
        /// </summary>
        const string LIBWINMM = "winmm.dll";
        [StructLayout(LayoutKind.Sequential)]
        public struct TimeCaps
        {
            public UInt32 wPeriodMin;
            public UInt32 wPeriodMax;
        };
        [DllImport(LIBWINMM)]
        static protected extern int timeBeginPeriod(int msec);
        [DllImport(LIBWINMM)]
        static protected extern int timeEndPeriod(int msec);
        [DllImport(LIBWINMM, SetLastError = true)]
        static extern UInt32 timeGetDevCaps(ref TimeCaps timeCaps, UInt32 sizeTimeCaps);

        /// <summary>
        /// LPTIMECALLBACK: Delegate to be used by timeSetEvent
        /// </summary>
        /// <param name="timerId">id of timer returned by timeSetEvent</param>
        /// <param name="msg">Reserved</param>
        /// <param name="user">Value given as 4th parameter of timeSetevent</param>
        /// <param name="dw1">Reserved</param>
        /// <param name="dw2">Reserved</param>
        protected delegate void _LPTimeCallbackType(int timerId, int msg, IntPtr user, int dw1, int dw2);
        /// <summary>
        /// Used to store internal timer callback
        /// </summary>
        protected _LPTimeCallbackType _LPTimerCallBack;
        /// <summary>
        /// Timer type
        /// </summary>
        [Flags]
        protected enum TimerType : int
        {
            /// <summary>
            /// the handler is called once after timer expiration
            /// </summary>
            TIME_ONESHORT = 0x0000,
            /// <summary>
            /// the handler is called periodically
            /// </summary>
            TIME_PERIODIC = 0x0001,
            /// <summary>
            /// (default) Call the function on timer expiration 
            /// </summary>
            TIME_CALLBACK_FUNCTION,
            /// <summary>
            /// The 'SetEvent' function is called on timer expiration
            /// </summary>
            TIME_CALLBACK_EVENT_SET = 0x0010,
            /// <summary>
            /// The 'PulseEvent' function is called on timer expiration
            /// </summary>
            TIME_CALLBACK_EVENT_PULSE = 0x0020,
            /// <summary>
            /// prevent event from occuring after the timeKillEvent function
            /// is called
            /// </summary>
            TIME_KILL_SYNCHRONOUS = 0x0100
        }
        /// <summary>
        /// Set a timer event
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="resolution"></param>
        /// <param name="handler"></param>
        /// <param name="user"></param>
        /// <param name="eventType"></param>
        /// <returns></returns>
        [DllImport(LIBWINMM, EntryPoint = "timeSetEvent")]
        static protected extern int timeSetEvent(int delay, int resolution, _LPTimeCallbackType handler, IntPtr user, int eventType);
        /// <summary>
        /// Kill a timer event
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [DllImport(LIBWINMM, EntryPoint = "timeKillEvent")]
        static protected extern int timeKillEvent(int id);


        #endregion Dll imports


        /// <summary>
        /// TimerId
        /// </summary>
        protected int _TimerId = -1;
        /// <summary>
        /// reentry lock on the timerevent
        /// </summary>
        protected object _Lock = new object();
        protected TimeSpan _TickPeriod;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="period_ms"></param>
        /// <param name="type"></param>
        public MultimediaTimer(int period_ms)
        {
            this.Tick = 0;
            this.Period_ms = period_ms;
            // Define tick period
            this._TickPeriod = TimeSpan.FromMilliseconds(Period_ms);
            // save callback to prevent the garbage collector to release it
            this._LPTimerCallBack = m_LPTimerCallBack;
        }
        ~MultimediaTimer()
        {
            Stop();
        }


        /// <summary>
        /// Start the timer operation
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            SetTickGranularityOnWindows();
            // Save time instant when we started
            _EventArg.StartTimestamp_hrtick = RefTimer.ElapsedTicks;
            // create multimedia time event, it will start it
            this._TimerId = timeSetEvent(this.Period_ms, 0,
                _LPTimerCallBack, IntPtr.Zero, (int)TimerType.TIME_PERIODIC);
            return this._TimerId != 0;
        }

        /// <summary>
        /// Stop the timer
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            // Terminate multimedia time event
            int err = timeKillEvent(_TimerId);
            // release callbacks for garbage collector
            this._LPTimerCallBack = null;
            this.Handler = null;
            // Ensure all callbacks are drained before leaving to caller
            for (int i = 0; i < 10; i++) {
                Thread.Sleep(this.Period_ms);
            }
            // Restore OS granularity
            RestoreTickGranularityOnWindows();
            return err==0;
        }
        public void Pause()
        {
            m_OnHold = true;
        }
        public void Resume()
        {
            m_OnHold = false;
        }

        /// <summary>
        /// OnHold state of the scheduler
        /// </summary>
        protected volatile bool m_OnHold = false;
        /// <summary>
        /// Semaphore lock to avoid multiple call
        /// </summary>
        protected int _TickLock = 0;
        protected bool _SkipFirstExecutionOverRun = true;
        /// <summary>
        /// Event argument to pass to handlers
        /// </summary>
        private EventArgs _EventArg = new EventArgs();

        /// <summary>
        /// Multimedia Timer Call back
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <param name="user"></param>
        /// <param name="dw1"></param>
        /// <param name="dw2"></param>
        protected void m_LPTimerCallBack(int id, int msg, IntPtr user, int dw1, int dw2)
        {
            if (Thread.CurrentThread.Name == null) {
                Thread.CurrentThread.Name = "RT Thread";
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
#if DEBUG_CPUAFFINITY
                // Lock to CPU 0
                SetThreadProcessorAffinity(0);
#endif // DEBUG_CPUAFFINITY
            }


            // Check whether timer is running or paused
            if (m_OnHold) {
                return;
            }

            // Check whether a tick is already running
            var oldValue = Interlocked.CompareExchange(ref _TickLock, 1, 0);
            if (1 == oldValue) {
                // Tick already running - skip this run as we are probably
                // entering an overrun!
                return;
            }

            // Timings for tick work
            TimeSpan beginTick = RefTimer.Elapsed;
            // Increase tick number for this run
            Tick++;
            // Save event args
            _EventArg.Tick = Tick;
            _EventArg.CurrentTimestamp_hrtick = RefTimer.ElapsedTicks;
            _EventArg.CurrentTime = beginTick;

            // Get list of handlers
            var h = this.Handler;
            if (h != null) {
                // Do some work
                h(this, _EventArg);
            }

            // Get effective run time
            TimeSpan endTick = RefTimer.Elapsed;
            // Free semaphore
            Interlocked.Decrement(ref _TickLock);

            // Detect overrun
            _EventArg.LastExecutionTime = endTick - beginTick;
            // If it is the first execution, then skip overrun detection
            if (_SkipFirstExecutionOverRun) {
                _SkipFirstExecutionOverRun = false;
            } else if (_EventArg.LastExecutionTime > this._TickPeriod) {
                // Remeber overrun occured
                _EventArg.OverrunOccured = true;

                /*
                var handler = TickOverrunHandler;
                if (handler!=null) {
                    handler(diff);
                }*/
            }
        }


        /// <summary>
        /// Change OS granularity for process scheduling to x ms on windows
        /// </summary>
        public static void SetTickGranularityOnWindows(int ms = 1)
        {
            const int TIMERR_NOERROR = 0;
            const int TIMERR_NOCANDO = 97;
            TimeCaps caps = new TimeCaps();
            var res = timeGetDevCaps(ref caps, (uint)Marshal.SizeOf(caps));
            Logger.Log("[TIMER] Timer caps = " + caps.wPeriodMin + " -> " + caps.wPeriodMax, LogLevels.DEBUG);
            if (ms<caps.wPeriodMin) {
                Logger.Log("[TIMER] Unable to set process tick granularity to " + ms + ", minimum is " + caps.wPeriodMin, LogLevels.IMPORTANT);
            }
            // Define process priority and granularity for the whole process
            switch (timeBeginPeriod(ms)) {
                case TIMERR_NOERROR:
                    Logger.Log("[TIMER] Set Timer " + ms + "ms", LogLevels.DEBUG);
                    break;
                case TIMERR_NOCANDO:
                    throw (new Exception("ERROR: Cannot set timer period"));
            }
        }

        /// <summary>
        /// Restore OS granularity for process scheduling on windows
        /// </summary>
        public static void RestoreTickGranularityOnWindows(int ms = 1)
        {
            Logger.Log("[TIMER] Restore Timer " + ms + "ms", LogLevels.DEBUG);
            // Restore OS granularity
            timeEndPeriod(ms);
        }




        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static protected extern int GetCurrentProcessorNumber();
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static protected extern int GetCurrentThreadId();
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static protected extern bool GetProcessAffinityMask(IntPtr currentProcess, ref Int64 lpProcessAffinityMask, ref Int64 lpSystemAffinityMask);

        static protected readonly int ProcessorCount = Environment.ProcessorCount;

        /// <summary>
        /// Define CPU affinity for current thread using OS's threading 
        /// capabilities. This may not work on all platforms.
        /// </summary>
        /// <param name="cpus">[in] list of integers that maps to a CPU number
        /// </param>
        static protected void SetThreadProcessorAffinity(params int[] cpus)
        {
            if (cpus == null)
                throw new ArgumentNullException("cpus");
            if (cpus.Length == 0)
                throw new ArgumentException("You must specify at least one CPU.", "cpus");

            // Supports up to 64 processors. Build up CPU bitmask
            long cpuMask = 0;
            foreach (int cpu in cpus) {
                if (cpu < 0 || cpu >= ProcessorCount)
                    throw new ArgumentException("Invalid CPU number.");

                cpuMask |= 1L << cpu;
            }

            // Ensure managed thread is linked to OS thread; does nothing on 
            // default host in current .Net versions
            Thread.BeginThreadAffinity();

            // Find the ProcessThread for this thread.
            ProcessThread thread = Process.GetCurrentProcess().Threads.Cast<ProcessThread>()
                                       .Where(t => t.Id == GetCurrentThreadId()).Single();
            // Set the thread's processor affinity
            thread.ProcessorAffinity = (IntPtr)cpuMask;
        }
    }
}
