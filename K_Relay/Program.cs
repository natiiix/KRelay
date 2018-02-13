using System;
using System.Reflection;
using System.Windows.Forms;

namespace K_Relay
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            EmbeddedAssembly.Load("K_Relay.MetroFramework.dll", "K_Relay.MetroFramework.dll");
            EmbeddedAssembly.Load("K_Relay.MetroFramework.Design.dll", "K_Relay.MetroFramework.Design.dll");
            EmbeddedAssembly.Load("K_Relay.MetroFramework.Fonts.dll", "K_Relay.MetroFramework.Fonts.dll");

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            DoAppSetup();
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }

        private static void DoAppSetup()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMainMetro());
        }
    }
}
