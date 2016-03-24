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
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
#endregion

namespace SmallGame
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

        // a block of vertices that calling AddVertex will fill. Flush will draw using
        // this array, and will determine how many primitives to draw from
        // positionInBuffer.
        VertexPositionColor[] vertices = new VertexPositionColor[DefaultBufferSize];

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

        private bool loop = false;

        #endregion

        public Matrix Transform { get; set; }

        // the constructor creates a new PrimitiveBatch and sets up all of the internals
        // that PrimitiveBatch will need.
        public PrimitiveBatch(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice == null)
            {
                throw new ArgumentNullException("graphicsDevice");
            }
            device = graphicsDevice;
            Transform = Matrix.Identity;

            // set up a new basic effect, and enable vertex colors.
            basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.VertexColorEnabled = true;

            // projection uses CreateOrthographicOffCenter to create 2d projection
            // matrix with 0,0 in the upper left.
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter
                (0, graphicsDevice.Viewport.Width,
                graphicsDevice.Viewport.Height, 0,
                0, 1);
            this.basicEffect.World = Matrix.Identity;
            this.basicEffect.View = Matrix.CreateLookAt(Vector3.Zero, Vector3.Forward,
                Vector3.Up);
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

        // Begin is called to tell the PrimitiveBatch what kind of primitives will be
        // drawn, and to prepare the graphics card to render those primitives.
        public void Begin(PrimitiveType primitiveType, bool loop=false)
        {
            if (hasBegun)
            {
                throw new InvalidOperationException
                    ("End must be called before Begin can be called again.");
            }


            this.primitiveType = primitiveType;
            this.loop = loop;
            // how many verts will each of these primitives require?
            //this.numVertsPerPrimitive = NumVertsPerPrimitive(primitiveType);

            //tell our basic effect to begin.
            basicEffect.CurrentTechnique.Passes[0].Apply();

            // flip the error checking boolean. It's now ok to call AddVertex, Flush,
            // and End.
            hasBegun = true;
        }

        // AddVertex is called to add another vertex to be rendered. To draw a point,
        // AddVertex must be called once. for lines, twice, and for triangles 3 times.
        // this function can only be called once begin has been called.
        // if there is not enough room in the vertices buffer, Flush is called
        // automatically.
        public void AddVertex(Vector3 vertex, Color color)
        {
            if (!hasBegun)
            {
                throw new InvalidOperationException
                    ("Begin must be called before AddVertex can be called.");
            }

            var add = new Action<Vector3, Color>((v, c) =>
            {

                vertices[positionInBuffer].Position = v;
                vertices[positionInBuffer].Color = c;
                positionInBuffer++;

            });
            var addOld = new Action<int>(i => add(vertices[i].Position, vertices[i].Color));
            

            if (primitiveType == PrimitiveType.LineStrip)
            {
                add(vertex, color);
            }
            else if (primitiveType == PrimitiveType.TriangleStrip)
            {
                if (positionInBuffer > 2)
                {
                    var last = positionInBuffer -1;
                    addOld(0);
                    addOld(last);
                }
                add(vertex, color);
            }

        }

        public void AddVertex(Vector2 vertex, Color color)
        {
            AddVertex(new Vector3(vertex, 0), color );
        }

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
            if (positionInBuffer < 2)
            {
                return;
            }

            if (loop)
            {
                AddVertex(vertices[0].Position, vertices[0].Color);
            }

            // how many primitives will we draw?
            int primitiveCount = 0;


            switch (primitiveType)
            {
                case PrimitiveType.LineStrip:

                    if (positionInBuffer == 2)
                    {
                        primitiveCount = 1;
                    }
                    else
                    {
                        primitiveCount = positionInBuffer - 1;
                    }

                    break;

                case PrimitiveType.TriangleStrip:
                    primitiveCount = (positionInBuffer ) /3;
                    primitiveType = PrimitiveType.TriangleList;
                    break;
                default:
                    throw new Exception("Invalid primitve type");
            }

            for (int i = 0; i < positionInBuffer; i++)
            {
                vertices[i].Position = Vector3.Transform(vertices[i].Position, Transform);
            }
            // submit the draw call to the graphics card
            device.DrawUserPrimitives<VertexPositionColor>(primitiveType, vertices, 0,
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