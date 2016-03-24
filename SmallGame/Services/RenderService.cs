using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmallGame.GameObjects;
using SmallGame.Render;

namespace SmallGame.Services
{
    /// <summary>
    /// The RenderService allows GameObjects to customize their rendering process
    /// </summary>
    public interface IRenderService : IGameService
    {

        RenderStrategy Strategy { get; }
        GraphicsDevice Graphics { get;  }
        Camera2D ActiveCamera { get; }

        void Configure(GameServices services, GraphicsDevice graphics);
        
        /// <summary>
        /// Clears all subscribers of the OnRender event
        /// </summary>
        void Empty();

        /// <summary>
        /// Invokes all subscribers of the OnRender event
        /// </summary>
        void Render();

        P GetPass<P>() where P : RenderPass;

        RenderStrategy SetStrategy(RenderStrategy strategy);
        void Init();

    }

    //public class RenderEventArgs : EventArgs
    //{
    //    public SpriteBatch SpriteBatch { get; set; }
    //    public PrimitiveBatch PrimitiveBatch { get; set; }
    //    public GraphicsDevice Graphics { get { return SpriteBatch.GraphicsDevice; } }

    //    public RenderEventArgs(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
    //    {
    //        SpriteBatch = spriteBatch;
    //        PrimitiveBatch = primitiveBatch;
    //    }
    //}

    public class RenderService : IRenderService
    {

        public RenderStrategy Strategy { get; private set; }
        public Camera2D ActiveCamera { get; private set; }
        public GraphicsDevice Graphics { get; private set; }
        public GameServices Services { get; private set; }
        
        public RenderService()
        {

        }

        public P GetPass<P>() where P : RenderPass
        {
            if (Strategy == null) return null; 

            return Strategy.GetPass<P>();
        }

        public void Configure(GameServices services, GraphicsDevice graphics)
        {
            Graphics = graphics;
            Services = services;
            ActiveCamera = new Camera2D();
            ActiveCamera.Init(services);
            Empty();
        }

        public RenderStrategy SetStrategy(RenderStrategy strategy)
        {
            Strategy = strategy;
            //if (Strategy == null)
            //{
            //    Strategy = new SimpleRenderStrategy();
            //}
            Init();
            return strategy;
        }

        public void Init()
        {
            if (Strategy == null) Strategy = new SimpleRenderStrategy();

            Strategy.Init(Services, Graphics);
        }

        public void Empty()
        {
            if (Strategy == null) return; 

            Strategy.Init(Services, Graphics);
        }

        public void Render()
        {
            if (Strategy == null) return; 

            Strategy.Render();

        }

    }
}
