﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace ModelNawiigator
{
    public class OrbitCamera : Microsoft.Xna.Framework.GameComponent
    {
        //Camera matrices
        public Matrix view
        {
            get
            {
                return Matrix.CreateLookAt(cameraPosition, cameraTarget, cameraUp);
            }
            protected set
            {

            }
        }
        public Matrix projection
        {
            get
            {
                return Matrix.CreatePerspectiveFieldOfView(cameraFov,
                    (float)Game.Window.ClientBounds.Width /
                    (float)Game.Window.ClientBounds.Height,
                    1, 28000);
            }
            protected set
            {
            }
        }

        private float cameraFov;
        private Vector3 cameraPosition;
        private Vector3 cameraTarget;
        private Vector3 cameraUp;

        private Vector3 cameraPosition_orig;
        private Vector3 cameraTarget_orig;
        private Vector3 cameraUp_orig;

        private Vector3 pomeraj = Vector3.Zero;


        private Vector3 angle = Vector3.Zero;

        public OrbitCamera(Game game, Vector3 pos, Vector3 reference, Vector3 up)
            : base(game)
        {
            cameraFov = MathHelper.PiOver4;
            cameraPosition = pos;
            cameraUp = up;
            cameraTarget = cameraPosition + reference;

            cameraPosition_orig = pos;
            cameraUp_orig = up;
            cameraTarget_orig = cameraPosition + reference;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            KeyboardState kState = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(kState.IsKeyDown(Keys.Down))
            {
                angle += Vector3.UnitX;
            }

            if (kState.IsKeyDown(Keys.Up))
            {
                angle -= Vector3.UnitX;
            }


            if (kState.IsKeyDown(Keys.Right))
            {
                angle += Vector3.UnitY;
            }

            if (kState.IsKeyDown(Keys.Left))
            {
                angle -= Vector3.UnitY;
            }

            if (kState.IsKeyDown(Keys.PageDown))
            {
                angle += Vector3.UnitZ;
            }

            if (kState.IsKeyDown(Keys.PageUp))
            {
                angle -= Vector3.UnitZ;
            }

            if (kState.IsKeyDown(Keys.Add))
            {
                cameraFov -= MathHelper.ToRadians(1);
                if (MathHelper.ToDegrees(cameraFov) < 1.0f)
                {
                    cameraFov = MathHelper.ToRadians(1.0f);
                }
            }

            if (kState.IsKeyDown(Keys.Subtract))
            {
                cameraFov += MathHelper.ToRadians(1);
                if (MathHelper.ToDegrees(cameraFov) > 90.0f)
                {
                    cameraFov = MathHelper.ToRadians(90.0f);
                }
            }


            /*if (kState.IsKeyDown(Keys.W))
            {
                pomeraj -= 363 * Vector3.Transform(Vector3.UnitZ, rot);
                //cameraUp -= Vector3.UnitZ;
            }

            if (kState.IsKeyDown(Keys.S))
            {
                pomeraj += 363 * Vector3.Transform(Vector3.UnitZ, rot);
            }

            if (kState.IsKeyDown(Keys.A))
            {
                pomeraj -= 363 * Vector3.Transform(Vector3.UnitX, rot);
            }

            if (kState.IsKeyDown(Keys.D))
            {
                pomeraj += 363 * Vector3.Transform(Vector3.UnitX, rot);
            }

            if (kState.IsKeyDown(Keys.F))
            {
                pomeraj -= 363 * Vector3.Transform(Vector3.UnitY, rot);
            }

            if (kState.IsKeyDown(Keys.R))
            {
                pomeraj += 363 * Vector3.Transform(Vector3.UnitY, rot);
            }*/



            Vector3 r = ((Game1)(this.Game)).pc.getRotate();
            angle.X += MathHelper.ToRadians(r.X * 20); 
            angle.Y += MathHelper.ToRadians(r.Y * 20);
            angle.Z += MathHelper.ToRadians(r.Z * 20);

            Matrix rot = Matrix.CreateRotationX(MathHelper.ToRadians(angle.X)) * Matrix.CreateRotationY(MathHelper.ToRadians(angle.Y)) * Matrix.CreateRotationZ(MathHelper.ToRadians(angle.Z));

            Vector3 t = ((Game1)(this.Game)).pc.getTranslation();

            pomeraj += Vector3.Transform(new Vector3(0, 0, t.Z), rot);
            pomeraj += Vector3.Transform(new Vector3(t.X, 0, 0), rot);
            pomeraj += Vector3.Transform(new Vector3(0, t.Y, 0), rot);

            cameraFov += MathHelper.ToRadians(((Game1)(this.Game)).pc.getScale());
            
            if (MathHelper.ToDegrees(cameraFov) > 90.0f)
            {
                cameraFov = MathHelper.ToRadians(90.0f);
            }
            if (MathHelper.ToDegrees(cameraFov) < 1.0f)
            {
                cameraFov = MathHelper.ToRadians(1.0f);
            }


            //Matrix tran = Matrix.CreateTranslation(pomeraj) * Matrix.CreateRotationX(MathHelper.ToRadians(angle.X)) * Matrix.CreateRotationY(MathHelper.ToRadians(angle.Y)) * Matrix.CreateRotationZ(MathHelper.ToRadians(angle.Z));
            Matrix tran = Matrix.CreateRotationX(MathHelper.ToRadians(angle.X)) * Matrix.CreateRotationY(MathHelper.ToRadians(angle.Y)) * Matrix.CreateRotationZ(MathHelper.ToRadians(angle.Z)) * Matrix.CreateTranslation(pomeraj);
            cameraUp = Vector3.Transform(cameraUp_orig, rot);
            cameraPosition = Vector3.Transform(cameraPosition_orig, tran);
            cameraTarget = Vector3.Transform(cameraTarget_orig, tran);


            /*
             * 
             *  Matrix rot = Matrix.CreateRotationX(MathHelper.ToRadians(-0.1f * (float)deltaTime));
                Vector3 q = (cameraTarget - cameraPosition);
                cameraPosition = cameraTarget - Vector3.Transform(q, rot) + cameraPosition;
             * 
             */
        }
    }
}