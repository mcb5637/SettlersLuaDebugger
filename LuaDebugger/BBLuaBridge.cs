using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LuaDebugger
{
    [Flags]
    public enum LuaHookType
    {
        Nothing = 0,
        Call = 1,
        Return = 2,
        Line = 4,
        Count = 8
    };

    public enum LuaEvent
    {
        Call,
        Return,
        Line,
        Count,
        TailReturn
    }

    public enum LuaType
    {
        Nil,
        Boolean,
        LightUserData,
        Number,
        String,
        Table,
        Function,
        UserData,
        Thread
    }

    public enum LuaPseudoIndices
    {
        REGISTRYINDEX = (-10000),
        LUA_GLOBALSINDEX = (-10001)
    }

    public enum LuaResult
    {
        OK,
        ERRRUN,
        ERRFILE,
        ERRSYNTAX,
        ERRMEM,
        ERRERR
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct LuaStackRecord
    {
        [FieldOffset(0)]
        public LuaEvent debugEvent;
        [FieldOffset(20)]
        public int currentline;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LuaDebugRecord
    {
        public int debugEvent;
        public string name;	/* (n) */
        public string namewhat;	/* (n) `global', `local', `field', `method' */
        public string what;	/* (S) `Lua', `C', `main', `tail' */
        public string source;	/* (S) */
        public int currentline;	/* (l) */
        public int nups;		/* (u) number of upvalues */
        public int linedefined;	/* (S) */
#if S6
        int lastlinedefined;	/* (S) */               //added in lua 5.1
#endif
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 60)]
        public string short_src; /* (S) */

        /* private part */
        private int privateInt;  /* active function */
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = false, ThrowOnUnmappableChar = false)]
    public delegate void LuaDebugHook(UIntPtr L, IntPtr ptr);

    public static class BBLua
    {

        /*
        ** basic stack manipulation
        */

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_gettop(UIntPtr L);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_settop(UIntPtr L, int idx);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushvalue(UIntPtr L, int idx);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_remove(UIntPtr L, int idx);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_insert(UIntPtr L, int idx);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_replace(UIntPtr L, int idx);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_checkstack(UIntPtr L, int sz);


        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_xmove(UIntPtr from, UIntPtr to, int n);


        /*
        ** access functions (stack -> C)
        */


        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_isnumber(UIntPtr L, int idx);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_isstring(UIntPtr L, int idx);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_iscfunction(UIntPtr L, int idx);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_isuserdata(UIntPtr L, int idx);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaType lua_type(UIntPtr L, int idx);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern string lua_typename(UIntPtr L, LuaType tp);


        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_equal(UIntPtr L, int idx1, int idx2);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_rawequal(UIntPtr L, int idx1, int idx2);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_lessthan(UIntPtr L, int idx1, int idx2);


        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern double lua_tonumber(UIntPtr L, int idx);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_toboolean(UIntPtr L, int idx);

#if S5
        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern string lua_tostring(UIntPtr L, int idx);
#elif S6
        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern string lua_tolstring(UIntPtr L, int idx, int setNull);

        public static string lua_tostring(UIntPtr L, int idx)
        {
                return lua_tolstring(L, idx, 0);
        }
#endif
        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_strlen(UIntPtr L, int idx);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_tocfunction(UIntPtr L, int idx);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_touserdata(UIntPtr L, int idx);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_tothread(UIntPtr L, int idx);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_topointer(UIntPtr L, int idx);


        /*
        ** push functions (C -> stack)
        */

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushnil(UIntPtr L);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushnumber(UIntPtr L, double n);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushstring(UIntPtr L, string s);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushcclosure(UIntPtr L, IntPtr fn, int n);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushboolean(UIntPtr L, int b);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushlightuserdata(UIntPtr L, UIntPtr p);


        /*
        ** get functions (Lua -> stack)
        */

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_gettable(UIntPtr L, int idx);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawget(UIntPtr L, int idx);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawgeti(UIntPtr L, int idx, int n);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_newtable(UIntPtr L);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr lua_newuserdata(UIntPtr L, int sz);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_getmetatable(UIntPtr L, int objindex);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_getfenv(UIntPtr L, int idx);

        public static void lua_getglobal(UIntPtr L, string name)
        {
            BBLua.lua_pushstring(L, name);
            BBLua.lua_rawget(L, (int)LuaPseudoIndices.LUA_GLOBALSINDEX);
        }

        /*
        ** set functions (stack -> Lua)
        */

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_settable(UIntPtr L, int idx);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawset(UIntPtr L, int idx);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawseti(UIntPtr L, int idx, int n);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_setmetatable(UIntPtr L, int objindex);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_setfenv(UIntPtr L, int idx);

        public static void lua_setglobal(UIntPtr L, string name)
        {
            BBLua.lua_pushstring(L, name);
            BBLua.lua_insert(L, -2);
            BBLua.lua_rawset(L, (int)LuaPseudoIndices.LUA_GLOBALSINDEX);
        }


        /*
        ** `load' and `call' functions (load and run Lua code)
        */

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_call(UIntPtr L, int nargs, int nresults);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaResult lua_pcall(UIntPtr L, int nargs, int nresults, int errfunc);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaResult lua_cpcall(UIntPtr L, UIntPtr func, UIntPtr ud);
        /*
[DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
public static extern int   lua_load (UIntPtr L, lua_Chunkreader reader, UIntPtr dt,
                        string chunkname);


[DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
public static extern int lua_dump (UIntPtr L, lua_Chunkwriter writer, void *data);
        */

        /*
        ** coroutine functions
        */

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_yield(UIntPtr L, int nresults);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_resume(UIntPtr L, int narg);

        /*
        ** garbage-collection functions
        */

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_getgcthreshold(UIntPtr L);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_getgccount(UIntPtr L);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_setgcthreshold(UIntPtr L, int newthreshold);

        /*
        ** miscellaneous functions
        */


        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern string lua_version();


        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_error(UIntPtr L);


        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_next(UIntPtr L, int idx);


        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_concat(UIntPtr L, int n);


        /* AUX LIB */
        /*  
  [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
  public static extern void luaL_openlib (UIntPtr L, string libname,
                                 const luaL_reg *l, int nup);*/

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_getmetafield(UIntPtr L, int obj, string e);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_callmeta(UIntPtr L, int obj, string e);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_typerror(UIntPtr L, int narg, string tname);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_argerror(UIntPtr L, int numarg, string extramsg);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern string luaL_checklstring(UIntPtr L, int numArg, out int strLen);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern string luaL_optlstring(UIntPtr L, int numArg,
                                                   string def, out int strLen);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern double luaL_checknumber(UIntPtr L, int numArg);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern double luaL_optnumber(UIntPtr L, int nArg, double def);


        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_checkstack(UIntPtr L, int sz, string msg);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_checktype(UIntPtr L, int narg, int t);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_checkany(UIntPtr L, int narg);


        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_newmetatable(UIntPtr L, string tname);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_getmetatable(UIntPtr L, string tname);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr luaL_checkudata(UIntPtr L, int ud, string tname);


        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_where(UIntPtr L, int lvl);
        /*
[DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
public static extern int luaL_error (UIntPtr L, string fmt, ...);


[DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
public static extern int luaL_findstring (string st, string const lst[]);*/


        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_ref(UIntPtr L, int t);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_unref(UIntPtr L, int t, int refID);


        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_getn(UIntPtr L, int t);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_setn(UIntPtr L, int t, int n);


        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_loadfile(UIntPtr L, string filename);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_loadbuffer(UIntPtr L, string buff, int sz,
                                        string name);

        /*
        ** Compatibility macros and functions
        */


        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_dofile(UIntPtr L, string filename);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_dostring(UIntPtr L, string str);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_dobuffer(UIntPtr L, string buff, uint sz,
                                       string n);


        /*
         ** Debug funcs
         */



        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_getstack(UIntPtr L, int level, IntPtr sr);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_getinfo(UIntPtr L, string what, IntPtr sr);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern string lua_getlocal(UIntPtr L, IntPtr ar, int n);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern string lua_setlocal(UIntPtr L, IntPtr ar, int n);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern string lua_getupvalue(UIntPtr L, int funcindex, int n);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern string lua_setupvalue(UIntPtr L, int funcindex, int n);


        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_sethook(UIntPtr L, LuaDebugHook hookFn, LuaHookType mask, int count);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_gethook(UIntPtr L);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaHookType lua_gethookmask(UIntPtr L);

        [DllImport(GlobalState.LuaDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_gethookcount(UIntPtr L);
    }
}
