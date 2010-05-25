using System;
using System.IO;
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
using Common;
using WiiApi;

namespace Caliibrator
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        public Camera camera { get; protected set; }
        ModelManager modelManager;
        Grid grid1;

        private String message = "";

        public Kontroler wLeft;
        public Kontroler wRight;
        public bool connected {get; set;}
        public bool wiimode { get; set; }

        public Vector3 wiipos1 { get; set; }
        public Vector3 wiipos2 { get; set; }

        private const float XFACTOR = 300.0f;//30.0f;
        private const float YFACTOR = -120.0f;//12.0f;
        private const float ZFACTOR = -360.0f;//36.0f;

        float minX = 0;
        float maxX = 1;
        float minY = 0;
        float maxY = 1;
        float minZ = 1.5f;
        float maxZ = 10;

        private bool calibrateMinX = false;
        private bool calibrateMaxX = false;
        private bool calibrateMinY = false;
        private bool calibrateMaxY = false;
        private bool calibrateMinZ = false;
        private bool calibrateMaxZ = false;

        private bool calibrated = false;

        private CircularBuffer<Vector3> positions1 = new CircularBuffer<Vector3>(30);
        private CircularBuffer<Vector3> positions2 = new CircularBuffer<Vector3>(30);

        private static float FILTERING_THRESHOLD = 12.0f;

        private delegate void Obrada(ParametriDogadjaja p);

        private bool dofilter = false;
        TextWriter tw;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            connected = false;
            tw = new StreamWriter("data.txt");
        }

        public Vector3 getCalibrationMinimum()
        {
            return new Vector3(minX, minY, minZ);
        }

        public Vector3 getCalibrationMaximum()
        {
            return new Vector3(maxX, maxY, maxZ);
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
            camera = new Camera(this, new Vector3(0, 0, 250),Vector3.Zero, Vector3.Up);
            Components.Add(camera);

            modelManager = new ModelManager(this);
            Components.Add(modelManager);

            grid1 = new Grid(this, camera);
            grid1.Visible = true;
            grid1.GridX = 256;
            grid1.GridY = 256;
            grid1.Spacing = 32.0f;
            grid1.Position = new Vector3(0, -100, -25);
            grid1.Rotation = Vector3.Zero;
            Components.Add(grid1);

            wiipos1 = new Vector3();
            wiipos2 = new Vector3();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>(@"fonts\arial");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            //WiiFabrika.dobaviInstancu().iskljuci(wLeft);
            //WiiFabrika.dobaviInstancu().iskljuci(wRight);
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
            /*if (Keyboard.GetState().IsKeyDown(Keys.Delete))
            {
                tw.Flush();
                tw.Close();
                this.Exit();
            }*/

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                connect();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.K))
            {
                wiimode = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Add))
            {
                FILTERING_THRESHOLD += 1.0f;

            }

            if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
            {
                FILTERING_THRESHOLD -= 1.0f;
                if (FILTERING_THRESHOLD < 0)
                {
                    FILTERING_THRESHOLD = 0;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                dofilter = true;
                positions1.clear();
                positions2.clear();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.U))
            {
                dofilter = false;
            }
            

            if (wiimode)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    calibrateMinX = true;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    calibrateMaxX = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    calibrateMinY = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    calibrateMaxY = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.PageUp))
                {
                    calibrateMinZ = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.PageDown))
                {
                    calibrateMaxZ = true;
                }
                if(Keyboard.GetState().IsKeyDown(Keys.C))
                {
                    calibrated = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    calibrated = false;
                }
            }


            message = "";

            if (wiimode) calculate();

            message += (wiimode) ? "Mode: Wiimote" : "Mode: Keyboard";
            message += "\n";
            message += (calibrated) ? "Calibrated: Yes" : "Calibrated: No";
            message += "\n";
            message += (dofilter) ? "Filtered: Yes" : "Filtered: No";
            message += "\n";
            message += "Threshold: " + Math.Round(FILTERING_THRESHOLD, 2);
            message += "\n";

            // TODO: Add your update logic here

            base.Update(gameTime);
        }
        private void connect()
        {
            if (wLeft == null || wRight == null)
            {
                WiiFabrika.dobaviInstancu().postaviTipKontrolera(WiiTip.WII_KONTROLER);
                wLeft = WiiFabrika.dobaviInstancu().kreirajKontroler();
                wRight = WiiFabrika.dobaviInstancu().kreirajKontroler();
                connected = true;
                wiimode = true;

                wLeft.kreni(true);
                wRight.kreni(true);
                wLeft.postaviLED(0, true);
                wLeft.postaviLED(1, false);
                wLeft.postaviLED(2, true);
                wLeft.postaviLED(3, false);

                wRight.postaviLED(0, false);
                wRight.postaviLED(1, true);
                wRight.postaviLED(2, false);
                wRight.postaviLED(3, true);
            }
            else
            {
                wiimode = true;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);
            spriteBatch.Begin();
            // Draw fonts
            spriteBatch.DrawString(font, message,new Vector2(10, 10), Color.Black, 0, Vector2.Zero,1, SpriteEffects.None, 1);
            spriteBatch.End();
            GraphicsDevice.RenderState.DepthBufferEnable = true;
            GraphicsDevice.PresentationParameters.EnableAutoDepthStencil = true;
            base.Draw(gameTime);
        }


        private void calculate()
        {
            if (wLeft.Stanje.Senzori[0].Nadjen && wRight.Stanje.Senzori[0].Nadjen)
            {
                float lx = wLeft.Stanje.Senzori[0].X;
                float ly = wLeft.Stanje.Senzori[0].Y;

                float rx = wRight.Stanje.Senzori[0].X;
                float ry = wRight.Stanje.Senzori[0].Y;

                float cx = (lx + rx) / 2;
                float cy = (ly + ry) / 2;

                double alpha = (1 - lx) * Math.PI / 4 + (Math.PI / 8) * 3;
                double beta = rx * Math.PI / 4 + (Math.PI / 8) * 3;

                float cz = (float)(1 / ((1 / Math.Tan(alpha)) + (1 / Math.Tan(beta))));

                //koristimo trenutno izracunate podatke za kalibraciju
                calibrate(cx, cy, cz);

                float x, y, z;

                //primena kalibracije
                if (calibrated)
                {
                    x = (cx - minX) / (maxX - minX);
                    y = (cy - minY) / (maxY - minY);
                    z = (cz - minZ) / (maxZ - minZ);
                }
                else
                {
                    x = cx;
                    y = cy;
                    z = (cz - 1.5f) / 8.5f;
                }
                wiipos1 = new Vector3((x - 0.5f) * XFACTOR, -(y - 0.5f) * YFACTOR, (z - 0.5f) * ZFACTOR);
                if (calibrated && dofilter)
                {
                    wiipos1 = filter(wiipos1, positions1);
                }
                positions1.add(wiipos1);
            }

            if (wLeft.Stanje.Senzori[1].Nadjen && wRight.Stanje.Senzori[1].Nadjen)
            {
                float lx = wLeft.Stanje.Senzori[1].X;
                float ly = wLeft.Stanje.Senzori[1].Y;

                float rx = wRight.Stanje.Senzori[1].X;
                float ry = wRight.Stanje.Senzori[1].Y;

                float cx = (lx + rx) / 2;
                float cy = (ly + ry) / 2;

                double alpha = (1 - lx) * Math.PI / 4 + (Math.PI / 8) * 3;
                double beta = rx * Math.PI / 4 + (Math.PI / 8) * 3;

                float cz = (float)(1 / ((1 / Math.Tan(alpha)) + (1 / Math.Tan(beta))));

                float x, y, z;

                //primena kalibracije
                if (calibrated)
                {
                    x = (cx - minX) / (maxX - minX);
                    y = (cy - minY) / (maxY - minY);
                    z = (cz - minZ) / (maxZ - minZ);
                }
                else
                {
                    x = cx;
                    y = cy;
                    z = (cz - 1.5f) / 8.5f;
                }
                wiipos2 = new Vector3((x - 0.5f) * XFACTOR, -(y - 0.5f) * YFACTOR, (z - 0.5f) * ZFACTOR);                
                if (calibrated && dofilter)
                {
                    wiipos2 = filter(wiipos2, positions2);
                }
                positions2.add(wiipos2);
            }
            message += "X1 : " + wiipos1.X + " " + "X2: " + wiipos2.X + "\n";
            message += "Y1 : " + wiipos1.Y + " " + "Y2: " + wiipos2.Y + "\n";
            message += "Z1 : " + wiipos1.Z + " " + "Z2: " + wiipos2.Z + "\n";
        }

        private Vector3 filter(Vector3 input, CircularBuffer<Vector3> history){
            Vector3 result;
            float x = 0;
            float y = 0;
            float z = 0;

            if (history.Count == 0)
            {
                x = input.X;
                y = input.Y;
                z = input.Z;
            }
            else
            {
                Vector3 last = history[history.Count - 1];
                if (Math.Abs(last.Z - input.Z) < FILTERING_THRESHOLD)
                {
                    x = input.X;
                    y = input.Y;
                    z = last.Z;
                }
                else
                {
                    x = input.X;
                    y = input.Y;
                    z = input.Z;
                }
            }
            result = new Vector3(x, y, z);
            return result;
        }

        private void calibrate(float x, float y, float z)
        {
            if (calibrateMinX)
                minX = x;
            if (calibrateMaxX)
                maxX = x;
            if (calibrateMinY)
                minY = y;
            if (calibrateMaxY)
                maxY = y;
            if (calibrateMinZ)
                minZ = z;
            if (calibrateMaxZ)
                maxZ = z;

            calibrateMinX = false;
            calibrateMaxX = false;
            calibrateMinY = false;
            calibrateMaxY = false;
            calibrateMinZ = false;
            calibrateMaxZ = false;
        }



    }
}
