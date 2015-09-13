using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LuaDebugger.Plugins
{
    public interface IPluginDescriptor
    {
        string Name { get; }
        bool CheckSettlersVersion(int version);
        bool IsOpenableForState(LuaState luaState);
        ILuaDebuggerPlugin CreateInstance(LuaState luaState);
    }

    public interface ILuaDebuggerPlugin
    {
        void ShowInState(LuaState luaState, Control parent);
    }

    public static class PluginSystem
    {
        public static List<IPluginDescriptor> Plugins = new List<IPluginDescriptor>()
        {
            new S5CutsceneEditor.S5CutsceneEditorProps()
        };
    }
}
