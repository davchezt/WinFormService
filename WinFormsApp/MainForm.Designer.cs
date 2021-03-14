
namespace WinFormsApp
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.btnInstallUninstall = new System.Windows.Forms.Button();
            this.tmrUpdateButton = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(12, 12);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(129, 55);
            this.btnStartStop.TabIndex = 0;
            this.btnStartStop.Text = "Start";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // btnInstallUninstall
            // 
            this.btnInstallUninstall.Location = new System.Drawing.Point(353, 12);
            this.btnInstallUninstall.Name = "btnInstallUninstall";
            this.btnInstallUninstall.Size = new System.Drawing.Size(129, 55);
            this.btnInstallUninstall.TabIndex = 1;
            this.btnInstallUninstall.Text = "Install";
            this.btnInstallUninstall.UseVisualStyleBackColor = true;
            this.btnInstallUninstall.Click += new System.EventHandler(this.btnInstallUninstall_Click);
            // 
            // tmrUpdateButton
            // 
            this.tmrUpdateButton.Enabled = true;
            this.tmrUpdateButton.Tick += new System.EventHandler(this.tmrUpdateButton_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 79);
            this.Controls.Add(this.btnInstallUninstall);
            this.Controls.Add(this.btnStartStop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main App";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Button btnInstallUninstall;
        private System.Windows.Forms.Timer tmrUpdateButton;
    }
}

