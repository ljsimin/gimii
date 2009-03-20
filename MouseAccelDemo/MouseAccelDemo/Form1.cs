using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WiimoteLib;


namespace MouseAccelDemo
{
    public partial class Form1 : Form
    {
        Wiimote kontroler = new Wiimote();
        bool connected = false;
        Graphics g = null;
        //System.Threading.Thread t;
        //long time = 0;
        float movementX = 0;
        float movementY = 0;


        float accelerationX = 0;
        float accelerationY = 0;
        
        float threshold = 0.05f;

        float posX = 0;
        float posY = 0;

        String buttonState = "";

        
        private delegate void UpdateWiimoteStateDelegate(WiimoteChangedEventArgs args);
        private delegate void LogStopHandler();

        public Form1()
        {
            InitializeComponent();
            g = pnlCanvas.CreateGraphics();
            //System.Diagnostics.Debug.WriteLine("RAAAAAADDI!");
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Connect");

            try
            {
                if (connected)
                {
                    kontroler.Disconnect();
                    kontroler.WiimoteChanged -= UpdateState;
                    btnConnect.Text = "Connect";
                    connected = false;
                    lblMessage.Text = "Wiimote disconnected";
                }
                else
                {
                    kontroler.Connect();
                    if (kontroler.WiimoteState.ExtensionType != ExtensionType.BalanceBoard)
                        kontroler.SetReportType(InputReport.IRExtensionAccel, IRSensitivity.Maximum, true);
                    kontroler.WiimoteChanged += UpdateState;
                    connected = true;
                    btnConnect.Text = "Disconnect";
                    lblMessage.Text = "Wiimote connected";
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

            accelerationX = ws.AccelState.Values.X;
            accelerationY = ws.AccelState.Values.Z;

            if (ws.ButtonState.A)
                buttonState = "Click!";
            else if (ws.ButtonState.B)
                buttonState = "Double click!";
            else
                buttonState = "";

         

            if ((Math.Abs(accelerationX) > threshold) || (Math.Abs(accelerationY) > threshold))
            {
                movementX = accelerationX * 0.002f;
                movementY = accelerationY * 0.002f;

                posX += movementX;
                posY -= movementY;
            }

            g.Clear(Color.Black);                    
            g.DrawString("X: " + posX.ToString() + "Y: " + posY.ToString(), new Font("Arial", 12f), new SolidBrush(Color.White), 0, 0);
            g.DrawString(buttonState, new Font("Arial", 16f), new SolidBrush(Color.Yellow), 0, 20);
            g.FillEllipse(new SolidBrush(Color.Red), posX * pnlCanvas.Size.Width - 3,
                                                     posY * pnlCanvas.Size.Height - 3,
                                                     6,
                                                     6);

            
        }

        

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            kontroler.Disconnect();
        }

        private void pnlCanvas_Paint(object sender, PaintEventArgs e)
        {
        
        }
    }
}
