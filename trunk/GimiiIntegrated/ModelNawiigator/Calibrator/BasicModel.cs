using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caliibrator
{
    /// <summary>
    /// Bazicna klasa sa modele kalibratora. 
    /// </summary>
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
            //Bazna klasa ne radi nista sto je vremenski zavisno
        }

        public void Draw(Camera camera)
        {
            //Postavi matricu transformacija za sve povezane elemente modela
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            //Za svaki mesh u modelu
            foreach (ModelMesh mesh in model.Meshes)
            {
                //Za svaki efekat u efektima tog mesh-a
                foreach (BasicEffect be in mesh.Effects)
                {
                    //Postavi opcije efekta koje su neophodne, to ukljucuje:
                    //Teksturu, ako je namestena
                    if (texture != null)
                    {
                        be.Texture = texture;
                        be.TextureEnabled = true;
                    }
                    //Podrazumevano osvetljenje
                    be.EnableDefaultLighting();
                    //Projekciju i pogled kamere
                    be.Projection = camera.projection;
                    be.View = camera.view;
                    //Postaraj se da artikulacija radi
                    be.World = GetWorld() * mesh.ParentBone.Transform;
                }
                //iscrtaj mesh
                mesh.Draw();
            }
        }

        public virtual Matrix GetWorld()
        {
            return world;
        }
    }
}
