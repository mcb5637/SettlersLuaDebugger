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
        protected delegate void Action(); //.net3 WHY?!?!
        protected Action fireStateChangedEvent;
        protected Action unfakeCallback = null;
        public int CurrentActiveFunction { get; private set; } = -1;

        public DebugEngine(LuaStateWrapper ls)
        {
            this.ls = ls;
            RegisterLibFunctions();
            ErrorHook.SetErrorHandler(ls.L, new ErrorHook.LuaErrorCaught(this.ErrorCaughtHook));
            this.fireStateChangedEvent = new Action(FireStateChangedEvent);
            SetHook();
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

        [LuaLibFunction("GetLocal")]
        public static int GetLocal(LuaState L)
        {
            int lvl = L.CheckInt(1);
            DebugInfo i = L.GetStackInfo(lvl, false);
            string name = L.CheckString(2);
            int l = 1;
            while (true)
            {
                string ln = L.GetLocalName(i, l);
                if (ln == null)
                    break;
                if (ln.Equals(name))
                {
                    L.GetLocal(i, l);
                    return 1;
                }
                l++;
            }
            L.PushDebugInfoFunc(i);
            l = 1;
            while (true)
            {
                string up = L.GetUpvalueName(-1, l);
                if (up == null)
                    break;
                if (up.Equals(name))
                {
                    L.GetUpvalue(-1, l);
                    return 1;
                }
            }
            return 0;
        }
        [LuaLibFunction("SetLocal")]
        public static int SetLocal(LuaState L)
        {
            int lvl = L.CheckInt(1);
            DebugInfo i = L.GetStackInfo(lvl, false);
            string name = L.CheckString(2);
            L.CheckAny(3);
            int l = 1;
            while (true)
            {
                string ln = L.GetLocalName(i, l);
                if (ln == null)
                    break;
                if (ln.Equals(name))
                {
                    L.PushValue(3);
                    L.SetLocal(i, l);
                    return 0;
                }
                l++;
            }
            L.PushDebugInfoFunc(i);
            l = 1;
            while (true)
            {
                string up = L.GetUpvalueName(-1, l);
                if (up == null)
                    break;
                if (up.Equals(name))
                {
                    L.PushValue(3);
                    L.SetUpvalue(-2, l);
                    return 0;
                }
            }
            return 0;
        }
        [LuaLibFunction("HandleXPCallErrorMessage")]
        public static int HandleXPCallErrorMessage(LuaState L)
        {
            return ErrorHook.ErrorCatcher(L);
        }

        protected static void DebugHook(LuaState L, DebugInfo i)
        {
            DebugEngine th = GlobalState.L2State[L.State].DebugEngine;
            if (i.Event == LuaHook.Call)
                th.callStack++;
            else if (i.Event == LuaHook.Ret || i.Event == LuaHook.TailRet)
            {
                th.callStack--;
                if (th.callStack == 0 && (th.CurrentRequest == DebugRequest.StepIn || th.CurrentRequest == DebugRequest.StepToLevel))
                {
                    th.CurrentRequest = DebugRequest.Resume;
                    th.CurrentState = DebugState.Running;
                    th.FireStateChangedEvent();
                }
            }
            else if (th.CurrentRequest == DebugRequest.Pause || th.CurrentRequest == DebugRequest.StepIn)
                th.NormalBreak();
            else if (th.CurrentRequest == DebugRequest.StepToLevel && th.callStack <= th.targetCallStackLevel)
                th.NormalBreak();
            else
            {
                List<Breakpoint> bpsAtLine;
                if (!th.lineToBP.TryGetValue(i.CurrentLine, out bpsAtLine))
                    return;
                foreach (Breakpoint bp in bpsAtLine)
                {
                    if (bp.File.Filename == i.Source)
                    {
                        th.NormalBreak();
                        break;
                    }
                }
            }
        }

        protected void ErrorCaughtHook(string errMessage)
        {
            if (this.BreakOnError)
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

                Thread.Sleep(10);
            }
            UnfakeIfNeccessary();
            GlobalState.FreezeCount--;

            this.CurrentState = DebugState.Running;

            if (this.CurrentRequest == DebugRequest.Resume)
                FireStateChangedEvent();
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
        }

        public void RemoveBreakpoint(Breakpoint bp)
        {
            List<Breakpoint> bpsAtLine = this.lineToBP[bp.Line];
            if (bpsAtLine.Count == 1)
                this.lineToBP.Remove(bp.Line);
            else
                bpsAtLine.Remove(bp);
        }

        public void ManualPause()
        {
            ManualPause(false);
        }

        public void ManualPause(bool silent)
        {
            this.surpressStateChangedEvent = silent;

            this.CurrentRequest = DebugRequest.Pause;

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

        public void SetHook()
        {
            ls.L.SetHook(DebugHook, LuaHookMask.Line | LuaHookMask.Call | LuaHookMask.Ret, 0);
        }

        public void RemoveHook()
        {
            ls.L.SetHook(null, LuaHookMask.None, 0);
        }

        protected void UnfakeIfNeccessary()
        {
            if (this.unfakeCallback != null)
            {
                this.unfakeCallback();
                this.unfakeCallback = null;
            }
        }

        public void FakeEnvironment(LuaFunctionInfo lfi, int lvl)
        {
            UnfakeIfNeccessary();
            CurrentActiveFunction = lvl;
            this.unfakeCallback = () => CurrentActiveFunction = -1;
        }

        public bool IsLocalOrUpvalueInActiveStack(string name)
        {
            int lvl = CurrentActiveFunction;
            if (lvl < 0)
                return false;
            DebugInfo i = ls.L.GetStackInfo(lvl, false);
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
            }
            ls.L.Pop(1);
            return false;
        }
    }
}
