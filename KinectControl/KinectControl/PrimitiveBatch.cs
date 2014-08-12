#region File Description
//-----------------------------------------------------------------------------
// PrimitiveBatch.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KinectControl.Common;
#endregion

namespace KinectControl
{
    // PrimitiveBatch is a class that handles efficient rendering automatically for its
    // users, in a similar way to SpriteBatch. PrimitiveBatch can render lines, points,
    // and triangles to the screen. In this sample, it is used to draw a spacewars
    // retro scene.
    public class PrimitiveBatch : IDisposable
    {
        #region Constants and Fields

        // this constant controls how large the vertices buffer is. Larger buffers will
        // require flushing less often, which can increase performance. However, having
        // buffer that is unnecessarily large will waste memory.
        const int DefaultBufferSize = 500;

        const float HalfMaxDepth = 500f;

        // a block of vertices that calling AddVertex will fill. Flush will draw using
        // this array, and will determine how many primitives to draw from
        // positionInBuffer.
        VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[DefaultBufferSize];

        // keeps track of how many vertices have been added. this value increases until
        // we run out of space in the buffer, at which time Flush is automatically
        // called.
        int positionInBuffer = 0;

        // a basic effect, which contains the shaders that we will use to draw our
        // primitives.
        BasicEffect basicEffect;

        // the device that we will issue draw calls to.
        GraphicsDevice device;

        // this value is set by Begin, and is the type of primitives that we are
        // drawing.
        PrimitiveType primitiveType;

        // how many verts does each of these primitives take up? points are 1,
        // lines are 2, and triangles are 3.
        int numVertsPerPrimitive;

        // hasBegun is flipped to true once Begin is called, and is used to make
        // sure users don't call End before Begin is called.
        bool hasBegun = false;

        bool isDisposed = false;

        int texWidth, texHeight;

        #endregion

        // the constructor creates a new PrimitiveBatch and sets up all of the internals
        // that PrimitiveBatch will need.
        public PrimitiveBatch(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice == null)
            {
                throw new ArgumentNullException("graphicsDevice");
            }
            device = graphicsDevice;

            // set up a new basic effect, and enable vertex colors.
            basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.VertexColorEnabled = true;

