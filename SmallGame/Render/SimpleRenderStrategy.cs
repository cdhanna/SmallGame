using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace SmallGame.Render
{
    public class SimpleRenderStrategy : RenderStrategy
    {
        protected override void OnInit(GraphicsDevice graphics)
        {
            AddPass(new SimplePrimtivePass());
            AddPass(new SimpleSpritePass());
        }
    }

}
