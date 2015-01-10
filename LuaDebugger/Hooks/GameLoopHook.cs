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
        private static bool pauseGame = false;
        private static bool gameIsPaused = false;

        private const uint WM_START_BLOCK = 0x7001;
        private static WinAPI.WndProc wndProcDelegate;
        private static uint oldWndProcReference;

        public static bool InstallHook()
        {
            wndProcDelegate = new WinAPI.WndProc(InjectedWndProc);
            IntPtr newWndProcPtr = Marshal.GetFunctionPointerForDelegate(wndProcDelegate);

            oldWndProcReference = WinAPI.SetWindowLong(GlobalState.SettlersWindowHandle, -4, (uint)newWndProcPtr);
            return true;
        }

        private static IntPtr InjectedWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == WM_START_BLOCK)
            {
                BlockGameLoop();
                return IntPtr.Zero;
            }
            else
                return WinAPI.CallWindowProc(oldWndProcReference, hWnd, msg, wParam, lParam);
        }

        private static void BlockGameLoop()
        {
            if (pauseGame)
            {
                gameIsPaused = true;

                while (pauseGame)
                    Thread.Sleep(10);

                gameIsPaused = false;
            }
        }

        //returns whether pausing the game was successful
        //and if modifying the luastate is safe
        public static bool PauseGame()
        {
            if (pauseGame)          //another thread has currently paused the game
                return false;

            pauseGame = true;
            WinAPI.SendMessageTimeout(GlobalState.SettlersWindowHandle, WM_START_BLOCK, 0, 0, SendMessageTimeoutFlags.SMTO_NORMAL, 100, 0);
            
            if (!gameIsPaused)
                pauseGame = false;

            return gameIsPaused;
        }

        public static void ResumeGame()
        {
            pauseGame = false;
        }
    }
}
