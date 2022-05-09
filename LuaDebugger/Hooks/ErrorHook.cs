using LuaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace LuaDebugger
{
    static class ErrorHook
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int LuaPcallHook(IntPtr L, int nargs, int nresults, int errfunc);

        public delegate void LuaErrorCaught(string message);

        static EasyHook.LocalHook hook;

        static Dictionary<IntPtr, LuaErrorCaught> stateErrHandler = new Dictionary<IntPtr, LuaErrorCaught>();

        public static bool InstallHook()
        {
            LuaPcallHook pcallHook = new LuaPcallHook(ErrorHook.FakePcall);

            hook = EasyHook.LocalHook.Create(EasyHook.LocalHook.GetProcAddress(GlobalState.LuaDll, "lua_pcall"), pcallHook, null);
            hook.ThreadACL.SetExclusiveACL(new int[] { }); // hook all threads
            return true;
        }

        public static void SetErrorHandler(LuaState L, LuaErrorCaught callback)
        {
            stateErrHandler.Add(L.State, callback);
        }

        public static void RemoveErrorHandler(LuaState L)
        {
            stateErrHandler.Remove(L.State);
        }

        static int FakePcall(IntPtr L, int nargs, int nresults, int errfunc)
        {
            LuaState s = GlobalState.L2State[L].L;
            if (!GlobalState.CatchErrors)
            {
                return s.PCall_Debug(nargs, nresults, errfunc);
            }
            int t = s.Top;

            s.Push(ErrorCatcher);
            int ecpos = s.ToAbsoluteIndex(-nargs - 2);
            s.Insert(ecpos);

            t = s.Top;
            int r = s.PCall_Debug(nargs, nresults, ecpos);
            t = s.Top;

            if (r != 0)
            {
                s.Pop(1); // err msg
                if (nresults != s.MULTIRETURN)
                    for (int i = 0; i < nresults; i++)
                    s.Push();
            }

            s.Remove(ecpos);
            t = s.Top;

            return 0;
        }

        internal static int ErrorCatcher(LuaState L)
        {
            string errMsg = L.ToString(-1);

            if (stateErrHandler.TryGetValue(L.State, out LuaErrorCaught callback))
                callback(errMsg);

            return 1;
        }
    }
}
