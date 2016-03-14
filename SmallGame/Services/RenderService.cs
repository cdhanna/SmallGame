using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SmallGame.Services
{
    /// <summary>
    /// The RenderService allows GameObjects to customize their rendering process
    /// </summary>
    public interface IRenderService : IGameService
    {
        /// <summary>
        /// Gets or Sets the EventHandler for the rendering point
        /// </summary>
        EventHandler<RenderEventArgs> OnRender { get; set; }

        /// <summary>
        /// Gets or Sets the buffer clear color
        /// </summary>
        Color ClearColor { get; set; }

        /// <summary>
        /// Configures the service
        /// </summary>
        /// <param name="spriteBatch">The spritebatch that will be used and passed to all subscribers</param>
        /// <param name="primitiveBatch">The primitivebatch that will be used and passed to all subscribers</param>
        void Configure(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch);
        
        /// <summary>
        /// Clears all subscribers of the OnRender event
        /// </summary>
        void Empty();

        /// <summary>
        /// Invokes all subscribers of the OnRender event
        /// </summary>
        void Render();
    }

    public class RenderEventArgs : EventArgs
    {
        public SpriteBatch SpriteBatch { get; set; }
        public PrimitiveBatch PrimitiveBatch { get; set; }
        public GraphicsDevice Graphics { get { return SpriteBatch.GraphicsDevice; } }

        public RenderEventArgs(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
        {
            SpriteBatch = spriteBatch;
            PrimitiveBatch = primitiveBatch;
        }
    }

    public class RenderService : IRenderService
    {

        public SpriteBatch SpriteBatch { get; set; }
        public PrimitiveBatch PrimitiveBatch { get; set; }
        public GraphicsDevice Graphics { get { return SpriteBatch.GraphicsDevice; } }
        public Color ClearColor { get; set; }

        public EventHandler<RenderEventArgs> OnRender { get; set; }
        
        public RenderService()
        {
            ClearColor = Color.Black;
            OnRender = (sender, args) => { };
        }

        public void Configure(SpriteBatch spriteBatch, PrimitiveBatch primitveBatch)
        {
            SpriteBatch = spriteBatch;
            PrimitiveBatch = primitveBatch;
        }

        public void Empty()
        {
            OnRender = (sender, args) => { };
        }

        public void Render()
        {
            Graphics.Clear(ClearColor);

            SpriteBatch.Begin();
            OnRender.Invoke(this, new RenderEventArgs(SpriteBatch, PrimitiveBatch));
            SpriteBatch.End();

        }

    }
}
