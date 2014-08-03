using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LuaDebugger
{
    static class ImportPatcher //for 32bit PE only!
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool VirtualProtect(uint lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern IntPtr ImageDirectoryEntryToData(IntPtr baseAddr, bool mappedAsImage, ushort directoryEntry, out uint size);

        [StructLayout(LayoutKind.Sequential)]
        struct IMAGE_IMPORT_DESCRIPTOR
        {
            public uint OriginalFirstThunk;
            public uint TimeDateStamp;
            public uint ForwarderChain;
            public uint Name;
            public uint FirstThunk;
        }

        public static bool ReplaceIATEntry(string dllName, string funcName, IntPtr newPtr)
        {
            uint sz;
            IMAGE_IMPORT_DESCRIPTOR iDesc;

            IntPtr dllBase = GetModuleHandle(dllName);
            IntPtr oldFuncPtr = GetProcAddress(dllBase, funcName);

            IntPtr imageBase = GetModuleHandle(null);
            IntPtr importDirPtr = ImageDirectoryEntryToData(imageBase, false, 1, out sz);
            int iDescSize = Marshal.SizeOf(typeof(IMAGE_IMPORT_DESCRIPTOR));

            for (; ; importDirPtr = new IntPtr(importDirPtr.ToInt32() + iDescSize))
            {
                iDesc = (IMAGE_IMPORT_DESCRIPTOR)Marshal.PtrToStructure(importDirPtr, typeof(IMAGE_IMPORT_DESCRIPTOR));
                if (iDesc.Name == 0)
                    return false;

                string moduleName = Marshal.PtrToStringAnsi(new IntPtr(imageBase.ToInt32() + iDesc.Name));
                if (moduleName != dllName)
                    continue;

                uint memAddress = (uint)imageBase.ToInt32() + iDesc.FirstThunk;

                for (; ; ) //find function in IAT and replace
                {
                    IntPtr memValue = Marshal.ReadIntPtr(new IntPtr(memAddress));

                    if (memValue == IntPtr.Zero)
                        return false;
                    if (memValue == oldFuncPtr)
                        return WriteProtectedPtr(new IntPtr(memAddress), newPtr);

                    memAddress += 4;
                }
            }
        }

        static bool WriteProtectedPtr(IntPtr address, IntPtr newValue)
        {
            uint oldProtValue;
            if (!VirtualProtect((uint)address.ToInt32(), 4, 0x08, out oldProtValue))
                return false;
            Marshal.WriteIntPtr(address, newValue);
            VirtualProtect((uint)address.ToInt32(), 4, oldProtValue, out oldProtValue);
            return true;
        }
    }
}
