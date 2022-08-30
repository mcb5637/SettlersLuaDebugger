using ICSharpCode.TextEditor.Document;
using LuaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LuaDebugger
{
    public class Breakpoint
    {
        public LuaFile File { protected set; get; }
        public int Line { protected set; get; }
        public Bookmark BpMark;

        public Breakpoint(LuaFile file, int line, Bookmark mark)
        {
            this.File = file;
            this.Line = line;
            this.BpMark = mark;
        }
    }

    public enum DebugState
    {
        Running,
        CaughtError,
        Paused
    }

    public enum DebugRequest
    {
        Resume,
        StepIn,
        StepToLevel,
        Pause
    }

    public class DebugStateChangedEventArgs : EventArgs
    {
        public LuaStateWrapper LuaState { get; protected set; }
        public DebugState State { get; protected set; }

        public DebugStateChangedEventArgs(DebugState newState, LuaStateWrapper ls)
        {
            this.State = newState;
            this.LuaState = ls;
        }
    }

    public class DebugEngine
    {
        protected LuaStateWrapper ls;
        protected Dictionary<int, List<Breakpoint>> lineToBP = new Dictionary<int, List<Breakpoint>>();

        protected int callStack = 0;
        protected int targetCallStackLevel = 0;

        public bool BreakOnError = true;
        public DebugState CurrentState { get; protected set; }
        public DebugRequest CurrentRequest { get; set; }
        public event EventHandler<DebugStateChangedEventArgs> OnDebugStateChange;
        public LuaStackTrace CurrentStackTrace { get; protected set; }
        public string CurrentError { get; protected set; }

        protected bool surpressStateChangedEvent = false;
        protected Action fireStateChangedEvent;
        protected Action ResetStackFunctionCB = null;
        public int CurrentActiveFunction { get; private set; } = -1;

        private readonly LinkedList<Action> ToExecuteInSHoKThread = new LinkedList<Action>();
        public bool HookActive { get; private set; }

        public DebugEngine(LuaStateWrapper ls)
        {
            this.ls = ls;
            RegisterLibFunctions();
            ErrorHook.SetErrorHandler(ls.L, new ErrorHook.LuaErrorCaught(this.ErrorCaughtHook));
            this.fireStateChangedEvent = new Action(FireStateChangedEvent);
            // set hook, but we cannot have breakpoints here already
        }

        protected void RegisterLibFunctions()
        {
            ls.L.Push("LuaDebugger");
            ls.L.NewTable();
            ls.L.RegisterFuncLib<DebugEngine>(-3);
            ls.L.SetTableRaw(ls.L.GLOBALSINDEX);
        }

        [LuaLibFunction("Log")]
        public static int LogCallback(LuaState L)
        {
            LuaStateWrapper w = GlobalState.L2State[L.State];
            L.PushValue(1);
            string text = w.TosToString();
            if (text.Contains("\n"))
                w.StateView.LuaConsole.AppendText("Log:");
            else
                text = "Log: " + text;
            w.StateView.LuaConsole.AppendText(text);
            return 0;
        }

        [LuaLibFunction("WriteTableToFile")]
        public static int WriteTableToFile(LuaState L)
        {
            string name = L.ToString(2);
            string file = L.ToString(1);
            L.PushValue(3);
            string txt = GlobalState.L2State[L.State].TosToString();
            File.WriteAllText(file, name + " = " + txt);
            return 0;
        }
        [LuaLibFunction("SearchLocal")]
        public static int SearchLocal(LuaState L)
        {
            DebugInfo i = null;
            if (L.IsNumber(1))
            {
                int lvl = L.CheckInt(1);
                i = L.GetStackInfo(lvl, true);
                if (i == null)
                    throw new LuaException("invalid stack level");
            }
            else
            {
                L.CheckType(1, LuaType.Function);
                L.PushValue(1);
            }
            if (L.IsCFunction(-1))
                throw new LuaException("not allowed to access locals/upvalues of c functions");
            string name = L.CheckString(2);
            int l = 1;
            if (i != null)
            {
                while (true)
                {
                    string ln = L.GetLocalName(i, l);
                    if (ln == null)
                        break;
                    if (ln.Equals(name))
                    {
                        L.GetLocal(i, l);
                        L.Push(l);
                        return 2;
                    }
                    l++;
                }
            }
            l = 1;
            while (true)
            {
                string up = L.GetUpvalueName(-1, l);
                if (up == null)
                    break;
                if (up.Equals(name))
                {
                    L.GetUpvalue(-1, l);
                    L.Push(-l);
                    return 2;
                }
                l++;
            }
            return 0;
        }
        [LuaLibFunction("GetLocal")]
        public static int GetLocal(LuaState L)
        {
            int lvl = L.CheckInt(1);
            DebugInfo i = L.GetStackInfo(lvl, true);
            if (i == null)
                throw new LuaException("invalid stack level");
            if (L.IsCFunction(-1))
                throw new LuaException("not allowed to access locals/upvalues of c functions");
            int lv = L.CheckInt(2);
            L.GetLocal(i, lv);
            L.Push(L.GetLocalName(i, lv));
            return 2;
        }
        [LuaLibFunction("SetLocal")]
        public static int SetLocal(LuaState L)
        {
            int lvl = L.CheckInt(1);
            DebugInfo i = L.GetStackInfo(lvl, true);
            if (i == null)
                throw new LuaException("invalid stack level");
            if (L.IsCFunction(-1))
                throw new LuaException("not allowed to access locals/upvalues of c functions");
            L.CheckAny(3);
            int lv = L.CheckInt(2);
            L.PushValue(3);
            L.SetLocal(i, lv);
            return 0;
        }
        [LuaLibFunction("GetUpvalue")]
        public static int GetUpvalue(LuaState L)
        {
            if (L.IsNumber(1))
            {
                int lvl = L.CheckInt(1);
                DebugInfo i = L.GetStackInfo(lvl, true);
                if (i == null)
                    throw new LuaException("invalid stack level");
            }
            else
            {
                L.CheckType(1, LuaType.Function);
                L.PushValue(1);
            }
            if (L.IsCFunction(-1))
                throw new LuaException("not allowed to access locals/upvalues of c functions");
            int lv = L.CheckInt(2);
            L.GetUpvalue(-1, lv);
            L.Push(L.GetUpvalueName(-2, lv));
            return 2;
        }
        [LuaLibFunction("SetUpvalue")]
        public static int SetUpvalue(LuaState L)
        {
            if (L.IsNumber(1))
            {
                int lvl = L.CheckInt(1);
                DebugInfo i = L.GetStackInfo(lvl, true);
                if (i == null)
                    throw new LuaException("invalid stack level");
            }
            else
            {
                L.CheckType(1, LuaType.Function);
                L.PushValue(1);
            }
            if (L.IsCFunction(-1))
                throw new LuaException("not allowed to access locals/upvalues of c functions");
            int lv = L.CheckInt(2);
            L.PushValue(3);
            L.SetUpvalue(-2, lv);
            return 0;
        }
        [LuaLibFunction("HandleXPCallErrorMessage")]
        public static int HandleXPCallErrorMessage(LuaState L)
        {
            return ErrorHook.ErrorCatcher(L);
        }
        [LuaLibFunction("IsHookActive")]
        public static int IsHookActive(LuaState L)
        {
            L.Push(GlobalState.L2State[L.State].DebugEngine.HookActive);
            return 1;
        }

#if S5
        protected unsafe static void DebugHook(LuaState50 L, LuaState50.LuaDebugRecord* i)
#else
        protected unsafe static void DebugHook(LuaState51 L, LuaState51.LuaDebugRecord* i)
#endif
        {
            DebugEngine th = GlobalState.L2State[L.State].DebugEngine;
            if (i->debugEvent == LuaHook.Call)
                th.callStack++;
            else if (i->debugEvent == LuaHook.Ret || i->debugEvent == LuaHook.TailRet)
            {
                th.callStack--;
                if (th.callStack == 0 && (th.CurrentRequest == DebugRequest.StepIn || th.CurrentRequest == DebugRequest.StepToLevel))
                {
                    th.CurrentRequest = DebugRequest.Resume;
                    th.CurrentState = DebugState.Running;
                    th.FireStateChangedEvent();
                }
            } // event == line
            else if (th.CurrentRequest == DebugRequest.Pause || th.CurrentRequest == DebugRequest.StepIn)
                th.NormalBreak();
            else if (th.CurrentRequest == DebugRequest.StepToLevel && th.callStack <= th.targetCallStackLevel)
                th.NormalBreak();
            else // request == resume
            {
                List<Breakpoint> bpsAtLine;
                if (!th.lineToBP.TryGetValue(L.HookGetLineNumber(i), out bpsAtLine))
                    return;
                L.HookFillInfo(i);
                foreach (Breakpoint bp in bpsAtLine)
                {
                    if (bp.File.Filename == ((IntPtr)i->source).MarshalToString())
                    {
                        th.NormalBreak();
                        break;
                    }
                }
            }
        }

        protected void ErrorCaughtHook(string errMessage)
        {
            if (this.BreakOnError && CurrentState != DebugState.CaughtError)
            {
                this.CurrentState = DebugState.CaughtError;
                this.CurrentError = errMessage;
                this.CurrentStackTrace = new LuaStackTrace(this.ls, 1); //skip error handler
                FreezeGame();
            }
            else
            {
                this.ls.StateView.Invoke((MethodInvoker)delegate
                {
                    this.ls.StateView.LuaConsole.AppendText("Lua Error: " + errMessage);
                });
            }
        }

        public void BreakFromGameEngine()
        {
            this.CurrentState = DebugState.Paused;
            this.CurrentStackTrace = new LuaStackTrace(this.ls, 1); //skip LuaDebugger.Break()
            callStack = CurrentStackTrace.Count + 1;
            SetHookForBreakpoints(true);
            FreezeGame();
        }

        protected void NormalBreak()
        {
            this.CurrentState = DebugState.Paused;
            this.CurrentStackTrace = new LuaStackTrace(this.ls);
            FreezeGame();
        }

        protected void FreezeGame()
        {
            this.CurrentRequest = DebugRequest.Pause;
            FireStateChangedEvent();

            GlobalState.FreezeCount++;
            while (this.CurrentRequest == DebugRequest.Pause)
            {
                //Application.DoEvents(); //can cause crashes
                FreezeMsgLoop.ProcessBasicEvents();

                CheckRunSafely();

                Thread.Sleep(10);
            }
            UnSetActiveStackFunctionIfNeccessary();
            GlobalState.FreezeCount--;

            this.CurrentState = DebugState.Running;

            if (this.CurrentRequest == DebugRequest.Resume)
            {
                FireStateChangedEvent();
                SetHookForBreakpoints(false);
            }
        }

        protected void FireStateChangedEvent()
        {
            if (surpressStateChangedEvent)
                return;

            if (GlobalState.DebuggerWindow.InvokeRequired)
                GlobalState.DebuggerWindow.BeginInvoke(this.fireStateChangedEvent);
            else
            {
                if (this.OnDebugStateChange == null)
                    GlobalState.DebuggerWindow.UpdateGUI();

                if (this.OnDebugStateChange != null)
                    this.OnDebugStateChange(this, new DebugStateChangedEventArgs(this.CurrentState, this.ls));
                else
                    MessageBox.Show("GUI not connected for Lua Event!", "WTF?");
            }
        }

        public void SetBreakpoint(Breakpoint bp)
        {
            List<Breakpoint> bpsAtLine;
            if (!this.lineToBP.TryGetValue(bp.Line, out bpsAtLine))
            {
                bpsAtLine = new List<Breakpoint>();
                this.lineToBP.Add(bp.Line, bpsAtLine);
            }
            bpsAtLine.Add(bp);
            RunSafely(() => SetHookForBreakpoints(false));
        }

        public void RemoveBreakpoint(Breakpoint bp)
        {
            List<Breakpoint> bpsAtLine = this.lineToBP[bp.Line];
            if (bpsAtLine.Count == 1)
                this.lineToBP.Remove(bp.Line);
            else
                bpsAtLine.Remove(bp);
            RunSafely(() => SetHookForBreakpoints(false));
        }

        public void ManualPause()
        {
            ManualPause(false);
        }

        public void ManualPause(bool silent)
        {
            this.surpressStateChangedEvent = silent;

            this.CurrentRequest = DebugRequest.Pause;

            RunSafely(() => SetHookForBreakpoints(true));
            for (int timeOut = 0; this.CurrentState != DebugState.Paused && timeOut < 10; timeOut++)
                Thread.Sleep(10);

            this.surpressStateChangedEvent = false;
        }

        public void StepLine()
        {
            StepLine(false);
        }

        public void StepLine(bool surpressEvent)
        {
            this.surpressStateChangedEvent = surpressEvent;

            this.targetCallStackLevel = this.callStack;
            this.CurrentRequest = DebugRequest.StepToLevel;

            while (this.CurrentState != DebugState.Paused && this.CurrentRequest == DebugRequest.Pause)
                Thread.Sleep(10);

            this.surpressStateChangedEvent = false;
        }

        public void StepOut()
        {
            StepOut(false);
        }

        public void StepOut(bool surpressEvent)
        {
            this.surpressStateChangedEvent = surpressEvent;

            this.targetCallStackLevel = this.callStack - 1;
            if (this.targetCallStackLevel < 0)
                this.targetCallStackLevel = 0;
            this.CurrentRequest = DebugRequest.StepToLevel;

            while (this.CurrentState != DebugState.Paused && this.CurrentRequest == DebugRequest.Pause)
                Thread.Sleep(10);

            this.surpressStateChangedEvent = false;
        }


        public void StepIn()
        {
            StepIn(false);
        }

        public void StepIn(bool surpressEvent)
        {
            this.surpressStateChangedEvent = surpressEvent;

            this.CurrentRequest = DebugRequest.StepIn;

            while (this.CurrentState != DebugState.Paused && this.CurrentRequest == DebugRequest.Pause)
                Thread.Sleep(10);

            this.surpressStateChangedEvent = false;
        }


        public void Resume()
        {
            Resume(false);
        }

        public void Resume(bool surpressEvent)
        {
            this.surpressStateChangedEvent = surpressEvent;

            this.CurrentRequest = DebugRequest.Resume;

            while (this.CurrentState != DebugState.Running && this.CurrentRequest == DebugRequest.Resume)
                Thread.Sleep(10);

            this.surpressStateChangedEvent = false;
        }

        public unsafe void SetHook()
        {
#if S5
            ((LuaState50)ls.L).SetHook(DebugHook, LuaHookMask.Line | LuaHookMask.Call | LuaHookMask.Ret, 0);
#else
            ((LuaState51)ls.L).SetHook(DebugHook, LuaHookMask.Line | LuaHookMask.Call | LuaHookMask.Ret, 0);
#endif
            HookActive = true;
        }

        public void RemoveHook()
        {
            ls.L.SetHook(null, LuaHookMask.None, 0);
            HookActive = false;
        }

        public void SetHookForBreakpoints(bool overide)
        {
            if (overide || lineToBP.Count > 0)
                SetHook();
            else
                RemoveHook();
        }

        protected void UnSetActiveStackFunctionIfNeccessary()
        {
            if (this.ResetStackFunctionCB != null)
            {
                this.ResetStackFunctionCB();
                this.ResetStackFunctionCB = null;
            }
        }

        public void SetActiveStackFunction(LuaFunctionInfo lfi, int lvl)
        {
            UnSetActiveStackFunctionIfNeccessary();
            CurrentActiveFunction = lvl;
            this.ResetStackFunctionCB = () => CurrentActiveFunction = -1;
        }

        public bool IsLocalOrUpvalueInActiveStack(string name)
        {
            int lvl = CurrentActiveFunction;
            if (lvl < 0)
                return false;
            DebugInfo i = ls.L.GetStackInfo(lvl, false);
            if (i == null)
                return false;
            int l = 1;
            while (true)
            {
                string ln = ls.L.GetLocalName(i, l);
                if (ln == null)
                    break;
                if (ln.Equals(name))
                {
                    return true;
                }
                l++;
            }
            l = 1;
            ls.L.PushDebugInfoFunc(i);
            while (true)
            {
                string up = ls.L.GetUpvalueName(-1, l);
                if (up == null)
                    break;
                if (up.Equals(name))
                {
                    ls.L.Pop(1);
                    return true;
                }
                l++;
            }
            ls.L.Pop(1);
            return false;
        }

        public void RunSafely(Action toRun)
        {
            lock (ToExecuteInSHoKThread)
            {
                ToExecuteInSHoKThread.AddLast(toRun);
            }
            if (CurrentState == DebugState.Running)
            {
                GameLoopHook.RunInGameThread(CheckRunSafely);
            }
        }
        internal void CheckRunSafely()
        {
            while (true)
            {
                Action r;
                lock (ToExecuteInSHoKThread)
                {
                    if (ToExecuteInSHoKThread.Count == 0)
                        break;
                    r = ToExecuteInSHoKThread.First.Value;
                    ToExecuteInSHoKThread.RemoveFirst();
                }
                r();
            }
        }
    }
}
