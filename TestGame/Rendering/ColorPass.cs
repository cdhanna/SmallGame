using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SmallGame;
using SmallGame.Render;

namespace TestGame.Rendering
{
    class ColorPass : RenderPass<ColorPass>
    {
        public PrimitiveBatch PBatch { get; private set; }
        
        public Dictionary<Color, List<Action<RenderArgs<ColorPass>>>> ColorActions { get; private set; } 

        public ColorPass()
        {
            ColorActions = new Dictionary<Color, List<Action<RenderArgs<ColorPass>>>>();
            ColorActions.Add(Color.Red, new List<Action<RenderArgs<ColorPass>>>());
            ColorActions.Add(Color.Green, new List<Action<RenderArgs<ColorPass>>>());
            ColorActions.Add(Color.Blue, new List<Action<RenderArgs<ColorPass>>>());
            ColorActions.Add(Color.Black, new List<Action<RenderArgs<ColorPass>>>());
            ColorActions.Add(Color.White, new List<Action<RenderArgs<ColorPass>>>());
        }

        protected override void OnInit(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphics)
        {
            PBatch = new PrimitiveBatch(graphics);
        }

        protected override void OnRender(RenderStrategy strategy, List<Action<RenderArgs<ColorPass>>> Actions,
            RenderArgs<ColorPass> args)
        {
            Graphics.Clear(Color.Transparent);
            ColorActions.Keys.ToList().ForEach(cak =>
            {
                var list = ColorActions[cak];
                list.ForEach(action =>
                {
                    action(args);
                });
            });
        }


    }
}
