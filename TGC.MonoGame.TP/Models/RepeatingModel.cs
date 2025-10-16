using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Models
{
    /// <summary>
    ///     Represents a 3D model with its associated texture.
    /// </summary>

    public class Rails : SimpleModel
    {

        static readonly Vector3[] colors =
        [
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0,1,0),
            new Vector3(0,1,0),
            new Vector3(0,1,0),
            new Vector3(0,0,1),
            new Vector3(0,0,1),
            new Vector3(0,0,1),
        ];

        private readonly float _lenght = 400;
        private readonly int _repetitions = 25;
        public Rails(Effect effect) : base(effect)
        {
        }

        public Rails(Model mesh, Effect effect) : base(mesh, effect)
        {
        }

        public Rails(ContentManager content, string modelPath, Effect effect) : base(content, modelPath, effect)
        {
        }

        public void Update(GameTime gameTime, float speed)
        {
            float newZ = Position.Z - speed * gameTime.ElapsedGameTime.Milliseconds / 1000f;
            if (-newZ > _lenght * _repetitions)
            {
                newZ += _lenght * _repetitions;
            }
            Position = new Vector3(Position.X, Position.Y, newZ);
            base.Update(gameTime);

        }

        public new void Draw()
        {
            for (int i = 0; i < _repetitions; i++)
            {
                var color = colors[i % colors.Length];
                _effect.Parameters["DiffuseColor"]?.SetValue(color);

                var totalOffset = Position.Z + i * _lenght;
                if (totalOffset + 400 < 0)
                {
                    totalOffset += _lenght * _repetitions;
                }
                Matrix translation = Matrix.CreateTranslation(0, 0, totalOffset);
                base.Draw(translation);
            }
        }
    }
}
