using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinfromPlugin
{
    public static class Plugin
    {
        public delegate void Action();

        public static int GetInt()
        {
            return 190830;
        }

        public static void MessageMsg(string text, string caption, Action action)
        {
            DialogResult result = MessageBox.Show(text, caption, MessageBoxButtons.OKCancel);

            if(result == DialogResult.OK)
            {
                action();
            }
        }
    }
}
