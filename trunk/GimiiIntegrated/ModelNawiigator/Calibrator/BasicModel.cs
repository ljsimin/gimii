using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caliibrator
{
    class BasicModel
    {
        public Model model { get; protected set; }
        public Texture2D texture { get; set; }
        protected Matrix world = Matrix.Identity;
        protected CalibrationMain game; 

        public BasicModel(Model m, Game g)
        {
            game = (CalibrationMain)g;
            model = m;
        }

        public virtual void Update(GameTime gameTime)
        {
            //Base class does nothing here
        }

        public void Draw(Camera camera)
        {
            //Set transfroms
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            //Loop through meshes and their effects 
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    //Set BasicEffect information
                    if (texture != null)
                    {
                        be.Texture = texture;
                        be.TextureEnabled = true;
                    }
                    be.EnableDefaultLighting();
                    be.Projection = camera.projection;
                    be.View = camera.view;
                    be.World = GetWorld() * mesh.ParentBone.Transform;
                }
                //Draw
                mesh.Draw();
            }
        }

        public virtual Matrix GetWorld()
        {
            return world;
        }
    }
}
