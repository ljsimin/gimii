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

using System.Timers;



using WiiApi;
namespace Headtracking2LEDs
{
    public partial class Primer1 : Form
    {

        //********************************************************
        //Inicijalizacija potrebnih promenljivih vezano za hardversku postavku sistema
        //********************************************************

        private bool fullscreen = true; //mod prikaza

        private const int sirinaEkrana = 1440;// rezolucija ekrana u pikselima

        private const int visinaEkrana = 900; //rezolucija ekrana u pikselima

        private const float razmakDiodaMM = 155f; //razmak izmedju leve i desne diode u milimetrima

        private const float visinaEkranaMM = 250f; //fizicka visina ekrana u milimetrima

        private const bool kontrolerIznadEkrana = true; // ako je kontroler iznad ekrana true, ako je ispod false

        private float faktorSkaliranja = 0.05f;// odnos jednog milimetra i jedinice velicine u sceni

        //************************************************************
        //************************************************************


        private Device uredjaj = null; //directx promenljiva

        private bool povezan = false; // true ako je kontroler konektovan, inace false

        private String infoTekst;

        WiiRekorderPolozaja rekorderPolozaja; //klasa iz api-ja koja generise dogadjaj promene polozaja

        private Vector3 polozajGlave = new Vector3(0.0f, 5.0f, 60.0f); // iz ove tacke se posmatra scena, obradjivac dogadjaja u ovu promenljivu upisuje izracunatu vrednost polozaja glave
        

       
        private VertexBuffer vbPodloga = null; //vertex buffer koji predstavlja podlogu (mreza kockica)

        private Microsoft.DirectX.Direct3D.Font infoFont;

      
        private Mesh lopta = null; //objekat lopta
        private Mesh zid = null;  //objekat zid

     
        
        private MatrixStack mStack = new MatrixStack();


        private delegate void PromenaStanjaKontroleraDelegat(PolozajGlave args); //delegat za reakciju na dogadjaj promene stanja kontrolera

        private bool prikaziInfo = true; // kontrolna promenljiva odlucuje da li ce se preko ekrana ispisivati tekst sa informacijama i koordinatama

        public Primer1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        }

     
        public void InicijalizujGrafikuIKameru()
        {
            // postavljanje parametara
            PresentParameters presentParams = new PresentParameters();

            presentParams.Windowed = true;
            presentParams.SwapEffect = SwapEffect.Discard;


            
            Format current = Manager.Adapters[0].CurrentDisplayMode.Format;

            if (fullscreen)
            {
                if (Manager.CheckDeviceType(0, DeviceType.Hardware, current, current, false))
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
                }
            }


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

            infoFont = new Microsoft.DirectX.Direct3D.Font(uredjaj, new System.Drawing.Font("Arial", 12.0f, FontStyle.Regular));
            mStack = new MatrixStack();
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
            uredjaj.Transform.View = Matrix.LookAtLH(polozajGlave, new Vector3(0f, 0f, 0f),new Vector3(0f,1f,0f));
            uredjaj.RenderState.CullMode = Cull.None;

            // podesavanje osvetljenja
            uredjaj.RenderState.Lighting = true;
            uredjaj.Lights[0].Type = LightType.Directional;
            uredjaj.Lights[0].Diffuse = Color.White;
            uredjaj.Lights[0].Direction = new Vector3(0.3f, -0.5f, -0.3f);
            uredjaj.Lights[0].Enabled = true;

            Color lightDiffColor = Color.White;
            if (povezan)
                    lightDiffColor = Color.Red;

            uredjaj.Lights[1].Type = LightType.Directional;
            uredjaj.Lights[1].Diffuse = lightDiffColor;
            uredjaj.Lights[1].Direction = new Vector3(0.0f, 1.0f, 0.0f);
            uredjaj.Lights[1].Enabled = true;
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Color backColor = Color.DarkBlue;
                if (povezan)
                {
                    backColor = Color.DimGray;
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


                if (povezan)
                {
                    ctrl = "Pritisnuti R za centriranje po vertikali"
                        //+"  \nGlavaX: " + polozajGlave.X + "  GlavaY: " + polozajGlave.Y + "  GlavaZ: " + polozajGlave.Z;
                        + infoTekst;
                }

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
                if (povezan == false)
                {
                    povezan = true;
                    if (rekorderPolozaja == null)
                    {
                        WiiFabrika.dobaviInstancu().postaviTipKontrolera(WiiTip.WII_KONTROLER);
                        Kontroler w= WiiFabrika.dobaviInstancu().kreirajKontroler();
                        //WiiFabrika.dobaviInstancu().postaviTipKontrolera(WiiTip.WII_EMULATOR);
                        //Kontroler w = WiiFabrika.dobaviInstancu().kreirajKontroler(@"d:\My documents\naocari1");
                        rekorderPolozaja = new WiiRekorderPolozaja(PracenjeGlaveTip.PRACENJE_2_IZVORA, w, razmakDiodaMM, visinaEkranaMM, kontrolerIznadEkrana);
                        rekorderPolozaja.PromenaPolozaja += PromenaStanjaKontrolera;
                    }
                    rekorderPolozaja.PocniPracenje();
                }
                else
                {
                    povezan = false;
                    rekorderPolozaja.ZavrsiPracenje();
                }

            }
            if (e.KeyCode == Keys.R)
            {
                if(povezan)
                    rekorderPolozaja.PodesavanjeVertikalnogUgla();
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


        //metod se poziva kada api generise dogadjaj
        private void PromenaStanjaKontrolera(object sender, PolozajGlave args)
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
        private void ReakcijaNaPromenu(PolozajGlave args)
        {
            if (args.Uspesno)
            {
                //prilagodjavanje pomeraja velicini scene, sto je scena veca posmatracemo je iz vece daljine i vise se pomeramo
                polozajGlave.X = args.Polozaj.X * faktorSkaliranja;
                polozajGlave.Y = args.Polozaj.Y * faktorSkaliranja;
                polozajGlave.Z = args.Polozaj.Z * faktorSkaliranja;
                infoTekst = "";
            }
            else
            {
                infoTekst = "\n IZVORI NISU VIDLJIVI!!!!"; 
            }
        }


        public void CloseTheForm()
        {
            Cursor.Show();
            Close();
            if(rekorderPolozaja!=null && povezan)
                rekorderPolozaja.ZavrsiPracenje();
        }

        static void Main()
        {
            {
                Primer1 f1 = new Primer1();
                f1.InicijalizujGrafikuIKameru();
                f1.Show();
                Application.Run(f1);
            }
        }
    }
}
