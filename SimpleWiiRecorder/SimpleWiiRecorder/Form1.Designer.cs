namespace SimpleWiiRecorder
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.pnlLED = new System.Windows.Forms.Panel();
            this.btnRecord = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.lblAccelX = new System.Windows.Forms.Label();
            this.lblAccelY = new System.Windows.Forms.Label();
            this.lblAccelZ = new System.Windows.Forms.Label();
            this.chkButtonA = new System.Windows.Forms.CheckBox();
            this.chkButtonB = new System.Windows.Forms.CheckBox();
            this.lblLedS1X = new System.Windows.Forms.Label();
            this.lblLedS1Y = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblLedS2Y = new System.Windows.Forms.Label();
            this.lblLedS2X = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblLedS3Y = new System.Windows.Forms.Label();
            this.lblLedS3X = new System.Windows.Forms.Label();
            this.chkFoundS1 = new System.Windows.Forms.CheckBox();
            this.chkFoundS2 = new System.Windows.Forms.CheckBox();
            this.chkFoundS3 = new System.Windows.Forms.CheckBox();
            this.chkFoundS4 = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.lblLedS4Y = new System.Windows.Forms.Label();
            this.lblLedS4X = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblLedS1S = new System.Windows.Forms.Label();
            this.lblLedS3S = new System.Windows.Forms.Label();
            this.lblLedS4S = new System.Windows.Forms.Label();
            this.lblLedS2S = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblAccelZ);
            this.groupBox1.Controls.Add(this.lblAccelY);
            this.groupBox1.Controls.Add(this.lblAccelX);
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(239, 122);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Akcelerometer";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblLedS2S);
            this.groupBox2.Controls.Add(this.lblLedS4S);
            this.groupBox2.Controls.Add(this.lblLedS3S);
            this.groupBox2.Controls.Add(this.lblLedS1S);
            this.groupBox2.Controls.Add(this.chkFoundS4);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.lblLedS4Y);
            this.groupBox2.Controls.Add(this.lblLedS4X);
            this.groupBox2.Controls.Add(this.chkFoundS3);
            this.groupBox2.Controls.Add(this.chkFoundS2);
            this.groupBox2.Controls.Add(this.chkFoundS1);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.lblLedS3Y);
            this.groupBox2.Controls.Add(this.lblLedS3X);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.lblLedS2Y);
            this.groupBox2.Controls.Add(this.lblLedS2X);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.lblLedS1Y);
            this.groupBox2.Controls.Add(this.lblLedS1X);
            this.groupBox2.Location = new System.Drawing.Point(289, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(295, 122);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "IR LED";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkButtonB);
            this.groupBox3.Controls.Add(this.chkButtonA);
            this.groupBox3.Location = new System.Drawing.Point(12, 141);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(238, 89);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Dugmici";
            // 
            // pnlLED
            // 
            this.pnlLED.BackColor = System.Drawing.SystemColors.ControlText;
            this.pnlLED.Location = new System.Drawing.Point(292, 148);
            this.pnlLED.Name = "pnlLED";
            this.pnlLED.Size = new System.Drawing.Size(292, 157);
            this.pnlLED.TabIndex = 3;
            // 
            // btnRecord
            // 
            this.btnRecord.Location = new System.Drawing.Point(12, 250);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(75, 28);
            this.btnRecord.TabIndex = 4;
            this.btnRecord.Text = "Record";
            this.btnRecord.UseVisualStyleBackColor = true;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(178, 249);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(73, 29);
            this.btnLoad.TabIndex = 5;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(12, 295);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(73, 13);
            this.lblMessage.TabIndex = 6;
            this.lblMessage.Text = "Disconnected";
            // 
            // lblAccelX
            // 
            this.lblAccelX.AutoSize = true;
            this.lblAccelX.Location = new System.Drawing.Point(6, 16);
            this.lblAccelX.Name = "lblAccelX";
            this.lblAccelX.Size = new System.Drawing.Size(20, 13);
            this.lblAccelX.TabIndex = 0;
            this.lblAccelX.Text = "X: ";
            // 
            // lblAccelY
            // 
            this.lblAccelY.AutoSize = true;
            this.lblAccelY.Location = new System.Drawing.Point(6, 45);
            this.lblAccelY.Name = "lblAccelY";
            this.lblAccelY.Size = new System.Drawing.Size(20, 13);
            this.lblAccelY.TabIndex = 1;
            this.lblAccelY.Text = "Y: ";
            // 
            // lblAccelZ
            // 
            this.lblAccelZ.AutoSize = true;
            this.lblAccelZ.Location = new System.Drawing.Point(6, 74);
            this.lblAccelZ.Name = "lblAccelZ";
            this.lblAccelZ.Size = new System.Drawing.Size(20, 13);
            this.lblAccelZ.TabIndex = 2;
            this.lblAccelZ.Text = "Z: ";
            // 
            // chkButtonA
            // 
            this.chkButtonA.AutoSize = true;
            this.chkButtonA.Location = new System.Drawing.Point(13, 33);
            this.chkButtonA.Name = "chkButtonA";
            this.chkButtonA.Size = new System.Drawing.Size(33, 17);
            this.chkButtonA.TabIndex = 0;
            this.chkButtonA.Text = "A";
            this.chkButtonA.UseVisualStyleBackColor = true;
            // 
            // chkButtonB
            // 
            this.chkButtonB.AutoSize = true;
            this.chkButtonB.Location = new System.Drawing.Point(199, 33);
            this.chkButtonB.Name = "chkButtonB";
            this.chkButtonB.Size = new System.Drawing.Size(33, 17);
            this.chkButtonB.TabIndex = 1;
            this.chkButtonB.Text = "B";
            this.chkButtonB.UseVisualStyleBackColor = true;
            // 
            // lblLedS1X
            // 
            this.lblLedS1X.AutoSize = true;
            this.lblLedS1X.Location = new System.Drawing.Point(58, 21);
            this.lblLedS1X.Name = "lblLedS1X";
            this.lblLedS1X.Size = new System.Drawing.Size(17, 13);
            this.lblLedS1X.TabIndex = 0;
            this.lblLedS1X.Text = "X:";
            // 
            // lblLedS1Y
            // 
            this.lblLedS1Y.AutoSize = true;
            this.lblLedS1Y.Location = new System.Drawing.Point(108, 20);
            this.lblLedS1Y.Name = "lblLedS1Y";
            this.lblLedS1Y.Size = new System.Drawing.Size(17, 13);
            this.lblLedS1Y.TabIndex = 1;
            this.lblLedS1Y.Text = "Y:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Senzor 1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Senzor 2";
            // 
            // lblLedS2Y
            // 
            this.lblLedS2Y.AutoSize = true;
            this.lblLedS2Y.Location = new System.Drawing.Point(108, 44);
            this.lblLedS2Y.Name = "lblLedS2Y";
            this.lblLedS2Y.Size = new System.Drawing.Size(17, 13);
            this.lblLedS2Y.TabIndex = 4;
            this.lblLedS2Y.Text = "Y:";
            // 
            // lblLedS2X
            // 
            this.lblLedS2X.AutoSize = true;
            this.lblLedS2X.Location = new System.Drawing.Point(58, 45);
            this.lblLedS2X.Name = "lblLedS2X";
            this.lblLedS2X.Size = new System.Drawing.Size(17, 13);
            this.lblLedS2X.TabIndex = 3;
            this.lblLedS2X.Text = "X:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 74);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Senzor 3";
            // 
            // lblLedS3Y
            // 
            this.lblLedS3Y.AutoSize = true;
            this.lblLedS3Y.Location = new System.Drawing.Point(108, 73);
            this.lblLedS3Y.Name = "lblLedS3Y";
            this.lblLedS3Y.Size = new System.Drawing.Size(17, 13);
            this.lblLedS3Y.TabIndex = 7;
            this.lblLedS3Y.Text = "Y:";
            // 
            // lblLedS3X
            // 
            this.lblLedS3X.AutoSize = true;
            this.lblLedS3X.Location = new System.Drawing.Point(58, 74);
            this.lblLedS3X.Name = "lblLedS3X";
            this.lblLedS3X.Size = new System.Drawing.Size(17, 13);
            this.lblLedS3X.TabIndex = 6;
            this.lblLedS3X.Text = "X:";
            // 
            // chkFoundS1
            // 
            this.chkFoundS1.AutoSize = true;
            this.chkFoundS1.Location = new System.Drawing.Point(233, 16);
            this.chkFoundS1.Name = "chkFoundS1";
            this.chkFoundS1.Size = new System.Drawing.Size(56, 17);
            this.chkFoundS1.TabIndex = 9;
            this.chkFoundS1.Text = "Found";
            this.chkFoundS1.UseVisualStyleBackColor = true;
            // 
            // chkFoundS2
            // 
            this.chkFoundS2.AutoSize = true;
            this.chkFoundS2.Location = new System.Drawing.Point(233, 40);
            this.chkFoundS2.Name = "chkFoundS2";
            this.chkFoundS2.Size = new System.Drawing.Size(56, 17);
            this.chkFoundS2.TabIndex = 10;
            this.chkFoundS2.Text = "Found";
            this.chkFoundS2.UseVisualStyleBackColor = true;
            this.chkFoundS2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // chkFoundS3
            // 
            this.chkFoundS3.AutoSize = true;
            this.chkFoundS3.Location = new System.Drawing.Point(233, 69);
            this.chkFoundS3.Name = "chkFoundS3";
            this.chkFoundS3.Size = new System.Drawing.Size(56, 17);
            this.chkFoundS3.TabIndex = 11;
            this.chkFoundS3.Text = "Found";
            this.chkFoundS3.UseVisualStyleBackColor = true;
            // 
            // chkFoundS4
            // 
            this.chkFoundS4.AutoSize = true;
            this.chkFoundS4.Location = new System.Drawing.Point(233, 96);
            this.chkFoundS4.Name = "chkFoundS4";
            this.chkFoundS4.Size = new System.Drawing.Size(56, 17);
            this.chkFoundS4.TabIndex = 15;
            this.chkFoundS4.Text = "Found";
            this.chkFoundS4.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 101);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(49, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "Senzor 4";
            // 
            // lblLedS4Y
            // 
            this.lblLedS4Y.AutoSize = true;
            this.lblLedS4Y.Location = new System.Drawing.Point(108, 100);
            this.lblLedS4Y.Name = "lblLedS4Y";
            this.lblLedS4Y.Size = new System.Drawing.Size(17, 13);
            this.lblLedS4Y.TabIndex = 13;
            this.lblLedS4Y.Text = "Y:";
            // 
            // lblLedS4X
            // 
            this.lblLedS4X.AutoSize = true;
            this.lblLedS4X.Location = new System.Drawing.Point(58, 101);
            this.lblLedS4X.Name = "lblLedS4X";
            this.lblLedS4X.Size = new System.Drawing.Size(17, 13);
            this.lblLedS4X.TabIndex = 12;
            this.lblLedS4X.Text = "X:";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(93, 249);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(79, 29);
            this.btnConnect.TabIndex = 7;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblLedS1S
            // 
            this.lblLedS1S.AutoSize = true;
            this.lblLedS1S.Location = new System.Drawing.Point(161, 20);
            this.lblLedS1S.Name = "lblLedS1S";
            this.lblLedS1S.Size = new System.Drawing.Size(20, 13);
            this.lblLedS1S.TabIndex = 16;
            this.lblLedS1S.Text = "S: ";
            // 
            // lblLedS3S
            // 
            this.lblLedS3S.AutoSize = true;
            this.lblLedS3S.Location = new System.Drawing.Point(161, 70);
            this.lblLedS3S.Name = "lblLedS3S";
            this.lblLedS3S.Size = new System.Drawing.Size(17, 13);
            this.lblLedS3S.TabIndex = 17;
            this.lblLedS3S.Text = "S:";
            // 
            // lblLedS4S
            // 
            this.lblLedS4S.AutoSize = true;
            this.lblLedS4S.Location = new System.Drawing.Point(161, 97);
            this.lblLedS4S.Name = "lblLedS4S";
            this.lblLedS4S.Size = new System.Drawing.Size(17, 13);
            this.lblLedS4S.TabIndex = 18;
            this.lblLedS4S.Text = "S:";
            // 
            // lblLedS2S
            // 
            this.lblLedS2S.AutoSize = true;
            this.lblLedS2S.Location = new System.Drawing.Point(161, 45);
            this.lblLedS2S.Name = "lblLedS2S";
            this.lblLedS2S.Size = new System.Drawing.Size(17, 13);
            this.lblLedS2S.TabIndex = 19;
            this.lblLedS2S.Text = "S:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 317);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnRecord);
            this.Controls.Add(this.pnlLED);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "SimpleWiiRecorder";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Panel pnlLED;
        private System.Windows.Forms.Button btnRecord;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblAccelZ;
        private System.Windows.Forms.Label lblAccelY;
        private System.Windows.Forms.Label lblAccelX;
        private System.Windows.Forms.CheckBox chkButtonB;
        private System.Windows.Forms.CheckBox chkButtonA;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblLedS3Y;
        private System.Windows.Forms.Label lblLedS3X;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblLedS2Y;
        private System.Windows.Forms.Label lblLedS2X;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblLedS1Y;
        private System.Windows.Forms.Label lblLedS1X;
        private System.Windows.Forms.CheckBox chkFoundS3;
        private System.Windows.Forms.CheckBox chkFoundS2;
        private System.Windows.Forms.CheckBox chkFoundS1;
        private System.Windows.Forms.CheckBox chkFoundS4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblLedS4Y;
        private System.Windows.Forms.Label lblLedS4X;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblLedS2S;
        private System.Windows.Forms.Label lblLedS4S;
        private System.Windows.Forms.Label lblLedS3S;
        private System.Windows.Forms.Label lblLedS1S;
    }
}

