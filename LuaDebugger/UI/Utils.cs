using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LuaDebugger.UI
{
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 300,
                Height = 80,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            TextBox textBox = new TextBox() { Left = 20, Top = 15, Width = 200, SelectedText=text };
            Button confirmation = new Button() { Text = "OK", Left = 230, Width = 40, Top = 15, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }

    public class WaterMarkTextBox : TextBox
    {
        protected string watermarkText = "Type here";
        public string WaterMarkText
        {
            get { return watermarkText; }
            set { watermarkText = value; WaterMark_TextChanged(this, new EventArgs()); }
        }
        public bool WaterMarkActive { get; set; } = true;
        public Color WaterMarkColor { get; set; } = Color.Gray;
        protected Color normalColor = Color.Black;

        public WaterMarkTextBox()
        {
            normalColor = ForeColor;
            ForeColor = Color.Gray;

            GotFocus += WaterMark_Remove;
            LostFocus += WaterMark_Apply;
            TextChanged += WaterMark_TextChanged;
        }

        private void WaterMark_TextChanged(object sender, EventArgs e)
        {
            if (!Focused)
                WaterMark_Apply(sender, e);
        }

        private void WaterMark_Apply(object sender, EventArgs e)
        {
            if (!WaterMarkActive && string.IsNullOrEmpty(this.Text) || ForeColor == WaterMarkColor)
            {
                WaterMarkActive = true;
                Text = WaterMarkText;
                ForeColor = WaterMarkColor;
            }
        }

        private void WaterMark_Remove(object sender, EventArgs e)
        {
            if (WaterMarkActive)
            {
                WaterMarkActive = false;
                Text = "";
                ForeColor = normalColor;
            }
        }
    }
}
