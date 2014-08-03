using CSUACSelfElevation;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace LuaDebuggerStarter
{
    public partial class frmLDStarter : Form
    {
        protected string tmpPath, s5Path, s6Path;

        protected Dictionary<Button, string> s5Paths, s6Paths;
        protected Dictionary<object, string> startDict = new Dictionary<object, string>();

        public frmLDStarter()
        {
            InitializeComponent();
            UnpackEnvironment();

            if (!IsRunAsAdmin() && Environment.OSVersion.Version.Major >= 6)
            {
                this.btnS6DevM.FlatStyle = FlatStyle.System;
                NativeMethods.SendMessage(btnS6DevM.Handle, NativeMethods.BCM_SETSHIELD, 0, (IntPtr)1);
            }

            s5Paths = new Dictionary<Button, string>()
                {
                    { btnS5Main, "bin/settlershok.exe" },
                    { btnS5AO1, "extra1/bin/settlershok.exe" },
                    { btnS5AO2, "extra2/bin/settlershok.exe" }
                };

            s6Paths = new Dictionary<Button, string>()
                {
                    { btnS6Main, "base/bin/settlers6.exe" },
                    { btnS6AO1, "extra1/bin/settlers6.exe" }
                };


            s5Path = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Blue Byte\The Settlers - Heritage of Kings", "InstallPath", null);
            s6Path = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Ubisoft\The Settlers 6\GameUpdate", "InstallDir", null);

            RecheckAll();
        }

        public void CheckS6DevM()
        {
            var s6RegKey = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Ubisoft\The Settlers 6\Development", "DevelopmentMachine", 0);

            if (s6RegKey == null || (Int32)s6RegKey == 0 || (Int32)s6RegKey != Program.CalculateDevHash(System.Environment.MachineName))
                btnS6DevM.Visible = true;
            else
                btnS6DevM.Visible = false;
        }

        public void RecheckAll()
        {
            startDict.Clear();
            if (s5Path != null && CheckPaths(s5Paths, s5Path))
                lblInstS5.Text = "Installation found";
            else
                lblInstS5.Text = "Installation not found!";

            if (s6Path != null && CheckPaths(s6Paths, s6Path))
            {
                lblInstS6.Text = "Installation found";
                CheckS6DevM();
            }
            else
                lblInstS6.Text = "Installation not found!";
        }

        public bool CheckPaths(Dictionary<Button, string> dic, string basePath)
        {
            bool foundNothing = true;

            foreach (KeyValuePair<Button, string> kvp in dic)
            {
                string fullPath = basePath + "\\" + kvp.Value;
                Button btn = (Button)kvp.Key;

                if (File.Exists(fullPath))
                {
                    btn.Enabled = true;
                    btn.Click -= btn_Click;
                    btn.Click += btn_Click;
                    startDict.Add(kvp.Key, fullPath);
                    foundNothing = false;
                }
                else
                    btn.Enabled = false;
            }

            return !foundNothing;
        }

        void btn_Click(object sender, EventArgs e)
        {
            ProcessStartInfo si = new ProcessStartInfo(startDict[sender], "-DevM");
            si.EnvironmentVariables["Path"] += ";" + tmpPath;
            si.UseShellExecute = false;
            Process p = Process.Start(si);
        }

        public void UnpackEnvironment()
        {
            string res = "LuaDebuggerStarter.Unpack.";
            tmpPath = Path.GetTempPath() + "luaDebugger-yoq/";

            Assembly localAssembly = Assembly.GetExecutingAssembly();
            string[] resourceFiles = localAssembly.GetManifestResourceNames();

            try
            {
                string s6Path = tmpPath + "/base/_dbg/bin/release/";
                Directory.CreateDirectory(tmpPath);
                Directory.CreateDirectory(s6Path);

                Stream s5Stream = localAssembly.GetManifestResourceStream(res + "LuaDebugger.dll");
                Stream s6Stream = localAssembly.GetManifestResourceStream(res + "BBLuaDebugger.dll");

                using (FileStream fs = new FileStream(tmpPath + "LuaDebugger.dll", FileMode.Create))
                    s5Stream.CopyTo(fs);

                using (FileStream fs = new FileStream(s6Path + "BBLuaDebugger.dll", FileMode.Create))
                    s6Stream.CopyTo(fs);

            }
            catch
            {
                MessageBox.Show("A problem occured creating Debugger files!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void lblInstS5_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                s5Path = fbd.SelectedPath;
                RecheckAll();
            }
        }

        private void lblS6Inst_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                s6Path = fbd.SelectedPath;
                RecheckAll();
            }
        }


        private void btnS6DevM_Click(object sender, EventArgs e)
        {
            if (!IsRunAsAdmin())
            {
                // Launch itself as administrator
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = Environment.CurrentDirectory;
                proc.FileName = Application.ExecutablePath;
                proc.Arguments = "setkey";
                proc.Verb = "runas";

                try
                {
                    Process.Start(proc).WaitForExit();
                    CheckS6DevM();
                }
                catch
                {
                    // The user refused the elevation.
                    // Do nothing and return directly ...
                    return;
                }
            }
            else
            {
                Program.SetS6DevKey();
                CheckS6DevM();
            }

            
        }

        internal bool IsRunAsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
