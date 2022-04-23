using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Linq;

namespace LuaDebugger
{
    public static class DebuggerDllExports
    {

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // Ignore missing resources
            if (args.Name.Contains(".resources"))
                return null;

            // check for assemblies already loaded
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (assembly != null)
                return assembly;

            // Try to load by filename - split out the filename of the full assembly name
            // and append the base path of the original assembly (ie. look in the same dir)
            string filename = args.Name.Split(',')[0] + ".dll".ToLower();

            string asmFile = Path.GetTempPath() + "luaDebugger-yoq/" + filename;

            try
            {
                return System.Reflection.Assembly.LoadFrom(asmFile);
            }
            catch (Exception)
            {
                return null;
            }
        }

        static DebuggerDllExports()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve; // set own function to load dlls windows cant find on its own

            if (Environment.GetEnvironmentVariable("ldbWaitForDebugger") == "yes")
            {
                while (!Debugger.IsAttached)
                    Thread.Sleep(1);

                GlobalState.IsInVisualStudio = true;
            }

            if (!Application.ExecutablePath.ToLower().Contains(GlobalState.SettlersExe))
            {
                MessageBox.Show("This DLL only works with " + GlobalState.SettlersExe);
                Environment.Exit(0);
            }

            GlobalState.SettlersThread = Thread.CurrentThread;
            GlobalState.SettlersWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
            //WindowStyle settlersWndStyle = (WindowStyle)WinAPI.GetWindowLong(GlobalState.SettlersWindowHandle, WinAPI.GWL_STYLE);
            //settlersWndStyle |= WindowStyle.WS_MINIMIZEBOX | WindowStyle.WS_SIZEBOX | WindowStyle.WS_MAXIMIZEBOX;
            //WinAPI.SetWindowLong(GlobalState.SettlersWindowHandle, WinAPI.GWL_STYLE, (uint)settlersWndStyle);

            //TickHook.InstallHook();
            GameLoopHook.InstallHook();
            ErrorHook.InstallHook();
            Thread uiThread = new Thread(new ThreadStart(DbgThread.RunMessageLoop));
            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();
            GlobalState.UIThread = uiThread;
        }

        static int s5StateCount = 0;
        //S5
        [DllExport("_AddLuaState@4", CallingConvention = CallingConvention.StdCall)]
        public static void AddLuaState(UIntPtr L)
        {
            s5StateCount++;
            string name = s5StateCount == 1 ? "Main Menu" : "Game";

            lock (GlobalState.GuiUpdateLock)
            {
                LuaState ls = new LuaState(L, name);
                GlobalState.L2State.Add(L, ls);
                GlobalState.LuaStates.Add(ls);
                GlobalState.UpdateStatesView = true;
            }
        }

        //S6
        [DllExport("_AddLuaState@8", CallingConvention = CallingConvention.StdCall)]
        public static void AddLuaStateS6(UIntPtr L, string name)
        {
            lock (GlobalState.GuiUpdateLock)
            {
                TextInfo ti = Thread.CurrentThread.CurrentCulture.TextInfo;
                LuaState ls = new LuaState(L, ti.ToTitleCase(name));
                GlobalState.L2State.Add(L, ls);
                GlobalState.LuaStates.Add(ls);
                GlobalState.UpdateStatesView = true;
            }
        }

        [DllExport("_RemoveLuaState@4", CallingConvention = CallingConvention.StdCall)]
        public static void RemoveLuaState(UIntPtr L)
        {
            lock (GlobalState.GuiUpdateLock)
            {
                LuaState ls = GlobalState.L2State[L];
                ls.RemovedByGame();
                GlobalState.L2State.Remove(L);
                GlobalState.LuaStates.Remove(ls);
                ErrorHook.RemoveErrorHandler(L); // todo: refactor into luastate?
                GlobalState.UpdateStatesView = true;
            }
        }

        [DllExport("_NewFile@16", CallingConvention = CallingConvention.StdCall)]
        public static void NewFile(UIntPtr L, string filename, IntPtr content, int contentLen)
        {
            if (filename == null)
                return;//Immediate Action

            LuaState ls = GlobalState.L2State[L];


            lock (GlobalState.GuiUpdateLock)
            {
                if (ls.LoadedFiles.ContainsKey(filename))
                    ls.LoadedFiles.Remove(filename);
                string fileContents = Marshal.PtrToStringAnsi(content, contentLen);
                ls.LoadedFiles.Add(filename, new LuaFile(filename, fileContents));
                ls.UpdateFileList = true;
            }

        }

        [DllExport("_Show@0", CallingConvention = CallingConvention.StdCall)]
        public static void Show()
        {
            //MessageBox.Show("show");
        }

        [DllExport("_Hide@0", CallingConvention = CallingConvention.StdCall)]
        public static void Hide()
        {
            //MessageBox.Show("hide");
        }

        [DllExport("_Break@4", CallingConvention = CallingConvention.StdCall)]
        public static void Break(UIntPtr L)
        {
            GlobalState.L2State[L].DebugEngine.BreakFromGameEngine();
        }

        [DllExport("_ShowExecuteLine@0", CallingConvention = CallingConvention.StdCall)]
        public static void ShowExecuteLine()
        {
            //MessageBox.Show("ShowExecuteLine");
        }

        //S6
        [DllExport("_IsBreaked@0", CallingConvention = CallingConvention.StdCall)]
        public static int IsBreaked()
        {
            return GlobalState.FreezeCount;
        }
    }
}
