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
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int LuaLoadHook(IntPtr L, IntPtr chunkreader, IntPtr data, IntPtr chunkname);

        public delegate void LuaErrorCaught(string message);

        static EasyHook.LocalHook hook;
        static EasyHook.LocalHook loadHook;

        static Dictionary<IntPtr, LuaErrorCaught> stateErrHandler = new Dictionary<IntPtr, LuaErrorCaught>();

        public static bool InstallHook()
        {
            hook = EasyHook.LocalHook.Create(EasyHook.LocalHook.GetProcAddress(GlobalState.LuaDll, "lua_pcall"), (LuaPcallHook)FakePcall, null);
            hook.ThreadACL.SetExclusiveACL(new int[] { }); // hook all threads
            loadHook = EasyHook.LocalHook.Create(EasyHook.LocalHook.GetProcAddress(GlobalState.LuaDll, "lua_load"), (LuaLoadHook)FakeLoad, null);
            loadHook.ThreadACL.SetExclusiveACL(new int[] { }); // hook all threads
            return true;
        }

        private const string ErrorHandlerRegKey = "LuaDebugger_ErrorHandler";

        public static void SetErrorHandler(LuaState L, LuaErrorCaught callback)
        {
            L.Push(ErrorHandlerRegKey);
            L.Push(ErrorCatcher);
            L.SetTableRaw(L.REGISTRYINDEX);
            stateErrHandler.Add(L.State, callback);
        }

        public static void RemoveErrorHandler(LuaState L)
        {
            stateErrHandler.Remove(L.State);
        }


        [DllImport(Globals.Lua50Dll, EntryPoint = "lua_load", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Lua_load(IntPtr l, IntPtr chunkreader, IntPtr data, IntPtr chunkname);

        [DllImport(Globals.Lua50Dll, EntryPoint = "lua_pcall", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Lua_pcall(IntPtr l, int nargs, int nres, int errfunc);

        static int FakeLoad(IntPtr L, IntPtr chunkreader, IntPtr data, IntPtr chunkname)
        {
            if (!GlobalState.L2State.TryGetValue(L, out LuaStateWrapper w))
                return Lua_load(L, chunkreader, data, chunkname);
            LuaState s = w.L;
            int code = Lua_load(L, chunkreader, data, chunkname);
            if (!GlobalState.CatchErrors)
            {
                return code;
            }
            if (code != 0)
            {
                ErrorCatcher(s);
            }
            return code;
        }

        static int FakePcall(IntPtr L, int nargs, int nresults, int errfunc)
        {
            if (!GlobalState.L2State.TryGetValue(L, out LuaStateWrapper w))
                return Lua_pcall(L, nargs, nresults, errfunc);
            LuaState s = w.L;
            if (!GlobalState.CatchErrors)
            {
                return Lua_pcall(L, nargs, nresults, errfunc);
            }

            s.Push(ErrorHandlerRegKey);
            s.GetTableRaw(s.REGISTRYINDEX);
            int ecpos = s.ToAbsoluteIndex(-nargs - 2);
            s.Insert(ecpos);

            int r = Lua_pcall(L, nargs, nresults, ecpos);

            if (r != 0)
            {
                s.Pop(1); // err msg
                if (nresults != s.MULTIRETURN)
                    for (int i = 0; i < nresults; i++)
                    s.Push();
            }

            s.Remove(ecpos);

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
