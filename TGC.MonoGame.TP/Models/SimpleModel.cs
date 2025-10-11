using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Models
{
    /// <summary>
    ///     Represents a 3D model with its associated texture.
    /// </summary>
    public class SimpleModel
    {
        protected Model _model;
        protected readonly Effect _effect;

        public Texture2D Texture { get; set; }

        public Matrix World { get; set; }

        public SimpleModel(Effect effect)
        {
            _effect = effect;
            World = Matrix.Identity;
        }
        public SimpleModel(ContentManager content, string modelPath, Effect effect) : this(effect)
        {
            _model = content.Load<Model>(modelPath);
            foreach (var mesh in _model.Meshes)
            {
                // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = _effect;
                }
            }
        }

        public SimpleModel(Model mesh, Effect effect) : this(effect)
        {
            _model = mesh;
        }
        public void Update(GameTime gameTime)
        {

        }

        public void Draw()
        {
            foreach (var mesh in _model.Meshes)
            {
                _effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * World);
                mesh.Draw();
            }
        }
    }
}
