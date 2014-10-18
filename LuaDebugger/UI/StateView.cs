using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using System.Globalization;

namespace LuaDebugger
{
    public partial class StateView : UserControl
    {
        public DebugState CurrentState
        {
            get { return this.debugEngine.CurrentState; }
        }

        protected LuaState ls;
        protected DebugEngine debugEngine = null;
        protected LuaFile currentFile;
        protected TreeNode mapScripts, internalScripts;
        protected int fileSaveTimeout = 5;

        public event EventHandler<DebugStateChangedEventArgs> OnDebugStateChange;

        public StateView()
        {
            InitializeComponent();
            LuaConsole.Location = new Point(0, 0);
            luaConsole_LocationChanged(this, new EventArgs());
        }

        public void InitState(LuaState state)
        {
            if (this.debugEngine != null)
                this.debugEngine.OnDebugStateChange -= debugEngine_OnDebugStateChange;


            ShowNoFileOpen();
            this.ls = state;
            this.debugEngine = ls.DebugEngine;
            this.debugEngine.OnDebugStateChange += debugEngine_OnDebugStateChange;
            state.StateView = this;
            this.LuaConsole.InitState(state);
            mapScripts = this.tvFiles.Nodes["MapScripts"];
            internalScripts = this.tvFiles.Nodes["Internal"];
        }

        void debugEngine_OnDebugStateChange(object sender, DebugStateChangedEventArgs e)
        {
            //todo
            if (this.OnDebugStateChange != null)
                this.OnDebugStateChange(this, e);
            
            if (e.State == DebugState.Running)
            {
                if (this.currentFile != null)
                {
                    this.currentFile.Arrow.IsEnabled = false;
                    this.currentFile.Editor.Invalidate();
                }
                this.errorView.Visible = false;
                this.stackTraceView.Visible = false;
            }
            else
            {
                this.stackTraceView.ShowStackTrace(this.debugEngine.CurrentStackTrace);
                this.stackTraceView.Visible = true;

                if (e.State == DebugState.CaughtError)
                {
                    this.errorView.Visible = true;
                    this.errorView.ErrorMessage = this.debugEngine.CurrentError;
                }
            }
        }

        protected void ShowStackLevel(int n)
        {
            LuaFunctionInfo lfi = this.debugEngine.CurrentStackTrace[n];

            if (this.ls.LoadedFiles.Count == 0 && this.fileSaveTimeout > 0) 
                this.ls.RestoreLoadedFiles();
                
            if (this.ls.UpdateFileList)
                UpdateView();

            LuaFile file;

            if (this.ls.LoadedFiles.TryGetValue(lfi.Source, out file))
            {
                file.Arrow.NormalLineNr = lfi.Line;
                file.Arrow.IsEnabled = true;
                SwitchToFile(file, lfi.Line);
                this.ls.DebugEngine.FakeEnvironment(lfi);
            }
            else
                ShowSourceUnavailable();
        }

        protected void CenterLineInEditor(TextEditorControl tec, int line)
        {
            if (line > 0)
                tec.ActiveTextAreaControl.ScrollTo(line - 1);
            //int centerOffset = (tec.Height / tec.ActiveTextAreaControl.TextArea.TextView.FontHeight + 1) / 2;
            // tec.ActiveTextAreaControl.sc
            //tec.Document.TotalNumberOfLines
        }

        protected void ShowSourceUnavailable()
        {
            scUpper.Panel2.Controls.Clear();
            this.currentFile = null;
            this.tvFiles.SelectedNode = null;
            scUpper.Panel2.Controls.Add(this.pnlNoSource);
        }

        protected void ShowNoFileOpen()
        {
            scUpper.Panel2.Controls.Clear();
            this.currentFile = null;
            this.tvFiles.SelectedNode = null;
            scUpper.Panel2.Controls.Add(this.pnlNoFileOpen);
        }

