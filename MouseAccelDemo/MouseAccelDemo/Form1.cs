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
        long prevTime = 0;
        float movementX = 0;
        float movementY = 0;


        float accelerationX = 0;
        float accelerationY = 0;
        float accelerationZ = 0;

        float threshold = 0.05f;

        float posX = 0.5f;
        float posY = 0.5f;

        float factor = 1 / 5000f;

        Color foregroundColor = Color.Yellow;
        Color backgroundColor = Color.Black;

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
            accelerationY = ws.AccelState.Values.Y;
            accelerationZ = ws.AccelState.Values.Z;

            Color brushColor = foregroundColor;
            Boolean toClear = true;

            if (ws.ButtonState.A)
            {
                buttonState = "Click!";
                toClear = false;
                brushColor = foregroundColor;
            }
            else if (ws.ButtonState.B)
            {
                buttonState = "Double click!";
                toClear = true;
                brushColor = foregroundColor;
            }
            else
            {
                buttonState = "";
                toClear = true;
                brushColor = foregroundColor;
            }

            long nowTime = System.DateTime.Now.Millisecond;
            long deltaTime = nowTime >= prevTime ? nowTime - prevTime : nowTime + 1000 - prevTime;

            if (deltaTime >= 1)
            {
                prevTime = nowTime;
            }
            else
            {
                return;
            }



            //long dTime = System.DateTime.Now.Ticks/100000000000 - time;
            //time = System.DateTime.Now.Ticks / 100000000000;
            //dTime *= dTime;
            //dTime /= 200000;

            if (ws.ButtonState.Home)
            {
                posX = 0.5f;
                posY = 0.5f;

            }

            if ((Math.Abs(accelerationX) > threshold) || (Math.Abs(accelerationY) > threshold))
            {
                /*
                //Ver1
                movementX = accelerationX * 0.003f;
                movementY = accelerationY * 0.003f;
                */


                //Ver2
                movementX = factor * accelerationX * deltaTime * deltaTime / 2;
                movementY = factor * accelerationY * deltaTime * deltaTime / 2;

                if ((posX + movementX) >= 0 && (posX + movementX) <= 1)
                    posX += movementX;

                if ((posY + movementY) >= 0 && (posY + movementY) <= 1)
                    posY += movementY;
            }            

            if(toClear)
                g.Clear(backgroundColor);

            //g.DrawString("X: " + posX.ToString() + " Y: " + posY.ToString() + " Mili : " + nowTime, new Font("Arial", 12f), new SolidBrush(Color.White), 0, 0);
            //g.DrawString("deltaTime: " + deltaTime.ToString(), new Font("Arial", 12f), new SolidBrush(Color.White), 0, 25);
            g.DrawString("Ax: " + accelerationX.ToString() + " Ay: " + accelerationY.ToString(), new Font("Arial", 12f), new SolidBrush(Color.White), 0, 50);
            //g.DrawString("MovementX: " + movementX.ToString() + " MovementY: " + movementY.ToString(), new Font("Arial", 12f), new SolidBrush(Color.White), 0, 75);

            g.DrawString(buttonState, new Font("Arial", 16f), new SolidBrush(Color.Yellow), 20, 100);
            g.FillEllipse(new SolidBrush(brushColor), posX * pnlCanvas.Size.Width - 3,
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

        private void pnlCanvas_Resize(object sender, EventArgs e)
        {
            g = pnlCanvas.CreateGraphics();
        }
    }
}
