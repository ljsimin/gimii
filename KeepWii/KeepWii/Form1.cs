using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WiimoteLib;

namespace KeepWii
{
    public partial class Form1 : Form
    {
        Wiimote kontroler1 = null;
        Wiimote kontroler2 = null;
        private delegate void UpdateFirstWiimoteStateDelegate(WiimoteChangedEventArgs args);
        private delegate void UpdateSecondWiimoteStateDelegate(WiimoteChangedEventArgs args);

        public void Connect() 
        {
            WiimoteCollection wmc = new WiimoteCollection();

            try
            {
                wmc.FindAllWiimotes();
            }
            catch (WiimoteNotFoundException wnfe)
            {
                MessageBox.Show("Kontroler nije nadjen", "Kontroler nije nadjen", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (wmc.Count > 0)
            {
                kontroler1 = wmc[0];
                kontroler1.Connect();
                if (kontroler1.WiimoteState.ExtensionType != ExtensionType.BalanceBoard)
                {
                    kontroler1.SetReportType(InputReport.IRExtensionAccel, IRSensitivity.Maximum, true);
                }
                kontroler1.WiimoteChanged += UpdateStateFirst;
                kontroler1.SetLEDs(true, false, false, false);
                label1.Text = "Connected";
            }
            
            if (wmc.Count > 1)
            {
                kontroler2 = wmc[1];
                kontroler2.Connect();
                if (kontroler2.WiimoteState.ExtensionType != ExtensionType.BalanceBoard)
                {
                    kontroler2.SetReportType(InputReport.IRExtensionAccel, IRSensitivity.Maximum, true);
                }
                kontroler2.WiimoteChanged += UpdateStateSecond;
                kontroler2.SetLEDs(false, true, false, false);
                label2.Text = "Connected";
            }
        }


        public void UpdateStateFirst(object sender, WiimoteChangedEventArgs args)
        {
            try
            {
                BeginInvoke(new UpdateFirstWiimoteStateDelegate(UpdateFirstWiimoteChanged), args);
            }
            catch (Exception ex)
            {
                Application.Exit();
            }
        }

        public void UpdateFirstWiimoteChanged(WiimoteChangedEventArgs args)
        {
            panel1.BackColor = Color.Black;

            WiimoteState ws = args.WiimoteState;

            if (ws.ButtonState.A || ws.ButtonState.B || ws.ButtonState.Down || ws.ButtonState.Home || ws.ButtonState.Left || ws.ButtonState.Minus || ws.ButtonState.One || ws.ButtonState.Plus || ws.ButtonState.Right || ws.ButtonState.Two || ws.ButtonState.Up)
                panel1.BackColor = Color.Green;

            if ((Math.Abs(ws.AccelState.Values.X) > 2) || (Math.Abs(ws.AccelState.Values.Y) > 2) || (Math.Abs(ws.AccelState.Values.Z) > 2))
                panel1.BackColor = Color.LightGreen;
        }

        public void UpdateStateSecond(object sender, WiimoteChangedEventArgs args)
        {
            try
            {
                BeginInvoke(new UpdateSecondWiimoteStateDelegate(UpdateSecondWiimoteChanged), args);
            }
            catch (Exception ex)
            {
                Application.Exit();
            }
        }

        public void UpdateSecondWiimoteChanged(WiimoteChangedEventArgs args)
        {
            panel1.BackColor = Color.Black;

            WiimoteState ws = args.WiimoteState;

            if (ws.ButtonState.A || ws.ButtonState.B || ws.ButtonState.Down || ws.ButtonState.Home || ws.ButtonState.Left || ws.ButtonState.Minus || ws.ButtonState.One || ws.ButtonState.Plus || ws.ButtonState.Right || ws.ButtonState.Two || ws.ButtonState.Up)
                panel1.BackColor = Color.Yellow;

            if ((Math.Abs(ws.AccelState.Values.X) > 2) || (Math.Abs(ws.AccelState.Values.Y) > 2) || (Math.Abs(ws.AccelState.Values.Z) > 2))
                panel1.BackColor = Color.LightYellow;
        }

        public Form1()
        {
            InitializeComponent();
            Connect();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
                Hide();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnReconnect_Click(object sender, EventArgs e)
        {
            Connect();
        }
    }
}
