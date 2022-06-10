using LuaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LuaSharp
{
    internal static class Globals
    {
        public const string Lua50Dll = LuaDebugger.GlobalState.LuaDll;// "lua50/lua50.dll";
        public const string Lua51Dll = LuaDebugger.GlobalState.LuaDll;// "lua51/lua51.dll";
    }
}