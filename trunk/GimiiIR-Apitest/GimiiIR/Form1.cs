using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WiimoteLib;
using WiiApi;

namespace GimiiIR
{
    public partial class frmMain : Form
    {
        Graphics g;
        Kontroler w;
        bool connected = false;
        private delegate void Obrada(ParametriDogadjaja p);
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
            WiiFabrika.dobaviInstancu().postaviTipKontrolera(WiiTip.WII_KONTROLER);
            w = WiiFabrika.dobaviInstancu().kreirajKontroler();
            //w = WiiFabrika.dobaviInstancu().kreirajKontroler("C:\\zzz");
            try
            {
                w.kreni(true);
                connected = true;
                w.PromenaStanja += promenaStanja;
                w.postaviLED(0, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void promenaStanja(object sender, ParametriDogadjaja p)
        {
            BeginInvoke(new Obrada(UpdateWiimoteChanged), p);
        }

        public void UpdateWiimoteChanged(ParametriDogadjaja args)
        {
            int n, x, y, size;
            try
            {
                if (args.Stanje.Dugmici.PLUS)
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

                if (args.Stanje.Dugmici.MINUS)
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

                if (!args.Stanje.Dugmici.A)
                {
                    g.Clear(Color.Black);
                }
                Point3F p = getPoint(args.Stanje.Senzori, out n);
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
                if (args.Stanje.Dugmici.B)
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

        public Point3F getPoint(ICSenzor[] s, out int found)
        {
            Point3F p = new Point3F();
            p.X = 0;
            p.Y = 0;
            p.Z = 0;
            int t = 0;
            for (int i = 0; i < 4; i++)
            {
                if (s[i].Nadjen)
                {
                    p.X += s[i].X;
                    p.Y += s[i].Y;
                    t++;
                }
            }
            p.X = p.X / ((t == 0) ? 1 : t);
            p.Y = p.Y / ((t == 0) ? 1 : t);
            if (t > 1)
            {
                p.Z = (float)Math.Sqrt(Math.Pow(s[0].X - s[1].X, 2.0) +
                                        Math.Pow(s[0].Y - s[1].Y, 2.0));
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
                    w.prekiniKomunikaciju();
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
