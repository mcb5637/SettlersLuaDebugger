using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace LuaDebugger
{
    static class ErrorHook
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate LuaResult LuaPcallHook(UIntPtr L, int nargs, int nresults, int errfunc);

        public delegate void LuaErrorCaught(string message);

        static LuaPcallHook pcallHook;
        static LuaCFunc errorHandler;

        static IntPtr errorHandlerPtr;

        static Dictionary<UIntPtr, LuaErrorCaught> stateErrHandler = new Dictionary<UIntPtr, LuaErrorCaught>();

        public static bool InstallHook()
        {
            ErrorHook.errorHandler = new LuaCFunc(ErrorHook.ErrorCatcher);
            ErrorHook.errorHandlerPtr = Marshal.GetFunctionPointerForDelegate(ErrorHook.errorHandler);

            ErrorHook.pcallHook = new LuaPcallHook(ErrorHook.FakePcall);
            IntPtr pcallHookPtr = Marshal.GetFunctionPointerForDelegate(ErrorHook.pcallHook);
            
            return ImportPatcher.ReplaceIATEntry(GlobalState.LuaDll, "lua_pcall", pcallHookPtr);
        }

        public static void SetErrorHandler(UIntPtr L, LuaErrorCaught callback)
        {
            stateErrHandler.Add(L, callback);
        }

        public static void RemoveErrorHandler(UIntPtr L)
        {
            stateErrHandler.Remove(L);
        }

        static LuaResult FakePcall(UIntPtr L, int nargs, int nresults, int errfunc)
        {
            /*if (GlobalState.DebuggerWindow != null && !GlobalState.DebuggerWindow.InvokeRequired)
                MessageBox.Show("WAT");*/
            //StartMenu.S00_ToSingleplayerMenu=function()return lol[9];end
            if (!GlobalState.CatchErrors)
                return BBLua.lua_pcall(L, nargs, nresults, errfunc);

            BBLua.lua_pushcclosure(L, ErrorHook.errorHandlerPtr, 0);
            BBLua.lua_insert(L, -nargs - 2);

            LuaResult res;

            if (nresults >= 0)
                res = BBLua.lua_pcall(L, nargs, nresults, -nargs - 2);
            else  //LUA_MULTRET
            {
                int stackBefore = BBLua.lua_gettop(L);
                res = BBLua.lua_pcall(L, nargs, nresults, -nargs - 2);
                int stackNow = BBLua.lua_gettop(L);

                if (res == LuaResult.OK)
                    nresults = stackNow - stackBefore + 1;
                else
                    nresults = 0;
            }

            if (res != LuaResult.OK)
            {
                BBLua.lua_settop(L, -2); //remove errormsg

                for (int i = 0; i < nresults; i++)  //push dummy returns, hopefully shok accepts this
                    BBLua.lua_pushnil(L);
            }
            BBLua.lua_remove(L, -nresults - 1); //remove error handler

            return LuaResult.OK;
        }

        static int ErrorCatcher(UIntPtr L) //could be moved into DebugEngine, but this would cost perfomance to fetch the correct delegate for each pcall
        {
            string errMsg = BBLua.lua_tostring(L, -1);
            BBLua.lua_settop(L, -2);

            LuaErrorCaught callback;
            if (stateErrHandler.TryGetValue(L, out callback))
                callback(errMsg);

            return 0;
        }

    }
}
