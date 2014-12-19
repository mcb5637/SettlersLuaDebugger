using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace LuaDebugger
{
    public static class GlobalState
    {
#if S5
        public const string LuaDll = "S5Lua5.dll";
        public const string SettlersExe = "settlershok";
#elif S6
        public const string LuaDll = "BBLua51.dll";
        public const string SettlersExe = "settlers6";
#endif

        public static IntPtr SettlersWindowHandle;
        public static Thread UIThread;
        public static Thread SettlersThread;
        public static DbgMain DebuggerWindow;
        public static List<LuaState> LuaStates = new List<LuaState>();
        public static Dictionary<UIntPtr, LuaState> L2State = new Dictionary<UIntPtr, LuaState>();
        public static int FreezeCount = 0;
        public static bool UpdateStatesView = false;
        public static object GuiUpdateLock = new object();

        public static bool CatchErrors = true;


        public static bool IsInVisualStudio = false;

    }
}
