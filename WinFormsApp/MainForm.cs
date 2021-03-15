using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Windows.Forms;

namespace WinFormsApp
{
    public partial class MainForm : Form
    {
        static string ServiceName = "DaVcheztService";
        static string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\DaVcheztService.exe";

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (!IsServiceInstalled()) return;

            try
            {
                ServiceController sc = new ServiceController(ServiceName);
                if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    sc.Start();                }
                else
                {
                    if (sc.Status == ServiceControllerStatus.Running && sc.CanStop)
                    {
                        sc.Stop();
                    }
                    else throw new Exception("Failed to stoping service");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        internal DialogResult ShowlaunchArgs(string launchArgs)
        {
            return MessageBox.Show(null, "args: " + launchArgs, MessageBoxButtons.OKCancel);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            if (MessageBox.Show(this, "Are you sure?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnInstallUninstall_Click(object sender, EventArgs e)
        {
            if (File.Exists(filePath))
            {
                Assembly assembly = Assembly.LoadFrom(filePath);
                if (IsServiceInstalled())
                {
                    UninstallService(assembly);
                }
                else
                {
                    InstallService(assembly);
                }
            }
        }

        public void InstallService(Assembly assembly)
        {
            if (IsServiceInstalled()) return;

            using (AssemblyInstaller installer = GetInstaller(assembly))
            {
                IDictionary state = new Hashtable();
                try
                {
                    installer.Install(state);
                    installer.Commit(state);
                    installer.Dispose();
                }
                catch
                {
                    try
                    {
                        installer.Rollback(state);
                        installer.Dispose();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    throw;
                }
            }
        }

        public void UninstallService(Assembly assembly)
        {
            btnInstallUninstall.Text = "Processing...";
            using (AssemblyInstaller installer = GetInstaller(assembly))
            {
                try
                {
                    installer.Uninstall(null);
                    installer.Dispose();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public bool IsServiceInstalled()
        {
            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == ServiceName);
            return ctl != null;

            /*using (ServiceController controller = new ServiceController(ServiceName))
            {
                try
                {
                    ServiceControllerStatus status = controller.Status;
                }
                catch
                {
                    return false;
                }

                return true;
            }*/
        }

        private AssemblyInstaller GetInstaller(Assembly assembly)
        {
            AssemblyInstaller installer = new AssemblyInstaller(assembly, null);
            installer.UseNewContext = true;

            return installer;
        }

        private void UpdateButtons()
        {
            try
            {
                string btnInstallUninstallText = IsServiceInstalled() ? "Uninstall" : "Install";
                btnInstallUninstall.Text = btnInstallUninstallText;

                btnStartStop.Enabled = IsServiceInstalled();
                if (IsServiceInstalled())
                {
                    ServiceController sc = new ServiceController(ServiceName);
                    bool isRunning = sc.Status == ServiceControllerStatus.Running;

                    string btnStartStopText = isRunning ? "Stop" : "Start";
                    btnStartStop.Text = btnStartStopText;

                    btnInstallUninstall.Enabled = isRunning ? false : true;
                }
            }
            catch { }
        }

        private void tmrUpdateButton_Tick(object sender, EventArgs e)
        {
            UpdateButtons();
        }
    }
}
