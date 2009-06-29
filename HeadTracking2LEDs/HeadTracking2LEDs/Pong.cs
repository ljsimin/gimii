using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using DI = Microsoft.DirectX.DirectInput;

using DXAV = Microsoft.DirectX.AudioVideoPlayback;
using DSND = Microsoft.DirectX.DirectSound;

using System.Timers;
using System.Threading;

namespace HeadTracking2LEDs
{
    public partial class Pong : Form
    {
        private Device device = null;

        public const int ScreenWidth = 800;
        public const int ScreenHeight = 600;

        private bool fullscreen = false;

        private VertexBuffer vbTerrain = null;

        private Microsoft.DirectX.Direct3D.Font infoFont;
        
        private Mesh ball = null;
        private Mesh table = null;

        private Color gridColor = Color.DarkViolet;

        static private Vector3 ballPosition;
        static private Vector3 ballDirection;
        static private Vector3 tablePosition;

        private const float FieldLength = 40;
        private const float FieldWidth = 30;
        private const float FieldHeight = 20;
        private const float TableSize = 4;
        private const float Ballradius = 1;
        
        private float angleTor = 0.0f;
        private float angleTorStep = 0.002f;

        private float pyrUpDown = 0.0f;
        private float pyrUpDownStep = 0.005f;

        private float angleGlob = 0.0f;
        private float angleGlobStep = 0.02f;

        private float moveStep = 0.25f;

        private Camera myCamera = null;

        private Microsoft.DirectX.DirectInput.Device keyb;

        protected System.Timers.Timer myTimer;

        private int TIMER_INTERVAL = 40;

        private MatrixStack mStack = new MatrixStack();

        private WiiController wii;

        private bool showInfo = true;

        public Pong()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        }

        public void InitializeTimer()
        {
            myTimer = new System.Timers.Timer();

            myTimer.Elapsed += new ElapsedEventHandler(TimerEventProcessor);
            myTimer.Interval = TIMER_INTERVAL;
            myTimer.Start();
        }

        public void InitializeWii()
        {
            wii = new WiiController();
        }

        public void InitializeKeyboard()
        {
            keyb = new DI.Device(DI.SystemGuid.Keyboard);
            keyb.SetCooperativeLevel(this, DI.CooperativeLevelFlags.Background | DI.CooperativeLevelFlags.NonExclusive);
            keyb.Acquire();
        }



        public void InitializeGraphicsAndCamera()
        {
            // Set presentation parameters
            PresentParameters presentParams = new PresentParameters();

            presentParams.Windowed = true;
            presentParams.MultiSample = MultiSampleType.FourSamples;
            presentParams.SwapEffect = SwapEffect.Discard;


            // Start up full screen
            Format current = Manager.Adapters[0].CurrentDisplayMode.Format;

            if (Manager.CheckDeviceType(0, DeviceType.Hardware, current, current, false))
            {
                // Perfect, this is valid
                presentParams.Windowed = true;
                presentParams.BackBufferFormat = current;
                presentParams.BackBufferCount = 1;
                presentParams.BackBufferWidth = ScreenWidth;
                presentParams.BackBufferHeight = ScreenHeight;
                Cursor.Hide();
                fullscreen = false;
            }
            else
            {
                presentParams.Windowed = true;
                fullscreen = false;
            }


            // Depth Buffer
            presentParams.EnableAutoDepthStencil = true;
            presentParams.AutoDepthStencilFormat = DepthFormat.D16;

            // Create device
            device = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);

           
            ball = Mesh.Sphere(device, Ballradius, 16, 16);
            table = Mesh.Box(device, TableSize, TableSize, 0.1f);
            vbTerrain = new VertexBuffer(typeof(CustomVertex.PositionNormalColored), 800, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalColored.Format, Pool.Default);
            vbTerrain.Created += new EventHandler(this.OnVertexBufferCreate);
            OnVertexBufferCreate(vbTerrain, null);

