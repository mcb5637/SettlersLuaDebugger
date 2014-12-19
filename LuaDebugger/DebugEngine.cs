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
        public LuaState LuaState { get; protected set; }
        public DebugState State { get; protected set; }

        public DebugStateChangedEventArgs(DebugState newState, LuaState ls)
        {
            this.State = newState;
            this.LuaState = ls;
        }
    }

    public class DebugEngine
    {
        protected LuaState ls;
        protected Dictionary<int, List<Breakpoint>> lineToBP = new Dictionary<int, List<Breakpoint>>();

        //prevent the gc from deleting the delegates
        protected LuaDebugHook debugHook;
        protected LuaCFunc logCallback;

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

        public DebugEngine(LuaState ls)
        {
            this.ls = ls;
            this.debugHook = new LuaDebugHook(this.DebugHook);
            this.logCallback = new LuaCFunc(this.LogCallback);
            RegisterLogFunction();
            ErrorHook.SetErrorHandler(ls.L, new ErrorHook.LuaErrorCaught(this.ErrorCaughtHook));
            this.fireStateChangedEvent = new Action(FireStateChangedEvent);
            SetHook();
        }

        protected void RegisterLogFunction()
        {
            BBLua.lua_pushstring(this.ls.L, "LuaDebugger");
            BBLua.lua_newtable(this.ls.L);
            BBLua.lua_pushstring(this.ls.L, "Log");
            BBLua.lua_pushcclosure(this.ls.L, Marshal.GetFunctionPointerForDelegate(this.logCallback), 0);
            BBLua.lua_rawset(this.ls.L, -3);
            BBLua.lua_rawset(this.ls.L, (int)LuaPseudoIndices.GLOBALSINDEX);
        }

        protected int LogCallback(UIntPtr L)
        {
            string text = this.ls.TosToString();
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
                lines[i] = "Log: " + lines[i];

            this.ls.StateView.LuaConsole.AppendText(string.Join("\n", lines));
            return 0;
        }

        protected unsafe void DebugHook(UIntPtr L, IntPtr ptr) //unsafe for speed
        {
            LuaStackRecord* sr = (LuaStackRecord*)ptr;
            if (sr->debugEvent == LuaEvent.Call)
                this.callStack++;
            else if (sr->debugEvent == LuaEvent.Return || sr->debugEvent == LuaEvent.TailReturn)
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
                if (!this.lineToBP.TryGetValue(sr->currentline, out bpsAtLine))
                    return; //no breakpoints on this line

                BBLua.lua_getinfo(L, "S", ptr);
                LuaDebugSourceRecord dr = (LuaDebugSourceRecord)Marshal.PtrToStructure(ptr, typeof(LuaDebugSourceRecord));

                foreach (Breakpoint bp in bpsAtLine)
                {
                    if (bp.File.Filename == dr.source)
                    {
                        NormalBreak();
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
            BBLua.lua_sethook(this.ls.L, this.debugHook, LuaHookType.Line | LuaHookType.Call | LuaHookType.Return, 0);
        }

        public void RemoveHook()
        {
            BBLua.lua_sethook(this.ls.L, this.debugHook, LuaHookType.Nothing, 0);
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
