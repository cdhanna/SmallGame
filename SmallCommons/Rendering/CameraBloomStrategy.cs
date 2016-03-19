using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using SmallCommons.Rendering.Bloom;
using SmallGame.Render;

namespace SmallCommons.Rendering
{
    public class CameraBloomStrategy : RenderStrategy
    {
        private string _extractPath, _combinePath, _blurPath;

        public CameraBloomStrategy(string extractPath, string combinePath, string blurPath)
        {
            _extractPath = extractPath;
            _combinePath = combinePath;
            _blurPath = blurPath;
        }

        protected override void OnInit(GraphicsDevice graphics)
        {
            // AddPass(new ScreenBloomPass<SimpleSpritePass>("sprites", _extractPath, _combinePath, _blurPath));

            AddPass(new CameraSpritePass());
            AddPass(new ScreenBloomPass<CameraSpritePass>("sprites", _extractPath, _combinePath, _blurPath));

            AddPass(new SimpleSpritePass());
           

            //TODO make setting assignment more intuitive. 
            GetPass<ScreenBloomPass<CameraSpritePass>>().Bloom.Settings = BloomSettings.PresetSettings[5];

        }
    }
}
