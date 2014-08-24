using ICSharpCode.TextEditor.Document;
using System;
using System.Collections.Generic;
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
        public DebugState State { get; protected set; }

        public DebugStateChangedEventArgs(DebugState newState)
        {
            this.State = newState;
        }
    }

    public class DebugEngine
    {
        protected LuaState ls;
        protected Dictionary<int, List<Breakpoint>> lineToBP = new Dictionary<int, List<Breakpoint>>();
        protected LuaDebugHook hookFnPtr; //prevent the gc from deleting the delegate

        protected int callStack = 0;
        protected int targetCallStackLevel = 0;

        public DebugState CurrentState { get; protected set; }
        public DebugRequest CurrentRequest { get; set; }
        public event EventHandler<DebugStateChangedEventArgs> OnDebugStateChange;
        public LuaStackTrace CurrentStackTrace { get; protected set; }
        public string CurrentError { get; protected set; }

        protected delegate void Action(); //.net3 WHY?!?!
        protected Action fireStateChangedEvent;
        protected Action unfakeCallback = null;

        public DebugEngine(LuaState ls)
        {
            this.ls = ls;
            this.hookFnPtr = new LuaDebugHook(this.DebugHook);
            ErrorHook.SetErrorHandler(ls.L, new ErrorHook.LuaErrorCaught(this.ErrorCaughtHook));
            this.fireStateChangedEvent = new Action(FireStateChangedEvent);
            SetHook();
        }

        protected void DebugHook(UIntPtr L, IntPtr ptr)
        {
            LuaStackRecord sr = (LuaStackRecord)Marshal.PtrToStructure(ptr, typeof(LuaStackRecord));
            if (sr.debugEvent == LuaEvent.Call)
                this.callStack++;
            else if (sr.debugEvent == LuaEvent.Return || sr.debugEvent == LuaEvent.TailReturn)
            {
                this.callStack--;

                if ((this.callStack == 0) && //stepping into engine code is not possible -> resume
                    (this.CurrentRequest == DebugRequest.StepIn || this.CurrentRequest == DebugRequest.StepToLevel))
                {
                    this.CurrentRequest = DebugRequest.Resume;
                    this.CurrentState = DebugState.Running;
                    FireStateChangedEvent();
                }
            }
            // event == line
            else if (this.CurrentRequest == DebugRequest.Pause || this.CurrentRequest == DebugRequest.StepIn)
                NormalBreak();
            else if (this.CurrentRequest == DebugRequest.StepToLevel && (this.callStack <= this.targetCallStackLevel))
                NormalBreak();
            // request == resume
            else
            {
                List<Breakpoint> bpsAtLine;
                if (!this.lineToBP.TryGetValue(sr.currentline, out bpsAtLine))
                    return; //no breakpoints on this line

                BBLua.lua_getinfo(L, "nSl", ptr);
                LuaDebugRecord dr = (LuaDebugRecord)Marshal.PtrToStructure(ptr, typeof(LuaDebugRecord));
                foreach (Breakpoint bp in bpsAtLine)
                {
                    if (bp.File.Filename == dr.source)
                        NormalBreak();
                }
            }
        }

        protected void ErrorCaughtHook(string errMessage)
        {
            this.CurrentState = DebugState.CaughtError;
            this.CurrentError = errMessage;
            this.CurrentStackTrace = new LuaStackTrace(this.ls, 1); //skip error handler
            FreezeGame();
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
            if (GlobalState.DebuggerWindow.InvokeRequired)
                GlobalState.DebuggerWindow.BeginInvoke(this.fireStateChangedEvent);
            else
            {
                if (this.OnDebugStateChange == null)
                    GlobalState.DebuggerWindow.UpdateGUI();

                if (this.OnDebugStateChange != null)
                    this.OnDebugStateChange(this, new DebugStateChangedEventArgs(this.CurrentState));
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

        public void ManualPause(bool block)
        {
            this.CurrentRequest = DebugRequest.Pause;

            if (block) //Timeout: lua state inactive -> no problem to issue commands via the console
            {
                for (int timeOut = 0; this.CurrentState != DebugState.Paused && timeOut < 5; timeOut++)
                    Thread.Sleep(10);
            }
        }

        public void StepLine()
        {
            this.targetCallStackLevel = this.callStack;
            this.CurrentRequest = DebugRequest.StepToLevel;

            while (this.CurrentState != DebugState.Paused && this.CurrentRequest == DebugRequest.Pause)
                Thread.Sleep(10);
        }

        public void StepOut()
        {
            this.targetCallStackLevel = this.callStack - 1;
            if (this.targetCallStackLevel < 0)
                this.targetCallStackLevel = 0;
            this.CurrentRequest = DebugRequest.StepToLevel;

            while (this.CurrentState != DebugState.Paused && this.CurrentRequest == DebugRequest.Pause)
                Thread.Sleep(10);
        }

        public void StepIn()
        {
            this.CurrentRequest = DebugRequest.StepIn;

            while (this.CurrentState != DebugState.Paused && this.CurrentRequest == DebugRequest.Pause)
                Thread.Sleep(10);
        }

        public void Resume()
        {
            this.CurrentRequest = DebugRequest.Resume;

            while (this.CurrentState != DebugState.Running && this.CurrentRequest == DebugRequest.Resume )
                Thread.Sleep(10);
        }

        public void SetHook()
        {
            BBLua.lua_sethook(this.ls.L, this.hookFnPtr, LuaHookType.Line | LuaHookType.Call | LuaHookType.Return, 0);
        }

        public void RemoveHook()
        {
            BBLua.lua_sethook(this.ls.L, this.hookFnPtr, LuaHookType.Nothing, 0);
        }

        protected void UnfakeIfNeccessary()
        {
            if (this.unfakeCallback != null)
            {
                this.unfakeCallback();
                this.unfakeCallback = null;
            }
        }

        public void FakeEnvironment(LuaFunctionInfo lfi)
        {
            UnfakeIfNeccessary();
            lfi.FakeG();
            this.unfakeCallback = new Action(lfi.UnFakeG);
        }
    }
}
