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

        protected bool s5Found = false, s6Found = false;
        protected Dictionary<Button, string> s5Paths, s6Paths;
        protected Dictionary<object, string> startDict = new Dictionary<object, string>();

        protected const string s5KeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Blue Byte\The Settlers - Heritage of Kings";
        protected const string s5ValueName = "InstallPath";
        protected const string s6KeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Ubisoft\The Settlers 6\GameUpdate";
        protected const string s6ValueName = "InstallDir";

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


            s5Path = (string)Registry.GetValue(s5KeyName, s5ValueName, null);
            s6Path = (string)Registry.GetValue(s6KeyName, s6ValueName, null);

            RecheckAll();
        }



        public void CheckS6DevM()
        {
            if (Program.IsS6DevM())
                lblDevMS6.Text = "ON";
            else
            {
                lblDevMS6.Text = "OFF";
                btnS6Main.Enabled = false;
                btnS6AO1.Enabled = false;
            }
        }

        public void RecheckAll()
        {
            startDict.Clear();

            s5Found = (s5Path != null && CheckPaths(s5Paths, s5Path));
            s6Found = (s6Path != null && CheckPaths(s6Paths, s6Path));

            if(s5Found)
                lblInstS5.Text = "Installation found";
            else
                lblInstS5.Text = "Installation not found!";

            if (s6Found)
                lblInstS6.Text = "Installation found";
            else
                lblInstS6.Text = "Installation not found!";

            CheckS6DevM();
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
            ProcessStartInfo si = new ProcessStartInfo(startDict[sender], "-debugscript -DISPLAYSCRIPTERRORS -DevM");
            si.EnvironmentVariables["Path"] += ";" + tmpPath;
            si.UseShellExecute = false;
            string[] cmdLineArgs = Environment.GetCommandLineArgs();
            if (cmdLineArgs.Length > 1)
                si.Arguments += " " + string.Join(" ", cmdLineArgs, 1, cmdLineArgs.Length - 1);
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
                string s6DllPath = tmpPath + "/base/_dbg/bin/release/";
                Directory.CreateDirectory(tmpPath);
                Directory.CreateDirectory(s6DllPath);

                Stream s5Stream = localAssembly.GetManifestResourceStream(res + "LuaDebugger.dll");
                Stream s6Stream = localAssembly.GetManifestResourceStream(res + "BBLuaDebugger.dll");

                using (FileStream fs = new FileStream(tmpPath + "LuaDebugger.dll", FileMode.Create))
                    s5Stream.CopyTo(fs);

                using (FileStream fs = new FileStream(s6DllPath + "BBLuaDebugger.dll", FileMode.Create))
                    s6Stream.CopyTo(fs);

                using (FileStream fs = new FileStream(tmpPath + "ICSharpCode.TextEditor.dll", FileMode.Create))
                    localAssembly.GetManifestResourceStream(res + "ICSharpCode.TextEditor.dll").CopyTo(fs);

                using (FileStream fs = new FileStream(tmpPath + "EasyHook.dll", FileMode.Create))
                    localAssembly.GetManifestResourceStream(res + "EasyHook.dll").CopyTo(fs);

                using (FileStream fs = new FileStream(tmpPath + "EasyHook32.dll", FileMode.Create))
                    localAssembly.GetManifestResourceStream(res + "EasyHook32.dll").CopyTo(fs);
            }
            catch
            {
                MessageBox.Show("A problem occured creating Debugger files!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private bool ConfirmPathSave()
        {
            return MessageBox.Show("Do you want to permanently save the selected path?", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private void lblInstS5_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = s5Path;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                s5Path = fbd.SelectedPath;
                RecheckAll();
                if (s5Found && ConfirmPathSave())
                    WriteRegistryString(s5KeyName, s5ValueName, s5Path);
            }
        }

        private void lblS6Inst_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = s6Path;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                s6Path = fbd.SelectedPath;
                RecheckAll();
                if (s6Found && ConfirmPathSave())
                    WriteRegistryString(s6KeyName, s6ValueName, s6Path);
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
                proc.Arguments = "!s6key";
                proc.Verb = "runas";

                try
                {
                    Process.Start(proc).WaitForExit();
                    RecheckAll();
                }
                catch
                {
                    // The user refused the elevation.
                    return;
                }
            }
            else
            {
                Program.ToggleS6DevM();
                RecheckAll();
            }
        }

        private void WriteRegistryString(string keyName, string valueName, string value)
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.UseShellExecute = true;
            proc.FileName = "REG";
            proc.Arguments = "ADD \"" + keyName + "\" /v \"" + valueName + "\" /t REG_SZ /d \"" + value + "\" /f";

            if (Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432") != null)
                proc.Arguments += " /reg:32";

            if (!IsRunAsAdmin())
                proc.Verb = "runas";

            try
            {
                Process.Start(proc).WaitForExit();
            }
            catch { }
        }


        internal bool IsRunAsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void frmLDStarter_Load(object sender, EventArgs e)
        {
            this.Text += VersionHelper.GetVersion();
        }
    }
}
