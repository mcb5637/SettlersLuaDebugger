using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LuaDebugger.Helper
{
    public static class DpiAwareness
    {

        public enum PROCESS_DPI_AWARENESS : uint
        {
            DPI_UNAWARE = 0,
            SYSTEM_DPI_AWARE = 1,
            PER_MONITOR_DPI_AWARE = 2
        };

        [DllImport("Shcore.dll")]
        public static extern int SetProcessDpiAwareness(PROCESS_DPI_AWARENESS awareness);
        
        /*HRESULT WINAPI GetProcessDpiAwareness(
  _In_  HANDLE                hprocess,
  _Out_ PROCESS_DPI_AWARENESS *value
);*/

        [DllImport("Shcore.dll")]
        public static extern int GetProcessDpiAwareness(IntPtr processHandle, out PROCESS_DPI_AWARENESS awareness);
    }
}
