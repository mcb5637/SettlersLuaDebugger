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
                    bool o = GlobalState.CatchErrors;
                    try
                    {
                        GlobalState.CatchErrors = false;
                        L.LoadBuffer(asExpression, "from console");
                    }
                    catch (LuaException)
                    {
                        GlobalState.CatchErrors = o;
                        L.LoadBuffer(asStatement, "from console");
                    }
                    GlobalState.CatchErrors = o;
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

        private static string FormatString(LuaState L, int i)
        {
            return "\"" + L.ToString(i).Replace("\n", "\\n").Replace("\r", "\\r").Replace("\"", "\\\"") + "\"";
        }
        private static string FormatFunc(LuaState L, int i)
        {
            if (L.IsCFunction(i))
            {
                return $"<function, defined in C: 0x{(uint)L.ToCFunction(i):X}>";
            }
            int t = L.Top;
            L.PushValue(i);
            DebugInfo d = L.GetFuncInfo();
            L.Top = t;
            return $"<function \b{d.Source}:{d.LineDefined}\b>";
        }
        public string TosToString(bool noExpand = false)
        {
            string r = L.ToDebugString(-1, noExpand ? 0 : 10, FormatString, FormatFunc, 0, null);
            L.Pop(1);
            return r;
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
