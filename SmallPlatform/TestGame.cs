using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmallGame;
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
            
            
            DataLoader.LoadAndWatch<TestLevel>("sample.json", (level) => lvl = SetLevel(level));

            var renderingSrvc = Services.RequestService<RenderService>();
            renderingSrvc.ClearColor = Color.Maroon;

            ScriptService.LoadAndWatch("TestScripts.script.cs");
        }
    }
}
