using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LuaDebugger
{
    public class StateRemovedEventArgs : EventArgs
    {
        public LuaState LuaState { get; protected set; }

        public StateRemovedEventArgs(LuaState ls)
        {
            this.LuaState = ls;
        }
    }

    public class LuaState
    {
        public UIntPtr L { get; protected set; }
        public string Name { get; protected set; }
        public Dictionary<string, LuaFile> LoadedFiles = new Dictionary<string, LuaFile>();
        public bool UpdateFileList = false;
        public bool UpdateAfterFileListRestore = false;
        public DebugEngine DebugEngine { get; protected set; }
        public DebugState CurrentState { get { return this.DebugEngine.CurrentState; } }
        public event EventHandler<StateRemovedEventArgs> OnStateRemoved;

        public StateView StateView;

        public LuaState(UIntPtr L, string name)
        {
            this.L = L;
            this.Name = name;
            this.DebugEngine = new DebugEngine(this);
        }

        public string EvaluateLua(string expression)
        {
            bool unfreeze = false;
            if (this.CurrentState == DebugState.Running)
            {
                unfreeze = true;
                if (!GameLoopHook.PauseGame())
                    return "Error: Game is busy!";
            }
            this.DebugEngine.RemoveHook();

            string asStatement = expression;
            expression = "return " + expression;

            string result = "";
            LuaResult err = BBLua.luaL_loadbuffer(this.L, expression, expression.Length, "from console");
            if (err == LuaResult.OK)
            {
                int stackTop = BBLua.lua_gettop(this.L);
                err = BBLua.lua_pcall(this.L, 0, -1, 0);
                int nResults = 1 + BBLua.lua_gettop(this.L) - stackTop;

                if(nResults == 1)
                    result = TosToString();
                else if (nResults > 1)
                {
                    string[] results = new string[nResults];
                    do
                    {
                        nResults--;
                        results[nResults] = TosToString(true);
                    } while (nResults != 0);

                    result = "(" + string.Join(", ", results) + ")";
                }
            }
            else
            {
                string parseErrExpr = TosToString();

                err = BBLua.luaL_loadbuffer(this.L, asStatement, asStatement.Length, "from console");
                if (err == LuaResult.OK)
                    err = BBLua.lua_pcall(this.L, 0, 0, 0); //statement -> no return values

                if (err != LuaResult.OK)
                    result = TosToString();
            }

            this.DebugEngine.SetHook();
            if (unfreeze)
                GameLoopHook.ResumeGame();

            return result;
        }

        public bool RunDelegateSafely(MethodInvoker dlg)
        {
            bool unfreeze = false;
            if (this.CurrentState == DebugState.Running)
            {
                unfreeze = true;
                if (!GameLoopHook.PauseGame())
                    return false;
            }
            this.DebugEngine.RemoveHook();

            dlg();

            this.DebugEngine.SetHook();
            if (unfreeze)
                GameLoopHook.ResumeGame();
            return true;
        }

        protected static Regex alphaNumeric = new Regex("^[a-zA-Z0-9_]*$");

        public string TosToString()
        {
            return TosToString(true, false, new Dictionary<IntPtr, bool>());
        }

        public string TosToString(bool noExpand)
        {
            return TosToString(true, noExpand, new Dictionary<IntPtr, bool>());
        }

        public string TosToString(bool popStack, bool noExpand, Dictionary<IntPtr, bool> printedTables)
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
                    result = "\"" + BBLua.toStringMarshal(this.L, -1) + "\"";
                    break;
                case LuaType.Function:
                    result = GetTosFunctionInfo();
                    break;
                case LuaType.Table:
                    if (noExpand)
                        goto default;

                    IntPtr tblPtr = BBLua.lua_topointer(this.L, -1);
                    if (printedTables.ContainsKey(tblPtr))
                    {
                        result = "<Table, recursion>";
                        break;
                    }

                    result = "{";
                    BBLua.lua_pushnil(this.L);
                    while (BBLua.lua_next(this.L, -2) != 0)
                    {
                        printedTables.Add(tblPtr, true);
                        string val = IndentMultiLine(TosToString(true, false, printedTables));
                        string key = TosToString(false, true, printedTables);
                        printedTables.Remove(tblPtr);

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
                    result += "\n}";
                    break;
                case LuaType.UserData:
                case LuaType.LightUserData:
                    IntPtr ptr = BBLua.lua_touserdata(this.L, -1);
                    result = "<" + type.ToString() + ", at 0x" + ptr.ToInt32().ToString("X") + ">";
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
            BBLua.lua_getinfo(this.L, ">nSf", memBlock); //fill structure and pop func back onto stack after removing it
            LuaDebugRecord ldr = (LuaDebugRecord)Marshal.PtrToStructure(memBlock, typeof(LuaDebugRecord));
            Marshal.FreeHGlobal(memBlock);

            string source;
            if (ldr.what == "C")
                source = "Game Engine at 0x" + BBLua.lua_tocfunction(this.L, -1).ToInt32().ToString("X");
            else
                source = '\b' + ldr.source + ':' + ldr.linedefined + '\b';//" (line " + ldr.linedefined + ")";

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

        protected string CreateFileString()
        {
            using (MemoryStream mem = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(mem, CompressionMode.Compress, true))
                {
                    BinaryWriter bw = new BinaryWriter(gZipStream);
                    bw.Write(this.LoadedFiles.Count);

                    foreach (KeyValuePair<string, LuaFile> kvp in this.LoadedFiles)
                    {
                        bw.Write(kvp.Key);
                        bw.Write(kvp.Value.Contents);
                    }
                    bw.Write(0);

                    mem.Position = 0;

                    var compressedData = new byte[mem.Length];
                    mem.Read(compressedData, 0, compressedData.Length);
                    return Convert.ToBase64String(compressedData);
                }
            }
        }

        protected void RestoreFromFileString(string data)
        {
            byte[] gZipBuffer = Convert.FromBase64String(data);
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(gZipBuffer, 0, gZipBuffer.Length);
                memoryStream.Position = 0;

                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    BinaryReader br = new BinaryReader(gZipStream);
                    int fileCnt = br.ReadInt32();
                    for (int i = 0; i < fileCnt; i++)
                    {
                        string filename = br.ReadString();
                        string fileContents = br.ReadString();
                        this.LoadedFiles.Add(filename, new LuaFile(filename, fileContents));
                    }
                }
            }
            this.UpdateAfterFileListRestore = true;
            this.UpdateFileList = true;
        }

        //returns false if game was busy
        public bool SaveLoadedFiles()
        {
            bool wasRunning = this.DebugEngine.CurrentState == DebugState.Running;
            if (wasRunning)
            {
                if (!GameLoopHook.PauseGame())
                    return false;
            }

            string data = CreateFileString();

            BBLua.lua_newtable(this.L);
            int i = 1;
            foreach (string substr in data.SplitBy(15000)) //the limit in shok seems to be at ~ 2^14, otherwise crashes savegame loading
            {
                BBLua.lua_pushstring(this.L, substr);
                BBLua.lua_rawseti(this.L, -2, i);
                i++;
            }
            BBLua.lua_setglobal(this.L, "_LuaDebugger_FileData");

            if (wasRunning)
                GameLoopHook.ResumeGame();

            return true;
        }

        //returns false if game was busy
        public bool RestoreLoadedFiles()
        {
            bool wasRunning = this.DebugEngine.CurrentState == DebugState.Running;
            if (wasRunning)
            {
                if (!GameLoopHook.PauseGame())
                    return false;
            }

            BBLua.lua_getglobal(this.L, "_LuaDebugger_FileData");
            if (BBLua.lua_type(this.L, -1) != LuaType.Table)
            {
                if (wasRunning)
                    GameLoopHook.ResumeGame();
                return true;
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 1; ; i++)
            {
                BBLua.lua_rawgeti(this.L, -1, i);
                if (BBLua.lua_type(this.L, -1) != LuaType.String)
                {
                    BBLua.lua_settop(this.L, -2);
                    break;
                }
                sb.Append(BBLua.toStringMarshal(this.L, -1));
                BBLua.lua_settop(this.L, -2);
            }

            if (wasRunning)
                GameLoopHook.ResumeGame();

            this.LoadedFiles.Clear();

            RestoreFromFileString(sb.ToString());

            return true;
        }

        public void RemovedByGame()
        {
            if (GlobalState.DebuggerWindow.InvokeRequired)
                GlobalState.DebuggerWindow.BeginInvoke((MethodInvoker)this.RemovedByGame);
            else
            {
                if (this.OnStateRemoved != null)
                    this.OnStateRemoved(this, new StateRemovedEventArgs(this));
            }
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
}
