using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using WiiApi;

namespace WindowsGame1
{
    /* Author: Oliver Sipos e11627
     * Ova klasa predstavlja igru skvosa, sa navigacijom pomocu Wii kontrolera ili tastature
     */
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;    

        Matrix worldMatrix;
        Matrix cameraMatrix;
        Matrix projectionMatrix;
        float angle = 0f;
        Vector3 cameraPosition = new Vector3(0, 0, 25);
        Vector3 cameraLookAt = new Vector3(0, 0, 0);

        Vector3 reketLokacija = new Vector3(0, 0, 8);
        Vector3 loptaLokacija = new Vector3(1, 1, 1);
        Vector3 pocetnaBrzinaLopte = new Vector3(-0.1f, 0.2f, -0.3f);
        Vector3 brzinaLopte = new Vector3(-0.1f, 0.2f, -0.3f);        
        float gravitacija = -0.01f;
        
        int udaracReketa = 0; //sluzi za animaciju udarca
        const int duzinaUdarca = 18;

        //naredne promenljive sluze za onemogucavanje "lepljenja" loptice za povrsinu       
        int odbitakOdLevogDesnogZida = 0;        
        int odbitakOdPlafonaPoda = 0;       
        int odbitakOdPrednjegZida = 0;

        int vibriranje = 0;
        
        enum Mode { AkcAku=0, Akc=1, IRBar=2}
        Mode mode = Mode.AkcAku;//sluzi za odabiranje nacina navigacije pomocu Wii kontrolera
        int zastitaOdVisestrukogUnosa = 0;//prilikom odabiranja stanja pomocu Wii kontrolera

        BasicEffect sobaEfekti;
        Cube soba = new Cube(new Vector3(6, 7, 8), new Vector3(0, 0, 0));        

        GameObject reket = new GameObject();
        GameObject lopta = new GameObject();
        Vector3 centarReketa = new Vector3();//cuva sredinu povrsine kojom se moze udariti loptica

        WiiFabrika wiiFabrika;
        Kontroler kontroler;

        SoundEffect wallSound;//zvuk udara lopte o zid
        SoundEffect racketSound;//zvuk udara lopte o reket
        
        int brojUzastopnihUdaraca = 0;//skor
        int najboljiRez = 0;

        //naredne promenljive sluze za ispis poruka na ekranu
        SpriteFont font;//font za ispisivanje moda
        Vector2 fontPos;
        SpriteFont font2;//font za ispisivanje skora
        Vector2 font2Pos;
        String output2 = "";
        string output = "Mode: Akcelerometar akumulativno";

        //prethodna stanja koja se koriste za finiju navigaciju (manje drhtanja), 
        //jer kontroler vraca razlicite vrednosti akcelerometra u vremenu i pri mirovanju kontrolera.
        //za sto manje drhtanja, povecati broj prethodnih stanja, ali se time dobija vece kasnjenje.
        Vector3[] prethodnaStanja = new Vector3[10];        

        public Game1()
        {            
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            wiiFabrika = WiiFabrika.dobaviInstancu();
            wiiFabrika.postaviTipKontrolera(WiiTip.WII_KONTROLER);
            kontroler = wiiFabrika.kreirajKontroler();

            InitGraphicsMode(1024, 768, false);                            
        }

        private bool InitGraphicsMode(int iWidth, int iHeight, bool bFullScreen)
        {
            // If we aren't using a full screen mode, the height and width of the window can
            // be set to anything equal to or smaller than the actual screen size.
            if (bFullScreen == false)
            {
                if ((iWidth <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                    && (iHeight <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    graphics.PreferredBackBufferWidth = iWidth;
                    graphics.PreferredBackBufferHeight = iHeight;
                    graphics.IsFullScreen = bFullScreen;
                    graphics.ApplyChanges();
                    return true;
                }
            }
            else
            {
                // If we are using full screen mode, we should check to make sure that the display
                // adapter can handle the video mode we are trying to set.  To do this, we will
                // iterate thorugh the display modes supported by the adapter and check them against
                // the mode we want to set.
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    // Check the width and height of each mode against the passed values
                    if ((dm.Width == iWidth) && (dm.Height == iHeight))
                    {
                        // The mode is supported, so set the buffer formats, apply changes and return
                        graphics.PreferredBackBufferWidth = iWidth;
                        graphics.PreferredBackBufferHeight = iHeight;
                        graphics.IsFullScreen = bFullScreen;
                        graphics.ApplyChanges();
                        return true;
                    }
                }
            }
            return false;
        }
        public void initializeWorld()
        {
            cameraMatrix = Matrix.CreateLookAt(
                cameraPosition, cameraLookAt, new Vector3(0, 1, 0));
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                Window.ClientBounds.Width / Window.ClientBounds.Height, 1.0f, 50.0f);
           
            //sledece promenljive sluze za postavljanje odredjenih osobina sobe
            sobaEfekti = new BasicEffect(GraphicsDevice, null);
            sobaEfekti.World = worldMatrix;
            sobaEfekti.View = cameraMatrix;
            sobaEfekti.Projection = projectionMatrix;
            sobaEfekti.TextureEnabled = true;
            sobaEfekti.EnableDefaultLighting();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
            initializeWorld();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            //sledece promenljive povezuju objekte u klasi sa objektima u fajlovima
            wallSound = Content.Load<SoundEffect>("Audio\\WallBall");
            racketSound = Content.Load<SoundEffect>("Audio\\RacketBall");

            soba.shapeTexture = Content.Load<Texture2D>("Textures\\wall_texture");          

            reket.model = Content.Load<Model>("Models\\racket");
            reket.scale = 0.005f;
            reket.position = reketLokacija;

            lopta.model = Content.Load<Model>("Models\\ball");
            lopta.scale = 0.0001f;
            lopta.position = loptaLokacija;

            font = Content.Load<SpriteFont>("Text\\font");
            font2 = Content.Load<SpriteFont>("Text\\font2");

            fontPos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2,15);
            font2Pos = new Vector2(100 ,100);
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
          
