using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WiiApi;

namespace TwoPaneDemo
{
    public partial class Kalibracija : Form
    {
        Kontroler wLeft;
        Kontroler wRight;
        bool connected = false;
        private delegate void Obrada(ParametriDogadjaja p);
        Graphics gLeft;
        Graphics gRight;
        Graphics gCalc;
        float size = 5;

        float minX = 0;
        float maxX = 1;
        float minY = 0;
        float maxY = 1;
        float minZ = 1.5f;
        float maxZ = 10;

        private bool calibrateMinX = false;
        private bool calibrateMaxX = false;
        private bool calibrateMinY = false;
        private bool calibrateMaxY = false;
        private bool calibrateMinZ = false;
        private bool calibrateMaxZ = false;

        public Kalibracija()
        {
            InitializeComponent();
            gLeft = pnlLeft.CreateGraphics();
            gRight = pnlRight.CreateGraphics();
            gCalc = pnlCalculated.CreateGraphics();
        }

        private void Kalibracija_Load(object sender, EventArgs e)
        {
            WiiFabrika.dobaviInstancu().postaviTipKontrolera(WiiTip.WII_KONTROLER);
            wLeft = WiiFabrika.dobaviInstancu().kreirajKontroler();
            wRight = WiiFabrika.dobaviInstancu().kreirajKontroler();
            //lblMessages.Text = wLeft.Identifikator.ToString() + "\n" + wRight.Identifikator.ToString();
            /*Kontroler k = null;
            while ((k = WiiFabrika.dobaviInstancu().kreirajKontroler()) != null)
            {
                Guid x = k.Identifikator;
                lblMessages.Text += x.ToString() + "\n";
            }*/
            //lblMessages.Text = wLeft.Identifikator.ToString() + " ****** " + wRight.Identifikator.ToString();
            try
            {
                wLeft.kreni(true);
                wRight.kreni(true);
                connected = true;
                wLeft.PromenaStanja += promenaStanjaLeft;
                wRight.PromenaStanja += promenaStanjaRight;
                wLeft.postaviLED(0, true);
                wLeft.postaviLED(1, false);
                wLeft.postaviLED(2, true);
                wLeft.postaviLED(3, false);
                //wRight.postaviVibrator(true);
                wRight.postaviLED(0, false);
                wRight.postaviLED(1, true);
                wRight.postaviLED(2, false);
                wRight.postaviLED(3, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void promenaStanjaLeft(object sender, ParametriDogadjaja p)
        {
            BeginInvoke(new Obrada(UpdateLeftWiimoteChanged), p);
        }

        public void promenaStanjaRight(object sender, ParametriDogadjaja p)
        {
            BeginInvoke(new Obrada(UpdateRightWiimoteChanged), p);
        }

        public void UpdateLeftWiimoteChanged(ParametriDogadjaja args)
        {
            Stanje left = args.Stanje;
            gLeft.Clear(Color.Black);
            if (left.Senzori[0].Nadjen)
            {
                cbLeftFound1.Checked = true;
                txtLeftX1.Text = left.Senzori[0].X.ToString();
                txtLeftY1.Text = left.Senzori[0].Y.ToString();
                float x = left.Senzori[0].X * 320;
                float y = left.Senzori[0].Y * 320;
                gLeft.DrawEllipse(new Pen(Color.Red), x - size / 2, y - size / 2, size, size);
                gLeft.FillEllipse(new SolidBrush(Color.Red), x - size / 2, y - size / 2, size, size);
            }
            else
            {
                txtLeftX1.Text = "";
                txtLeftY1.Text = "";
                cbLeftFound1.Checked = false;
            }
            if (left.Senzori[1].Nadjen)
            {
                cbLeftFound2.Checked = true;
                txtLeftX2.Text = left.Senzori[1].X.ToString();
                txtLeftY2.Text = left.Senzori[1].Y.ToString();
                float x = left.Senzori[1].X * 320;
                float y = left.Senzori[1].Y * 320;
                gLeft.DrawEllipse(new Pen(Color.Lime), x - size / 2, y - size / 2, size, size);
                gLeft.FillEllipse(new SolidBrush(Color.Lime), x - size / 2, y - size / 2, size, size);
            }
            else
            {
                txtLeftX2.Text = "";
                txtLeftY2.Text = "";
                cbLeftFound2.Checked = false;
            }
            calculate();
        }

        public void UpdateRightWiimoteChanged(ParametriDogadjaja args)
        {
            Stanje right = args.Stanje;
            gRight.Clear(Color.Black);
            if (right.Dugmici.A)
            {
                wRight.postaviVibrator(false);
            }
            if (right.Senzori[0].Nadjen)
            {
                cbRightFound1.Checked = true;
                txtRightX1.Text = right.Senzori[0].X.ToString();
                txtRightY1.Text = right.Senzori[0].Y.ToString();
                float x = right.Senzori[0].X * 320;
                float y = right.Senzori[0].Y * 320;
                gRight.DrawEllipse(new Pen(Color.Red), x - size / 2, y - size / 2, size, size);
                gRight.FillEllipse(new SolidBrush(Color.Red), x - size / 2, y - size / 2, size, size);
            }
            else
            {
                txtRightX1.Text = "";
                txtRightY1.Text = "";
                cbRightFound1.Checked = false;
            }
            if (right.Senzori[1].Nadjen)
            {
                cbRightFound2.Checked = true;
                txtRightX2.Text = right.Senzori[1].X.ToString();
                txtRightY2.Text = right.Senzori[1].Y.ToString();
                float x = right.Senzori[1].X * 320;
                float y = right.Senzori[1].Y * 320;
                gRight.DrawEllipse(new Pen(Color.Lime), x - size / 2, y - size / 2, size, size);
                gRight.FillEllipse(new SolidBrush(Color.Lime), x - size / 2, y - size / 2, size, size);
            }
            else
            {
                txtRightX2.Text = "";
                txtRightY2.Text = "";
                cbRightFound2.Checked = false;
            }
            calculate();
        }

        private void calculate()
        {
            gCalc.Clear(Color.Black);
            if (wLeft.Stanje.Senzori[0].Nadjen && wRight.Stanje.Senzori[0].Nadjen)
            {
                enableCalibration(true);

                float lx = wLeft.Stanje.Senzori[0].X;
                float ly = wLeft.Stanje.Senzori[0].Y;

                float rx = wRight.Stanje.Senzori[0].X;
                float ry = wRight.Stanje.Senzori[0].Y;

                float cx = (lx + rx) / 2;
                float cy = (ly + ry) / 2;

                double alpha = (1 - lx) * Math.PI / 4 + (Math.PI / 8) * 3;
                double beta = rx * Math.PI / 4 + (Math.PI / 8) * 3;

                float cz = (float)(1 / ((1 / Math.Tan(alpha)) + (1 / Math.Tan(beta))));

                txtCalculatedX1.Text = cx.ToString();
                txtCalculatedY1.Text = cy.ToString();
                txtCalculatedZ1.Text = cz.ToString();

                //koristimo trenutno izracunate podatke za kalibraciju
                calibrate(cx, cy, cz);

                float x, y, z;

                //primena kalibracije
                if (cbCalibrated.Checked)
                {
                    x = (cx - minX) / (maxX - minX);
                    y = (cy - minY) / (maxY - minY);
                    z = (cz - minZ) / (maxZ - minZ);
                }
                else
                {
                    x = cx;
                    y = cy;
                    z = (cz - 1.5f) / 8.5f;
                }

                float zsize = 15f / z;

                gCalc.DrawEllipse(new Pen(Color.Purple), x * 320 - zsize / 2, y * 320 - zsize / 2, zsize, zsize);
                gCalc.FillEllipse(new SolidBrush(Color.Purple), x * 320 - zsize / 2, y * 320 - zsize / 2, zsize, zsize);
            }
            else
            {
                enableCalibration(false);
            }

            if (wLeft.Stanje.Senzori[1].Nadjen && wRight.Stanje.Senzori[1].Nadjen)
            {
                float lx = wLeft.Stanje.Senzori[1].X;
                float ly = wLeft.Stanje.Senzori[1].Y;

                float rx = wRight.Stanje.Senzori[1].X;
                float ry = wRight.Stanje.Senzori[1].Y;

                float cx = (lx + rx) / 2;
                float cy = (ly + ry) / 2;

                double alpha = (1 - lx) * Math.PI / 4 + (Math.PI / 8) * 3;
                double beta = rx * Math.PI / 4 + (Math.PI / 8) * 3;

                float cz = (float)(1 / ((1 / Math.Tan(alpha)) + (1 / Math.Tan(beta))));

                txtCalculatedX2.Text = cx.ToString();
                txtCalculatedY2.Text = cy.ToString();
                txtCalculatedZ2.Text = cz.ToString();

                float x, y, z;

                //primena kalibracije
                if (cbCalibrated.Checked)
                {
                    x = (cx - minX) / (maxX - minX);
                    y = (cy - minY) / (maxY - minY);
                    z = (cz - minZ) / (maxZ - minZ);
                }
                else
                {
                    x = cx;
                    y = cy;
                    z = (cz - 1.5f) / 8.5f;
                }

                float zsize = 15f / z;

                gCalc.DrawEllipse(new Pen(Color.Yellow), x * 320 - zsize / 2, y * 320 - zsize / 2, zsize, zsize);
                gCalc.FillEllipse(new SolidBrush(Color.Yellow), x * 320 - zsize / 2, y * 320 - zsize / 2, zsize, zsize);
            }
        }

        private void calibrate(float x, float y, float z)
        {
            if (calibrateMinX)
                minX = x;
            if (calibrateMaxX)
                maxX = x;
            if (calibrateMinY)
                minY = y;
            if (calibrateMaxY)
                maxY = y;
            if (calibrateMinZ)
                minZ = z;
            if (calibrateMaxZ)
                maxZ = z;

            calibrateMinX = false;
            calibrateMaxX = false;
            calibrateMinY = false;
            calibrateMaxY = false;
            calibrateMinZ = false;
            calibrateMaxZ = false;
        }

        private void enableCalibration(bool enabled)
        {
            btnMinX.Enabled = enabled;
            btnMinY.Enabled = enabled;
            btnMinZ.Enabled = enabled;
            btnMaxX.Enabled = enabled;
            btnMaxY.Enabled = enabled;
            btnMaxZ.Enabled = enabled;
        }

        private void Kalibracija_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (connected)
                {
                    wLeft.prekiniKomunikaciju();
                    wRight.prekiniKomunikaciju();
                }
            }
            catch (Exception ex)
            {
                Application.Exit();
            }
        }

        private void calibrateButtion_Click(object sender, EventArgs e)
        {
            string btn = ((Button)sender).Name;

            switch (btn)
            {
                case "btnMinX": calibrateMinX = true; break;
                case "btnMaxX": calibrateMaxX = true; break;
                case "btnMinY": calibrateMinY = true; break;
                case "btnMaxY": calibrateMaxY = true; break;
                case "btnMinZ": calibrateMinZ = true; break;
                case "btnMaxZ": calibrateMaxZ = true; break;
            }
        }
    }
}
