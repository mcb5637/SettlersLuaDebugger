using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LuaDebugger.Plugins.S5CutsceneEditor
{
    static class S5Direct
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate int ReloadCutsFn(IntPtr objPtr, string path);
        private static ReloadCutsFn rcFunction = (ReloadCutsFn)Marshal.GetDelegateForFunctionPointer(new IntPtr(0x59AA2A), typeof(ReloadCutsFn));

        public static void ReloadCutscenes()
        {
            ReloadCutscenes("maps/externalmap");
        }

        public static void ReloadCutscenes(string path)
        {
            IntPtr ptrToObj = Marshal.ReadIntPtr(new IntPtr(0xA0344C));
            IntPtr objPtr = Marshal.ReadIntPtr(ptrToObj);
            rcFunction(objPtr, path);
        }
    }
}
