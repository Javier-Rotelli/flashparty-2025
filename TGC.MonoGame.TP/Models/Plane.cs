using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP
{
    /// <summary>
    ///     Represents a heightmap model, which is a 3D terrain generated from a grayscale image where pixel brightness
    ///     corresponds to height.
    /// </summary>
    public class Plane
    {
        private readonly GraphicsDevice _graphicsDevice;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private readonly int _primitiveCount;

        public Plane(GraphicsDevice graphicsDevice, int width, int length, int sizeScale = 1)
        {
            _graphicsDevice = graphicsDevice;
            CreateVertexBuffer(width, length, sizeScale);
            CreateIndexBuffer(width - 1, length - 1);
            _primitiveCount = 2 * (width - 1) * (length - 1);
        }



        /// <summary>
        ///     Create a Vertex Buffer from a HeightMap.
        /// </summary>
        /// <param name="heightMap">The Heightmap which specifies height for each vertex</param>
        /// <param name="scaleXZ">The distance between the vertices in both the X and Z axis</param>
        /// <param name="scaleY">The scale in the Y axis for the vertices of the HeightMap</param>
        private void CreateVertexBuffer(int width, int length, float scaleXZ)
        {
            var offsetX = width * scaleXZ * 0.5f;
            var offsetZ = length * scaleXZ * 0.5f;

            // Amount of subdivisions in X times amount of subdivisions in Z.
            var vertexCount = width * length;

            // Create temporary array of vertices.
            var vertices = new VertexPositionNormalTexture[vertexCount];

            var index = 0;

            for (var x = 0; x < width; x++)
            {
                for (var z = 0; z < length; z++)
                {
                    var position = new Vector3(x * scaleXZ - offsetX, 0, z * scaleXZ - offsetZ);
                    var textureCoordinates = new Vector2((float)x / width, (float)z / length);
                    var normal = Vector3.Up;
                    vertices[index] = new VertexPositionNormalTexture(position, normal, textureCoordinates);
                    index++;
                }
            }

            // Create the actual vertex buffer.
            _vertexBuffer = new VertexBuffer(_graphicsDevice, VertexPositionNormalTexture.VertexDeclaration,
                vertexCount, BufferUsage.None);
            _vertexBuffer.SetData(vertices);
        }

        private void CreateIndexBuffer(int quadsInX, int quadsInZ)
        {
            var indexCount = 3 * 2 * quadsInX * quadsInZ;

            var indices = new ushort[indexCount];
            var index = 0;

            var vertexCountX = quadsInX + 1;
            for (var x = 0; x < quadsInX; x++)
            {
                for (var z = 0; z < quadsInZ; z++)
                {
                    var right = x + 1;
                    var bottom = z * vertexCountX;
                    var top = (z + 1) * vertexCountX;

                    //  d __ c  
                    //   | /|
                    //   |/_|
                    //  a    b

                    var a = (ushort)(x + bottom);
                    var b = (ushort)(right + bottom);
                    var c = (ushort)(right + top);
                    var d = (ushort)(x + top);

                    // ACB
                    indices[index] = a;
                    index++;
                    indices[index] = c;
                    index++;
                    indices[index] = b;
                    index++;

                    // ADC
                    indices[index] = a;
                    index++;
                    indices[index] = d;
                    index++;
                    indices[index] = c;
                    index++;
                }
            }

            _indexBuffer =
                new IndexBuffer(_graphicsDevice, IndexElementSize.SixteenBits, indexCount, BufferUsage.None);
            _indexBuffer.SetData(indices);
        }


        public void Draw(Effect effect)
        {
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;
            effect.Parameters["World"].SetValue(Matrix.Identity);
            effect.Parameters["InverseTransposeWorld"]?.SetValue(Matrix.Transpose(Matrix.Invert(Matrix.Identity)));

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _primitiveCount);
            }
        }
    }
}