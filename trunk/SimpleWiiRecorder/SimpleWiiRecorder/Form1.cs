﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WiimoteLib;
using System.IO;

namespace SimpleWiiRecorder
{
    public partial class Form1 : Form
    {
        Wiimote kontroler = new Wiimote();
        bool connected = false;
        Graphics g = null;
        Stream rec = null;
        Stream input = null;
        BinaryWriter wrec;
        BinaryReader rinput;
        Logmote l;
        System.Threading.Thread t;
        bool recording = false;
        bool playing = false;
        long time = 0;

        private delegate void UpdateWiimoteStateDelegate(WiimoteChangedEventArgs args);
        private delegate void LogStopHandler();

        public Form1()
        {
            InitializeComponent();
            g = pnlLED.CreateGraphics();
            //System.Diagnostics.Debug.WriteLine("RAAAAAADDI!");
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (connected)
                {
                    kontroler.Disconnect();
                    kontroler.WiimoteChanged -= UpdateState;
                    btnConnect.Text = "Connect";
                    connected = false;
                    lblMessage.Text = "Disconnected";
                }
                else
                {
                    btnConnect.Text = "Disconnect";
                    kontroler.Connect();
                    if (kontroler.WiimoteState.ExtensionType != ExtensionType.BalanceBoard)
                        kontroler.SetReportType(InputReport.IRExtensionAccel, IRSensitivity.Maximum, true);
                    kontroler.WiimoteChanged += UpdateState;
                    connected = true;
                    lblMessage.Text = "Connected";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void UpdateState(object sender, WiimoteChangedEventArgs args)
        {
            try
            {
                BeginInvoke(new UpdateWiimoteStateDelegate(UpdateWiimoteChanged), args);
            }
            catch (Exception ex)
            {
                Application.Exit();
            }
        }

        public void UpdateWiimoteChanged(WiimoteChangedEventArgs args)
        {
            WiimoteState ws = args.WiimoteState;

            //Buttons
            chkButtonA.Checked = ws.ButtonState.A;
            chkButtonB.Checked = ws.ButtonState.B;

            //Accelerometer
            lblAccelX.Text = ws.AccelState.Values.X.ToString();
            lblAccelY.Text = ws.AccelState.Values.Y.ToString();
            lblAccelZ.Text = ws.AccelState.Values.Z.ToString();

            //IR LED
            chkFoundS1.Checked = ws.IRState.IRSensors[0].Found;
            chkFoundS2.Checked = ws.IRState.IRSensors[1].Found;
            chkFoundS3.Checked = ws.IRState.IRSensors[2].Found;
            chkFoundS4.Checked = ws.IRState.IRSensors[3].Found;

            g.Clear(Color.Black);

            if (ws.IRState.IRSensors[0].Found)
            {
                lblLedS1X.Text = "X: " + ws.IRState.IRSensors[0].Position.X.ToString();
                lblLedS1Y.Text = "Y: " + ws.IRState.IRSensors[0].Position.Y.ToString();
                lblLedS1S.Text = "S: " + ws.IRState.IRSensors[0].Size.ToString();
                g.FillEllipse(new SolidBrush(Color.Green), ws.IRState.IRSensors[0].Position.X * pnlLED.Size.Width - 3,
                                                        ws.IRState.IRSensors[0].Position.Y * pnlLED.Size.Height - 3,
                                                        6, 
                                                        6);

                
            }
            if (ws.IRState.IRSensors[1].Found)
            {
                lblLedS2X.Text = "X: " + ws.IRState.IRSensors[1].Position.X.ToString();
                lblLedS2Y.Text = "Y: " + ws.IRState.IRSensors[1].Position.Y.ToString();
                lblLedS2S.Text = "S: " + ws.IRState.IRSensors[1].Size.ToString();
                g.FillEllipse(new SolidBrush(Color.Red), ws.IRState.IRSensors[1].Position.X * pnlLED.Size.Width - 3,
                                                        ws.IRState.IRSensors[1].Position.Y * pnlLED.Size.Height - 3,
                                                        6,
                                                        6);
            }
            if (ws.IRState.IRSensors[2].Found)
            {
                lblLedS3X.Text = "X: " + ws.IRState.IRSensors[2].Position.X.ToString();
                lblLedS3Y.Text = "Y: " + ws.IRState.IRSensors[2].Position.Y.ToString();
                lblLedS3S.Text = "S: " + ws.IRState.IRSensors[2].Size.ToString();
                g.FillEllipse(new SolidBrush(Color.Cyan), ws.IRState.IRSensors[2].Position.X * pnlLED.Size.Width - 3,
                                                        ws.IRState.IRSensors[2].Position.Y * pnlLED.Size.Height - 3,
                                                        6,
                                                        6);
            }
            if (ws.IRState.IRSensors[3].Found)
            {
                lblLedS4X.Text = "X: " + ws.IRState.IRSensors[3].Position.X.ToString();
                lblLedS4Y.Text = "Y: " + ws.IRState.IRSensors[3].Position.Y.ToString();
                lblLedS4S.Text = "S: " + ws.IRState.IRSensors[3].Size.ToString();
                g.FillEllipse(new SolidBrush(Color.Yellow), ws.IRState.IRSensors[3].Position.X * pnlLED.Size.Width - 3,
                                                        ws.IRState.IRSensors[3].Position.Y * pnlLED.Size.Height - 3,
                                                        6,
                                                        6);
            }
            if (recording)
            {
                write(ws);
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            kontroler.Disconnect();
        }

        public void write(WiimoteState ws)
        {
            if (rec != null && wrec != null)
            {
                wrec.Write(System.DateTime.Now.Ticks - time);
                wrec.Write(ws.ButtonState.A);
                wrec.Write(ws.ButtonState.B);
                wrec.Write(ws.AccelState.Values.X);
                wrec.Write(ws.AccelState.Values.Y);
                wrec.Write(ws.AccelState.Values.Z);
                for (int i = 0; i < 4; i++)
                {
                    wrec.Write(ws.IRState.IRSensors[i].Found);
                    wrec.Write(ws.IRState.IRSensors[i].Position.X);
                    wrec.Write(ws.IRState.IRSensors[i].Position.Y);
                    wrec.Write(ws.IRState.IRSensors[i].Size);
                }
            }
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            if(recording){
                btnRecord.Text = "Record";
                recording = false;
                btnLoad.Enabled = true;
                wrec.Close();
                rec.Close();
                time = System.DateTime.Now.Ticks;
            }
            else{
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.CheckFileExists = false;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    rec = new FileStream(dlg.FileName, FileMode.Create);
                    btnRecord.Text = "Stop";
                    btnLoad.Enabled = false;
                    recording = true;
                    wrec = new BinaryWriter(rec);
                }
            }
        }

        public void onStop(Object sender)
        {
            BeginInvoke(new LogStopHandler(stopHandler));
        }

        private void stopHandler()
        {
            playing = false;
            rinput.Close();
            input.Close();
            l.LogmoteChange -= UpdateState;
            l.LogmoteStop -= onStop;
            btnRecord.Enabled = true;
            btnLoad.Text = "Load";
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (playing)
            {
                playing = false;
                rinput.Close();
                input.Close();
                l.LogmoteChange -= UpdateState;
                l.LogmoteStop -= onStop;
                btnRecord.Enabled = true;
                btnLoad.Text = "Load";
            }
            else
            {
                OpenFileDialog dlg = new OpenFileDialog();
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    input = new FileStream(dlg.FileName, FileMode.Open);
                    btnLoad.Text = "Stop";
                    btnRecord.Enabled = false;
                    playing = true;
                    rinput = new BinaryReader(input);
                    
                    //Disconnect from real thing
                    kontroler.Disconnect();
                    kontroler.WiimoteChanged -= UpdateState;
                    btnConnect.Text = "Connect";
                    connected = false;

                    //Aaand connect to the fake
                    l = new Logmote(rinput);
                    l.LogmoteChange += UpdateState;
                    l.LogmoteStop += onStop;
                    //l.run();
                    t = new System.Threading.Thread(new System.Threading.ThreadStart(l.run));
                    t.Start();
                }
            }
        }

    }
}