            //osvezavanje centra reketa
            centarReketa = new Vector3(reket.position.X+1.5f, reket.position.Y+0.4f, reket.position.Z+1);
                        
            UpravljanjeTastaturom(); 
           
            if (kontroler != null)
            {
                UpravljanjeWiiKontrolerom();               
            }

            RotacijaReketa();//sluzi za animaciju udarca reketa
            Vibracija();
            KretanjeLopte();           

            base.Update(gameTime);
        }

        protected void RotacijaReketa()
        {
            //animacija udaranja loptice reketom(rotacija reketa oko y ose)
            if (udaracReketa > duzinaUdarca / 2)
            {
                reket.rotation.Y = (duzinaUdarca - udaracReketa) * 0.07f;
                udaracReketa--;
            }
            else if (udaracReketa > 0)
            {
                reket.rotation.Y *= udaracReketa * 0.07f;
                udaracReketa--;
            }
            else
            {
                reket.rotation.Y = 0;
            }      
        }

        protected void UpravljanjeWiiKontrolerom()
        {
            if (kontroler.Stanje.Dugmici.A && zastitaOdVisestrukogUnosa == 0)
            {
                if (mode == Mode.IRBar)
                {
                    mode = Mode.AkcAku;
                    output = "Mode: Akcelerometar akumulativno";
                }
                else if (mode == Mode.AkcAku)
                {
                    mode = Mode.Akc;
                    output = "Mode: Akcelerometar";
                }
                else
                {
                    mode = Mode.IRBar;
                    output = "Mode: IRBar";
                }
                System.Console.WriteLine("Mode: " + mode);
                //narednih 20 osvezavanja nece moci da se bira mod
                zastitaOdVisestrukogUnosa = 20;
            }
            if (zastitaOdVisestrukogUnosa > 0)
                zastitaOdVisestrukogUnosa--;

            if (kontroler.Stanje.Dugmici.GORE)
            {
                if (reket.position.Z >= soba.shapeSize.Z / 2)
                    reket.position.Z -= 0.15f;
            }
            if (kontroler.Stanje.Dugmici.DOLE)
            {
                if (reket.position.Z <= soba.shapeSize.Z + 2)
                    reket.position.Z += 0.15f;
            }

            if (mode == Mode.AkcAku)
            {
                if (kontroler.Stanje.Akcelerometar.X > 0.1 && reket.position.X < (soba.shapeSize.X - 2.4))
                    reket.position.X += kontroler.Stanje.Akcelerometar.X * 0.3f;
                if (kontroler.Stanje.Akcelerometar.X < -0.1 && reket.position.X > -(soba.shapeSize.X + 0.3))
                    reket.position.X += kontroler.Stanje.Akcelerometar.X * 0.3f;
                if (kontroler.Stanje.Akcelerometar.Y > 0.1 && reket.position.Y > -(soba.shapeSize.Y - 0.5))
                    reket.position.Y -= kontroler.Stanje.Akcelerometar.Y * 0.5f;
                if (kontroler.Stanje.Akcelerometar.Y < -0.1 && reket.position.Y < soba.shapeSize.Y - 1.2)
                    reket.position.Y -= kontroler.Stanje.Akcelerometar.Y * 0.5f;
            }
            else if (mode == Mode.Akc)
            {

                for (int i = 1; i < prethodnaStanja.Length; i++)
                {
                    prethodnaStanja[prethodnaStanja.Length - i].X = prethodnaStanja[prethodnaStanja.Length - i - 1].X;
                    prethodnaStanja[prethodnaStanja.Length - i].Y = prethodnaStanja[prethodnaStanja.Length - i - 1].Y;
                    prethodnaStanja[prethodnaStanja.Length - i].Z = prethodnaStanja[prethodnaStanja.Length - i - 1].Z;
                }

                prethodnaStanja[0].X = kontroler.Stanje.Akcelerometar.X;
                prethodnaStanja[0].Y = kontroler.Stanje.Akcelerometar.Y;
                prethodnaStanja[0].Z = kontroler.Stanje.Akcelerometar.Z;

                reket.position.X = srednjaVrednostVektora(prethodnaStanja).X * 6;
                reket.position.Y = -srednjaVrednostVektora(prethodnaStanja).Y * 8;
            }
            else// ako je u IR bar modu
            {
                if (kontroler.Stanje.Senzori[0].Nadjen)
                {
                    System.Console.WriteLine(kontroler.Stanje.Senzori[0].y);
                    reket.position.X = -kontroler.Stanje.Senzori[0].x * 12 + 6;
                    reket.position.Y = -kontroler.Stanje.Senzori[0].y * 12 + 6;
                }
            }
        }
        protected void UpravljanjeTastaturom()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                graphics.ToggleFullScreen();
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                if (reket.position.Y < soba.shapeSize.Y - 1.2)
                    reket.position.Y += 0.15f;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                if (reket.position.Y > -(soba.shapeSize.Y - 0.5))
                    reket.position.Y -= 0.15f;
            }
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                if (reket.position.X > -(soba.shapeSize.X + 0.3))
                    reket.position.X -= 0.15f;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                if (reket.position.X < (soba.shapeSize.X - 2.4))
                    reket.position.X += 0.15f;
            }
            if (keyboardState.IsKeyDown(Keys.W))
            {
                if (reket.position.Z >= soba.shapeSize.Z/2)
                    reket.position.Z -= 0.15f;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                if(reket.position.Z <= soba.shapeSize.Z+2)
                    reket.position.Z += 0.15f;
            }
            //za sada nije omoguceno kontrolisanje udarca, samo se animira
            if (keyboardState.IsKeyDown(Keys.Space) && udaracReketa <= 0)
            {
                udaracReketa = duzinaUdarca;
            }
        }

        public Vector3 srednjaVrednostVektora(Vector3[] vektori){
            Vector3 akumulator = new Vector3(0, 0, 0);
            for (int i = 0; i < vektori.Length; i++)
            {
                akumulator += vektori[i];
            }
            return akumulator / vektori.Length;
        }    

        //Odredjuje duzinu vibracije
        protected void Vibracija()
        {            
            if (kontroler !=null && kontroler.Stanje.Reakcija.VIBRACIJA == true)
            {
                vibriranje++;
                if (vibriranje == 10)
                {
                    vibriranje = 0;
                    kontroler.postaviVibrator(false);
                }
            }
        }

        //Fizika kretanja loptice
        protected void KretanjeLopte()
        {
            //Logika vezana za zastitu od lepljenja loptice za zid
            odbitakOdLevogDesnogZida--;
            odbitakOdPlafonaPoda--;
            odbitakOdPrednjegZida--;

            lopta.position += brzinaLopte;
            brzinaLopte.Y += gravitacija;
            if (lopta.position.Z < soba.shapeSize.Z)//logika kretanja loptice dok je u sobi
            {
                if (lopta.position.Z < -soba.shapeSize.Z && odbitakOdPrednjegZida<=0)
                {
                    odbitakOdPrednjegZida = 10;
                    brzinaLopte.Z = -brzinaLopte.Z;
                    wallSound.Play();                   
                }
                if ((lopta.position.Y > soba.shapeSize.Y - 0.5) || lopta.position.Y < -(soba.shapeSize.Y - 0.5) && odbitakOdPlafonaPoda<=0)
                {
                    odbitakOdPlafonaPoda = 10;
                    brzinaLopte.Y = -brzinaLopte.Y;
                    brzinaLopte *= 0.97f;
                    wallSound.Play();
                }
                if (lopta.position.X > (soba.shapeSize.X - 0.5) || lopta.position.X < -(soba.shapeSize.X - 0.5) && odbitakOdLevogDesnogZida <= 0)
                {
                    odbitakOdLevogDesnogZida = 10;
                    brzinaLopte.X = -brzinaLopte.X;
                    wallSound.Play();
                }
            }else if (lopta.position.Z > soba.shapeSize.Z+2)//Ako izadje iz sobe, inicjalizuje lokaciju i brzinu lopte
            {
                if (brojUzastopnihUdaraca > najboljiRez)
                    najboljiRez = brojUzastopnihUdaraca;
                brojUzastopnihUdaraca = 0;
                lopta.position = loptaLokacija;
                
                Random random = new Random();
                float brzinaX = (float)random.NextDouble()/5-0.1f;
                float brzinaY = (float)random.NextDouble()/5+0.1f;
                float brzinaZ = pocetnaBrzinaLopte.Z;
                
                if (random.Next(1) == 1)
                {
                    brzinaX = -brzinaX;                   
                }
                if (random.Next(1) == 1)
                {                    
                    brzinaY = -brzinaY;
                }
                brzinaLopte = new Vector3(brzinaX, brzinaY, brzinaZ);
            }           
            
            //U slucaju da udari u reket          
            if (lopta.position.Z >= reket.position.Z - 1 && lopta.position.Z <= reket.position.Z - 0.8 && udaracReketa<=0)
            {
                if (Math.Abs(lopta.position.X - centarReketa.X) <= 1.2) //= 1.2
                {
                    if (Math.Abs(lopta.position.Y - centarReketa.Y) <= 1)// = 1
                    {
                        brojUzastopnihUdaraca++;//skor
                        brzinaLopte.Z = -brzinaLopte.Z;//odbijanje o reket

                        //naredne promenljive definisu fiziku loptice prilikom udara u reket
                        bool nisko = false;
                        if(Math.Abs(brzinaLopte.Y)<0.15f && lopta.position.Y<-2)
                            nisko = true;

                        if (lopta.position.Y > 0)
                        {
                            brzinaLopte.Z *= 1.6f;
                            brzinaLopte.Y = (float)((new Random().NextDouble()) / 5 + 0.1f);
                        }
                        else
                        {
                            brzinaLopte.Z *= 1.8f;
                            brzinaLopte.Y = (float)((new Random().NextDouble()) / 5 + 0.1f);
                        }

                        if (brzinaLopte.Z < -0.4f)
                            brzinaLopte.Z = -0.4f;

                        if (nisko)
                        {
                            brzinaLopte.Y += 0.1f;                            
                        }

                        brzinaLopte.X += (float)((new Random().NextDouble()) / 5 - 0.1f);
                        if (kontroler != null)
                            kontroler.postaviVibrator(true);
                        racketSound.Play();
                        udaracReketa = duzinaUdarca;
                    }
                }
            }
            //otpor vazduha            
            brzinaLopte *= 0.9965f;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkCyan);
            
            DrawGameObject(reket);
            DrawGameObject(lopta);            

            //crtanje sobe
            sobaEfekti.Begin();        

            worldMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(angle)) *
                Matrix.CreateRotationX(MathHelper.ToRadians(angle));
            sobaEfekti.World = worldMatrix;

            foreach (EffectPass pass in sobaEfekti.CurrentTechnique.Passes)
            {
                pass.Begin();
                
                sobaEfekti.Texture = soba.shapeTexture;
                soba.RenderShape(GraphicsDevice);                
                pass.End();
            }

            sobaEfekti.End();

            //Ispisivanje teksta(skora i moda)
            //Bitno!!! Ako mesate iscrtavanje 3D i 2D objekata, morate uvek sacuvati stanje neposredno pre iscrtavanja 2D objekata
            //To mozete uciniti tako sto cete postaviti SaveStateMode na SaveState u spriteBatch.Begin()
            //ali je to ponekad malo "skupo" jer cuva stanja svih promenljivih,
            //tako da mozete sami cuvati sve promenljive koje se menjaju
            spriteBatch.Begin(SpriteBlendMode.Additive,SpriteSortMode.Immediate,SaveStateMode.SaveState);
                       
            output2 = "Score: " + brojUzastopnihUdaraca.ToString() + "\nBest Score: " + najboljiRez;
                        
            Vector2 FontOrigin = font.MeasureString(output) / 2;
            
            spriteBatch.DrawString(font, output, fontPos, Color.Brown,
                0, FontOrigin, 2.0f, SpriteEffects.None, 0.5f);

            spriteBatch.DrawString(font2, output2, font2Pos, Color.DarkGray,
                0, font.MeasureString(output2)/2, 2.0f, SpriteEffects.None, 0.5f);
            
            spriteBatch.End();
            base.Draw(gameTime);
        }       

        void DrawGameObject(GameObject gameobject)
        {
            foreach (ModelMesh mesh in gameobject.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    effect.World =
                        Matrix.CreateFromYawPitchRoll(
                        gameobject.rotation.Y,
                        gameobject.rotation.X,
                        gameobject.rotation.Z) *

                        Matrix.CreateScale(gameobject.scale) *

                        Matrix.CreateTranslation(gameobject.position);

                    effect.Projection = projectionMatrix;
                    effect.View = cameraMatrix;
                }
                mesh.Draw();
            }
        }
    }
}
