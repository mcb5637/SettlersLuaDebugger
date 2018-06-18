using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace DebugDummy
{

    [ComImport, Guid("00000016-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleMessageFilter
    {
        [PreserveSig]
        int HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo);

        [PreserveSig]
        int RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, int dwRejectType);

        [PreserveSig]
        int MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType);
    }

    public class MessageFilter : IOleMessageFilter
    {
        private const int Handled = 0, RetryAllowed = 2, Retry = 99, Cancel = -1, WaitAndDispatch = 2;

        int IOleMessageFilter.HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo)
        {
            return Handled;
        }

        int IOleMessageFilter.RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, int dwRejectType)
        {
            return dwRejectType == RetryAllowed ? Retry : Cancel;
        }

        int IOleMessageFilter.MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType)
        {
            return WaitAndDispatch;
        }

        public static void Register()
        {
            CoRegisterMessageFilter(new MessageFilter());
        }

        public static void Revoke()
        {
            CoRegisterMessageFilter(null);
        }

        private static void CoRegisterMessageFilter(IOleMessageFilter newFilter)
        {
            IOleMessageFilter oldFilter;
            CoRegisterMessageFilter(newFilter, out oldFilter);
        }

        [DllImport("Ole32.dll")]
        private static extern int CoRegisterMessageFilter(IOleMessageFilter newFilter, out IOleMessageFilter oldFilter);
    }

    static class Program
    {
        static Process proc;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MessageFilter.Register();

            string solutionDir = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.IndexOf("DebugDummy") - 1);

            ProcessStartInfo si;

#pragma warning disable 0162

            string s5KeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Blue Byte\The Settlers - Heritage of Kings";
            string s5ValueName = "InstallPath";
            string s6KeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Ubisoft\The Settlers 6\GameUpdate";
            string s6ValueName = "InstallDir";

            string s5Path = (string)Registry.GetValue(s5KeyName, s5ValueName, null) + "/extra2/bin/settlershok.exe";
            string s6Path = (string)Registry.GetValue(s6KeyName, s6ValueName, null) + "/extra1/bin/settlers6.exe";

            if (LuaDebugger.GlobalState.SettlersNr == 5)
                si = new ProcessStartInfo(s5Path, "-debugscript");
            else
                si = new ProcessStartInfo(s6Path, "-DISPLAYSCRIPTERRORS -DevM");
#pragma warning restore 0162

            si.EnvironmentVariables["Path"] += ";" + solutionDir + "/dbgenv";
            si.UseShellExecute = false;
            si.EnvironmentVariables.Add("ldbWaitForDebugger", "yes");
            proc = Process.Start(si);
            //wait for the lua dll to load
            bool dllNotYetLoaded = true;
            do
            {
                var dbgProc = GetProcess(proc.Id);
                if (dbgProc == null)
                    return; //process exited, etc..

                foreach (EnvDTE.Program p in dbgProc.Programs)
                {
                    if (p.Name.Contains("Managed"))
                    {
                        dbgProc.Attach();
                        dllNotYetLoaded = false;
                        break;
                    }
                }
                if (dllNotYetLoaded) System.Threading.Thread.Sleep(10);
            } while (dllNotYetLoaded);
            proc.WaitForExit();

            return;
        }

        private static EnvDTE.DTE currentDTE = null;
        private static EnvDTE.DTE GetDTE()
        {
            if (currentDTE != null)
                return currentDTE;

            EnvDTE.DTE dte = null;
            try
            {
                dte = Marshal.GetActiveObject("VisualStudio.DTE") as EnvDTE.DTE;
            }
            catch { }

            currentDTE = dte ?? throw new Exception("Failed to aquire DTE");
            return dte;
        }

        private static EnvDTE.Process GetProcess(int processID)
        {
            EnvDTE.DTE dte = GetDTE();

            foreach (EnvDTE.Process proc in dte.Debugger.LocalProcesses)
            {
                if (proc.ProcessID == processID)
                    return proc;
            }
            return null;
        }
    }
}