        public void UpdateView()
        {
            if (this.ls.UpdateFileList)
            {
                this.ls.UpdateFileList = false;
                this.fileSaveTimeout = 3;

                this.mapScripts.Nodes.Clear();
                this.internalScripts.Nodes.Clear();
                foreach (KeyValuePair<string, LuaFile> kvp in this.ls.LoadedFiles)
                {
                    string filename = kvp.Key;
                    string shortName = Path.GetFileName(filename);
                    TreeNode fileNode = new TreeNode(shortName);
                    fileNode.ToolTipText = filename;
                    fileNode.Tag = kvp.Value;
                    kvp.Value.Node = fileNode;
                    if (filename.StartsWith("Data\\Script\\", StringComparison.CurrentCultureIgnoreCase) || //S5
                        filename.StartsWith("Script\\", StringComparison.CurrentCultureIgnoreCase))         //S6
                        this.internalScripts.Nodes.Add(fileNode);
                    else
                        this.mapScripts.Nodes.Add(fileNode);

                    if (kvp.Value.Editor == null)
                        CreateEditorForFile(kvp.Value);
                }
                //this.tvFiles.Sort();
            }

            if (this.fileSaveTimeout >= 0)
            {
                this.fileSaveTimeout--;

                if (this.fileSaveTimeout == 0)
                {
                    if (this.ls.LoadedFiles.Count > 0)
                        this.ls.SaveLoadedFiles();
                    else
                        this.ls.RestoreLoadedFiles();
                }
            }
        }

        private void tvFiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            LuaFile file = e.Node.Tag as LuaFile;
            if (file == null)
                return;

            SwitchToFile(file, -1);
        }

        private void SwitchToFile(LuaFile file, int line)
        {
            this.tvFiles.SelectedNode = file.Node;
            if (this.currentFile == file)
            {
                CenterLineInEditor(file.Editor, line);
                file.Editor.Invalidate();
                return;
            }

            scUpper.Panel2.Controls.Clear();
            this.currentFile = file;

            if (this.debugEngine.CurrentState == DebugState.Running)
                file.Arrow.IsEnabled = false;
            else
                CenterLineInEditor(file.Editor, line);

            scUpper.Panel2.Controls.Add(file.Editor);
        }

        private void CreateEditorForFile(LuaFile file)
        {
            TextEditorControl tec = new TextEditorControl();
            file.Editor = tec;

            tec.Dock = DockStyle.Fill;
            tec.Text = file.Contents;
            tec.IsReadOnly = true;
            tec.SetHighlighting("Lua");
            tec.IsIconBarVisible = true;
            tec.Document.ReadOnly = true;
            tec.ActiveTextAreaControl.TextArea.IconBarMargin.MouseDown += IconBarMargin_MouseDown;
            tec.Document.BookmarkManager.Removed += bm_Removed;
            tec.ActiveTextAreaControl.TextArea.MouseUp += TextArea_MouseUp;
            tec.ActiveTextAreaControl.TextArea.MouseEnter += TextArea_MouseEnter;

            ArrowMark am = new ArrowMark(tec.Document, new TextLocation(0, 0));
            am.IsEnabled = false;
            file.Arrow = am;
            tec.Document.BookmarkManager.AddMark(am);
        }

        void TextArea_MouseEnter(object sender, EventArgs e)
        {
            (sender as TextArea).Select();
        }

        void TextArea_MouseUp(object sender, MouseEventArgs e)
        {
            string selected = this.currentFile.Editor.ActiveTextAreaControl.SelectionManager.SelectedText;
            if (selected != "" && !selected.Contains("(") && !selected.Contains("\n"))
                this.LuaConsole.RunCommand(selected);
        }


        private void StateView_Load(object sender, EventArgs e)
        {
            MySyntaxModeProvider rsmp = new MySyntaxModeProvider();
            HighlightingManager.Manager.AddSyntaxModeFileProvider(rsmp);
            this.stackTraceView.OnStackLevelClick += stackTraceView_OnStackLevelClick;
            //this.tvFiles.TreeViewNodeSorter = new MyNodeSorter(this.mapScripts);
        }

