using Microsoft.Win32;
using System;
using System.Threading;
using System.Windows.Forms;

namespace WinFormsApp
{
    internal sealed class Program
    {
        static Mutex mutex = new Mutex(true, "{13AE3E4A-35CD-45F9-9AB4-B142EF8E900A}");
        static MainForm mainForm;

        public Program(bool showForm)
        {
            mainForm = new MainForm();
            if (showForm)
            {
                Show();
            }
        }

        public void Show()
        {
            mainForm.Show();
        }

        private void SetStartup(bool save)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (save)
                rk.SetValue("WinFormsApp", Application.ExecutablePath);
            else
                rk.DeleteValue("WinFormsApp", false);            

        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                try
                {
                    string[] launchArgs = Environment.GetCommandLineArgs();
                    if (launchArgs.Length > 1)
                    {
                        Program p = new Program(false);
                        if (mainForm.ShowlaunchArgs("args: " + launchArgs[1].Trim()) == DialogResult.OK)
                        {
                            p.Show();
                            Application.Run();
                        }
                    }
                    else
                    {
                        new Program(true);
                        Application.Run();
                    }
                }
                catch (UriFormatException) { }
            }
            else
            {
                MessageBox.Show("App is running");
            }
            mutex.ReleaseMutex();
        }
    }
}
