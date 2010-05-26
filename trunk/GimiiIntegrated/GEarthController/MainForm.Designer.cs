namespace GEarthController
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
            this.sliderLat = new System.Windows.Forms.TrackBar();
            this.sliderLong = new System.Windows.Forms.TrackBar();
            this.sliderZoom = new System.Windows.Forms.TrackBar();
            this.sliderAzimuth = new System.Windows.Forms.TrackBar();
            this.sliderTilt = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.sliderLat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sliderLong)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sliderZoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sliderAzimuth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sliderTilt)).BeginInit();
            this.SuspendLayout();
            // 
            // sliderLat
            // 
            this.sliderLat.Location = new System.Drawing.Point(12, 29);
            this.sliderLat.Maximum = 90;
            this.sliderLat.Minimum = -90;
            this.sliderLat.Name = "sliderLat";
            this.sliderLat.Size = new System.Drawing.Size(874, 45);
            this.sliderLat.TabIndex = 1;
            this.sliderLat.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.sliderLat.Scroll += new System.EventHandler(this.onChange);
            this.sliderLat.MouseDown += new System.Windows.Forms.MouseEventHandler(this.onMouseDown);
            this.sliderLat.MouseUp += new System.Windows.Forms.MouseEventHandler(this.onMouseUp);
            // 
            // sliderLong
            // 
            this.sliderLong.Location = new System.Drawing.Point(12, 97);
            this.sliderLong.Maximum = 180;
            this.sliderLong.Minimum = -180;
            this.sliderLong.Name = "sliderLong";
            this.sliderLong.Size = new System.Drawing.Size(874, 45);
            this.sliderLong.TabIndex = 2;
            this.sliderLong.TickFrequency = 2;
            this.sliderLong.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.sliderLong.Scroll += new System.EventHandler(this.onChange);
            this.sliderLong.MouseDown += new System.Windows.Forms.MouseEventHandler(this.onMouseDown);
            this.sliderLong.MouseUp += new System.Windows.Forms.MouseEventHandler(this.onMouseUp);
            // 
            // sliderZoom
            // 
            this.sliderZoom.LargeChange = 500;
            this.sliderZoom.Location = new System.Drawing.Point(12, 165);
            this.sliderZoom.Maximum = 20000000;
            this.sliderZoom.Minimum = 300;
            this.sliderZoom.Name = "sliderZoom";
            this.sliderZoom.Size = new System.Drawing.Size(874, 45);
            this.sliderZoom.SmallChange = 50;
            this.sliderZoom.TabIndex = 3;
            this.sliderZoom.TickFrequency = 1000000;
            this.sliderZoom.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.sliderZoom.Value = 10000000;
            this.sliderZoom.Scroll += new System.EventHandler(this.onChange);
            this.sliderZoom.MouseDown += new System.Windows.Forms.MouseEventHandler(this.onMouseDown);
            this.sliderZoom.MouseUp += new System.Windows.Forms.MouseEventHandler(this.onMouseUp);
            // 
            // sliderAzimuth
            // 
            this.sliderAzimuth.Location = new System.Drawing.Point(12, 233);
            this.sliderAzimuth.Maximum = 180;
            this.sliderAzimuth.Minimum = -180;
            this.sliderAzimuth.Name = "sliderAzimuth";
            this.sliderAzimuth.Size = new System.Drawing.Size(874, 45);
            this.sliderAzimuth.TabIndex = 4;
            this.sliderAzimuth.TickFrequency = 2;
            this.sliderAzimuth.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.sliderAzimuth.Scroll += new System.EventHandler(this.onChange);
            this.sliderAzimuth.MouseDown += new System.Windows.Forms.MouseEventHandler(this.onMouseDown);
            this.sliderAzimuth.MouseUp += new System.Windows.Forms.MouseEventHandler(this.onMouseUp);
            // 
            // sliderTilt
            // 
            this.sliderTilt.Location = new System.Drawing.Point(12, 301);
            this.sliderTilt.Maximum = 90;
            this.sliderTilt.Name = "sliderTilt";
            this.sliderTilt.Size = new System.Drawing.Size(874, 45);
            this.sliderTilt.TabIndex = 5;
            this.sliderTilt.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.sliderTilt.Scroll += new System.EventHandler(this.onChange);
            this.sliderTilt.MouseDown += new System.Windows.Forms.MouseEventHandler(this.onMouseDown);
            this.sliderTilt.MouseUp += new System.Windows.Forms.MouseEventHandler(this.onMouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Latitude";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Longitude";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 149);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Eye altitude";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 217);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Azimuth";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 285);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(21, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Tilt";
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 2000;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(898, 358);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sliderTilt);
            this.Controls.Add(this.sliderAzimuth);
            this.Controls.Add(this.sliderZoom);
            this.Controls.Add(this.sliderLong);
            this.Controls.Add(this.sliderLat);
            this.Name = "MainForm";
            this.Text = "Google Earth Controller";
            ((System.ComponentModel.ISupportInitialize)(this.sliderLat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sliderLong)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sliderZoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sliderAzimuth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sliderTilt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar sliderLat;
        private System.Windows.Forms.TrackBar sliderLong;
        private System.Windows.Forms.TrackBar sliderZoom;
        private System.Windows.Forms.TrackBar sliderAzimuth;
        private System.Windows.Forms.TrackBar sliderTilt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Timer timerUpdate;
    }
}

