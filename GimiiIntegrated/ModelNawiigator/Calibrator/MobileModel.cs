using System;
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

namespace XNAExample
{
    class MobileModel : BasicModel
    {
        private static int count = 1;
        private int id;
        private static int selected = 1;
        public Vector3 pos { get; set; }

        public MobileModel(Model m, Game g)
            : base(m, g)
        {
            pos = Vector3.Zero;
            id = count;
            count++;
        }

        public override void Update(GameTime gameTime)
        {
            if (game.wiimode)
            {
                if (id == 1)
                {
                    pos = game.wiipos1;
                }
                else if (id == 2)
                {
                    pos = game.wiipos2;
                }
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.D0 + id))
                {
                    selected = id;
                }
                if (selected == id)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    {
                        pos = new Vector3(pos.X - 1, pos.Y, pos.Z);
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    {
                        pos = new Vector3(pos.X + 1, pos.Y, pos.Z);
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    {
                        pos = new Vector3(pos.X, pos.Y + 1, pos.Z);
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    {
                        pos = new Vector3(pos.X, pos.Y - 1, pos.Z);
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.PageUp))
                    {
                        pos = new Vector3(pos.X, pos.Y, pos.Z - 1);
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.PageDown))
                    {
                        pos = new Vector3(pos.X, pos.Y, pos.Z + 1);
                    }
                }
            }
        }

        public override Matrix GetWorld()
        {
            return world * Matrix.CreateTranslation(pos) * Matrix.CreateRotationX(-(float)Math.PI / 2) * Matrix.CreateScale(0.5f);
        }
    }
}
