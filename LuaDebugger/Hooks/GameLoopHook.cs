using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LuaDebugger
{
    static class GameLoopHook
    {
        private static WinAPI.WndProc wndProcDelegate;
        private static uint oldWndProcReference;
        private static Action ToRun;
        private static bool HasSomethingToRun = false;
        private const uint WM_CHECK_RUN = 0x7001;

        public static bool InstallHook()
        {
            wndProcDelegate = new WinAPI.WndProc(InjectedWndProc);
            IntPtr newWndProcPtr = Marshal.GetFunctionPointerForDelegate(wndProcDelegate);

            oldWndProcReference = WinAPI.SetWindowLong(GlobalState.SettlersWindowHandle, -4, (uint)newWndProcPtr);
            return true;
        }

        private static IntPtr InjectedWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (HasSomethingToRun)
            {
                lock (wndProcDelegate)
                {
                    ToRun();
                    HasSomethingToRun = false;
                }
            }
            if (msg == WM_CHECK_RUN)
                return IntPtr.Zero;
            return WinAPI.CallWindowProc(oldWndProcReference, hWnd, msg, wParam, lParam);
        }

        public static void RunInGameThread(Action toRun)
        {
            lock (wndProcDelegate)
            {
                ToRun = toRun;
                HasSomethingToRun = true;
            }
            WinAPI.PostMessage(GlobalState.SettlersWindowHandle, WM_CHECK_RUN, 0, 0); // notify, so we run delegates even if nothing is in the message queue
        }
    }
}
