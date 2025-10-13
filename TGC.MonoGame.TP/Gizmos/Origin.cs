using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.Samples.Geometries
{
    public class Origin
    {
        private Arrow _xArrow;
        private Arrow _yArrow;
        private Arrow _zArrow;

        public Origin(GraphicsDevice device)
        {
            _xArrow = new Arrow(device)
            {
                FromPosition = Vector3.Zero,
                ToPosition = Vector3.Right * 10,
                BodyColor = Color.Red,
                HeadColor = Color.Orange,
            };
            _xArrow.UpdateValues();

            _yArrow = new Arrow(device)
            {
                FromPosition = Vector3.Zero,
                ToPosition = Vector3.Up * 10,
                BodyColor = Color.Green,
                HeadColor = Color.LightGreen,
            };
            _yArrow.UpdateValues();

            _zArrow = new Arrow(device)
            {
                FromPosition = Vector3.Zero,
                ToPosition = Vector3.Backward * 10,
                BodyColor = Color.Blue,
                HeadColor = Color.LightBlue,
            };
            _zArrow.UpdateValues();
        }

        public void Draw(Matrix view, Matrix projection)
        {
            _xArrow.Draw(Matrix.Identity, view, projection);
            _yArrow.Draw(Matrix.Identity, view, projection);
            _zArrow.Draw(Matrix.Identity, view, projection);
        }
    }
}