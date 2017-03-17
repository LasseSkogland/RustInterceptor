namespace Rust_Interceptor.Forms
{
    partial class Overlay
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Overlay));
            this.textBoxIp = new System.Windows.Forms.TextBox();
            this.labelIp = new System.Windows.Forms.Label();
            this.textBoxPuerto = new System.Windows.Forms.TextBox();
            this.labelPuerto = new System.Windows.Forms.Label();
            this.buttonEmpezar = new System.Windows.Forms.Button();
            this.labelPlayers = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxIp
            // 
            this.textBoxIp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.textBoxIp, "textBoxIp");
            this.textBoxIp.Name = "textBoxIp";
            // 
            // labelIp
            // 
            resources.ApplyResources(this.labelIp, "labelIp");
            this.labelIp.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.labelIp.Name = "labelIp";
            // 
            // textBoxPuerto
            // 
            this.textBoxPuerto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.textBoxPuerto, "textBoxPuerto");
            this.textBoxPuerto.Name = "textBoxPuerto";
            // 
            // labelPuerto
            // 
            resources.ApplyResources(this.labelPuerto, "labelPuerto");
            this.labelPuerto.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelPuerto.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.labelPuerto.Name = "labelPuerto";
            // 
            // buttonEmpezar
            // 
            resources.ApplyResources(this.buttonEmpezar, "buttonEmpezar");
            this.buttonEmpezar.Name = "buttonEmpezar";
            this.buttonEmpezar.UseVisualStyleBackColor = true;
            // 
            // labelPlayers
            // 
            resources.ApplyResources(this.labelPlayers, "labelPlayers");
            this.labelPlayers.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.labelPlayers.Name = "labelPlayers";
            // 
            // Overlay
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Controls.Add(this.labelPlayers);
            this.Controls.Add(this.buttonEmpezar);
            this.Controls.Add(this.labelPuerto);
            this.Controls.Add(this.textBoxPuerto);
            this.Controls.Add(this.labelIp);
            this.Controls.Add(this.textBoxIp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "Overlay";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Overlay_FormClosed);
            this.Load += new System.EventHandler(this.Overlay_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxIp;
        private System.Windows.Forms.Label labelIp;
        private System.Windows.Forms.TextBox textBoxPuerto;
        private System.Windows.Forms.Label labelPuerto;
        private System.Windows.Forms.Button buttonEmpezar;
        private System.Windows.Forms.Label labelPlayers;
    }
}