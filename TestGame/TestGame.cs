using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SmallGame;
using TestGame.GameObjects;
using TestGame.Rendering;

namespace TestGame
{
    class TestGame : CoreGame
    {
        protected override void Initialize()
        {
            base.Initialize();

            RenderService.SetStrategy(new TestRenderStrategy());

            var lvl = new TestLevel();
            LevelService.SetLevel(lvl);

           // new Wall(lvl, Color.Blue, new Vector2(100, 100), new Vector2(200, 200));
            new Wall(lvl, Color.Black, new Vector2(10, 420), new Vector2(790, 440));
            new Wall(lvl, Color.Black, new Vector2(100, 100), new Vector2(120, 440));

            new Wall(lvl, Color.Black, new Vector2(600, 100), new Vector2(620, 440));

            new Wall(lvl, Color.Green, new Vector2(200, 300), new Vector2(400, 320));

            new Wall(lvl, Color.Blue, new Vector2(300, 200), new Vector2(500, 220));

            var plr = new Player(lvl);
            plr.Position = new Vector2(400, 10);


            KeyboardHelper.OnDown(Microsoft.Xna.Framework.Input.Keys.Space, k => plr.Position = new Vector2(400, 10));

        }
    }
}
