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
using System.Linq;
using LuaSharp;

namespace LuaDebugger
{
    public class StateRemovedEventArgs : EventArgs
    {
        public LuaStateWrapper LuaState { get; protected set; }

        public StateRemovedEventArgs(LuaStateWrapper ls)
        {
            this.LuaState = ls;
        }
    }

    public class LuaStateWrapper
    {
        public LuaState L { get; protected set; }
        public string Name { get; protected set; }
        public Dictionary<string, LuaFile> LoadedFiles = new Dictionary<string, LuaFile>();
        public bool UpdateFileList = false;
        public bool UpdateAfterFileListRestore = false;
        public bool ReloadedLuaFiles = false;
        public DebugEngine DebugEngine { get; protected set; }
        public DebugState CurrentState { get { return this.DebugEngine.CurrentState; } }
        public event EventHandler<StateRemovedEventArgs> OnStateRemoved;

        public StateView StateView;

        public LuaStateWrapper(LuaState L, string name)
        {
            this.L = L;
            this.Name = name;
            this.DebugEngine = new DebugEngine(this);
        }

        public void EvaluateLua(string expression, Action<string, string> onDone, bool uivarname = false)
        {
            DebugEngine.RunSafely(() =>
            {
                DebugEngine.RemoveHook();

                int top = L.Top;

                if (uivarname && DebugEngine.IsLocalOrUpvalueInActiveStack(expression))
                {
                    expression = $"LuaDebugger.GetLocal({DebugEngine.CurrentActiveFunction + 2}, '{expression}')";
                }
                string asStatement = expression;
                string asExpression = "return " + expression;

                string result;
                try
                {
                    try
                    {
                        L.LoadBuffer(asExpression, "from console");
                    }
                    catch (LuaException)
                    {
                        L.LoadBuffer(asStatement, "from console");
                    }
                    int stackTop = L.Top;
                    L.PCall(0, L.MULTIRETURN);
                    int nResults = 1 + L.Top - stackTop;

                    result = EvalCreateResult(nResults);
                }
                catch (LuaException e)
                {
                    result = e.ToString();
                }
                L.Top = top;

                DebugEngine.SetHook();
                onDone(result, expression);
            });
        }

        private string EvalCreateResult(int nResults)
        {
            if (nResults <= 0)
                return "";
            if (nResults == 1)
                return TosToString();
            string[] results = new string[nResults];
            do
            {
                nResults--;
                results[nResults] = TosToString(true);
            } while (nResults != 0);

            return "(" + string.Join(", ", results) + ")";
        }

        protected static Regex alphaNumeric = new Regex("^[a-zA-Z0-9_]*$");

        public string TosToString(bool noExpand = false)
        {
            return TosToString(true, noExpand, new Dictionary<IntPtr, bool>());
        }

