using System;
namespace VRWiiPaint
{
    partial class Form1
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
            this.panelKalibracija = new System.Windows.Forms.Panel();
            this.panelCrtanje = new System.Windows.Forms.Panel();
            this.panelAlatke = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelKalibracija
            // 
            this.panelKalibracija.BackColor = System.Drawing.Color.Black;
            this.panelKalibracija.ForeColor = System.Drawing.Color.Black;
            this.panelKalibracija.Location = new System.Drawing.Point(730, 0);
            this.panelKalibracija.Name = "panelKalibracija";
            this.panelKalibracija.Size = new System.Drawing.Size(274, 220);
            this.panelKalibracija.TabIndex = 0;
            // 
            // panelCrtanje
            // 
            this.panelCrtanje.BackColor = System.Drawing.Color.White;
            this.panelCrtanje.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelCrtanje.Location = new System.Drawing.Point(0, 0);
            this.panelCrtanje.Name = "panelCrtanje";
            this.panelCrtanje.Size = new System.Drawing.Size(730, 700);
            this.panelCrtanje.TabIndex = 6;
            // 
            // panelAlatke
            // 
            this.panelAlatke.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panelAlatke.ForeColor = System.Drawing.Color.Black;
            this.panelAlatke.Location = new System.Drawing.Point(730, 220);
            this.panelAlatke.Name = "panelAlatke";
            this.panelAlatke.Size = new System.Drawing.Size(274, 480);
            this.panelAlatke.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Controls.Add(this.panelAlatke);
            this.Controls.Add(this.panelCrtanje);
            this.Controls.Add(this.panelKalibracija);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1016, 736);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1016, 736);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Wii Paint";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelKalibracija;
        private System.Windows.Forms.Panel panelCrtanje;
        private System.Windows.Forms.Panel panelAlatke;
        
    }
}

