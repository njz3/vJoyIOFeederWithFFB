using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using SharpDX.DirectInput;
using SharpDX.XInput;
using vJoyIOFeeder.FFBAgents;
using vJoyIOFeeder.IOCommAgents;
using vJoyIOFeeder.Utils;
using vJoyIOFeeder.vJoyIOFeederAPI;

namespace vJoyIOFeeder
{
    public static class Program
    {
        static ManagingThread Manager;
        static int Main(string[] args)
        {
            Manager = new ManagingThread();
            Manager.StartManagingThread();

            return 0;
        } // Main


    } // class Program
} // namespace FeederDemoCS
