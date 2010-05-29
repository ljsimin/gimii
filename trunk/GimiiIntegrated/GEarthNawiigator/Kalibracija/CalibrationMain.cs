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
using WiiApi.Tracking3d;
using WiiApi;

namespace Caliibrator
{
    /// <summary>
    /// Glavna klasa kalibratora.
    /// </summary>
    public class CalibrationMain : Microsoft.Xna.Framework.Game
    {
        //Osnovne klase, za iscrtavanje
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        SpriteFont helpFont;

        public Camera camera { get; protected set; }
        ModelManager modelManager;
        Grid grid;

        private String message = ""; //sta se iscrtava na ekranu
        private String helpMessage = @"
                            HELP               
                * W => povezi wii kontroler
                * K => raskini vezu sa kontrolerom
                * + => povecaj prag filtriranja za 1
                * - => smani prag filtriranja za 1
                * F => ukljuci filter
                * U => iskljuci filter
                * C => ukljuci postavljenu kalibraciju
                * R => resetuj kalibraciju
                * Levo, desno, gore, dole => postavi levu, 
                * desnu, donju odn. gornju granicu na 
                * trenutni polozaj markera
                * Page up, page down => postavlja najudaljeniju 
                * odn. najblizu granicu na trenutni polozaj ";//pomoc

        public Kontroler wLeft;
        public Kontroler wRight;
        public Tracker3d tracker;

        public bool connected {get; set;}
        public bool wiimode { get; set; }

        public Vector3 wiipos1 { get; set; }
        public Vector3 wiipos2 { get; set; }

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

        private bool help = false;

        private delegate void Obrada(ParametriDogadjaja p);

        private bool dofilter = false;

        

        public CalibrationMain()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            connected = false;
            //tw = new StreamWriter("save.txt");
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

            grid = new Grid(this, camera);
            grid.Visible = true;
            grid.GridX = 256;
            grid.GridY = 256;
            grid.Spacing = 32.0f;
            grid.Position = new Vector3(0, -100, -25);
            grid.Rotation = Vector3.Zero;
            Components.Add(grid);

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
            helpFont = Content.Load<SpriteFont>(@"fonts\helpfont");

            // TODO: use this.Content to load your game content here
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

            /*
                * W => povezi wii kontroler
                * K => raskini vezu sa kontrolerom
                * + => povecaj prag filtriranja za 1
                * - => smani prag filtriranja za 1
                * F => ukljuci filter
                * U => iskljuci filter
                * C => ukljuci postavljenu kalibraciju
                * R => resetuj kalibraciju
                * Levo, desno, gore, dole => postavi levu, desnu, donju 
                * odn. gornju granicu na trenutni polozaj markera
                * Page up, page down => postavlja najudaljeniju 
                * odn. najblizu granicu na trenutni polozaj 
             */

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
                tracker.FILTERING_THRESHOLD += 1.0f;

            }

            if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
            {
                tracker.FILTERING_THRESHOLD -= 1.0f;
                if (tracker.FILTERING_THRESHOLD < 0)
                {
                    tracker.FILTERING_THRESHOLD = 0;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                dofilter = true;
                tracker.clearHistory();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.U))
            {
                dofilter = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                //SAVE CALIBRATION DATA
                FileStream f = File.Open("save.txt", FileMode.Create);
                TextWriter tw = new StreamWriter(f);
                tw.WriteLine(minX);
                tw.WriteLine(maxX);
                tw.WriteLine(minY);
                tw.WriteLine(maxY);
                tw.WriteLine(minZ);
                tw.WriteLine(maxZ);
                tw.Close();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.L))
            {
                TextReader tr = new StreamReader("save.txt");
                String s = tr.ReadLine();
                minX = float.Parse(s);
                s = tr.ReadLine();
                maxX = float.Parse(s);
                s = tr.ReadLine();
                minY = float.Parse(s);
                s = tr.ReadLine();
                maxY = float.Parse(s);
                s = tr.ReadLine();
                minZ = float.Parse(s);
                s = tr.ReadLine();
                maxZ = float.Parse(s);
                tr.Close();
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

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                help = true;
            }
            else
            {
                help = false;
            }


            message = "";

            if (wiimode) calculate();

            message += (wiimode) ? "Mode: Wiimote" : "Mode: Keyboard";
            message += "\n";
            message += (calibrated) ? "Calibrated: Yes" : "Calibrated: No";
            message += "\n";
            message += (dofilter) ? "Filtered: Yes" : "Filtered: No";
            message += "\n";
            message += "Threshold: " + Math.Round((tracker == null) ? 12.0f : tracker.FILTERING_THRESHOLD, 2);
            message += "\n";


            base.Update(gameTime);
        }
        private void connect()
        {
            if (wLeft == null || wRight == null)
            {
                if (wLeft != null) WiiFabrika.dobaviInstancu().iskljuci(wLeft);
                if (wRight != null) WiiFabrika.dobaviInstancu().iskljuci(wRight);
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
            tracker = new Tracker3d(wLeft, wRight);
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
            if (help)
            {
                Vector2 size = helpFont.MeasureString(helpMessage);
                //Vector2 position = new Vector2(Window.ClientBounds.Width / 2 - size.X / 2, Window.ClientBounds.Height / 2 - size.Y / 2);
                Vector2 position = new Vector2(20, 20);
                spriteBatch.DrawString(helpFont, helpMessage, position, Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                spriteBatch.End();
                return;
            }
            else
            {
                spriteBatch.DrawString(font, message, new Vector2(10, 10), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            }
            spriteBatch.End();
            GraphicsDevice.RenderState.DepthBufferEnable = true;
            GraphicsDevice.PresentationParameters.EnableAutoDepthStencil = true;
            base.Draw(gameTime);
        }


        private void calculate()
        {
            if (tracker.found(0))
            {
                tracker.Calibrated = false;
                tracker.Filtered = false;
                tracker.Level = TrackerLevel.RAW;
                Vector3 raw = convert(tracker.getPosition(0));
                calibrate(raw.X, raw.Y, raw.Z);
                tracker.Calibrated = calibrated;
                tracker.Filtered = dofilter;
                tracker.Level = TrackerLevel.COOKED;
                if (calibrated)
                {
                    tracker.calibrate(minX, maxX, minY, maxY, minZ, maxZ);
                }
                wiipos1 = convert(tracker.getPosition(0));
            }
            if (tracker.found(1))
            {
                tracker.Calibrated = calibrated;
                tracker.Filtered = dofilter;
                tracker.Level = TrackerLevel.COOKED;
                if (calibrated)
                {
                    tracker.calibrate(minX, maxX, minY, maxY, minZ, maxZ);
                }
                wiipos2 = convert(tracker.getPosition(1));
            }
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

        private Vector3 convert(Vector x)
        {
            return new Vector3(x.X, x.Y, x.Z);
        }

    }
}
