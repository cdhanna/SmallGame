using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmallCommons.Rendering.Bloom;
using SmallGame.Render;

namespace SmallCommons.Rendering
{
    public class ExampleRenderStrategy : RenderStrategy
    {
        protected override void OnInit(GraphicsDevice graphics)
        {

            var extractPath = "Content/BloomExtract.fx.ogl.mgfxo";
            var combinePath = "Content/BloomCombine.fx.ogl.mgfxo";
            var blurPath = "Content/GaussianBlur.fx.ogl.mgfxo";

            AddPass(new SimpleSpritePass());
            AddPass(new ScreenBloomPass<SimpleSpritePass>("sprites", extractPath, combinePath, blurPath));

            AddPass(new SimplePrimtivePass());
            AddPass(new ScreenBloomPass<SimplePrimtivePass>("primtives",  extractPath, combinePath, blurPath));

            GetPass<ScreenBloomPass<SimpleSpritePass>>().Bloom.Settings = BloomSettings.PresetSettings[5];

        }
    }

}
