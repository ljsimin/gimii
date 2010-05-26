using System;
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

namespace ModelNawiigator
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class NawiigatorMain : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont helpFont;

        public OrbitCamera camera { get; protected set; }
        ModelManager modelManager;

        private bool help = false;
        private string helpMessage = @"
                                HELP               
               * W => povezi wii kontroler
               * K => raskini vezu sa kontrolerom
               * + => povecaj prag filtriranja za 1
               * - => smani prag filtriranja za 1
               * F => ukljuci filter
               * U => iskljuci filter
               * Strelice i Page Up/Down => rotacija
               * Numpad => Translacija
               * Home/End => Zoom in/out 
        ";

        private Vector3 cMin;
        private Vector3 cMax;

        public PositionCalculator pc
        {
            get;
            protected set;
        }

        public NawiigatorMain(Vector3 min, Vector3 max, Kontroler wLeft, Kontroler wRight)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            cMin = min;
            cMax = max;
            pc = new PositionCalculator(this, min, max, wLeft, wRight);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Vector3 pos = new Vector3(0, 0, 6000);
            Vector3 refer = new Vector3(0, 0, -6000);

            camera = new OrbitCamera(this, pos, -pos, Vector3.Up);
            Components.Add(camera);

            modelManager = new ModelManager(this);
            Components.Add(modelManager);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            helpFont = Content.Load<SpriteFont>(@"fonts\helpfont");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
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
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                help = true;
            }
            else
            {
                help = false;
            }

            pc.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);
            spriteBatch.Begin();
            if (help)
            {
                Vector2 size = helpFont.MeasureString(helpMessage);
                //Vector2 position = new Vector2(Window.ClientBounds.Width / 2 - size.X / 2, Window.ClientBounds.Height / 2 - size.Y / 2);
                Vector2 position = new Vector2(20, 20);
                spriteBatch.DrawString(helpFont, helpMessage, position, Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                spriteBatch.End();
                return;
            }
            spriteBatch.End();


            GraphicsDevice.RenderState.DepthBufferEnable = true;
            GraphicsDevice.PresentationParameters.EnableAutoDepthStencil = true;

            base.Draw(gameTime);
        }
    }
}
