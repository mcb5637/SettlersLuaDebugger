using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LuaDebugger
{
    // hooks the winapi function DispatchMessage()
    // to pause the game in a save state to execute 
    // lua commands entered in the console
    static class TickHook
    {
        private static bool pauseGame = false;
        private static bool gameIsPaused = false;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate UIntPtr DispatchMessageDelegate(UIntPtr msg);

        private static DispatchMessageDelegate dmDelegate;

        [DllImport("user32.dll")]
        private static extern UIntPtr DispatchMessage(UIntPtr msg);

        public static bool InstallHook()
        {
            dmDelegate = new DispatchMessageDelegate(DispatchMessageHook);
            IntPtr hookPtr = Marshal.GetFunctionPointerForDelegate(dmDelegate);

            try
            {
                return ImportPatcher.ReplaceIATEntry("user32.dll",
#if S5
                    "DispatchMessageA", 
#elif S6
 "DispatchMessageW",
#endif
 hookPtr);
            }
            catch (Exception)
            {
                MessageBox.Show("Couldn't patch DispatchMessage!");
                return false;
            }
        }

        private static UIntPtr DispatchMessageHook(UIntPtr msg)
        {
            if (pauseGame)
            {
                gameIsPaused = true;

                while (pauseGame)
                    Thread.Sleep(10);

                gameIsPaused = false;
            }
            return DispatchMessage(msg);
        }

        //returns whether pausing the game was successful
        //and if modifying the luastate is safe
        public static bool PauseGame()
        {
            if (pauseGame)          //another thread has currently paused the game
                return false;
            
            pauseGame = true;

            for (int timeOut = 0; !gameIsPaused; timeOut++)
            {
                if (timeOut == 10)
                {
                    pauseGame = false;
                    return false;
                }

                Thread.Sleep(10);
            }

            return true;
        }

        public static void ResumeGame()
        {
            pauseGame = false;
        }

    }
}
