using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using SmallGame.GameObjects;
using SmallGame.Services;

namespace SmallGame.Render
{

    //class RenderPassCollection
    //{
    //    private List<RenderPass> _passes;

    //    public RenderPassCollection(List<RenderPass> passes )
    //    {
    //        _passes = passes;
    //    }

    //    public RenderPass Get(string name)
    //    {
            
    //    }

    //}

    public class RenderArgs
    {
        public RenderStrategy Strategy { get; protected set; }
        public GraphicsDevice Graphics { get; protected set; }
        public RenderTarget2D Target { get; protected set; }
        public RenderPass Pass { get; protected set; }

        

        public RenderArgs(RenderPass pass, RenderStrategy strategy, GraphicsDevice device, RenderTarget2D target)
        {
            Pass = pass;
            Strategy = strategy;
            Graphics = device;
            Target = target;
        }

    }

    public class RenderArgs<P> : RenderArgs where P : RenderPass
    {
        public new P Pass { get; private set; }

        public RenderArgs(P pass, RenderArgs args) : this(pass, args.Strategy, args.Graphics, args.Target)
        {
        }

        public RenderArgs(P pass, RenderStrategy strategy, GraphicsDevice device, RenderTarget2D target)
            : base(pass, strategy, device, target)
        {
            Pass = pass;
        }
    }

    public abstract class RenderPass<T> : RenderPass where T : RenderPass<T>
    {

        private List<Action<RenderArgs<T>>> RenderActions { get; set; } 

        protected RenderPass(string name) : base(name)
        {
            RenderActions = new List<Action<RenderArgs<T>>>();
            if (!typeof(T).IsAssignableFrom(this.GetType()))
                throw new Exception("A pass must be genericed on its own type. ");
        }

        protected RenderPass() : this(typeof (T).Name)
        {
        }


        public void AddAction(Action<RenderArgs<T>> action)
        {
            //var betterAction = new Action<RenderArgs>(args =>
            //    action(new RenderArgs<T>( (T) args.Pass, args.Strategy, args.Graphics, args.Target)));

            RenderActions.Add(action);
        }

        protected abstract void OnRender(RenderStrategy strategy, List<Action<RenderArgs<T>>> Actions, RenderArgs<T> args );
        public override void Render(RenderStrategy strategy)
        {
            Graphics.SetRenderTarget(Output);
            var args = new RenderArgs<T>((T)this, strategy, Graphics, Output);
            OnRender(strategy, RenderActions, args);
        }

        protected override void PreInit(GraphicsDevice graphics)
        {
            RenderActions.Clear();
            base.PreInit(graphics);
        }
    }

    public abstract class RenderPass
    {
        public string Name { get; protected set; }
        public RenderTarget2D Output { get; protected set; }
        protected GraphicsDevice Graphics { get; private set; }
        protected GameServices Services { get; private set; }

      //  protected List<Action<RenderArgs>> RenderActions { get; set; } 

        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public bool MipMap { get; protected set; }
        public SurfaceFormat SurfaceFormat { get; protected set; }
        public DepthFormat DepthFormat { get; protected set; }


        protected RenderPass(string name)
        {
            Name = name;
            //RenderActions = new List<Action<RenderArgs>>();
        }

        protected abstract void OnInit(GraphicsDevice graphics);
        protected virtual void PreInit(GraphicsDevice graphics)
        {
            // allow the subclass to configure things. 
        }

        public void Init(GameServices services, GraphicsDevice graphics)
        {
            Services = services;
            // standard defaults for render target.
            Width = graphics.PresentationParameters.BackBufferWidth;
            Height = graphics.PresentationParameters.BackBufferHeight;
            MipMap = false;
            SurfaceFormat = SurfaceFormat.Color;
            DepthFormat = DepthFormat.Depth24Stencil8;

            Graphics = graphics;
            PreInit(graphics);
            OnInit(graphics);
            Output = new RenderTarget2D(Graphics, Width, Height, MipMap, SurfaceFormat, DepthFormat);

        }


        //public void Render(RenderStrategy strategy)
        //{
        //    Graphics.SetRenderTarget(Output);

         

        //    OnRender(strategy);
        //}


        public abstract void Render(RenderStrategy strategy);

    }
}
