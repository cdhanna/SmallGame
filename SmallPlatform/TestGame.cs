using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmallCommons.Rendering;
using SmallGame;
using SmallGame.GameObjects;
using SmallGame.Render;
using SmallGame.Services;
using SmallPlatform.Levels;
using SmallPlatform.Objects;

namespace SmallPlatform
{
    class TestGame : CoreGame
    {
        private TestLevel lvl;

        protected override void Initialize()
        {
            base.Initialize();
            
            DataLoader.RegisterParser(
                new StandardGameObjectParser<Geometry>(),
                StandardGameObjectParser.For<BasicObject>(),
                StandardGameObjectParser.For<SpriteObject>());
            
            

            ShaderCompiler.Init("Content", @"C:\proj\SmallGame\DataCopy\ShaderBuildTools");

            DataLoader.LoadAndWatch<TestLevel>("sample.json", (level) => lvl = SetLevel(level));

            //var renderingSrvc = Services.RenderService;
            RenderService.SetStrategy(new SimpleBloomStrategy(
                "Content/Shaders/BloomExtra.fx.mgfxo",
                "Content/Shaders/BloomCombine.fx.mgfxo",
                "Content/Shaders/GaussianBlur.fx.mgfxo"));
            
            ScriptService.LoadAndWatch("TestScripts.script.cs");
        }
    }
}
