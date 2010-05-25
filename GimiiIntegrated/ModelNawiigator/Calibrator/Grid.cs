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


namespace Caliibrator
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Grid : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public Camera camera
        {
            get;
            set;
        }


        VertexPositionColor[] nonIndexedLines;

        private Matrix world;
        private Matrix view;
        private Matrix projection;

        private GraphicsDevice graphics;
        private BasicEffect effect;
        private VertexDeclaration decl;
        private int numlines;


        #region Properties
        /// <summary>
        /// Properties that can be set on the component by the user. Our aim is to allow
        /// enough control of the grid from the design view so that users don't have to mess
        /// with the component code directly
        /// </summary>
        private bool show = true;
        private int width = 32;
        private int height = 32;
        private float unitSize = 1.0f;

        public int GridX
        {
            get { return width; }
            set { width = value; }
        }

        public int GridY
        {
            get { return height; }
            set { height = value; }
        }

        public bool Visible
        {
            get { return show; }
            set { show = value; }
        }

        public float Spacing
        {
            get { return unitSize; }
            set { unitSize = value; }
        }

        public Vector3 Position
        {
            get;
            set;
        }
        public Vector3 Rotation
        {
            get;
            set;
        }
        #endregion

        public Grid(Game game, Camera c)
            : base(game)
        {
            this.camera = c;
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            graphics = Game.GraphicsDevice;
            effect = new BasicEffect(graphics, null);
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            world = Matrix.CreateTranslation(Position);
            if (Rotation.X != 0)
            {
                world *= Matrix.CreateRotationX(Rotation.X);
            }
            else if (Rotation.Y != 0)
            {
                world *= Matrix.CreateRotationY(Rotation.Y);
            }
            else if (Rotation.Z != 0)
            {
                world *= Matrix.CreateRotationZ(Rotation.Z);
            }
            CreateGrid();
            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            // Only draw the grid if we have it enabled
            if (!show)
                return;

            // draw the grid to the screen

            // for our view matrix we place the camera slightly above the origin and slightly back in the z direction
            // we are looking at the origin with negative z going into the screen
            view = camera.view;

            // A standard projection matrix (change this to suit)
            projection = camera.projection;

            // Begin our scene (we assume that the game component has checked for valid devices
            //graphics.GraphicsDevice.BeginScene();

            graphics.VertexDeclaration = decl;
            graphics.RenderState.CullMode = CullMode.None;

            // Render using a basic effect
            effect.Begin();
            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            // apply some lighting in our case we have a grey ambient colour
            // but this can be changed to render the grid in any colour you choose
            effect.Alpha = 1.0f;
            effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
            effect.AmbientLightColor = new Vector3(0.0f, 0.0f, 0.5f);

            effect.LightingEnabled = true;
            effect.CommitChanges();

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, nonIndexedLines, 0, numlines);                                       
                pass.End();
            }

            effect.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Create the grid . It can be any x or y size and does not need to be a square
        /// </summary>
        private void CreateGrid()
        {
            nonIndexedLines = new VertexPositionColor[((width + height) * 2) + 4];
            int counter = 0;

            float xstep = unitSize;
            float ystep = unitSize;

            float startxgrid = (-1f * (xstep * (float)width)) / 2.0f;
            float lineXLength = xstep * width;

            float startygrid = (-1f * (ystep * (float)height)) / 2.0f;
            float lineYLength = ystep * height;

            float currentpos = startygrid;

            for (int counti = 0; counti <= height; counti++)
            {
                nonIndexedLines[counter] = new VertexPositionColor(new Vector3(startxgrid, 0f, currentpos), Color.Cyan);
                nonIndexedLines[counter + 1] = new VertexPositionColor(new Vector3(startxgrid + lineXLength, 0f, currentpos), Color.Cyan);
                currentpos += xstep;
                counter += 2;
            }

            currentpos = startxgrid;

            for (int countj = 0; countj <= width; countj++)
            {
                nonIndexedLines[counter] = new VertexPositionColor(new Vector3(currentpos, 0f, startygrid), Color.Cyan);
                nonIndexedLines[counter + 1] = new VertexPositionColor(new Vector3(currentpos, 0f, startygrid + lineYLength), Color.Cyan);
                currentpos += ystep;
                counter += 2;
            }

            decl = new VertexDeclaration(graphics, VertexPositionColor.VertexElements);
            numlines = width + height + 2;
        }

    }
}