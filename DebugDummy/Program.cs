using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DebugDummy
{
    static class Program
    {
        static Process proc;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string solutionDir = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.IndexOf("DebugDummy")-1);

            ProcessStartInfo si;
            
#pragma warning disable 0162
            if (LuaDebugger.GlobalState.SettlersExe == "settlershok")
                si = new ProcessStartInfo("D:/Program Files (x86)/DEdK/extra2/bin/settlershok.exe");
            else
                si = new ProcessStartInfo("D:/Program Files (x86)/S6/extra1/bin/Settlers6_.exe", "-DevM");
#pragma warning restore 0162

            si.EnvironmentVariables["Path"] += ";" + solutionDir + "/dbgenv";
            si.UseShellExecute = false;
            proc = Process.Start(si);
            proc.WaitForExit();
            
            return;
        }
    }
}
