using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using WiiApi;


namespace BG_3D
{
    /// <summary>
    /// Author: Microsoft
    /// Modified by: Oliver Sipos e11627
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameObject terrain = new GameObject();
        GameObject missileLauncherBase = new GameObject();
        GameObject missileLauncherHead = new GameObject();

        const int numMissiles = 20;
        GameObject[] missiles;

        GamePadState previousState;
#if !XBOX
        KeyboardState previousKeyboardState;
#endif

        const float launcherHeadMuzzleOffset = 20.0f;
        const float missilePower = 20.0f;

        AudioEngine audioEngine;
        SoundBank soundBank;
        WaveBank waveBank;

        Random r = new Random();
        const int numEnemyShips = 3;
        GameObject[] enemyShips;

        Vector3 shipMinPosition = new Vector3(-2000.0f, 300.0f, -6000.0f);
        Vector3 shipMaxPosition = new Vector3(2000.0f, 800.0f, -4000.0f);
        const float shipMinVelocity = 5.0f;
        const float shipMaxVelocity = 10.0f;

        Vector3 cameraPosition = new Vector3(0.0f, 60.0f, 160.0f);
        Vector3 cameraLookAt = new Vector3(0.0f, 50.0f, 0.0f);
        Matrix cameraProjectionMatrix;
        Matrix cameraViewMatrix;

        ////// POCETAK DODATOG KODA 
        //koristi se za dobavljanje kontrolera
        WiiFabrika wiiFabrika;
        Kontroler kontroler;       

        //prethodna stanja koja se koriste za finiju navigaciju (manje drhtanja), 
        //jer kontroler vraca razlicite vrednosti akcelerometra u vremenu i pri mirovanju kontrolera.
        //za sto manje drhtanja, povecati broj prethodnih stanja, ali se time dobija vece kasnjenje.
        Vector3[] prethodnaStanja = new Vector3[10];
        //manja vrednost => veca brzina pucanja.
        const int razmakIzmedjuDvaPucnja = 5;
        int brzinaPucanja = razmakIzmedjuDvaPucnja;
        
        //koristi ze za vibraciju prilikom pogotka
        const float trajanjeVibriranja = 10;
        int vibriranje = 0;
        ////// KRAJ DODATOG KODA
      


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            ////// POCETAK DODATOG KODA 
            wiiFabrika = WiiFabrika.dobaviInstancu();
            wiiFabrika.postaviTipKontrolera(WiiTip.WII_KONTROLER);   
            kontroler = wiiFabrika.kreirajKontroler();
            ////// KRAJ DODATOG KODA
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
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            audioEngine = new AudioEngine("Content\\Audio\\TutorialAudio.xgs");
            waveBank = new WaveBank(audioEngine, "Content\\Audio\\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content\\Audio\\Sound Bank.xsb");

            cameraViewMatrix = Matrix.CreateLookAt(
                cameraPosition,
                cameraLookAt,
                Vector3.Up);

            cameraProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45.0f),
                graphics.GraphicsDevice.Viewport.AspectRatio,
                1.0f,
                10000.0f);

            terrain.model = Content.Load<Model>(
                "Models\\terrain");

            missileLauncherBase.model = Content.Load<Model>(
                "Models\\launcher_base");
            missileLauncherBase.scale = 0.2f;

            missileLauncherHead.model = Content.Load<Model>(
                "Models\\launcher_head");
            missileLauncherHead.scale = 0.2f;
            missileLauncherHead.position = missileLauncherBase.position +
                new Vector3(0.0f, 20.0f, 0.0f);

            missiles = new GameObject[numMissiles];
            for (int i = 0; i < numMissiles; i++)
            {
                missiles[i] = new GameObject();
                missiles[i].model =
                    Content.Load<Model>("Models\\missile");
                missiles[i].scale = 3.0f;
            }

            enemyShips = new GameObject[numEnemyShips];
            for (int i = 0; i < numEnemyShips; i++)
            {
                enemyShips[i] = new GameObject();
                enemyShips[i].model = Content.Load<Model>(
                    "Models\\enemy");
                enemyShips[i].scale = 0.1f;
                enemyShips[i].rotation = new Vector3(
                    0.0f, MathHelper.Pi, 0.0f);
            }

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


            KeyboardState keyboardState = Keyboard.GetState();
            if(keyboardState.IsKeyDown(Keys.Left))
            {
                missileLauncherHead.rotation.Y += 0.05f;
            }
            if(keyboardState.IsKeyDown(Keys.Right))
            {
                missileLauncherHead.rotation.Y -= 0.05f;
            }
            if(keyboardState.IsKeyDown(Keys.Up))
            {
                missileLauncherHead.rotation.X += 0.05f;
            }
            if(keyboardState.IsKeyDown(Keys.Down))
            {
                missileLauncherHead.rotation.X -= 0.05f;
            }

            ////// POCETAK DODATOG KODA
            if (kontroler != null)
            { 
                if (kontroler.Stanje.Dugmici.B == true && brzinaPucanja == 5)
                {
                    FireMissile();                    
                }
                //realizacija duzine trajanja vibracije
                if (kontroler.Stanje.Reakcija.VIBRACIJA == true)
                {
                    vibriranje++;
                    if (vibriranje == 10)
                    {
                        vibriranje = 0;
                        kontroler.postaviVibrator(false);
                    }
                }
                //postavljanje novih stanja u niz, i izbacivanje najstarijih u nizu
                for (int i = 1; i < prethodnaStanja.Length; i++)
                {
                    prethodnaStanja[prethodnaStanja.Length - i].X = prethodnaStanja[prethodnaStanja.Length - i - 1].X;
                    prethodnaStanja[prethodnaStanja.Length - i].Y = prethodnaStanja[prethodnaStanja.Length - i - 1].Y;
                    prethodnaStanja[prethodnaStanja.Length - i].Z = prethodnaStanja[prethodnaStanja.Length - i - 1].Z;
                }
                prethodnaStanja[0].X = kontroler.Stanje.Akcelerometar.X;
                prethodnaStanja[0].Y = kontroler.Stanje.Akcelerometar.Y;
                prethodnaStanja[0].Z = kontroler.Stanje.Akcelerometar.Z;

                //Povezivanje vrednosti akcelerometra sa nisanom (topom) 
                missileLauncherHead.rotation.Y = -(float)(srednjaVrednost(prethodnaStanja).X);
                missileLauncherHead.rotation.X = -(float)(srednjaVrednost(prethodnaStanja).Y);
            }           
            
            if (brzinaPucanja-- == 0)
            {
                brzinaPucanja = razmakIzmedjuDvaPucnja;
            }           
            ////// KRAJ DODATOG KODA


            missileLauncherHead.rotation.Y = MathHelper.Clamp(
                missileLauncherHead.rotation.Y,
                -MathHelper.PiOver4, MathHelper.PiOver4);

            missileLauncherHead.rotation.X = MathHelper.Clamp(
                missileLauncherHead.rotation.X,
                0, MathHelper.PiOver4);           

            if(keyboardState.IsKeyDown(Keys.Space) &&
                previousKeyboardState.IsKeyUp(Keys.Space))
            {
                FireMissile();
            }

            UpdateMissiles();
            audioEngine.Update();
            UpdateEnemyShips();          

            previousKeyboardState = keyboardState;


            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        ////// POCETAK DODATOG KODA
        public Vector3 srednjaVrednost(Vector3[] vektori)
        {
            Vector3 akumulator = new Vector3(0, 0, 0);
            for (int i = 0; i < vektori.Length; i++)
            {
                akumulator += vektori[i];
            }
            return akumulator / vektori.Length;
        }
        ////// KRAJ DODATOG KODA

        void FireMissile()
        {
            foreach (GameObject missile in missiles)
            {
                if (!missile.alive)
                {
                    soundBank.PlayCue("missilelaunch");

                    missile.velocity = GetMissileMuzzleVelocity();
                    missile.position = GetMissileMuzzlePosition();
                    missile.rotation = missileLauncherHead.rotation;
                    missile.alive = true;
                    break;
                }
            }
        }

        Vector3 GetMissileMuzzleVelocity()
        {
            Matrix rotationMatrix =
                Matrix.CreateFromYawPitchRoll(
                missileLauncherHead.rotation.Y,
                missileLauncherHead.rotation.X,
                0);

            return Vector3.Normalize(
                Vector3.Transform(Vector3.Forward,
                rotationMatrix)) * missilePower;
        }

        Vector3 GetMissileMuzzlePosition()
        {
            return missileLauncherHead.position +
                (Vector3.Normalize(
                GetMissileMuzzleVelocity()) *
                launcherHeadMuzzleOffset);
        }

        void UpdateMissiles()
        {
            foreach (GameObject missile in missiles)
            {
                if (missile.alive)
                {
                    missile.position += missile.velocity;
                    if (missile.position.Z < -6000.0f)
                    {
                        missile.alive = false;
                    }
                    else
                    {
                        TestCollision(missile);
                    }
                }
            }
        }

        void UpdateEnemyShips()
        {
            foreach (GameObject ship in enemyShips)
            {
                if (ship.alive)
                {
                    ship.position += ship.velocity;
                    if (ship.position.Z > 500.0f)
                    {
                        ship.alive = false;
                    }
                }
                else
                {
                    ship.alive = true;
                    ship.position = new Vector3(
                        MathHelper.Lerp(
                        shipMinPosition.X,
                        shipMaxPosition.X,
                        (float)r.NextDouble()),

                        MathHelper.Lerp(
                        shipMinPosition.Y,
                        shipMaxPosition.Y,
                        (float)r.NextDouble()),

                        MathHelper.Lerp(
                        shipMinPosition.Z,
                        shipMaxPosition.Z,
                        (float)r.NextDouble()));

                    ship.velocity = new Vector3(
                        0.0f,
                        0.0f,
                        MathHelper.Lerp(shipMinVelocity,
                        shipMaxVelocity, (float)r.NextDouble()));
                }
            }
        }

        void TestCollision(GameObject missile)
        {
            BoundingSphere missilesphere =
                missile.model.Meshes[0].BoundingSphere;
            missilesphere.Center = missile.position;
            missilesphere.Radius *= missile.scale;

            foreach (GameObject ship in enemyShips)
            {
                if (ship.alive)
                {
                    BoundingSphere shipsphere =
                        ship.model.Meshes[0].BoundingSphere;
                    shipsphere.Center = ship.position;
                    shipsphere.Radius *= ship.scale;

                    if (shipsphere.Intersects(missilesphere))
                    {
                        ////// POCETAK DODATOG KODA
                        if(kontroler !=null)
                            kontroler.postaviVibrator(true);
                        ////// KRAJ DODATOG KODA

                        soundBank.PlayCue("explosion");
                        missile.alive = false;
                        ship.alive = false;
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            DrawGameObject(terrain);
            DrawGameObject(missileLauncherBase);
            DrawGameObject(missileLauncherHead);

            foreach (GameObject missile in missiles)
            {
                if (missile.alive)
                {
                    DrawGameObject(missile);
                }
            }

            foreach (GameObject enemyship in enemyShips)
            {
                if (enemyship.alive)
                {
                    DrawGameObject(enemyship);
                }
            }

            // TODO: Add your drawing code here

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

                    effect.Projection = cameraProjectionMatrix;
                    effect.View = cameraViewMatrix;
                }
                mesh.Draw();
            }
        }
    }
}
