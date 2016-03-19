using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmallGame.GameObjects;

namespace SmallGame.Render
{

    public class SimpleSpritePass : RenderPass<SimpleSpritePass>
    {
        public SpriteBatch SpriteBatch { get; private set; }
        
        protected override void OnInit(GraphicsDevice graphics)
        {
            SpriteBatch = new SpriteBatch(graphics);
        }

        protected override void OnRender(RenderStrategy strategy, List<Action<RenderArgs<SimpleSpritePass>>> Actions, RenderArgs<SimpleSpritePass> args)
        {
            Graphics.Clear(Color.Transparent);
            SpriteBatch.Begin();
            Actions.ForEach(a => a(args)); // simple invocation. 
            SpriteBatch.End();
        }
    }

    public class CameraSpritePass : SimpleSpritePass
    {
        public Camera2D Camera { get; set; }

        protected override void OnInit(GraphicsDevice graphics)
        {
            Camera = new Camera2D();
            Services.LevelService.Level.Objects.Add(Camera); // TODO this is fucking disgusting. We need a better way to submit objects to the level.
            
            base.OnInit(graphics);
        }

        protected override void OnRender(RenderStrategy strategy, List<Action<RenderArgs<SimpleSpritePass>>> Actions, RenderArgs<SimpleSpritePass> args)
        {
            Graphics.Clear(Color.Transparent);
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Camera.Transform);
            Actions.ForEach(a => a(args)); // simple invocation. 
            SpriteBatch.End();
        }
    }

    public class SimplePrimtivePass : RenderPass<SimplePrimtivePass>
    {
        public PrimitiveBatch PrimitiveBatch { get; private set; }
        protected override void OnInit(GraphicsDevice graphics)
        {
            PrimitiveBatch = new PrimitiveBatch(graphics);
        }

        protected override void OnRender(RenderStrategy strategy, List<Action<RenderArgs<SimplePrimtivePass>>> Actions, RenderArgs<SimplePrimtivePass> args)
        {
            Graphics.Clear(Color.Transparent);
            Actions.ForEach(a => a(args));
        }
    }

    public class ScreenShaderPass : RenderPass<ScreenShaderPass>
    {
        public ScreenShaderPass(string name, Effect file)
        {
            
        }

        protected override void OnRender(RenderStrategy strategy, List<Action<RenderArgs<ScreenShaderPass>>> Actions, RenderArgs<ScreenShaderPass> args)
        {
            throw new NotImplementedException();
        }

        protected override void OnInit(GraphicsDevice graphics)
        {
            throw new NotImplementedException();
        }
    }

    
}
