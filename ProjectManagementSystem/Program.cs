using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ProjectManagementSystem
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool result;
            var mutex = new System.Threading.Mutex(true, "UniqueAppId", out result);
            if (!result)
            {
                MessageBox.Show("Another instance is already running.", "Single instance App.");
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            GC.KeepAlive(mutex);
        }
    }
}
