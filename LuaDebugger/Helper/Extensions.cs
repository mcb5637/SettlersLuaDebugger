using System;
using System.Collections.Generic;
using System.Text;

namespace LuaDebugger
{
    public static class EnumerableEx
    {
        public static IEnumerable<string> SplitBy(this string str, int chunkLength)
        {
            if (String.IsNullOrEmpty(str)) throw new ArgumentException();
            if (chunkLength < 1) throw new ArgumentException();

            for (int i = 0; i < str.Length; i += chunkLength)
            {
                if (chunkLength + i > str.Length)
                    chunkLength = str.Length - i;

                yield return str.Substring(i, chunkLength);
            }
        }
    }
}

namespace System.Windows.Forms
{
    public static class ControlExtensions
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);

        public static void SuspendUpdate(this Control control)
        {
            LockWindowUpdate(control.Handle);
        }

        public static void ResumeUpdate(this Control control)
        {
            LockWindowUpdate(IntPtr.Zero);
        }

    }
}

// .net3 wtf!
//namespace System.Runtime.CompilerServices
//{
//    public class ExtensionAttribute : Attribute { }
//}