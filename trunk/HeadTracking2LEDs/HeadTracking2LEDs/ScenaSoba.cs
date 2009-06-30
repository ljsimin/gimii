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
    public partial class ScenaSoba : Form
    {
        private Device device = null;

        public const int ScreenWidth = 1280;
        public const int ScreenHeight = 800;

        private bool fullscreen = false;

        private VertexBuffer vbTerrain = null;
        private IndexBuffer ib = null;
        private IndexBuffer ib2 = null;

        private Microsoft.DirectX.Direct3D.Font infoFont;


        private Mesh deoZida1Mesh = null;
        private Mesh ball = null;
        private Mesh podMesh = null;
        private Mesh zidBezProzoraMesh = null;
        private Mesh deoZida2Mesh = null;

        private Texture texI = null;

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

        private int TIMER_INTERVAL = 20;

        private MatrixStack mStack = new MatrixStack();

        private WiiController wii;

        private bool showInfo = true;

        public ScenaSoba()
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

            deoZida1Mesh = Mesh.Box(device, 12, 15, 0.1f);
            podMesh = Mesh.Box(device, 30, 0.1f, 30);
            zidBezProzoraMesh = Mesh.Box(device, 0.1f, 15, 30);
            deoZida2Mesh = Mesh.Box(device, 6, 5, 0.1f);
            ball = Mesh.Sphere(device, 1, 36, 36);

            vbTerrain = new VertexBuffer(typeof(CustomVertex.PositionNormalColored), 1600, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalColored.Format, Pool.Default);
            vbTerrain.Created += new EventHandler(this.OnVertexBufferCreate);
            OnVertexBufferCreate(vbTerrain, null);

            texI = new Texture(device, new Bitmap("i.jpg"), 0, Pool.Managed);

            infoFont = new Microsoft.DirectX.Direct3D.Font(device, new System.Drawing.Font("Arial", 12.0f, FontStyle.Regular));

            mStack = new MatrixStack();

            myCamera = new Camera(new Vector3(0.0f, 5.0f, -15.0f), new Vector3(0.0f, 5.0f, 40.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f));

        }

        private void OnVertexBufferCreate(object sender, EventArgs e)
        {
            VertexBuffer buffer = (VertexBuffer)sender;

            if (buffer == vbTerrain)
            {
                CustomVertex.PositionNormalColored[] vertsTerrain = new CustomVertex.PositionNormalColored[1600];
                for (int i = 0; i < 40; i++)
                {
                    int index = i;
                    vertsTerrain[index * 4] = new CustomVertex.PositionNormalColored(new Vector3((float)i - 20, 0.0f, -20.0f), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());
                    vertsTerrain[index * 4 + 1] = new CustomVertex.PositionNormalColored(new Vector3((float)i - 20, 0.0f, 80.0f), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());
                    vertsTerrain[index * 4 + 2] = new CustomVertex.PositionNormalColored(new Vector3((float)i - 20, 20.0f, -20.0f), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());
                    vertsTerrain[index * 4 + 3] = new CustomVertex.PositionNormalColored(new Vector3((float)i - 20, 20.0f, 80.0f), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());

                }
                for (int i = 40; i < 140; i++)
                {
                    int index = i;
                    vertsTerrain[index * 4] = new CustomVertex.PositionNormalColored(new Vector3((-20.0f), 0.0f, (float)i - 60), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());
                    vertsTerrain[index * 4 + 1] = new CustomVertex.PositionNormalColored(new Vector3(20.0f, 0.0f, (float)i - 60), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());
                    vertsTerrain[index * 4 + 2] = new CustomVertex.PositionNormalColored(new Vector3((-20.0f), 20.0f, (float)i - 60), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());
                    vertsTerrain[index * 4 + 3] = new CustomVertex.PositionNormalColored(new Vector3(20.0f, 20.0f, (float)i - 60), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());

                }
                for (int i = 140; i < 161; i++)
                {
                    int index = i;
                    vertsTerrain[index * 4] = new CustomVertex.PositionNormalColored(new Vector3(20.0f, (float)i - 140, -20.0f), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());
                    vertsTerrain[index * 4 + 1] = new CustomVertex.PositionNormalColored(new Vector3(20.0f, (float)i - 140, 80.0f), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());
                    vertsTerrain[index * 4 + 2] = new CustomVertex.PositionNormalColored(new Vector3(-20.0f, (float)i - 140, -20.0f), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());
                    vertsTerrain[index * 4 + 3] = new CustomVertex.PositionNormalColored(new Vector3(-20.0f, (float)i - 140, 80.0f), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());

                }
                for (int i = 161; i < 180; i++)
                {
                    int index = i;
                    vertsTerrain[index * 4] = new CustomVertex.PositionNormalColored(new Vector3((-20.0f), (float)i - 160, 80.0f), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());
                    vertsTerrain[index * 4 + 1] = new CustomVertex.PositionNormalColored(new Vector3(20.0f, (float)i - 160, 80.0f), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());
                    vertsTerrain[index * 4 + 2] = new CustomVertex.PositionNormalColored(new Vector3((-20.0f), (float)i - 160, -20.0f), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());
                    vertsTerrain[index * 4 + 3] = new CustomVertex.PositionNormalColored(new Vector3(20.0f, (float)i - 160, -20.0f), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());

                }
                for (int i = 180; i < 221; i++)
                {
                    int index = i;
                    vertsTerrain[index * 4] = new CustomVertex.PositionNormalColored(new Vector3((float)i - 200, 0.0f, 80.0f), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());
                    vertsTerrain[index * 4 + 1] = new CustomVertex.PositionNormalColored(new Vector3((float)i - 200, 20.0f, 80.0f), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());
                    vertsTerrain[index * 4 + 2] = new CustomVertex.PositionNormalColored(new Vector3((float)i - 200, 0.0f, -20.0f), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());
                    vertsTerrain[index * 4 + 3] = new CustomVertex.PositionNormalColored(new Vector3((float)i - 200, 20.0f, -20.0f), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());

                }
                for (int i = 221; i < 321; i++)
                {
                    int index = i;
                    vertsTerrain[index * 4] = new CustomVertex.PositionNormalColored(new Vector3(20.0f, 0.0f, (float)i - 240), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());
                    vertsTerrain[index * 4 + 1] = new CustomVertex.PositionNormalColored(new Vector3(20.0f, 20.0f, (float)i - 240), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());
                    vertsTerrain[index * 4 + 2] = new CustomVertex.PositionNormalColored(new Vector3(-20.0f, 0.0f, (float)i - 240), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());
                    vertsTerrain[index * 4 + 3] = new CustomVertex.PositionNormalColored(new Vector3(-20.0f, 20.0f, (float)i - 240), new Vector3(0.0f, 1.0f, -0.4f), Color.DarkGreen.ToArgb());

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
            device.Lights[0].Ambient = Color.FromArgb(20,20,20);
            device.Lights[0].Type = LightType.Directional;
            device.Lights[0].Diffuse = Color.White;
            device.Lights[0].Position = new Vector3(150.0f, 50.0f, 150.0f);
            device.Lights[0].Direction = new Vector3(0.0f, -0.5f, -1.0f);
            device.Lights[0].Enabled = true;

            Color lightDiffColor = Color.White;
            if (wii != null)
                if (wii.Connected)
                    lightDiffColor = Color.Red;

            device.Lights[1].Type = LightType.Directional;
            device.Lights[1].Diffuse = lightDiffColor;
            //device.Lights[1].Direction = new Vector3(0.0f, 1.0f, 0.0f);
            device.Lights[1].Direction = new Vector3(0.0f, 1.0f, 0.0f);
            device.Lights[1].Enabled = false;
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

            //mStack.LoadMatrix(device.Transform.View);

            mStack.Push();
            {
                mStack.MultiplyMatrixLocal(
                        Matrix.Identity
                        );
                device.Transform.World = mStack.Top;

                device.VertexFormat = CustomVertex.PositionNormalColored.Format;
                device.SetStreamSource(0, vbTerrain, 0);
                device.DrawPrimitives(PrimitiveType.LineList, 0, 800);
            }
            mStack.Pop();
            //zidovi
            mStack.Push();
            {

                Material myMaterial = new Material();
                myMaterial.Ambient = Color.FromArgb(20, Color.White);
                myMaterial.Diffuse = Color.FromArgb(20, Color.White);
                myMaterial.Specular = Color.FromArgb(20, Color.White);
                myMaterial.Emissive = Color.FromArgb(50, 50, 50);

                device.Material = myMaterial;

                device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
                mStack.Push();
                {
                    mStack.MultiplyMatrixLocal(
                        Matrix.Translation(-15.0f, 7.5f, 0.0f)
                        );
                    device.Transform.World = mStack.Top;
                    zidBezProzoraMesh.DrawSubset(0);
                    mStack.MultiplyMatrixLocal(
                        Matrix.Translation(30.0f, 0.0f, 0.0f)
                        );
                    device.Transform.World = mStack.Top;
                    zidBezProzoraMesh.DrawSubset(0);
                    mStack.MultiplyMatrixLocal(
                        Matrix.RotationAxis(new Vector3(0, 1, 0), -(float)Math.PI / 2) *
                        Matrix.Translation(-15.0f, 0.0f, -15.0f)
                        );
                    device.Transform.World = mStack.Top;
                    zidBezProzoraMesh.DrawSubset(0);


                }
                mStack.Pop();
                mStack.Push();
                {
                    mStack.MultiplyMatrixLocal(
                        // Matrix.RotationAxis(new Vector3(1, 0, 0), -(float)Math.PI / 2) *
                        //  Matrix.Scaling(0.5f, 1.0f, 1.0f) *
                        Matrix.Translation(0.0f, 0.0f, 0.0f)

                        );
                    device.Transform.World = mStack.Top;
                    podMesh.DrawSubset(0);
                    mStack.MultiplyMatrixLocal(
                        // Matrix.RotationAxis(new Vector3(1, 0, 0), -(float)Math.PI / 2) *
                        //  Matrix.Scaling(0.5f, 1.0f, 1.0f) *
                        Matrix.Translation(0.0f, 15.0f, 0.0f)

                        );
                    device.Transform.World = mStack.Top;
                    podMesh.DrawSubset(0);
                }
                mStack.Pop();
                mStack.Push();
                {
                    myMaterial.Emissive = Color.FromArgb(30, 30, 30);
                    device.Material = myMaterial;
                    mStack.MultiplyMatrixLocal(
                        Matrix.Translation(-9.0f, 7.5f, 15.0f)

                        );
                    device.Transform.World = mStack.Top;
                    deoZida1Mesh.DrawSubset(0);
                    mStack.MultiplyMatrixLocal(
                        Matrix.Translation(18.0f, 0.0f, 0.0f)

                        );
                    device.Transform.World = mStack.Top;
                    deoZida1Mesh.DrawSubset(0);
                }
                mStack.Pop();
                mStack.Push(); 
                {
                    mStack.MultiplyMatrixLocal(
                           Matrix.Translation(0, 2.5f, 15.0f)
                           );
                    device.Transform.World = mStack.Top;
                    deoZida2Mesh.DrawSubset(0);
                    mStack.MultiplyMatrixLocal(
                           Matrix.Translation(0, 10, 0.0f)
                           );
                    device.Transform.World = mStack.Top;
                    deoZida2Mesh.DrawSubset(0);
                
                
                }
                mStack.Pop();
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
                myCamera.MoveStrafeLeftRight(moveStep);
            }
            if (keys[DI.Key.D])
            {
                myCamera.MoveStrafeLeftRight(-moveStep);
            }
            if (keys[DI.Key.W])
            {
                myCamera.MoveForwardBackward(moveStep);
            }
            if (keys[DI.Key.S])
            {
                myCamera.MoveForwardBackward(-moveStep);
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

        }

        protected void readWii()
        {

            myCamera.SetPosition(new Vector3(-5 * wii.HeadX, 5 * wii.HeadY, -5 * wii.HeadDist));

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

        static void Main()
        {
            {
                ScenaSoba f1 = new ScenaSoba();
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