            infoFont = new Microsoft.DirectX.Direct3D.Font(device, new System.Drawing.Font("Arial", 12.0f, FontStyle.Regular));

            mStack = new MatrixStack();

            myCamera = new Camera(new Vector3(15.0f, 10.0f, -27.0f), new Vector3(15.0f, 10.0f, 50.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f));

        }

        private void OnVertexBufferCreate(object sender, EventArgs e)
        {
            VertexBuffer buffer = (VertexBuffer)sender;

            if (buffer == vbTerrain)
            {
                CustomVertex.PositionNormalColored[] vertsTerrain = new CustomVertex.PositionNormalColored[800];
               for (int i = 0; i < 31; i++)
                {
                    int index = i;
                    vertsTerrain[index * 4 ] = new CustomVertex.PositionNormalColored(new Vector3((float)i, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, -0.4f), gridColor.ToArgb());
                    vertsTerrain[index * 4 + 1] = new CustomVertex.PositionNormalColored(new Vector3((float)i, 0.0f, 40.0f), new Vector3(0.0f, 1.0f, -0.4f), gridColor.ToArgb());
                    vertsTerrain[index * 4 +2] = new CustomVertex.PositionNormalColored(new Vector3((float)i , 20.0f, 0.0f), new Vector3(0.0f, 1.0f, -0.4f), gridColor.ToArgb());
                    vertsTerrain[index * 4 +3] = new CustomVertex.PositionNormalColored(new Vector3((float)i, 20.0f, 40.0f), new Vector3(0.0f, 1.0f, -0.4f), gridColor.ToArgb());
                
                }
                for (int i = 31; i < 71; i++)
                {
                  int index = i;
                  vertsTerrain[index * 4] = new CustomVertex.PositionNormalColored(new Vector3((0.0f), 0.0f, (float)i-31), new Vector3(0.0f, 1.0f, -0.4f), gridColor.ToArgb());
                  vertsTerrain[index * 4 + 1] = new CustomVertex.PositionNormalColored(new Vector3(30.0f, 0.0f, (float)i-31), new Vector3(0.0f, 1.0f, -0.4f), gridColor.ToArgb());
                  vertsTerrain[index * 4 + 2] = new CustomVertex.PositionNormalColored(new Vector3((0.0f), 20.0f, (float)i - 31), new Vector3(0.0f, 1.0f, -0.4f), gridColor.ToArgb());
                  vertsTerrain[index * 4 + 3] = new CustomVertex.PositionNormalColored(new Vector3(30.0f, 20.0f, (float)i - 31), new Vector3(0.0f, 1.0f, -0.4f), gridColor.ToArgb());
               
                }
                for (int i = 71; i < 91; i++)
                {
                    int index = i;
                    vertsTerrain[index * 4] = new CustomVertex.PositionNormalColored(new Vector3(0.0f, (float)i-70, 0.0f), new Vector3(0.0f, 1.0f, -0.4f), gridColor.ToArgb());
                    vertsTerrain[index * 4 + 1] = new CustomVertex.PositionNormalColored(new Vector3(0.0f, (float)i-70, 40.0f), new Vector3(0.0f, 1.0f, -0.4f), gridColor.ToArgb());
                    vertsTerrain[index * 4 + 2] = new CustomVertex.PositionNormalColored(new Vector3(30.0f, (float)i - 70, 0.0f), new Vector3(0.0f, 1.0f, -0.4f), gridColor.ToArgb());
                    vertsTerrain[index * 4 + 3] = new CustomVertex.PositionNormalColored(new Vector3(30.0f, (float)i - 70, 40.0f), new Vector3(0.0f, 1.0f, -0.4f), gridColor.ToArgb());

                }
                for (int i = 91; i < 112; i++)
                {
                    int index = i;
                    vertsTerrain[index * 4] = new CustomVertex.PositionNormalColored(new Vector3(0, (float)i-91,40.0f), new Vector3(0.0f, 1.0f, -0.4f), gridColor.ToArgb());
                    vertsTerrain[index * 4 + 1] = new CustomVertex.PositionNormalColored(new Vector3(30.0f,(float)i-91,40.0f), new Vector3(0.0f, 1.0f, -0.4f), gridColor.ToArgb());

                }
                for (int i = 112; i < 143; i++)
                {
                    int index = i;
                    vertsTerrain[index * 4] = new CustomVertex.PositionNormalColored(new Vector3((float)i-112, 0.0f, 40.0f), new Vector3(0.0f, 1.0f, -0.4f), gridColor.ToArgb());
                    vertsTerrain[index * 4 + 1] = new CustomVertex.PositionNormalColored(new Vector3((float)i-112, 20.0f, 40.0f), new Vector3(0.0f, 1.0f, -0.4f), gridColor.ToArgb());
                 
                }
                for (int i = 143; i < 184; i++)
                {
                    int index = i;
                    vertsTerrain[index * 4] = new CustomVertex.PositionNormalColored(new Vector3(30.0f,0.0f, (float)i - 143), new Vector3(0.0f, 1.0f, -0.4f), gridColor.ToArgb());
                    vertsTerrain[index * 4 + 1] = new CustomVertex.PositionNormalColored(new Vector3(30.0f, 20.0f, (float)i - 143), new Vector3(0.0f, 1.0f, -0.4f), gridColor.ToArgb());
                    vertsTerrain[index * 4 + 2] = new CustomVertex.PositionNormalColored(new Vector3(0.0f, 0.0f, (float)i - 143), new Vector3(0.0f, 1.0f, -0.4f), gridColor.ToArgb());
                    vertsTerrain[index * 4 + 3] = new CustomVertex.PositionNormalColored(new Vector3(0.0f, 20.0f, (float)i - 143), new Vector3(0.0f, 1.0f, -0.4f), gridColor.ToArgb());

                }
                buffer.SetData(vertsTerrain, 0, LockFlags.None);
            }

        }

        private void SetupCamera()
        {
            device.Transform.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 3, this.Width / this.Height, 1.0f, 10000.0f);
            //device.Transform.View = Matrix.LookAtLH(new Vector3(0, 0, -15.0f), new Vector3(0,0,0), new Vector3(0, 1, 0));
            device.Transform.View = myCamera.LookLeftHanded();

            // Setting back face culling
            device.RenderState.CullMode = Cull.None;

            // Setting lights

            device.RenderState.Lighting = true;

            //device.RenderState.Ambient = Color.Bisque;
            //device.Lights[0].Ambient = Color.FromArgb(20,20,20);
            device.Lights[0].Type = LightType.Directional;
            //device.Lights[0].Diffuse = gridColor;
            device.Lights[0].Diffuse = Color.White;
            device.Lights[0].Direction = new Vector3(0.0f, -0.5f, 1.0f);
            device.Lights[0].Enabled = true;

            Color lightDiffColor = Color.White;
            if (wii != null)
                if (wii.Connected)
                    lightDiffColor = Color.Red;

            device.Lights[1].Type = LightType.Directional;
            device.Lights[1].Diffuse = lightDiffColor;
            //device.Lights[1].Direction = new Vector3(0.0f, 1.0f, 0.0f);
            device.Lights[1].Direction = new Vector3(0.0f, 1.0f, 0.0f);
            device.Lights[1].Enabled = true;
        }
        private void MoveBall(){
        // process ball
            ballPosition += ballDirection;

            // check end
            if (ballPosition.Z < -15)
            {
               // score2++;
                ReStart();
                return;
            }
            // check panel hit
            if (ballPosition.Z < 1.3 && ballPosition.Z > 0.3)
            {
                if ((ballPosition.X +0.5f > tablePosition.X - TableSize / 2 && ballPosition.X - 0.5f < tablePosition.X + TableSize / 2) &&
                       (ballPosition.Y + 0.5f > tablePosition.Y - TableSize / 2 && ballPosition.Y - 0.5f < tablePosition.Y + TableSize / 2)) 
                
                {

                    ballDirection = Reflect(ballDirection, new Vector3(0, 0, 1));
                }

            }
            // check wall hit
            if (ballPosition.X < 0.6f) ballDirection = Reflect(ballDirection, new Vector3(1, 0, 0));
            if (ballPosition.X > FieldWidth - 0.6f) ballDirection = Reflect(ballDirection, new Vector3(-1, 0, 0));
            if (ballPosition.Y < 0.6f) ballDirection = Reflect(ballDirection, new Vector3(0, 1, 0));
            if (ballPosition.Y > FieldHeight-0.6f) ballDirection = Reflect(ballDirection, new Vector3(0, -1, 0));
            if (ballPosition.Z > FieldLength-0.6f) ballDirection = Reflect(ballDirection, new Vector3(0, 0, -1));
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Color backColor = Color.Black;

            if (wii != null)
                if (wii.Connected)
                    backColor = Color.DimGray;

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, backColor, 1.0f, 0);

            SetupCamera();

            device.BeginScene();

                
            mStack.Push();
            {
                mStack.MultiplyMatrixLocal(
                        Matrix.Identity
                        );
                device.Transform.World = mStack.Top;

                device.VertexFormat = CustomVertex.PositionNormalColored.Format;
                device.SetStreamSource(0, vbTerrain, 0);
                device.DrawPrimitives(PrimitiveType.LineList, 0, 400);
            }
            mStack.Pop();
            //lopte
            mStack.Push();
                {
                    
                    Material myMaterial = new Material();
                    myMaterial.Ambient = Color.Cyan;
                    myMaterial.Diffuse = Color.Cyan;
                    myMaterial.Specular = Color.Cyan;
                    device.Material = myMaterial;

                                              
                    device.Transform.World = Matrix.Translation(ballPosition);

                    ball.DrawSubset(0);
                                       
                    myMaterial.Diffuse = Color.Transparent;
                    myMaterial.Ambient = Color.Transparent;
                    device.Material = myMaterial;
                    device.Transform.World = Matrix.Translation(tablePosition);
                    table.DrawSubset(0);
                    device.RenderState.FillMode = FillMode.Solid;
            }
                mStack.Pop();
            if (showInfo)
            {
                infoFont.DrawText(null, "Press Insert to Connect/Disconnect, Escape to Exit", new Rectangle(25, 25, 0, 0),
               DrawTextFormat.NoClip, Color.PaleGreen);
                infoFont.DrawText(null, "Press I to turn info On/Off", new Rectangle(25, 45, 0, 0),
               DrawTextFormat.NoClip, Color.PaleGreen);

                string ctrl = "Wii controller not connected";

                if (wii != null)
                    if (wii.Connected)
                        ctrl = "Press R to calibrate"
                            + "\nX1: " + wii.X1 + "  Y1: " + wii.Y1 + "  X2: " + wii.X2 + "  Y2: " + wii.Y2 +
                            "  \nHeadX: " + wii.HeadX + "  HeadY: " + wii.HeadY + "  HeadDist: " + wii.HeadDist;

                infoFont.DrawText(null, ctrl, new Rectangle(25, 65, 0, 0),
               DrawTextFormat.NoClip, Color.PaleGreen);


            }

            device.EndScene();
            device.Present();
            this.Invalidate();
        }


        protected override void OnKeyUp(KeyEventArgs e)
        {
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert)
            {
                wii.ConnectAction();
                if (wii.Connected)
                    wii.setRumbling(100);
            }
            if (e.KeyCode == Keys.R)
            {
                wii.CalibrateAngle();
            }
            if (e.KeyCode == Keys.I)
            {
                showInfo = !showInfo;
            }
            if (e.KeyCode == Keys.Escape)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(CloseTheForm));
                }
                else
                {
                    CloseTheForm();
                }
            }
        }

        protected void readKeyboard()
        {
            DI.KeyboardState keys = keyb.GetCurrentKeyboardState();
            if (keys[DI.Key.F1])
            {
                myCamera.SetPosition(new Vector3(0.0f, 20.0f, -15.0f));
            }
            if (keys[DI.Key.F2])
            {
                myCamera.SetPosition(new Vector3(-2.0f, 2.0f, -15.0f));
            }
            if (keys[DI.Key.F3])
            {
                myCamera.SetPosition(new Vector3(30.0f, -20.0f, -15.0f));
            }


            if (keys[DI.Key.A])
            {
                if (tablePosition.X > 0 + TableSize / 2) tablePosition.X -= 1;
            }
            if (keys[DI.Key.D])
            {
                if (tablePosition.X < FieldWidth - TableSize / 2) tablePosition.X += 1;
            }
            if (keys[DI.Key.S])
            {
                if (tablePosition.Y > 0 + TableSize / 2) tablePosition.Y -= 1;
            }
            if (keys[DI.Key.W])
            {
                if (tablePosition.Y < FieldHeight - TableSize / 2) tablePosition.Y += 1;
            }


            if (keys[DI.Key.C])
            {
                myCamera.MoveUpDown(-moveStep);
            }
            if (keys[DI.Key.Space])
            {
                myCamera.MoveUpDown(moveStep);
            }
            if (keys[DI.Key.Up])
            {
                myCamera.Pitch(0.02f);
            }
            if (keys[DI.Key.Down])
            {
                myCamera.Pitch(-0.02f);
            }
            if (keys[DI.Key.Left])
            {
                if (keys[DI.Key.LeftAlt])
                    myCamera.Yaw(0.02f);
                else
                    myCamera.MoveViewRotateLeftRight(0.02f);
            }
            if (keys[DI.Key.Right])
            {
                if (keys[DI.Key.LeftAlt])
                    myCamera.Yaw(-0.02f);
                else
                    myCamera.MoveViewRotateLeftRight(-0.02f);
            }
            MoveBall();
        }

        protected void readWii()
        {

            tablePosition = new Vector3(-5 * wii.HeadX, 5 * wii.HeadY, 0);

        }


        protected void setAnimationParameters()
        {
            if (pyrUpDown > 1 || pyrUpDown < -1)
                pyrUpDownStep *= -1;

            pyrUpDown += pyrUpDownStep;

            if (angleTor > 0.3 || angleTor < -0.3)
                angleTorStep *= -1;

            angleTor += angleTorStep;

            if (angleGlob >= 2 * Math.PI)
                angleGlob = 0.0f;

            angleGlob += angleGlobStep;

        }

        public void CloseTheForm()
        {
            if (wii.Connected)
                wii.ConnectAction();
            Cursor.Show();
            Close();
        }

        protected void TimerEventProcessor(object source, ElapsedEventArgs e)
        {
            readKeyboard();

            if (wii.Connected)
                readWii();

            setAnimationParameters();
            //setAudio();
        }
        static public void ReStart()
        {
            ballPosition = new Vector3(FieldWidth / 2, FieldHeight / 2, FieldLength / 2);
            ballDirection = new Vector3(0.2f, 0.1f, 0.7f);
            tablePosition = new Vector3(FieldWidth / 2, FieldHeight / 2, 0);
        }
        private Vector3 Reflect(Vector3 input, Vector3 normal)
        {
            // play sound
            // hitSound.Play(0, DSound.BufferPlayFlags.Default);

            return input - 2 * Vector3.Dot(input, normal) * normal;
        }
        static void Main()
        {
            {
                Pong f1 = new Pong();
                ReStart();
                f1.InitializeGraphicsAndCamera();
                //f1.InitializeAudio();
                f1.InitializeKeyboard();
                f1.InitializeWii();
                f1.InitializeTimer();
                f1.Show();
                Application.Run(f1);
            }
        }
    }
}
