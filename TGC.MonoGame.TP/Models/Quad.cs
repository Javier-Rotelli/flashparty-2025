using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP
{
    /// <summary>
    ///     Represents a heightmap model, which is a 3D terrain generated from a grayscale image where pixel brightness
    ///     corresponds to height.
    /// </summary>
    public class Quad
    {
        private readonly GraphicsDevice _graphicsDevice;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private Vector3 Position;
        private int _lenght = 800;
        private int _repetitions = 14;

        private int _offset = 0;
        private readonly int _primitiveCount;

        public Quad(GraphicsDevice graphicsDevice, int width, int length)
        {
            _graphicsDevice = graphicsDevice;
            CreateBuffers(width, length);
            _primitiveCount = 2;
        }



        /// <summary>
        ///     Create a Vertex Buffer from a HeightMap.
        /// </summary>
        private void CreateBuffers(int width, int length)
        {
            var offsetX = width * 0.5f;
            var offsetY = length * 0.5f;
            var vertexCount = 4;

            // Create temporary array of vertices.
            var vertices = new VertexPositionTexture[4];
            vertices[0].Position = new Vector3(-offsetX, -offsetY, 0f);
            vertices[0].TextureCoordinate = new Vector2(0f, 1f);

            vertices[1].Position = new Vector3(-offsetX, offsetY, 0f);
            vertices[1].TextureCoordinate = new Vector2(0f, 0f);

            vertices[2].Position = new Vector3(offsetX, -offsetY, 0f);
            vertices[2].TextureCoordinate = new Vector2(1f, 1f);

            vertices[3].Position = new Vector3(offsetX, offsetY, 0f);
            vertices[3].TextureCoordinate = new Vector2(1f, 0f);

            // Create the actual vertex buffer.
            _vertexBuffer = new VertexBuffer(_graphicsDevice, VertexPositionTexture.VertexDeclaration,
                vertexCount, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices);

            var indices = new ushort[6];

            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 3;
            indices[3] = 0;
            indices[4] = 3;
            indices[5] = 2;

            _indexBuffer = new IndexBuffer(_graphicsDevice, IndexElementSize.SixteenBits, 6, BufferUsage.WriteOnly);
            _indexBuffer.SetData(indices);
        }

        public void Update(GameTime gameTime, float speed)
        {
            float newZ = Position.Z - speed * gameTime.ElapsedGameTime.Milliseconds / 1000f;
            if (-newZ > _lenght)
            {
                newZ += _lenght;
                _offset++;
            }
            Position = new Vector3(Position.X, Position.Y, newZ);
        }


        public void Draw(Effect effect, GameTime gameTime)
        {

            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;
            var oldBlendState = _graphicsDevice.BlendState;
            _graphicsDevice.BlendState = BlendState.AlphaBlend;
            effect.Parameters["Time"]?.SetValue((float)gameTime.TotalGameTime.TotalSeconds);

            for (int i = _repetitions - 1; i >= 0; i--)
            {
                effect.Parameters["i"]?.SetValue(i + _offset);

                var totalOffset = Position.Z + i * _lenght;
                Matrix translation = Matrix.CreateTranslation(0, 0, totalOffset);
                Draw(effect, translation);
            }

            _graphicsDevice.BlendState = oldBlendState;
        }

        private void Draw(Effect effect, Matrix world)
        {
            effect.Parameters["World"].SetValue(world);
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _primitiveCount);
            }
        }
    }
}