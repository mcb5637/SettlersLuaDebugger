using System;
using System.Windows.Forms;

namespace LuaDebugger
{
    class CopyChangedTextBox : TextBox
    {
        protected override void WndProc(ref Message m)
        {
            // Trap WM_PASTE:
            if (m.Msg == 0x302 && Clipboard.ContainsText())
            {
                SelectedText = Clipboard.GetText().Replace('\n', ' ');
                return;
            }
            base.WndProc(ref m);
        }
    }
}