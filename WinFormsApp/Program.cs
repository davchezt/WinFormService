using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;

namespace WinFormsApp
{
    internal sealed class Program
    {
        static Mutex mutex = new Mutex(true, @"Global\41d7d4bf-58f7-4861-9f46-95f582ef26b6");
        static MainForm mainForm;

        public Program(bool showForm)
        {
            mainForm = (MainForm)GetOpenedForm<MainForm>();
            if (mainForm == null)
            {
                mainForm = new MainForm();
                if (showForm)
                {
                    Show();
                }
            }
            else
            {
                mainForm.Select();
            }
        }

        public void Show()
        {
            mainForm.Show();
        }

        public static Form GetOpenedForm<T>() where T : Form
        {
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm.GetType() == typeof(T))
                {
                    return openForm;
                }
            }
            return null;
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

                string[] launchArgs = Environment.GetCommandLineArgs();
                try
                {
                    if (launchArgs.Length > 1)
                    {
                        MainForm mForm = new MainForm();
                        if (MessageBox.Show(null, launchArgs[1].Trim(), MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            mForm.Show();
                            Application.Run();
                        }
                        else
                        {
                            mForm.Dispose();
                            Application.Exit();
                        }
                    }
                    else
                    {
                        Application.Run(new MainForm());
                    }
                }
                catch { }
                mutex.ReleaseMutex();
            }
            else
            {
                string[] launchArgs = Environment.GetCommandLineArgs();
                try
                {
                    if (launchArgs.Length > 1)
                    {
                        try
                        {
                            NativeMethods.sendWindowsStringMessage((int)NativeMethods.HWND_BROADCAST, 0, launchArgs[1].Trim());
                        }
                        catch { }
                    }
                    else
                    {
                        NativeMethods.PostMessage((IntPtr)NativeMethods.HWND_BROADCAST, NativeMethods.WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
                    }
                }
                catch { }
            }
        }
    }
}
