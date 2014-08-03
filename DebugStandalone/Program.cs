using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LuaDebugger;

namespace DebugStandalone
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            TestFn();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static void TestFn()
        {
            TestFn2();
            TestFn2();
            TestFn2();
        }
        static void TestFn2()
        {

        }
    }
}
