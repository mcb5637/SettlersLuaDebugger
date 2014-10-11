using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("LuaDebugger " + 
                                            #if S5
                                                "S5"
                                            #elif S6
                                                "S6"
                                            #endif
                         )]

[assembly: AssemblyDescription("")]
[assembly: AssemblyProduct("LuaDebugger")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("56c89ef9-ad5e-46f4-a58c-02c6a8d8d7cf")]