        void stackTraceView_OnStackLevelClick(object sender, StackTraceClickedEventArgs e)
        {
            ShowStackLevel(e.StackLevel);
        }

        void IconBarMargin_MouseDown(AbstractMargin sender, Point mousepos, MouseButtons mouseButtons)
        {
            if (mouseButtons != MouseButtons.Left)
                return;

            IconBarMargin marginObj = (IconBarMargin)sender;

            Rectangle viewRect = marginObj.TextArea.TextView.DrawingPosition;
            TextLocation logicPos = marginObj.TextArea.TextView.GetLogicalPosition(0, mousepos.Y - viewRect.Top);

            if (logicPos.Y >= 0 && logicPos.Y < marginObj.TextArea.Document.TotalNumberOfLines)
            {
                LineSegment l = marginObj.Document.GetLineSegment(logicPos.Y);

                string s = marginObj.Document.GetText(l);

                if (s.Trim().Length == 0)
                    return;

                int lineNr = logicPos.Y + 1;
                BookmarkManager bm = this.currentFile.Editor.Document.BookmarkManager;

                Bookmark mark = new BreakMark(marginObj.Document, logicPos);
                bm.AddMark(mark);
                Breakpoint bp = new Breakpoint(this.currentFile, lineNr, mark);
                this.debugEngine.SetBreakpoint(bp);
                this.currentFile.Breakpoints.Add(lineNr, bp);
                marginObj.TextArea.Refresh();
            }
        }

        void bm_Removed(object sender, BookmarkEventArgs e)
        {
            int lineNr = e.Bookmark.LineNumber + 1;
            if (this.currentFile.Breakpoints.ContainsKey(lineNr))
            {
                Breakpoint bp = this.currentFile.Breakpoints[lineNr];
                this.debugEngine.RemoveBreakpoint(bp);
                this.currentFile.Breakpoints.Remove(lineNr);
            }
        }

        public void PauseResume()
        {
            if (this.CurrentState == DebugState.Running)
                this.debugEngine.ManualPause();
            else
                this.debugEngine.Resume();
        }

        public void StepLine()
        {
            this.debugEngine.StepLine();
        }

        public void StepIn()
        {
            this.debugEngine.StepIn();
        }

        public void StepOut()
        {
            this.debugEngine.StepOut();
        }

        private void dbgSTBtn_Click(object sender, EventArgs e)
        {
            LuaStackTrace st = new LuaStackTrace(this.ls);
            MessageBox.Show(st.ToString());
        }

        private void luaConsole_LocationChanged(object sender, EventArgs e)
        {
            LuaConsole.Height = scSV.Panel2.Height - LuaConsole.Top;
        }

        private void scSV_Panel2_Resize(object sender, EventArgs e)
        {

            LuaConsole.Height = scSV.Panel2.Height - LuaConsole.Top;
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Lua files (*.lua)|*.lua|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
                this.LuaConsole.RunCommand("Script.Load(\"" + ofd.FileName.Replace('\\', '/') + "\")");
        }

        private void tvFiles_MouseEnter(object sender, EventArgs e)
        {
            (sender as TreeView).Select();
        }
    }

    public class MyNodeSorter : System.Collections.IComparer
    {
        protected TreeNode alwaysOnTop;
        protected StringComparer sc;

        public MyNodeSorter(TreeNode alwaysOnTop)
        {
            this.alwaysOnTop = alwaysOnTop;
            sc = StringComparer.Create(CultureInfo.InvariantCulture, false);
        }

        public int Compare(object x, object y)
        {
            TreeNode tx = (TreeNode)x;
            TreeNode ty = (TreeNode)y;
            if (tx.Equals(this.alwaysOnTop))
                return -1;
            else if (ty.Equals(this.alwaysOnTop))
                return 1;
            else
                return sc.Compare(tx.Text, ty.Text);
        }
    }
}
