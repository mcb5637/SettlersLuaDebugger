using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LuaDebugger.Plugins
{
    interface ILuaDebuggerPlugin
    {
        string PluginName { get; }
        bool IsOpenable(LuaState luaState);
        void ShowInState(LuaState luaState, Control parent);
    }

    static class PluginSystem
    {
        public static List<ILuaDebuggerPlugin> Plugins = new List<ILuaDebuggerPlugin>()
        {
            new S5CutsceneEditor.S5CutsceneEditorMain()
        };
    }
}
