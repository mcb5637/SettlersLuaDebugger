;# Settlers LuaDebugger#



## About ##

BlueByte has created a debugger for the Lua scripts in their games during development, but hasn't shipped this part of their software. The interface however can still be found in the production executables, and therefore it is possible to recreate the this debugger.
This project is complete Lua debugger, written from scatch, in C#

#### Interface in S5 ####
SHoK tries to load LuaDebugger.dll on startup, and calls certain functions in the DLL with information about lua states and loaded files, etc...
Settlers 5 uses Lua 5.0.2
#### Interface in S6 ####
The interface is almost the same, but the DLL is called BBLuaDebugger.dll is located in a special debug path, and the interface only becomes active in development mode. This is triggered by setting a registry key and calling the game with -DevM as argument.
Settlers 6 uses Lua 5.1.1

## Project Overview ##

### LuaDebugger ###
The main project which compiles to the DLL. Differences between the two versions are handled by compile-time conditionals `#if` etc...
The Editor used is [ICSharpCode.TextEditor](https://www.nuget.org/packages/ICSharpCode.TextEditor/), and the native DLL exports are created with [UnmanagedExports](https://www.nuget.org/packages/UnmanagedExports)

### LuaDebuggerStarter ###

Unpacks the DLLs into a temporary directory and starts the game with the correct debugger DLL. It also sets the registry key for development mode for S6.

### The Rest ###
Helpers to debug quicker, but with some hardcoded paths for now...

### Catching Lua Errors ###
Unfortunately there is no interface to catch Lua errors, so every call to `lua_pcall` is rerouted through the debugger using import table patching.

### The console ###
The Lua console gives direct access to the selected lua state, and can display most data types. When the debugger is in Pause mode, all locals and upvalues are copied into `_G` (as long as there is no global with the same name) so the behaves as if commands are executed in the context of the paused function. Before resuming, all values are copied back, and `_G` is cleaned.


