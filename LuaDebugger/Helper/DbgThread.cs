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
        static Dictionary<string, Assembly> LoadedAssemblies = new Dictionary<string, Assembly>();

        [STAThread] //attribute doesnt work, needs to be done explicitly on the thread object
        public static void RunMessageLoop()
        {
            string editorDll = "LuaDebugger.Resources.ICSharpCode.TextEditor.dll";
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stm = assembly.GetManifestResourceStream(editorDll))
            {
                byte[] ba = new byte[stm.Length];
                stm.Read(ba, 0, (int)stm.Length);
                Assembly a = Assembly.Load(ba);
                LoadedAssemblies.Add(a.FullName, a);
            }

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DbgMain());
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return LoadedAssemblies[args.Name];
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
