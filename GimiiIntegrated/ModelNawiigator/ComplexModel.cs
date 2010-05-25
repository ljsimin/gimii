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

namespace ModelNawiigator
{
    class ComplexModel : BasicModel
    {
        public Vector3 pos { get; set; }
        public ComplexModel(Model m, Game g) : base(m, g)
        {
            pos = Vector3.Zero;
        }

        public override Matrix GetWorld()
        {
            return world * Matrix.CreateTranslation(pos) * Matrix.CreateScale(1.0f);
        }

        public override void Draw(OrbitCamera camera)
        {
            //Set transfroms
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            //Loop through meshes and their effects 
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {                    
                    be.EnableDefaultLighting();
                    be.Projection = camera.projection;
                    be.View = camera.view;
                    be.World = mesh.ParentBone.Transform * GetWorld();
                }
                //Draw
                mesh.Draw();
            }
        }
    }
}