            // projection uses CreateOrthographicOffCenter to create 2d projection
            // matrix with 0,0 in the upper left.
            //basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width,
            //    graphicsDevice.Viewport.Height, 0, -1000, 1000);
            basicEffect.Projection = Matrix.CreatePerspective(Constants.screenWidth, -Constants.screenHeight, 1, HalfMaxDepth * 2);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !isDisposed)
            {
                if (basicEffect != null)
                    basicEffect.Dispose();

                isDisposed = true;
            }
        }

        public void Begin(PrimitiveType primitiveType, Texture2D texture)
        {
            Begin(primitiveType, texture, null);
        }

        // Begin is called to tell the PrimitiveBatch what kind of primitives will be
        // drawn, and to prepare the graphics card to render those primitives.
        public void Begin(PrimitiveType primitiveType, Texture2D texture, Effect effect)
        {
            if (hasBegun)
            {
                throw new InvalidOperationException
                    ("End must be called before Begin can be called again.");
            }

            // these three types reuse vertices, so we can't flush properly without more
            // complex logic. Since that's a bit too complicated for this sample, we'll
            // simply disallow them.
            if (primitiveType == PrimitiveType.LineStrip ||
                primitiveType == PrimitiveType.TriangleStrip)
            {
                throw new NotSupportedException
                    ("The specified primitiveType is not supported by PrimitiveBatch.");
            }

            this.primitiveType = primitiveType;

            // how many verts will each of these primitives require?
            this.numVertsPerPrimitive = NumVertsPerPrimitive(primitiveType);

            device.BlendState = BlendState.NonPremultiplied;
            device.RasterizerState = RasterizerState.CullNone;
            device.SamplerStates[0] = SamplerState.LinearClamp;

            if (texture != null)
            {
                basicEffect.Texture = texture;
                basicEffect.TextureEnabled = true;

                if (effect != null)
                    device.Textures[0] = texture;

                texWidth = texture.Width;
                texHeight = texture.Height;
            }
            else
                basicEffect.TextureEnabled = false;

            if (effect != null)
                effect.Parameters["Transform"].SetValue(basicEffect.Projection);

            //tell our basic effect to begin.
            (effect ?? basicEffect).CurrentTechnique.Passes[0].Apply();

            // flip the error checking boolean. It's now ok to call AddVertex, Flush,
            // and End.
            hasBegun = true;
        }

        // AddVertex is called to add another vertex to be rendered. To draw a point,
        // AddVertex must be called once. for lines, twice, and for triangles 3 times.
        // this function can only be called once begin has been called.
        // if there is not enough room in the vertices buffer, Flush is called
        // automatically.
        public void AddVertex(Vector2 vertex, Color color, Vector2 texCoord)
        {
            AddVertex(new Vector3(vertex, 0), color, texCoord);
        }

        public void AddVertex(Vector3 vertex, Color color, Vector2 texCoord)
        {
            if (!hasBegun)
                throw new InvalidOperationException
                    ("Begin must be called before AddVertex can be called.");

            // are we starting a new primitive? if so, and there will not be enough room
            // for a whole primitive, flush.
            bool newPrimitive = ((positionInBuffer % numVertsPerPrimitive) == 0);

            if (newPrimitive &&
                (positionInBuffer + numVertsPerPrimitive) >= vertices.Length)
            {
                Flush();
            }

            // once we know there's enough room, set the vertex in the buffer,
            // and increase position.

            vertex.Z -= HalfMaxDepth;
            vertices[positionInBuffer].Position =
                Vector3.Transform(vertex, Matrix.CreateScale(HalfMaxDepth, HalfMaxDepth, 1));

            vertices[positionInBuffer].Color = color;

            if (basicEffect.TextureEnabled)
                vertices[positionInBuffer].TextureCoordinate =
                    new Vector2(texCoord.X / texWidth, texCoord.Y / texHeight);

            positionInBuffer++;
        }

        //public void AddQuad(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4, Color color, Vector2 texCoord)
        //{
        //    if (primitiveType != PrimitiveType.TriangleList)
        //        throw new NotSupportedException("Only PrimitiveType.TriangleList is not supported by AddQuad.");

        //    AddVertex(v1, color, texCoord);
        //    AddVertex(v2, color, texCoord);
        //    AddVertex(v3, color, texCoord);

        //    AddVertex(v1, color, texCoord);
        //    AddVertex(v3, color, texCoord);
        //    AddVertex(v4, color, texCoord);
        //}

        public void AddRectangle(Vector2 position, float width, float height, Color color, Point texCoord)
        {
            AddRectangle(position, width, height, color,
                new Rectangle(texCoord.X, texCoord.Y, (int)width, (int)height), Matrix.Identity);
        }

        public void AddRectangle(Vector2 position, float width, float height, Color color, Point texCoord, Matrix transform)
        {
            AddRectangle(position, width, height, color,
                new Rectangle(texCoord.X, texCoord.Y, (int)width, (int)height), transform);
        }

        public void AddRectangle(Vector2 position, float width, float height, Color color, Rectangle srcRect)
        {
            AddRectangle(position, width, height, color, srcRect, Matrix.Identity);
        }

        public void AddRectangle(Vector2 position, float width, float height, Color color, Rectangle srcRect, Matrix transformation)
        {
            //AddQuad(position, new Vector2(position.X + width, position.Y),
            //    new Vector2(position.X + width, position.Y + height),
            //    new Vector2(position.X, position.Y + height), color, texCoord);

            float hw = width / 2f;
            float hh = height / 2f;

            position -= new Vector2(Constants.screenWidth / 2f, Constants.screenHeight / 2f);

            transformation = transformation * Matrix.CreateTranslation(position.X + hw, position.Y + hh, 0);

            //transformation = transformation * Matrix.CreateTranslation(position.X + hw, -position.Y - hh, -500) * Matrix.CreateScale(500, 500, 1);

            Vector3 v1 = Vector3.Transform(new Vector3(-hw, -hh, 0), transformation);
            Vector3 v2 = Vector3.Transform(new Vector3(hw, -hh, 0), transformation);
            Vector3 v3 = Vector3.Transform(new Vector3(hw, hh, 0), transformation);
            Vector3 v4 = Vector3.Transform(new Vector3(-hw, hh, 0), transformation);

            Vector2 c1 = new Vector2(srcRect.X, srcRect.Y);
            Vector2 c2 = new Vector2(srcRect.X + srcRect.Width, srcRect.Y);
            Vector2 c3 = new Vector2(srcRect.X + srcRect.Width, srcRect.Y + srcRect.Height);
            Vector2 c4 = new Vector2(srcRect.X, srcRect.Y + srcRect.Height);

            AddVertex(v1, color, c1);
            AddVertex(v2, color, c2);
            AddVertex(v3, color, c3);

            AddVertex(v1, color, c1);
            AddVertex(v3, color, c3);
            AddVertex(v4, color, c4);
        }

        public void AddRectangle(Vector2 position, float width, float height, Color color1, Color color2, bool isHorizontal, Vector2 texCoord)
        {
            if (primitiveType != PrimitiveType.TriangleList)
                throw new NotSupportedException("Only PrimitiveType.TriangleList is not supported by AddGradientRectangle.");

            Vector2 v1 = position;
            Vector2 v2 = new Vector2(position.X + width, position.Y);
            Vector2 v3 = new Vector2(position.X + width, position.Y + height);
            Vector2 v4 = new Vector2(position.X, position.Y + height);

            if (isHorizontal)
            {
                AddVertex(v1, color1, texCoord);
                AddVertex(v2, color2, texCoord);
                AddVertex(v3, color2, texCoord);

                AddVertex(v1, color1, texCoord);
                AddVertex(v3, color2, texCoord);
                AddVertex(v4, color1, texCoord);
            }
            else
            {
                AddVertex(v1, color1, texCoord);
                AddVertex(v2, color1, texCoord);
                AddVertex(v3, color2, texCoord);

                AddVertex(v1, color1, texCoord);
                AddVertex(v3, color2, texCoord);
                AddVertex(v4, color2, texCoord);
            }
        }

        public void AddCircle(Vector2 position, float radius, Color color, Vector2 texCoord)
        {
            position -= new Vector2(Constants.screenWidth / 2f, Constants.screenHeight / 2f);

            float y1, y2;
            y1 = radius;

            for (float x = 0; x < radius; x++)
            {
                y2 = (float)Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(x + 1, 2));

                AddVertex(position + new Vector2(+x + 1, +y2), color, texCoord);
                AddVertex(position + new Vector2(+x,     +y1), color, texCoord);
                AddVertex(position,                            color, texCoord);

                AddVertex(position + new Vector2(+x,     -y1), color, texCoord);
                AddVertex(position + new Vector2(+x + 1, -y2), color, texCoord);
                AddVertex(position,                            color, texCoord);

                AddVertex(position + new Vector2(-x,     +y1), color, texCoord);
                AddVertex(position + new Vector2(-x - 1, +y2), color, texCoord);
                AddVertex(position,                            color, texCoord);

                AddVertex(position + new Vector2(-x - 1, -y2), color, texCoord);
                AddVertex(position + new Vector2(-x,     -y1), color, texCoord);
                AddVertex(position,                            color, texCoord);

                y1 = y2;
            }
        }

        // IDEA: Make objects that represent draw calls, and make static helper methods to generate them.
        // These should have an array of VertexPositionColor, when sending them to PrimitiveBatch.Draw
        // these arrays should be copied to the one in PrimitiveBatch.

        // End is called once all the primitives have been drawn using AddVertex.
        // it will call Flush to actually submit the draw call to the graphics card, and
        // then tell the basic effect to end.
        public void End()
        {
            if (!hasBegun)
            {
                throw new InvalidOperationException
                    ("Begin must be called before End can be called.");
            }

            // Draw whatever the user wanted us to draw
            Flush();

            device.RasterizerState = RasterizerState.CullCounterClockwise;

            hasBegun = false;
        }

        // Flush is called to issue the draw call to the graphics card. Once the draw
        // call is made, positionInBuffer is reset, so that AddVertex can start over
        // at the beginning. End will call this to draw the primitives that the user
        // requested, and AddVertex will call this if there is not enough room in the
        // buffer.
        private void Flush()
        {
            if (!hasBegun)
            {
                throw new InvalidOperationException
                    ("Begin must be called before Flush can be called.");
            }

            // no work to do
            if (positionInBuffer == 0)
            {
                return;
            }

            // how many primitives will we draw?
            int primitiveCount = positionInBuffer / numVertsPerPrimitive;

            // submit the draw call to the graphics card
            device.DrawUserPrimitives<VertexPositionColorTexture>(primitiveType, vertices, 0,
                primitiveCount);

            // now that we've drawn, it's ok to reset positionInBuffer back to zero,
            // and write over any vertices that may have been set previously.
            positionInBuffer = 0;
        }

        #region Helper functions

        // NumVertsPerPrimitive is a boring helper function that tells how many vertices
        // it will take to draw each kind of primitive.
        static private int NumVertsPerPrimitive(PrimitiveType primitive)
        {
            int numVertsPerPrimitive;
            switch (primitive)
            {
                case PrimitiveType.LineList:
                    numVertsPerPrimitive = 2;
                    break;
                case PrimitiveType.TriangleList:
                    numVertsPerPrimitive = 3;
                    break;
                default:
                    throw new InvalidOperationException("primitive is not valid");
            }
            return numVertsPerPrimitive;
        }

        #endregion
    }
}
