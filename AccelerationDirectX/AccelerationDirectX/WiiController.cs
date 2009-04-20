using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

using WiimoteLib;

namespace AccelerationDirectX
{
    class WiiController
    {
        Wiimote kontroler = new Wiimote();
        bool connected = false;

        public bool Connected { get { return connected; } }

        long prevTime = 0;

        float movementX = 0;
        float movementY = 0;


        float accelerationX = 0;
        float accelerationY = 0;
        float accelerationZ = 0;

        public float AccelarationX { get { return accelerationX; } }
        public float AccelarationY { get { return accelerationY; } }
        public float AccelarationZ { get { return accelerationZ; } }

        bool buttonA = false;
        bool buttonPlus = false;
        bool buttonMinus = false;
        bool buttonUp = false;
        bool buttonDown = false;
        bool buttonLeft = false;
        bool buttonRight = false;
        
        bool buttonB = false;
        bool buttonHome = false;
        bool button1 = false;
        bool button2 = false;

        public bool ButtonA { get { return buttonA; } }
        public bool ButtonPlus { get { return buttonPlus; } }
        public bool ButtonMinus { get { return buttonMinus; } }
        public bool ButtonUp { get { return buttonUp; } }
        public bool ButtonDown { get { return buttonDown; } }
        public bool ButtonLeft { get { return buttonLeft; } }
        public bool ButtonRight { get { return buttonRight; } }

        public bool ButtonB { get { return buttonB; } }
        public bool ButtonHome { get { return buttonHome; } }
        public bool Button1 { get { return button1; } }
        public bool Button2 { get { return button2; } }

        int rumbleMSec = 0;

        float threshold = 0.05f;

        float posX = 0.5f;
        float posY = 0.5f;

        float factor = 1 / 5000f;


        private delegate void UpdateWiimoteStateDelegate(WiimoteChangedEventArgs args);
        private delegate void LogStopHandler();

        public WiiController()
        {

        }

        protected void ClearButtonsAndAcceleration()
        {
            accelerationX = 0;
            accelerationY = 0;
            accelerationZ = 0;
            buttonA = false;
            buttonPlus = false;
            buttonMinus = false;
            buttonUp = false;
            buttonDown = false;
            buttonLeft = false;
            buttonRight = false;

            buttonB = false;
            buttonHome = false;
            button1 = false;
            button2 = false;
        }

        public void ConnectAction()
        {
            //System.Diagnostics.Debug.WriteLine("Connect");
            try
            {
                if (connected)
                {
                    kontroler.SetLEDs(false, false, false, false);
                    kontroler.Disconnect();
                    kontroler.WiimoteChanged -= UpdateState;                    
                    ClearButtonsAndAcceleration();
                    connected = false;
                }
                else
                {
                    kontroler.Connect();
                    if (kontroler.WiimoteState.ExtensionType != ExtensionType.BalanceBoard)
                        kontroler.SetReportType(InputReport.IRExtensionAccel, IRSensitivity.Maximum, true);
                    kontroler.WiimoteChanged += UpdateState;
                    kontroler.SetLEDs(true, false, false, false);
                    connected = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void UpdateState(object sender, WiimoteChangedEventArgs args)
        {
            try
            {
                UpdateWiimoteStateDelegate uwsd = new UpdateWiimoteStateDelegate(this.UpdateWiimoteChanged);
                uwsd.BeginInvoke(args, null, null);
                //IAsyncResult result = uwsd.BeginInvoke(args, null, null);
                //BeginInvoke(new UpdateWiimoteStateDelegate(UpdateWiimoteChanged), args);
            }
            catch (Exception ex)
            {
                //Application.Exit();
                Console.WriteLine(ex.Message);
            }
        }

        public void setRumbling(int msec)
        {
            rumbleMSec = msec;
            Thread t1 = new Thread(new ThreadStart(Rumbling));
            t1.Start();
        }

        protected void Rumbling()
        {
            kontroler.SetRumble(true);
            Thread t = Thread.CurrentThread;
            Thread.Sleep(rumbleMSec);
            kontroler.SetRumble(false);
            rumbleMSec = 0;
        }

        public void UpdateWiimoteChanged(WiimoteChangedEventArgs args)
        {
            WiimoteState ws = args.WiimoteState;

            accelerationX = ws.AccelState.Values.X;
            accelerationY = ws.AccelState.Values.Y;
            accelerationZ = ws.AccelState.Values.Z;

            buttonA = ws.ButtonState.A;
            
            buttonPlus = ws.ButtonState.Plus;
            buttonMinus = ws.ButtonState.Minus;
            buttonUp = ws.ButtonState.Up;
            buttonDown = ws.ButtonState.Down;
            buttonLeft = ws.ButtonState.Left;
            buttonRight = ws.ButtonState.Right;


            buttonB = ws.ButtonState.B;
            buttonHome = ws.ButtonState.Home;
            button1 = ws.ButtonState.One;
            button2 = ws.ButtonState.Two;

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
        }

        private void Disconnect()
        {
            kontroler.Disconnect();
        }

    }
}
