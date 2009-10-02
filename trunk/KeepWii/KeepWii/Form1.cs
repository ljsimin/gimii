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
        Wiimote kontroler = new Wiimote();
        private delegate void UpdateWiimoteStateDelegate(WiimoteChangedEventArgs args);

        public void Connect() 
        {
            Boolean b = false;
            try
            {
                kontroler.Connect();
                if (kontroler.WiimoteState.ExtensionType != ExtensionType.BalanceBoard)
                {
                    kontroler.SetReportType(InputReport.IRExtensionAccel, IRSensitivity.Maximum, true);
                }
                kontroler.WiimoteChanged += UpdateState;
                label1.Text = "Connected";
            }
            catch (WiimoteNotFoundException wnfe)
            {
                DialogResult r = MessageBox.Show("Kontroler nije nadjen", "Kontroler nije nadjen", MessageBoxButtons.OK, MessageBoxIcon.Error);
                b = true;
            }
            finally
            {
                if (b)
                {
                    Application.Exit();
                }
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
            panel1.BackColor = Color.Black;

            WiimoteState ws = args.WiimoteState;

            if (ws.ButtonState.A || ws.ButtonState.B || ws.ButtonState.Down || ws.ButtonState.Home || ws.ButtonState.Left || ws.ButtonState.Minus || ws.ButtonState.One || ws.ButtonState.Plus || ws.ButtonState.Right || ws.ButtonState.Two || ws.ButtonState.Up)
                panel1.BackColor = Color.Green;

            if ((Math.Abs(ws.AccelState.Values.X) > 2) || (Math.Abs(ws.AccelState.Values.Y) > 2) || (Math.Abs(ws.AccelState.Values.Z) > 2))
                panel1.BackColor = Color.LightGreen;
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
    }
}
