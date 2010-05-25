namespace VRWiiPaint
{
    partial class Kalibracija
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
            this.btnMaxXY = new System.Windows.Forms.Button();
            this.btnMinXY = new System.Windows.Forms.Button();
            this.btnKalibracija = new System.Windows.Forms.Button();
            this.panelKalibracija = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.lbPoruka = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnMaxXY
            // 
            this.btnMaxXY.Location = new System.Drawing.Point(134, 408);
            this.btnMaxXY.Name = "btnMaxXY";
            this.btnMaxXY.Size = new System.Drawing.Size(75, 23);
            this.btnMaxXY.TabIndex = 8;
            this.btnMaxXY.Text = "MaxX, MaxY";
            this.btnMaxXY.UseVisualStyleBackColor = true;
            this.btnMaxXY.Click += new System.EventHandler(this.btnMaxXY_Click);
            // 
            // btnMinXY
            // 
            this.btnMinXY.Location = new System.Drawing.Point(53, 408);
            this.btnMinXY.Name = "btnMinXY";
            this.btnMinXY.Size = new System.Drawing.Size(75, 23);
            this.btnMinXY.TabIndex = 7;
            this.btnMinXY.Text = "MinX, MinY";
            this.btnMinXY.UseVisualStyleBackColor = true;
            this.btnMinXY.Click += new System.EventHandler(this.btnMinXY_Click);
            // 
            // btnKalibracija
            // 
            this.btnKalibracija.Enabled = false;
            this.btnKalibracija.Location = new System.Drawing.Point(239, 408);
            this.btnKalibracija.Name = "btnKalibracija";
            this.btnKalibracija.Size = new System.Drawing.Size(63, 23);
            this.btnKalibracija.TabIndex = 6;
            this.btnKalibracija.Text = "Kalibracija";
            this.btnKalibracija.UseVisualStyleBackColor = true;
            this.btnKalibracija.Click += new System.EventHandler(this.btnKalibracija_Click);
            // 
            // panelKalibracija
            // 
            this.panelKalibracija.BackColor = System.Drawing.Color.Black;
            this.panelKalibracija.ForeColor = System.Drawing.Color.Black;
            this.panelKalibracija.Location = new System.Drawing.Point(53, 67);
            this.panelKalibracija.Name = "panelKalibracija";
            this.panelKalibracija.Size = new System.Drawing.Size(424, 320);
            this.panelKalibracija.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(46, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(431, 37);
            this.label1.TabIndex = 10;
            this.label1.Text = "Kalibracija radne povrsine.";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(308, 408);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 11;
            this.btnReset.Text = "Resetovanje";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(420, 408);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(57, 23);
            this.btnOk.TabIndex = 12;
            this.btnOk.Text = "U redu";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // lbPoruka
            // 
            this.lbPoruka.AutoSize = true;
            this.lbPoruka.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPoruka.Location = new System.Drawing.Point(49, 471);
            this.lbPoruka.Name = "lbPoruka";
            this.lbPoruka.Size = new System.Drawing.Size(0, 19);
            this.lbPoruka.TabIndex = 13;
            // 
            // Kalibracija
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 499);
            this.Controls.Add(this.lbPoruka);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panelKalibracija);
            this.Controls.Add(this.btnMaxXY);
            this.Controls.Add(this.btnMinXY);
            this.Controls.Add(this.btnKalibracija);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(544, 535);
            this.MinimumSize = new System.Drawing.Size(544, 535);
            this.Name = "Kalibracija";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Wii Paint";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnMaxXY;
        private System.Windows.Forms.Button btnMinXY;
        private System.Windows.Forms.Button btnKalibracija;
        private System.Windows.Forms.Panel panelKalibracija;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label lbPoruka;
    }
}