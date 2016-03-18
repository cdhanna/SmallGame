using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmallCommons.Rendering.Bloom;
using SmallGame.Render;

namespace SmallCommons.Rendering
{

    class ScreenBloomPass<T> : ScreenBloomPass where T : RenderPass
    {
        public ScreenBloomPass(string extractEffect, string combineEffect, string blurEffect)
            : base(extractEffect, combineEffect, blurEffect) { }
        public ScreenBloomPass(string name, string extracteffect, string combineEffect, string blurEffect)
            : base(name, extracteffect, combineEffect, blurEffect) { }

        private T GetPass(RenderStrategy strategy)
        {
            return strategy.GetPass<T>();
        }

        protected override RenderTarget2D GetSceneToBloom(RenderStrategy strategy)
        {
            return GetPass(strategy).Output;
        }
    }

    class ScreenBloomPass : RenderPass<ScreenBloomPass>
    {
        private SpriteBatch _spriteBatch;
        public BloomComponent Bloom { get; private set; }
        private string _extractEffect, _combineEffect, _blurEffect;

        public ScreenBloomPass(string extractEffect, string combineEffect, string blurEffect)
            : this("ScreenBloomPass", extractEffect, combineEffect, blurEffect) { }
        public ScreenBloomPass(string name, string extractEffect, string combineEffect, string blurEffect)
        {
            Name = name;
            _extractEffect = extractEffect;
            _combineEffect = combineEffect;
            _blurEffect = blurEffect;
            Bloom = new BloomComponent();
        }

        protected virtual RenderTarget2D GetSceneToBloom(RenderStrategy strategy)
        {
            return strategy.GetPass<SimplePrimtivePass>().Output;
        }

        protected override void OnInit(GraphicsDevice graphics)
        {
            _spriteBatch = new SpriteBatch(graphics);
            Bloom.Init(graphics,
                Services.ResourceService.Load<Effect>(_extractEffect),
                Services.ResourceService.Load<Effect>(_combineEffect),
                Services.ResourceService.Load<Effect>(_blurEffect)
                );
        }

        protected override void OnRender(RenderStrategy strategy, List<Action<RenderArgs<ScreenBloomPass>>> Actions, RenderArgs<ScreenBloomPass> args)
        {
            Bloom.BeginDraw();
            Graphics.Clear(Color.Transparent);

            _spriteBatch.Begin();
            _spriteBatch.Draw(GetSceneToBloom(strategy), Vector2.Zero, Color.White);
            _spriteBatch.End();

            Output = Bloom.outputTarget;
            Bloom.draw();

        }
    }
}
