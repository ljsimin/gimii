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
    public partial class Primer1ZidLopta : Form
    {
        private Device uredjaj = null;

        private Vector3 polozajGlave = new Vector3(0.0f, 5.0f, 60.0f); // iz ove tacke se posmatra scena, obradjivac dogadjaja u ovu promenljivu upisuje izracunatu vrednost polozaja glave

        public const int sirinaEkrana = 1024;
        public const int visinaEkrana = 768;

       
        private VertexBuffer vbPodloga = null; //vertex buffer koji predstavlja podlogu (mreza kockica)

        private Microsoft.DirectX.Direct3D.Font infoFont;

      
        private Mesh lopta = null; //objekat lopta
        private Mesh zid = null;  //objekat zid

        //private Texture texI = null;


        private float korak = 0.2f; //korak pomeranja, kada se za kretanje koristi tastatura

        private Camera kamera = null; // klasa koja predstavlja kameru

        private Microsoft.DirectX.DirectInput.Device tastatura; // directx klasa koja predstavlja tastaturu

        protected System.Timers.Timer tajmer;

       private int TIMER_INTERVAL = 10; // vremenski interval generisanja dogadjaja tajmera
        
        private MatrixStack mStack = new MatrixStack();

        private HeadtrackingAdapterProba wii; // oponasa adapter iz apija

        private delegate void PromenaStanjaKontroleraDelegat(Vector3 args); //delegat za reakciju na dogadjaj promene stanja kontrolera

        private bool prikaziInfo = true; // kontrolna promenljiva odlucuje da li ce se preko ekrana ispisivati tekst sa informacijama i koordinatama

        public Primer1ZidLopta()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        }

        public void InicijalizujTajmer()
        {
            tajmer = new System.Timers.Timer();

            tajmer.Elapsed += new ElapsedEventHandler(DogadjajTajmera);
            tajmer.Interval = TIMER_INTERVAL;
            tajmer.Start();
        }

        public void InicijalizujWii()
        {
            wii = new HeadtrackingAdapterProba();
            wii.PromenaPolozaja += this.PromenaStanjaKontrolera;
            
        }

        public void inicijalizujTastaturu()
        {
            tastatura = new DI.Device(DI.SystemGuid.Keyboard);
            tastatura.SetCooperativeLevel(this, DI.CooperativeLevelFlags.Background | DI.CooperativeLevelFlags.NonExclusive);
            tastatura.Acquire();
        }



        public void InicijalizujGrafikuIKameru()
        {
            // postavljanje parametara
            PresentParameters presentParams = new PresentParameters();

            presentParams.Windowed = true;
            presentParams.SwapEffect = SwapEffect.Discard;


            // postavi full screen
            Format current = Manager.Adapters[0].CurrentDisplayMode.Format;

            /*if (Manager.CheckDeviceType(0, DeviceType.Hardware, current, current, false))
            {
                presentParams.Windowed = false;
                presentParams.BackBufferFormat = current;
                presentParams.BackBufferCount = 1;
                presentParams.BackBufferWidth = sirinaEkrana;
                presentParams.BackBufferHeight = visinaEkrana;
                Cursor.Hide();
            }
            else
            {
                presentParams.Windowed = true;
            }*/


            // Depth Buffer
            presentParams.EnableAutoDepthStencil = true;
            presentParams.AutoDepthStencilFormat = DepthFormat.D16;

            // kreiranje uredjaja
            uredjaj = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);

            // kreiranje objekata na sceni
     
            lopta = Mesh.Sphere(uredjaj, 1.0f, 72, 72);
            zid = Mesh.Box(uredjaj, 20.0f, 4.0f, 1.0f);
            vbPodloga = new VertexBuffer(typeof(CustomVertex.PositionNormalColored), 800, uredjaj, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalColored.Format, Pool.Default);
            vbPodloga.Created += new EventHandler(this.OnVertexBufferCreate);
            OnVertexBufferCreate(vbPodloga, null);

            //texI = new Texture(device, new Bitmap("i.jpg"), 0, Pool.Managed);

            infoFont = new Microsoft.DirectX.Direct3D.Font(uredjaj, new System.Drawing.Font("Arial", 12.0f, FontStyle.Regular));
            mStack = new MatrixStack();
            kamera = new Camera(polozajGlave, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f));
        }

        private void OnVertexBufferCreate(object sender, EventArgs e)
        {
            VertexBuffer buffer = (VertexBuffer)sender;

            if (buffer == vbPodloga)
            {
                CustomVertex.PositionNormalColored[] vertsTerrain = new CustomVertex.PositionNormalColored[800];
                for (int i = -100; i < 100; i++)
                {
                    int index = i + 100;
                    vertsTerrain[index * 4] = new CustomVertex.PositionNormalColored(new Vector3((-100.0f), 0.0f, (float)i), new Vector3(0.0f, 1.0f, -0.4f), Color.WhiteSmoke.ToArgb());
                    vertsTerrain[index * 4 + 1] = new CustomVertex.PositionNormalColored(new Vector3(100.0f, 0.0f, (float)i), new Vector3(0.0f, 1.0f, -0.4f), Color.WhiteSmoke.ToArgb());
                    vertsTerrain[index * 4 + 2] = new CustomVertex.PositionNormalColored(new Vector3((float)i, 0.0f, -100.0f), new Vector3(0.0f, 1.0f, -0.4f), Color.WhiteSmoke.ToArgb());
                    vertsTerrain[index * 4 + 3] = new CustomVertex.PositionNormalColored(new Vector3((float)i, 0.0f, 100.0f), new Vector3(0.0f, 1.0f, -0.4f), Color.WhiteSmoke.ToArgb());
                }
                buffer.SetData(vertsTerrain, 0, LockFlags.None);
            }
        }

        private void PostaviKameru()
        {
            //podesavanje projekcije
            uredjaj.Transform.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, (float)sirinaEkrana/visinaEkrana, 1.0f, 10000.0f);
            uredjaj.Transform.View = kamera.LookLeftHanded();
            uredjaj.RenderState.CullMode = Cull.None;

            // podesavanje osvetljenja
            uredjaj.RenderState.Lighting = true;
            uredjaj.Lights[0].Type = LightType.Directional;
            uredjaj.Lights[0].Diffuse = Color.White;
            uredjaj.Lights[0].Direction = new Vector3(0.3f, -0.5f, -0.3f);
            uredjaj.Lights[0].Enabled = true;

            Color lightDiffColor = Color.White;
            if (wii != null)
                if (wii.Connected)
                    lightDiffColor = Color.Red;

            uredjaj.Lights[1].Type = LightType.Directional;
            uredjaj.Lights[1].Diffuse = lightDiffColor;
            uredjaj.Lights[1].Direction = new Vector3(0.0f, 1.0f, 0.0f);
            uredjaj.Lights[1].Enabled = true;
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Color backColor = Color.DarkBlue;
            if (wii != null)
                if (wii.Connected)
                {
                    backColor = Color.DimGray;
                    kamera.SetPosition(polozajGlave);
                }

            uredjaj.Clear(ClearFlags.Target | ClearFlags.ZBuffer, backColor, 1.0f, 0);

            PostaviKameru();

            uredjaj.BeginScene();

            

            mStack.Push();
            {
                mStack.MultiplyMatrixLocal(
                        Matrix.Identity
                        );
                uredjaj.Transform.World = mStack.Top;

                uredjaj.VertexFormat = CustomVertex.PositionNormalColored.Format;
                uredjaj.SetStreamSource(0, vbPodloga, 0);
                uredjaj.DrawPrimitives(PrimitiveType.LineList, 0, 400);
            }
            mStack.Pop();

            mStack.Push();
            {
                mStack.Push();
                {
                    // LOPTA

                    Material loptaMaterial = new Material();
                    loptaMaterial.Ambient = Color.White;
                    loptaMaterial.Diffuse = Color.White;
                    loptaMaterial.Specular = Color.White;
                    uredjaj.Material = loptaMaterial;

                    uredjaj.VertexFormat = CustomVertex.PositionNormalTextured.Format;

                    mStack.Push();
                    {
                        mStack.MultiplyMatrixLocal(
                            Matrix.Translation(0.0f, 1.0f, 0.0f)
                            );
                        uredjaj.Transform.World = mStack.Top;
                        lopta.DrawSubset(0);
                    }
                    mStack.Pop();


                }
                mStack.Pop();


                mStack.Push();
                {
                    // ZID
                    Material zidMaterial = new Material();
                    zidMaterial.Ambient = Color.White;
                    zidMaterial.Diffuse = Color.Brown;
                    zidMaterial.Specular = Color.White;
                    uredjaj.Material = zidMaterial;

                    uredjaj.VertexFormat = CustomVertex.PositionNormalTextured.Format;

                    mStack.Push();
                    {
                        mStack.MultiplyMatrixLocal(
                            Matrix.Translation(0.0f, 2.0f, 10.0f)
                            );
                        uredjaj.Transform.World = mStack.Top;
                        zid.DrawSubset(0);
                    }
                    mStack.Pop();

                }
                mStack.Pop();

            }
            mStack.Pop();


            //prikaz info teksta
            if (prikaziInfo)
            {
                infoFont.DrawText(null, "Pritisnuti Insert za povezivanje/iskljucivanje kontrolera, Escape za izlazak", new Rectangle(25, 25, 0, 0),
               DrawTextFormat.NoClip, Color.PaleGreen);
                infoFont.DrawText(null, "Pritisnuti I za paljenje/gasenje info teksta", new Rectangle(25, 45, 0, 0),
               DrawTextFormat.NoClip, Color.PaleGreen);

                string ctrl = "Wii kontroler nije povezan";

                if (wii != null)
                    if (wii.Connected)
                        ctrl = "Pritisnuti R za centriranje po vertikali"
                            + "\nX1: " + wii.X1 + "  Y1: " + wii.Y1 + "  X2: " + wii.X2 + "  Y2: " + wii.Y2 +
                            "  \nGlavaX: " + polozajGlave.X + "  GlavaY: " + polozajGlave.Y + "  GlavaZ: " + polozajGlave.Z;

                infoFont.DrawText(null, ctrl, new Rectangle(25, 65, 0, 0),
               DrawTextFormat.NoClip, Color.PaleGreen);


            }

            uredjaj.EndScene();
            uredjaj.Present();
            this.Invalidate();
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
                prikaziInfo = !prikaziInfo;
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

        protected void citajTastaturu()
        {
            DI.KeyboardState keys = tastatura.GetCurrentKeyboardState();
            if (keys[DI.Key.F1])
            {
                kamera.SetPosition(new Vector3(0.0f, 20.0f, -15.0f));
            }
            if (keys[DI.Key.F2])
            {
                kamera.SetPosition(new Vector3(-2.0f, 2.0f, -15.0f));
            }
            if (keys[DI.Key.F3])
            {
                kamera.SetPosition(new Vector3(30.0f, -20.0f, -15.0f));
            }

            if (keys[DI.Key.A])
            {
                kamera.MoveStrafeLeftRight(-korak);
            }
            if (keys[DI.Key.D])
            {
                kamera.MoveStrafeLeftRight(korak);
            }
            if (keys[DI.Key.W])
            {
                kamera.MoveForwardBackward(-korak);
            }
            if (keys[DI.Key.S])
            {
                kamera.MoveForwardBackward(korak);
            }
            if (keys[DI.Key.C])
            {
                kamera.MoveUpDown(-korak);
            }
            if (keys[DI.Key.Space])
            {
                kamera.MoveUpDown(korak);
            }
            if (keys[DI.Key.Up])
            {
                kamera.Pitch(0.02f);
            }
            if (keys[DI.Key.Down])
            {
                kamera.Pitch(-0.02f);
            }
            if (keys[DI.Key.Left])
            {
                if (keys[DI.Key.LeftAlt])
                    kamera.Yaw(0.02f);
                else
                    kamera.MoveViewRotateLeftRight(0.02f);
            }
            if (keys[DI.Key.Right])
            {
                if (keys[DI.Key.LeftAlt])
                    kamera.Yaw(-0.02f);
                else
                    kamera.MoveViewRotateLeftRight(-0.02f);
            }

        }


        //metod se poziva kada api generise dogadjaj
        private void PromenaStanjaKontrolera(object sender, Vector3 args)
        {
            try
            {
                PromenaStanjaKontroleraDelegat uwsd = new PromenaStanjaKontroleraDelegat(this.ReakcijaNaPromenu);
                uwsd.BeginInvoke(args, null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        //kao reakcija na dogadjaj promene stanja kontrolera, osvezava koordinate glave
        private void ReakcijaNaPromenu(Vector3 args)
        {
            args.Multiply(0.05f); //prilagodjavanje pomeraja velicini scene, sto je scena veca posmatracemo je iz vece daljine i vise se pomeramo
            polozajGlave = args;
        }


        public void CloseTheForm()
        {
            if (wii.Connected)
                wii.ConnectAction();
            Cursor.Show();
            Close();
        }

        //u zadatim vremenskim intervalima se vrsi provera tastature i update polozaja
        protected void DogadjajTajmera(object source, ElapsedEventArgs e)
        {
            citajTastaturu();
             
        }

        static void Main()
        {
            {
                Primer1ZidLopta f1 = new Primer1ZidLopta();
                f1.InicijalizujGrafikuIKameru();
                f1.inicijalizujTastaturu();
                f1.InicijalizujWii();
                f1.InicijalizujTajmer();
                f1.Show();
                Application.Run(f1);
            }
        }
    }
}
