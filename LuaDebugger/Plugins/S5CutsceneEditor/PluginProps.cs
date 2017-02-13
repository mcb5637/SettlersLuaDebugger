using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuaDebugger.Plugins.S5CutsceneEditor
{
    public class S5CutsceneEditorProps : IPluginDescriptor
    {
        private S5CutsceneEditorMain _instance = null;

        public string Name
        {
            get { return "S5 Cutscene Editor"; }
        }

        public bool CheckSettlersVersion(int version)
        {
            return version == 5;
        }

        public bool IsOpenableForState(LuaState luaState)
        {
            return luaState.Name.Contains("Game") || GlobalState.IsInVisualStudio;
        }

        public ILuaDebuggerPlugin CreateInstance(LuaState luaState)
        {
            if (_instance == null)
                _instance = new S5CutsceneEditorMain();
            return _instance;
        }
    }
}
