namespace DaVcheztService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.sPInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.sInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // sPInstaller
            // 
            this.sPInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.sPInstaller.Password = null;
            this.sPInstaller.Username = null;
            // 
            // sInstaller
            // 
            this.sInstaller.Description = "DaVchezt Service";
            this.sInstaller.DisplayName = "DaVchezt.Service";
            this.sInstaller.ServiceName = "DaVcheztService";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.sPInstaller,
            this.sInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller sPInstaller;
        private System.ServiceProcess.ServiceInstaller sInstaller;
    }
}