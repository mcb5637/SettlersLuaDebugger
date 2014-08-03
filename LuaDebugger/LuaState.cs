using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LuaDebugger
{
    public class LuaState
    {
        public UIntPtr L { get; protected set; }
        public string Name { get; protected set; }
        public Dictionary<string, LuaFile> LoadedFiles = new Dictionary<string, LuaFile>();
        public bool UpdateFileList = false;
        public DebugEngine DebugEngine { get; protected set; }
        public DebugState CurrentState { get { return this.DebugEngine.CurrentState; } }

        public StateView StateView;

        public LuaState(UIntPtr L, string name)
        {
            this.L = L;
            this.Name = name;
            this.DebugEngine = new DebugEngine(this);
        }

        public string EvaluateLua(string cmd)
        {
            bool unfreeze = false;
            if (this.CurrentState == DebugState.Running)
            {
                unfreeze = true;
                this.DebugEngine.ManualPause(true);
            }
            this.DebugEngine.RemoveHook();

            string fallback = cmd;
            cmd = "return " + cmd;

            BBLua.luaL_loadbuffer(this.L, cmd, cmd.Length, "from console");
            LuaResult
                err = BBLua.lua_pcall(this.L, 0, 1, 0);
            string result = TosToString();

            if (err != LuaResult.OK)
            {
                BBLua.luaL_loadbuffer(this.L, fallback, fallback.Length, "from console");
                LuaResult err2 = BBLua.lua_pcall(this.L, 0, 1, 0);
                if (err2 == LuaResult.OK)
                    result = TosToString();
                else
                    result = "As Expression: " + result + "\nAs Statement: " + TosToString();
            }

            this.DebugEngine.SetHook();
            if (unfreeze)
                this.DebugEngine.Resume();

            return result;
        }

        protected static Regex alphaNumeric = new Regex("^[a-zA-Z0-9_]*$");

        public string TosToString()
        {
            return TosToString(true, 20);
        }

        public string TosToString(bool popStack, int expandTables)
        {
            LuaType type = BBLua.lua_type(this.L, -1);
            string result;

            switch (type)
            {
                case LuaType.Nil:
                    result = "nil";
                    break;
                case LuaType.Boolean:
                    result = BBLua.lua_toboolean(this.L, -1).ToString();
                    break;
                case LuaType.Number:
                    result = BBLua.lua_tonumber(this.L, -1).ToString();
                    break;
                case LuaType.String:
                    result = "\"" + BBLua.lua_tostring(this.L, -1) + "\"";
                    break;
                case LuaType.Function:
                    return GetTosFunctionInfo(); // pops stack
                case LuaType.Table:
                    if (expandTables == 0)
                        goto default;

                    result = "{";
                    BBLua.lua_pushnil(this.L);
                    while (BBLua.lua_next(this.L, -2) != 0)
                    {
                        //read key first to check whether it is _G, and stop recursion ;)
                        BBLua.lua_insert(this.L, -2);
                        string key = TosToString(false, 0);
                        BBLua.lua_insert(this.L, -2);

                        if (key == "\"_G\"")
                        {
                            result += "\n    [\"_G\"] = <_G>,";
                            BBLua.lua_settop(this.L, -2); //remove val
                        }
                        else
                        {
                            string val = IndentMultiLine(TosToString(true, expandTables - 1));

                            if (key[0] == '\"')
                            {
                                string rawString = key.Substring(1, key.Length - 2);
                                if (alphaNumeric.IsMatch(rawString))
                                    result += "\n    " + rawString + " = " + val + ",";
                                else
                                    result += "\n    [" + key + "] = " + val + ",";
                            }
                            else
                                result += "\n    [" + key + "] = " + val + ",";
                        }
                    }
                    result += "\n}";
                    break;
                default:
                    result = "<" + type.ToString() + ">";
                    break;
            }

            if (popStack)
                BBLua.lua_settop(this.L, -2);
            return result;
        }

        protected string GetTosFunctionInfo()
        {
            IntPtr memBlock = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(LuaDebugRecord)));
            BBLua.lua_getinfo(this.L, ">nS", memBlock);
            LuaDebugRecord ldr = (LuaDebugRecord)Marshal.PtrToStructure(memBlock, typeof(LuaDebugRecord));
            Marshal.FreeHGlobal(memBlock);

            string source;
            if (ldr.what == "C")
                source = "Game Engine";
            else
                source = ldr.source + " (line " + ldr.linedefined + ")";

            return "<Function, defined in " + source + ">";
        }

        protected string IndentMultiLine(string input)
        {
            string[] lines = input.Split('\n');
            if (lines.Length < 2)
                return input;

            for (int i = 1; i < lines.Length; i++)
                lines[i] = "    " + lines[i];

            return string.Join("\n", lines);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public class LuaFile
    {
        public string Filename { protected set; get; }
        public string Contents { protected set; get; }
        public Dictionary<int, Breakpoint> Breakpoints = new Dictionary<int, Breakpoint>();
        public TextEditorControl Editor;
        public TreeNode Node;

        public LuaFile(string filename, string contents)
        {
            this.Filename = filename;
            this.Contents = contents;
        }

        public ArrowMark Arrow { get; set; }
    }

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
                TryFakeVar(varName, i, this.fakedLocals);
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
            lfi.Source = ldr.source;
            lfi.Line = ldr.currentline;

            if (ldr.what == "C")
                lfi.FunctionName = "Game Engine (direct call)";
            else if (ldr.name == null)
                lfi.FunctionName = "Game Engine (code outside function)";
            else
            {
                lfi.FunctionName = ldr.namewhat + " " + ldr.name + "()";
                lfi.CanFakeEnvironment = true;
            }

            return lfi;
        }
    }
}
