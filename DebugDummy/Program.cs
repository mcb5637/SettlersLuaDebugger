using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
            if (LuaDebugger.GlobalState.SettlersExe == "settlershok")
                si = new ProcessStartInfo("C:/Program Files (x86)/Ubisoft/Blue Byte/DIE SIEDLER - Das Erbe der Könige - Gold Edition/extra2/bin/settlershok.exe", "-debugscript");
            else
                si = new ProcessStartInfo("D:/Program Files (x86)/S6/extra1/bin/Settlers6_.exe", "-DISPLAYSCRIPTERRORS -DevM");
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
                    if (p.Name.Contains("Managed"))
                    {
                        dbgProc.Attach();
                        dllNotYetLoaded = false;
                        break;
                    }
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
            string[] vsDTEs = new string[] 
            { 
                "VisualStudio.DTE.11.0",        //VS2012
                "VisualStudio.DTE.10.0",        //VS2010
                "VisualStudio.DTE.12.0"         //VS2013
            };
            
            foreach (string dteKey in vsDTEs)
            {
                try
                {
                    dte = Marshal.GetActiveObject(dteKey) as EnvDTE.DTE;
                }
                catch { }
            }
            if (dte == null)
                throw new Exception("Failed to aquire DTE");

            currentDTE = dte;
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
