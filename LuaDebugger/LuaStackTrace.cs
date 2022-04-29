using LuaSharp;
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

        public LuaStackTrace(LuaStateWrapper ls) : this(ls, 0) { }

        public LuaStackTrace(LuaStateWrapper ls, int startLevel)
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
        public string FunctionName { get; protected set; }
        public string Source { get; protected set; }
        public int Line { get; protected set; }

        protected LuaStateWrapper ls;

        protected LuaFunctionInfo(LuaStateWrapper ls)
        {
            this.ls = ls;
        }

        public static LuaFunctionInfo ReadFunctionInfo(LuaStateWrapper ls, int level)
        {
            try
            {
                DebugInfo i = ls.L.GetStackInfo(level);

                LuaFunctionInfo lfi = new LuaFunctionInfo(ls);

                if (i.Source.Length > 1 && i.Source[0] != '=')
                {
                    lfi.Source = i.Source;
                    lfi.Line = i.CurrentLine;
                }
                else
                {
                    lfi.Source = "unavailable";
                    lfi.Line = 0;
                }

                if (i.What == "C")
                    lfi.FunctionName = "Game Engine (direct call)";
                else if (i.What == "main")
                    lfi.FunctionName = "Game Engine (code outside function)";
                else if (i.Name != "" && i.NameWhat != "")
                    lfi.FunctionName = i.NameWhat + " " + i.Name + "()";
                else if (i.What == "Lua" || i.What == "tail")
                    lfi.FunctionName = "Lua Code";

                return lfi;
            }
            catch (LuaException)
            {
                return null;
            }
        }
    }
}