        public string TosToString(bool popStack, bool noExpand, Dictionary<IntPtr, bool> printedTables)
        {
            LuaType type = L.Type(-1);
            string result;

            switch (type)
            {
                case LuaType.Nil:
                    result = "nil";
                    break;
                case LuaType.Boolean:
                    result = L.ToBoolean(-1).ToString().ToLower();
                    break;
                case LuaType.Number:
                    result = L.ToNumber(-1).ToString();
                    break;
                case LuaType.String:
                    result = "\"" + L.ToString(-1).Replace("\n", "\\n").Replace("\r", "\\r").Replace("\"", "\\\"") + "\"";
                    break;
                case LuaType.Function:
                    result = GetTosFunctionInfo();
                    break;
                case LuaType.Table:
                    if (noExpand)
                        goto default;

                    IntPtr tblPtr = L.ToPointer(-1);
                    if (printedTables.ContainsKey(tblPtr))
                    {
                        result = "<Table, recursion>";
                        break;
                    }

                    Dictionary<string, string> tcontents = new Dictionary<string, string>();
                    result = "{";
                    printedTables.Add(tblPtr, true);
                    foreach (LuaType _ in L.Pairs(-1))
                    {
                        string val = IndentMultiLine(TosToString(false, false, printedTables));
                        string key = L.ToDebugString(-2);
                        tcontents.Add(key, val);
                    }
                    foreach (string key in tcontents.Keys.OrderBy((x) => x))
                    {
                        string val = tcontents[key];
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
                    IntPtr ptr = L.ToUserdata(-1);
                    result = $"<{type}, at 0x{(uint)ptr:X}>";
                    break;
                default:
                    result = $"<{type}>";
                    break;
            }

            if (popStack)
                L.Pop(1);
            return result;
        }

        protected string GetTosFunctionInfo()
        {
            if (L.IsCFunction(-1))
            {
                return $"<function, defined in C: 0x{(uint)L.ToCFunction(-1):X}>";
            }
            int t = L.Top;
            L.PushValue(-1);
            DebugInfo d = L.GetFuncInfo();
            L.Top = t;
            return $"<function \b{d.Source}:{d.LineDefined}\b>";
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
            lock (GlobalState.GuiUpdateLock)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    using (GZipStream gZipStream = new GZipStream(mem, CompressionMode.Compress))
                    using (BinaryWriter bw = new BinaryWriter(gZipStream))
                    {
                        bw.Write(this.LoadedFiles.Count);
                        foreach (KeyValuePair<string, LuaFile> kvp in this.LoadedFiles)
                        {
                            bw.Write(kvp.Key);
                            bw.Write(kvp.Value.Contents);
                        }
                        bw.Write(0);
                        bw.Flush();
                    }
                    return Convert.ToBase64String(mem.ToArray());
                }
            }
        }

        protected void RestoreFromFileString(string data)
        {
            lock (GlobalState.GuiUpdateLock)
            {
                byte[] gZipBuffer = Convert.FromBase64String(data);
                using (MemoryStream memoryStream = new MemoryStream(gZipBuffer))
                using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                using (BinaryReader br = new BinaryReader(gZipStream))
                {
                    int fileCnt = br.ReadInt32();
                    for (int i = 0; i < fileCnt; i++)
                    {
                        string filename = br.ReadString();
                        string fileContents = br.ReadString();
                        if (!LoadedFiles.ContainsKey(filename))
                            this.LoadedFiles.Add(filename, new LuaFile(filename, fileContents));
                    }
                }
                this.UpdateAfterFileListRestore = true;
                this.UpdateFileList = true; 
            }
        }

        public void SaveLoadedFiles(Action done)
        {
            DebugEngine.RunSafely(() =>
            {
                string data = CreateFileString();

                L.Push("_LuaDebugger_FileData");
                L.NewTable();
                int i = 1;
                foreach (string substr in data.SplitBy(15000)) //the limit in shok seems to be at ~ 2^14, otherwise crashes savegame loading
                {
                    L.Push(substr);
                    L.SetTableRaw(-2, i);
                    i++;
                }
                L.SetTableRaw(L.GLOBALSINDEX);
                done();
            });
        }

        public void RestoreLoadedFiles(Action done)
        {
            DebugEngine.RunSafely(() =>
            {
                L.Push("_LuaDebugger_FileData");
                L.GetTableRaw(L.GLOBALSINDEX);
                if (L.Type(-1) != LuaType.Table)
                {
                    L.Pop(1);
                    done();
                    return;
                }

                StringBuilder sb = new StringBuilder();

                for (int i = 1; ; i++)
                {
                    L.GetTableRaw(-1, i);
                    if (L.Type(-1) != LuaType.String)
                    {
                        L.Pop(2);
                        break;
                    }
                    sb.Append(L.ToString(-1));
                    L.Pop(1);
                }

                RestoreFromFileString(sb.ToString());
                done();
            });
        }

        public void RemovedByGame()
        {
            if (GlobalState.DebuggerWindow.InvokeRequired)
                GlobalState.DebuggerWindow.BeginInvoke((MethodInvoker)this.RemovedByGame);
            else
            {
                OnStateRemoved?.Invoke(this, new StateRemovedEventArgs(this));
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
