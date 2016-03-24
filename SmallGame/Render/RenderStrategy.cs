using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmallGame.GameObjects;
using SmallGame.Services;

namespace SmallGame.Render
{
    public abstract class RenderStrategy
    {

        private List<RenderPass> Passes { get; set; }
        private Dictionary<Type, List<RenderPass>> PassTypeTable { get; set; }
        private Dictionary<string, RenderPass> PassNameTable { get; set; }

        private SpriteBatch SpriteBatch { get; set; }

        public GraphicsDevice Graphics { get; private set; }
        public Color ClearColor { get; set; }
        public GameServices Services { get; set; }


        protected RenderStrategy()
        {
            Passes = new List<RenderPass>();
            PassTypeTable = new Dictionary<Type, List<RenderPass>>();
            PassNameTable = new Dictionary<string, RenderPass>();

            ClearColor = Color.DarkSlateGray;
           
            
        }

        public void AddPass<P>(P pass) where P : RenderPass
        {
            if (pass == null) throw new Exception("Cannot add a null pass to a render strategy");

            var name = pass.Name;
            // adding a pass. passes are run in order of add. 
            if (PassNameTable.ContainsKey(name)) throw new Exception("There is already a pass with that name. " + name);
            PassNameTable.Add(name, pass);

            if (!PassTypeTable.ContainsKey(typeof(P))) PassTypeTable.Add(typeof(P), new List<RenderPass>());
            PassTypeTable[typeof(P)].Add(pass);

            Passes.Add(pass);
        }

        public RenderPass GetPass(string name)
        {
            return PassNameTable[name];
        }

        public P GetPass<P>() where P : RenderPass
        {
            try
            {
                return (P) PassTypeTable[typeof (P)].FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Couldn't find the pass " + typeof(P).Name);
                return null;
            }
        }

        public List<P> GetPasses<P>() where P : RenderPass
        {
            return PassTypeTable[typeof (P)].ConvertAll(p => (P)p);
        }

        protected abstract void OnInit(GraphicsDevice graphics);
        public void Init(GameServices services, GraphicsDevice graphics)
        {
            // init every pass
            Graphics = graphics;
            Services = services;
            Passes = new List<RenderPass>();
            PassTypeTable = new Dictionary<Type, List<RenderPass>>();
            PassNameTable = new Dictionary<string, RenderPass>();

            SpriteBatch = new SpriteBatch(graphics);
            OnInit(graphics);
            Passes.ForEach(p => p.Init(services, graphics));
        }

        protected virtual void FinalizeRender(SpriteBatch spriteBatch)
        {
            
            Graphics.SetRenderTarget(null);
            Graphics.Clear(ClearColor);

            SpriteBatch.Begin();
            Passes.ForEach(p =>
                SpriteBatch.Draw(p.Output, Vector2.Zero, Color.White)                );
            SpriteBatch.End();
        }
        protected virtual void RenderPasses()
        {
            
            // render every pass. 
            Passes.ForEach(p => p.Render(this));


        }

        public void Render()
        {
            try
            {
                RenderPasses();
                FinalizeRender(SpriteBatch);
            }
            catch (Exception ex)
            {
                Console.WriteLine("RENDER FAILED");
            }
        }


    }
}
