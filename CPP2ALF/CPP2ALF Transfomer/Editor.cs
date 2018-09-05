using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CPP2ALF_Transfomer
{
    class Editor
    {
        private static Form editorForm = new Form();
        private static RichTextBox rtb = new RichTextBox();
        static Editor()
        {
            editorForm.WindowState = FormWindowState.Maximized;
            rtb.Dock = DockStyle.Fill;
            rtb.ReadOnly = true;
            editorForm.Controls.Add(rtb);
        }
        public static void ShowDialog(string text, string title)
        {
            editorForm.Text = title;
            rtb.Text = text;
            editorForm.ShowDialog();
        }
    }
}
