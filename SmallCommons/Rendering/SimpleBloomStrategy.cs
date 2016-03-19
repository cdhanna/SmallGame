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
    public class SimpleBloomStrategy : RenderStrategy
    {

        private string _extractPath, _combinePath, _blurPath;

        public SimpleBloomStrategy(string extractPath, string combinePath, string blurPath)
        {
            _extractPath = extractPath;
            _combinePath = combinePath;
            _blurPath = blurPath;
        }

        protected override void OnInit(GraphicsDevice graphics)
        {
            AddPass(new SimpleSpritePass());
            AddPass(new ScreenBloomPass<SimpleSpritePass>("sprites", _extractPath, _combinePath, _blurPath));

            AddPass(new SimplePrimtivePass());
            AddPass(new ScreenBloomPass<SimplePrimtivePass>("primtives", _extractPath, _combinePath, _blurPath));

            //TODO make setting assignment more intuitive. 
            GetPass<ScreenBloomPass<SimpleSpritePass>>().Bloom.Settings = BloomSettings.PresetSettings[5];

        }
    }

}
