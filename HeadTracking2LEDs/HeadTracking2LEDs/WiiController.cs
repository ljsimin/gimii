using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

using WiimoteLib;

namespace HeadTracking2LEDs
{
    class WiiController
    {

        Point firstPoint = new Point();
        Point secondPoint = new Point();
        int numvisible = 0;

        float dotDistanceInMM = 153.0f;//width of the wii sensor bar
        float screenHeightinMM = 114.5f;
        float radiansPerPixel = (float)(Math.PI / 4) / 1024.0f; //45 degree field of view with a 1024x768 camera
        float movementScaling = 1.0f;

        int m_dwWidth = 1024;
        int m_dwHeight = 768;
        float screenAspect = 0;
        float cameraVerticaleAngle = 0; //begins assuming the camera is point straight forward
        float relativeVerticalAngle = 0; //current head position view angle
        bool cameraIsAboveScreen = false;//has no affect until zeroing and then is set automatically.

        //headposition
        float headX = 0;
        float headY = 3;
        float headDist = 3;

        public float HeadX { get { return headX; } }
        public float HeadY { get { return headY; } }
        public float HeadDist { get { return headDist; } }
        public float X1 { get { return firstPoint.X; } }
        public float X2 { get { return secondPoint.X; } }
        public float Y1 { get { return firstPoint.Y; } }
        public float Y2 { get { return secondPoint.Y; } }

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
                    kontroler.SetLEDs(true, false, false, true);
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



            numvisible = 0;
            if (ws.IRState.IRSensors[0].Found)
            {
                firstPoint.X = ws.IRState.IRSensors[0].RawPosition.X;
                firstPoint.Y = ws.IRState.IRSensors[0].RawPosition.Y;
                numvisible = 1;
            }
            if (ws.IRState.IRSensors[1].Found)
            {
                if (numvisible == 0)
                {
                    firstPoint.X = ws.IRState.IRSensors[1].RawPosition.X;
                    firstPoint.Y = ws.IRState.IRSensors[1].RawPosition.Y;
                    numvisible = 1;
                }
                else
                {
                    secondPoint.X = ws.IRState.IRSensors[1].RawPosition.X;
                    secondPoint.Y = ws.IRState.IRSensors[1].RawPosition.Y;
                    numvisible = 2;
                }
            }
            if (ws.IRState.IRSensors[2].Found)
            {
                if (numvisible == 0)
                {
                    firstPoint.X = ws.IRState.IRSensors[2].RawPosition.X;
                    firstPoint.Y = ws.IRState.IRSensors[2].RawPosition.Y;
                    numvisible = 1;
                }
                else if (numvisible == 1)
                {
                    secondPoint.X = ws.IRState.IRSensors[2].RawPosition.X;
                    secondPoint.Y = ws.IRState.IRSensors[2].RawPosition.Y;
                    numvisible = 2;
                }
            }
            if (ws.IRState.IRSensors[3].Found)
            {
                if (numvisible == 1)
                {
                    secondPoint.X = ws.IRState.IRSensors[3].RawPosition.X;
                    secondPoint.Y = ws.IRState.IRSensors[3].RawPosition.Y;
                    numvisible = 2;
                }
            }

            if (numvisible == 2)
            {


                float dx = firstPoint.X - secondPoint.X;
                float dy = firstPoint.Y - secondPoint.Y;
                float pointDist = (float)Math.Sqrt(dx * dx + dy * dy);

                float angle = radiansPerPixel * pointDist / 2;
                //in units of screen hieght since the box is a unit cube and box hieght is 1
                headDist = movementScaling * (float)((dotDistanceInMM / 2) / Math.Tan(angle)) / screenHeightinMM;


                float avgX = (firstPoint.X + secondPoint.X) / 2.0f;
                float avgY = (firstPoint.Y + secondPoint.Y) / 2.0f;


                //should  calaculate based on distance

                headX = (float)(movementScaling * Math.Sin(radiansPerPixel * (avgX - 512)) * headDist);

                relativeVerticalAngle = (avgY - 384) * radiansPerPixel;//relative angle to camera axis

                if (cameraIsAboveScreen)
                    headY = .5f + (float)(movementScaling * Math.Sin(relativeVerticalAngle + cameraVerticaleAngle) * headDist);
                else
                    headY = -.5f + (float)(movementScaling * Math.Sin(relativeVerticalAngle + cameraVerticaleAngle) * headDist);
            }


        }
        public void CalibrateAngle()
        {
            //zeros the head position and computes the camera tilt
            double angle = Math.Acos(.5 / headDist) - Math.PI / 2;//angle of head to screen
            if (!cameraIsAboveScreen)
                angle = -angle;
            cameraVerticaleAngle = (float)((angle - relativeVerticalAngle));//absolute camera angle 
        }

        private void Disconnect()
        {
            kontroler.Disconnect();
        }

    }
}
