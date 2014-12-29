using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


[assembly: AssemblyCompany("d3 Inc.")]
[assembly: AssemblyCopyright("Copyright yoq © 2014")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision 

[assembly: AssemblyVersion("0.5.1")]

public static class VersionHelper
{
    public static string GetVersion()
    {
        Assembly asm = Assembly.GetExecutingAssembly();
        Version vers = asm.GetName().Version;
        string versionString = vers.Major + "." + vers.Minor + "." + vers.Build;

        string confStr = (asm.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false)[0] as AssemblyConfigurationAttribute).Configuration;
        return versionString + (confStr == "" ? "" : " " + confStr);
    }
}