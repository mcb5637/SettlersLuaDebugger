using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

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

            dllName = FindCorrectCaseForModule(dllName);

            IntPtr dllBase = GetModuleHandle(dllName);

            if (dllBase == IntPtr.Zero)
                throw new Exception("couldn't find module " + dllName);

            IntPtr oldFuncPtr = GetProcAddress(dllBase, funcName);

            if (oldFuncPtr == IntPtr.Zero)
                throw new Exception("couldn't find export " + funcName + " in " + dllName);

            IntPtr imageBase = GetModuleHandle(null);

            try
            {
                return StandardImportPatch(dllName, imageBase, oldFuncPtr, newPtr);
            }
            catch (Exception stdEx)
            {
                try
                {
                    return FallbackImportPatch(dllName, imageBase, oldFuncPtr, newPtr);
                }
                catch (Exception fbEx)
                {
                    throw new Exception("Standard Patcher: " + stdEx.Message + "\nFallback Patcher: " + fbEx.Message);
                }
            }
        }


        /* Locate the imports table for the dll and patch the entry in the linked IAT */
        static bool StandardImportPatch(string dllName, IntPtr imageBase, IntPtr oldFuncPtr, IntPtr newPtr)
        {
            uint sz;
            IMAGE_IMPORT_DESCRIPTOR iDesc;
            IntPtr importDirPtr = ImageDirectoryEntryToData(imageBase, false, 1, out sz);

            int iDescSize = Marshal.SizeOf(typeof(IMAGE_IMPORT_DESCRIPTOR));

            for (; ; importDirPtr = new IntPtr(importDirPtr.ToInt32() + iDescSize))
            {
                iDesc = (IMAGE_IMPORT_DESCRIPTOR)Marshal.PtrToStructure(importDirPtr, typeof(IMAGE_IMPORT_DESCRIPTOR));
                if (iDesc.Name == 0)
                    throw new Exception("reached end of import dir, " + dllName + " not found");

                string moduleName = Marshal.PtrToStringAnsi(new IntPtr(imageBase.ToInt32() + iDesc.Name));
                if (moduleName.ToLower() != dllName.ToLower())
                    continue;

                uint memAddress = (uint)imageBase.ToInt32() + iDesc.FirstThunk;

                for (; ; ) //find function in IAT and replace
                {
                    IntPtr memValue = Marshal.ReadIntPtr(new IntPtr(memAddress));

                    if (memValue == IntPtr.Zero)
                        throw new Exception("reached end of import table for " + dllName + " old ptr not found");
                    if (memValue == oldFuncPtr)
                        return WriteProtectedPtr(new IntPtr(memAddress), newPtr);

                    memAddress += 4;
                }
            }
        }

        static string FindCorrectCaseForModule(string module)
        {
            string modLower = module.ToLower();
            Process p = Process.GetCurrentProcess();
            foreach (ProcessModule pm in p.Modules)
                if (pm.ModuleName.ToLower() == modLower)
                    return pm.ModuleName;

            throw new Exception("module " + module + " does not exist in loaded modules");
        }

        static bool WriteProtectedPtr(IntPtr address, IntPtr newValue)
        {
            uint oldProtValue;
            if (!VirtualProtect((uint)address.ToInt32(), 4, 0x08, out oldProtValue))
                throw new Exception("couldn't unprotect ro memory");

            Marshal.WriteIntPtr(address, newValue);

            if (!VirtualProtect((uint)address.ToInt32(), 4, oldProtValue, out oldProtValue))
                throw new Exception("couldn't protect rw memory");
            else
                return true;
        }


        /* FALLBACK */

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_DOS_HEADER
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public char[] e_magic;       // Magic number
            public UInt16 e_cblp;    // Bytes on last page of file
            public UInt16 e_cp;      // Pages in file
            public UInt16 e_crlc;    // Relocations
            public UInt16 e_cparhdr;     // Size of header in paragraphs
            public UInt16 e_minalloc;    // Minimum extra paragraphs needed
            public UInt16 e_maxalloc;    // Maximum extra paragraphs needed
            public UInt16 e_ss;      // Initial (relative) SS value
            public UInt16 e_sp;      // Initial SP value
            public UInt16 e_csum;    // Checksum
            public UInt16 e_ip;      // Initial IP value
            public UInt16 e_cs;      // Initial (relative) CS value
            public UInt16 e_lfarlc;      // File address of relocation table
            public UInt16 e_ovno;    // Overlay number
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public UInt16[] e_res1;    // Reserved words
            public UInt16 e_oemid;       // OEM identifier (for e_oeminfo)
            public UInt16 e_oeminfo;     // OEM information; e_oemid specific
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public UInt16[] e_res2;    // Reserved words
            public Int32 e_lfanew;      // File address of new exe header
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_NT_HEADERS32
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public char[] Signature;
            public IMAGE_FILE_HEADER FileHeader;

            //[FieldOffset(24)]
            //public IMAGE_OPTIONAL_HEADER32 OptionalHeader;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_FILE_HEADER
        {
            public UInt16 Machine;
            public UInt16 NumberOfSections;
            public UInt32 TimeDateStamp;
            public UInt32 PointerToSymbolTable;
            public UInt32 NumberOfSymbols;
            public UInt16 SizeOfOptionalHeader;
            public UInt16 Characteristics;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_SECTION_HEADER
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public char[] Name;

            public UInt32 VirtualSize;
            public UInt32 VirtualAddress;
            public UInt32 SizeOfRawData;
            public UInt32 PointerToRawData;
            public UInt32 PointerToRelocations;
            public UInt32 PointerToLinenumbers;
            public UInt16 NumberOfRelocations;
            public UInt16 NumberOfLinenumbers;
            public uint Characteristics;

            public string SectionName
            {
                get { return new string(Name); }
            }
        }

        /* Assume the IAT resides at the top of the .rdata segment -> find offset&size of .rdata and patch the entry in the IAT */
        static bool FallbackImportPatch(string dllName, IntPtr imageBase, IntPtr oldFuncPtr, IntPtr newPtr)
        {
            IMAGE_DOS_HEADER dosHeader = (IMAGE_DOS_HEADER)Marshal.PtrToStructure(imageBase, typeof(IMAGE_DOS_HEADER));
            int ntHeaderPtr = imageBase.ToInt32() + dosHeader.e_lfanew;
            IMAGE_NT_HEADERS32 ntHeaders = (IMAGE_NT_HEADERS32)Marshal.PtrToStructure((IntPtr)ntHeaderPtr, typeof(IMAGE_NT_HEADERS32));

            int sectionHeaderPtr = ntHeaderPtr + 248 /* 248 = sizeof(IMAGE_NT_HEADERS32) */;
            for (int i = 0; i < ntHeaders.FileHeader.NumberOfSections; i++)
            {
                IMAGE_SECTION_HEADER sectionHeader = (IMAGE_SECTION_HEADER)Marshal.PtrToStructure((IntPtr)(sectionHeaderPtr), typeof(IMAGE_SECTION_HEADER));

                if (sectionHeader.SectionName.StartsWith(".rdata"))
                {
                    uint startAddr = (uint)imageBase.ToInt32() + sectionHeader.VirtualAddress;
                    for (uint memAddr = startAddr; memAddr < startAddr + sectionHeader.VirtualSize; memAddr += 4)
                    {
                        if (Marshal.ReadIntPtr((IntPtr)memAddr) == oldFuncPtr)
                            return WriteProtectedPtr((IntPtr)memAddr, newPtr);
                    }
                    throw new Exception("reached end of .rdata, func ptr not found");
                }

                sectionHeaderPtr += Marshal.SizeOf(typeof(IMAGE_SECTION_HEADER));
            }
            throw new Exception("read all sections, .rdata not found");
        }
    }
}
