using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WiimoteLib;

namespace GimiiIR
{
    public partial class frmMain : Form
    {
        Graphics g;
        Wiimote w;
        bool connected = false;
        private delegate void UpdateWiimoteStateDelegate(WiimoteChangedEventArgs args);
        int lastx = -1;
        int lasty = -1;
        int lastsize = -1;

        int zfactor = 200;

        bool plus;
        bool minus;

        public frmMain()
        {
            InitializeComponent();
            g = pnlMain.CreateGraphics();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            w = new Wiimote();
            try
            {
                w.Connect();
                if (w.WiimoteState.ExtensionType != ExtensionType.BalanceBoard)
                    w.SetReportType(InputReport.IRExtensionAccel, IRSensitivity.Maximum, true);
                connected = true;
                w.WiimoteChanged += UpdateState;
                w.SetLEDs(true, false, false, false);
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
            int n, x, y, size;
            try
            {
                if (args.WiimoteState.ButtonState.Plus)
                {
                    plus = true;
                }
                else
                {
                    if (plus)
                    {
                        plus = false;
                        zfactor += 10;
                    }
                }

                if (args.WiimoteState.ButtonState.Minus)
                {
                    minus = true;
                }
                else
                {
                    if (minus)
                    {
                        minus = false;
                        zfactor -= 10;
                    }
                }

                if (!args.WiimoteState.ButtonState.A)
                {
                    g.Clear(Color.Black);
                }
                Point3F p = getPoint(args.WiimoteState.IRState, out n);
                g.DrawString(n.ToString(), new Font("Times", 12.0f), new SolidBrush(Color.White), 0, 0);
                g.DrawString((p.Z * zfactor).ToString(), new Font("Times", 12.0f), new SolidBrush(Color.White), 0, 25);
                g.DrawString(zfactor.ToString(), new Font("Times", 12.0f), new SolidBrush(Color.White), 0, 50);
                if (n == 2)
                {
                    x = pnlMain.Width - (int)(pnlMain.Size.Width * p.X);
                    y = (int)(pnlMain.Size.Height * p.Y);
                    size = (int)(p.Z * zfactor);
                    if (size > 24) size = 24;
                    if (size < 6) size = 6;
                    lastx = x;
                    lasty = y;
                    lastsize = size;
                }
                else
                {
                    x = lastx;
                    y = lasty;
                    size = lastsize;
                }
                //size = 6;
                if (args.WiimoteState.ButtonState.B)
                {
                    g.DrawEllipse(new Pen(Color.Lime), x - size/2, y - size/2, size, size);
                    g.FillEllipse(new SolidBrush(Color.Red), x - size/2, y - size/2, size, size);
                }
                else
                {
                    g.DrawEllipse(new Pen(Color.Red), x - size/2, y - size/2, size, size);
                    g.FillEllipse(new SolidBrush(Color.Lime), x - size/2, y - size/2, size, size);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public Point3F getPoint(IRState s, out int found)
        {
            Point3F p = new Point3F();
            p.X = 0;
            p.Y = 0;
            p.Z = 0;
            int t = 0;
            for (int i = 0; i < 4; i++)
            {
                if (s.IRSensors[i].Found)
                {
                    p.X += s.IRSensors[i].Position.X;
                    p.Y += s.IRSensors[i].Position.Y;
                    t++;
                }
            }
            p.X = p.X / ((t == 0) ? 1 : t);
            p.Y = p.Y / ((t == 0) ? 1 : t);
            if (t > 1)
            {
                p.Z = (float)Math.Sqrt(Math.Pow(s.IRSensors[0].Position.X - s.IRSensors[1].Position.X, 2.0) +
                                        Math.Pow(s.IRSensors[0].Position.Y - s.IRSensors[1].Position.Y, 2.0));
            }
            if (p.X > 1.0f) p.X = 1.0f;
            if (p.Y > 1.0f) p.Y = 1.0f;
            found = t;
            return p;
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (connected)
                    w.Disconnect();
            }
            catch (Exception ex)
            {
                Application.Exit();
            }
        }

        private void pnlMain_Resize(object sender, EventArgs e)
        {
            lastx = -1;
            lasty = -1;
            g = pnlMain.CreateGraphics();
        }
    }
}
