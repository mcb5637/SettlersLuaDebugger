using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace LuaDebugger
{
    public static class DbgThread
    {
        [STAThread] //attribute doesnt work, needs to be done explicitly on the thread object
        public static void RunMessageLoop()
        {
            if (!GlobalState.IsInVisualStudio) //better to show exceptions directly in the debugger
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += Application_ThreadException;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DbgMain());
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show("Unhandled ThreadException:\n" + e.Exception.ToString());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Unhandled Exception:\n" + (e.ExceptionObject as Exception).ToString());
        }
    }
}
