using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace LuaDebugger
{
    [Flags]
    public enum WindowStyle : uint
    {
        WS_BORDER = 0x00800000,

        WS_CAPTION = 0x00C00000,

        WS_CHILD = 0x40000000,

        WS_CHILDWINDOW = 0x40000000,

        WS_CLIPCHILDREN = 0x02000000,

        WS_CLIPSIBLINGS = 0x04000000,

        WS_DISABLED = 0x08000000,

        WS_DLGFRAME = 0x00400000,

        WS_GROUP = 0x00020000,

        WS_HSCROLL = 0x00100000,

        WS_ICONIC = 0x20000000,

        WS_MAXIMIZE = 0x01000000,

        WS_MAXIMIZEBOX = 0x00010000,

        WS_MINIMIZE = 0x20000000,

        WS_MINIMIZEBOX = 0x00020000,

        WS_OVERLAPPED = 0x00000000,

        WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,

        WS_POPUP = 0x80000000,

        WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,

        WS_SIZEBOX = 0x00040000,

        WS_SYSMENU = 0x00080000,

        WS_TABSTOP = 0x00010000,

        WS_THICKFRAME = 0x00040000,

        WS_TILED = 0x00000000,

        WS_TILEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,

        WS_VISIBLE = 0x10000000,

        WS_VSCROLL = 0x00200000
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public UInt32 code;
        public IntPtr wParam;
        public IntPtr lParam;
        public UInt32 time;
        public System.Drawing.Point pt;
    }

    [Flags]
    public enum PeekMessageType : uint
    {
        PM_NOREMOVE = 0,
        PM_REMOVE = 1,
        PM_NOYIELD = 2
    }

    public enum WindowShowStyle : uint
    {
        Hide = 0,
        ShowNormal = 1,
        ShowMinimized = 2,
        ShowMaximized = 3,
        Maximize = 3,
        ShowNormalNoActivate = 4,
        Show = 5,
        Minimize = 6,
        ShowMinNoActivate = 7,
        ShowNoActivate = 8,
        Restore = 9,
        ShowDefault = 10,
        ForceMinimized = 11
    }

    [Flags]
    public enum ExtendedWindowStyles : uint
    {
        WS_EX_ACCEPTFILES = 0x00000010,
        WS_EX_APPWINDOW = 0x00040000,
        WS_EX_CLIENTEDGE = 0x00000200,
        WS_EX_COMPOSITED = 0x02000000,
        WS_EX_CONTEXTHELP = 0x00000400,
        WS_EX_CONTROLPARENT = 0x00010000,
        WS_EX_DLGMODALFRAME = 0x00000001,
        WS_EX_LAYERED = 0x00080000,
        WS_EX_LAYOUTRTL = 0x00400000,
        WS_EX_LEFT = 0x00000000,
        WS_EX_LEFTSCROLLBAR = 0x00004000,
        WS_EX_LTRREADING = 0x00000000,
        WS_EX_MDICHILD = 0x00000040,
        WS_EX_NOACTIVATE = 0x08000000,
        WS_EX_NOINHERITLAYOUT = 0x00100000,
        WS_EX_NOPARENTNOTIFY = 0x00000004,
        WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
        WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,
        WS_EX_RIGHT = 0x00001000,
        WS_EX_RIGHTSCROLLBAR = 0x00000000,
        WS_EX_RTLREADING = 0x00002000,
        WS_EX_STATICEDGE = 0x00020000,
        WS_EX_TOOLWINDOW = 0x00000080,
        WS_EX_TOPMOST = 0x00000008,
        WS_EX_TRANSPARENT = 0x00000020,
        WS_EX_WINDOWEDGE = 0x00000100

    }

    [Flags]
    public enum SendMessageTimeoutFlags : uint
    {
        SMTO_NORMAL = 0x0,
        SMTO_BLOCK = 0x1,
        SMTO_ABORTIFHUNG = 0x2,
        SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
        SMTO_ERRORONEXIT = 0x20
    }

    public static class WinAPI
    {
        [DllImport("user32.dll")]
        public static extern IntPtr CallWindowProc(uint lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SendMessageTimeout(
            IntPtr windowHandle,
            uint Msg,
            uint wParam,
            uint lParam,
            SendMessageTimeoutFlags flags,
            uint timeout,
            uint resPtr);
            //out IntPtr result);


        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern uint SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;

        [DllImport("user32.dll")]
        public static extern uint SetClassLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadIconA(IntPtr hInstance, uint lpStr);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, UInt32 wParam, UInt32 lParam);

        [DllImport("user32.dll")]
        public static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, PeekMessageType pmt);
        [DllImport("user32.dll")]
        public static extern sbyte GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
        [DllImport("user32.dll")]
        public static extern IntPtr DispatchMessage([In] ref MSG lpmsg);
        [DllImport("user32.dll")]
        public static extern bool TranslateMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

    }

    public static class FreezeMsgLoop
    {

        public static void ProcessBasicEvents()
        {
            MSG message;
            while (WinAPI.PeekMessage(out message, IntPtr.Zero, 0, 0, PeekMessageType.PM_REMOVE))
            {
                if (message.code == 0x12) //WM_QUIT
                    Environment.Exit(0);

                // basic window events + nonclient events
                if (message.code <= 0x2f || (message.code >= 0x81 && message.code <= 0xA9))
                {
                    WinAPI.TranslateMessage(ref message);
                    WinAPI.DispatchMessage(ref message);
                }
            }
        }
    }
}
