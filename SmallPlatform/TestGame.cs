using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
            
           // ShaderCompiler.Init("Content", @"C:\proj\SmallGame\DataCopy\ShaderBuildTools");

            DataLoader.LoadAndWatch<TestLevel>("sample2.json", (level) => lvl = LevelService.SetLevel(level));

            //var renderingSrvc = Services.RenderService;

            
            //RenderService.SetStrategy(new CameraSpriteStrategy())
            //    .AddPass(new ScreenBloomPass<CameraSpritePass>(
            //        "Content/Shaders/BloomExtra.fx.mgfxo",
            //        "Content/Shaders/BloomCombine.fx.mgfxo",
            //        "Content/Shaders/GaussianBlur.fx.mgfxo"));

            RenderService.SetStrategy(new CameraBloomStrategy(
                "Content/Shaders/BloomExtra.fx.mgfxo",
                "Content/Shaders/BloomCombine.fx.mgfxo",
                "Content/Shaders/GaussianBlur.fx.mgfxo"));
            
            ScriptService.LoadAndWatch("TestScripts.script.cs");

            var camera = RenderService.Strategy.GetPass<CameraSpritePass>().Camera;
            
            KeyboardHelper.OnDown(Keys.A, args => camera.ForceSum -= Vector2.UnitX);
            KeyboardHelper.OnDown(Keys.D, args => camera.ForceSum += Vector2.UnitX);

            KeyboardHelper.OnDown(Keys.W, args => camera.ForceSum -= Vector2.UnitY);
            KeyboardHelper.OnDown(Keys.S, args => camera.ForceSum += Vector2.UnitY);

            KeyboardHelper.OnDown(Keys.Q, args => camera.TorqueSum += .01f);
            KeyboardHelper.OnDown(Keys.E, args => camera.TorqueSum -= .01f);

            KeyboardHelper.OnDown(Keys.R, args => camera.ZoomForceSum += .01f);
            KeyboardHelper.OnDown(Keys.F, args => camera.ZoomForceSum -= .01f);
        }

        protected override void Update(GameTime gameTime)
        {




            base.Update(gameTime);
        }
    }
}
