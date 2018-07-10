using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SmallGame.Render;

namespace TestGame.Rendering
{
    class TestRenderStrategy : RenderStrategy
    {
        protected override void OnInit(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphics)
        {
            AddPass(new BackgroundPass());
            AddPass(new ColorPass());
        }
    }
}
