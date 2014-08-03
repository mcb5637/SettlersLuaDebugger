using ICSharpCode.TextEditor.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace LuaDebugger
{
 public class MySyntaxModeProvider : ISyntaxModeFileProvider
    {
        #region Fields

        List<SyntaxMode> syntaxModes = null;

        #endregion Fields

        #region Constructors

        public MySyntaxModeProvider()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream syntaxModeStream = assembly.GetManifestResourceStream("LuaDebugger.Resources.SyntaxModes.xml");
            if (syntaxModeStream != null) {
                syntaxModes = SyntaxMode.GetSyntaxModes(syntaxModeStream);
            } else {
                syntaxModes = new List<SyntaxMode>();
            }
        }

        #endregion Constructors

        #region Properties

        public ICollection<SyntaxMode> SyntaxModes
        {
            get {
                return syntaxModes;
            }
        }

        #endregion Properties

        #region Methods

        public XmlTextReader GetSyntaxModeFile(SyntaxMode syntaxMode)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return new XmlTextReader(assembly.GetManifestResourceStream("LuaDebugger.Resources." + syntaxMode.FileName));
        }

        public void UpdateSyntaxModeList()
        {
            // resources don't change during runtime
        }

        #endregion Methods
    }
}
