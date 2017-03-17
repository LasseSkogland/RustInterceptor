using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rust_Interceptor.Data;
using System.Diagnostics;
using System.Windows.Forms;

namespace Rust_Interceptor
{
    class Program 
    {

        public Program()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Forms.Overlay());
        }

        private static void Main(string[] args)
        {
            new Program();
        }
    }
}
