using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LuaDebugger
{

    public class LuaStackTrace
    {
        protected List<LuaFunctionInfo> lfiStack;
        public LuaFunctionInfo this[int n]
        {
            get { return this.lfiStack[n]; }
        }
        public int Count
        {
            get { return this.lfiStack.Count; }
        }

        public LuaStackTrace(LuaState ls) : this(ls, 0) { }

        public LuaStackTrace(LuaState ls, int startLevel)
        {
            lfiStack = new List<LuaFunctionInfo>();

            for (int level = startLevel; ; level++)
            {
                LuaFunctionInfo lfi = LuaFunctionInfo.ReadFunctionInfo(ls, level);
                if (lfi == null)
                    break;
                else
                    this.lfiStack.Add(lfi);
            }
        }

        public override string ToString()
        {
            string st = "";
            foreach (LuaFunctionInfo lfi in this.lfiStack)
                st += lfi.FunctionName + "\n";
            return st;
        }
    }

    public class LuaFunctionInfo
    {
        protected class FakeVar
        {
            public int Number { get; protected set; }
            public string VarName { get; protected set; }

            public FakeVar(int nr, string varName)
            {
                this.Number = nr;
                this.VarName = varName;
            }
        }

        public string FunctionName { get; protected set; }
        public string Source { get; protected set; }
        public int Line { get; protected set; }
        public bool CanFakeEnvironment = false;

        protected List<FakeVar> fakedLocals, fakedUpvalues;
        protected IntPtr funcInfo;
        protected LuaState ls;
        protected int upvaluesCnt;

        protected LuaFunctionInfo(IntPtr funcInfo, LuaState ls, int nups)
        {
            this.funcInfo = funcInfo;
            this.ls = ls;
            this.upvaluesCnt = nups;
        }

        ~LuaFunctionInfo()
        {
            Marshal.FreeHGlobal(this.funcInfo);
        }

        public void FakeG()
        {
            this.fakedLocals = new List<FakeVar>();
            this.fakedUpvalues = new List<FakeVar>();
            if (!this.CanFakeEnvironment)
                return;

            string varName;
            BBLua.lua_getinfo(ls.L, "f", this.funcInfo); // 'f': pushes func onto stack

            for (int i = 1; ; i++)
            {
                varName = BBLua.lua_getupvalue(ls.L, -1, i);
                if (varName == null) break;
                TryFakeVar(varName, i, this.fakedUpvalues);
            }

            BBLua.lua_settop(ls.L, -2); //remove func from stack

            for (int i = 1; ; i++)
            {
                varName = BBLua.lua_getlocal(ls.L, this.funcInfo, i);
                if (varName == null) break;
                if (varName.Length > 0 && varName[0] != '(')
                    TryFakeVar(varName, i, this.fakedLocals);
                else
                    BBLua.lua_settop(ls.L, -2); //remove value
            }
        }

        protected void TryFakeVar(string varName, int n, List<FakeVar> memory)
        {
            BBLua.lua_getglobal(ls.L, varName);
            bool fakePossible = BBLua.lua_type(ls.L, -1) == LuaType.Nil;
            BBLua.lua_settop(ls.L, -2); //remove nil
            if (fakePossible)
            {
                memory.Add(new FakeVar(n, varName));
                BBLua.lua_setglobal(ls.L, varName);
            }
            else
            {
                BBLua.lua_settop(ls.L, -2); //remove value
            }
        }

        public void UnFakeG()
        {
            if (!this.CanFakeEnvironment)
                return;

            BBLua.lua_getinfo(ls.L, "f", this.funcInfo); // 'f': pushes func onto stack

            foreach (FakeVar fv in this.fakedUpvalues)
            {
                GetVarAndCleanG(fv.VarName);
                string vn = BBLua.lua_setupvalue(ls.L, -2, fv.Number);
            }

            BBLua.lua_settop(ls.L, -2); //remove func

            foreach (FakeVar fv in this.fakedLocals)
            {
                GetVarAndCleanG(fv.VarName);
                string vn = BBLua.lua_setlocal(ls.L, this.funcInfo, fv.Number);
            }
        }

        protected void GetVarAndCleanG(string varName)
        {
            BBLua.lua_getglobal(ls.L, varName); //fetch value

            BBLua.lua_pushnil(ls.L);
            BBLua.lua_setglobal(ls.L, varName); //remove from _G
        }

        public static LuaFunctionInfo ReadFunctionInfo(LuaState ls, int level)
        {
            IntPtr memBlock = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(LuaDebugRecord)));
            if (BBLua.lua_getstack(ls.L, level, memBlock) == 0)
                return null;
            BBLua.lua_getinfo(ls.L, "nSlu", memBlock);
            LuaDebugRecord ldr = (LuaDebugRecord)Marshal.PtrToStructure(memBlock, typeof(LuaDebugRecord));

            LuaFunctionInfo lfi = new LuaFunctionInfo(memBlock, ls, ldr.nups);

            if (ldr.source.Length > 1 && ldr.source[0] != '=')
            {
                lfi.Source = ldr.source;
                lfi.Line = ldr.currentline;
            }
            else
            {
                lfi.Source = "unavailable";
                lfi.Line = 0;
            }

            if (ldr.what == "C")
                lfi.FunctionName = "Game Engine (direct call)";
            else if (ldr.what == "main")
                lfi.FunctionName = "Game Engine (code outside function)";
            else if (ldr.name != "" && ldr.namewhat != "")
                lfi.FunctionName = ldr.namewhat + " " + ldr.name + "()";
            else if (ldr.what == "Lua" || ldr.what == "tail")
                lfi.FunctionName = "Lua Code";


            if (ls.LoadedFiles.ContainsKey(lfi.Source))
                lfi.CanFakeEnvironment = true;

            return lfi;
        }
    }
}
