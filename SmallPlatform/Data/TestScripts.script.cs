using System;
using Microsoft.Xna.Framework;
using SmallGame;
using SmallGame.GameObjects;
using SmallGame.Services;
using SmallPlatform.Objects;

namespace SmallPlatform.Content
{
    class TestScripts : ScriptCollection
    {

        float t = 0;

        public void Test(BasicObject obj)
        {
            Console.WriteLine("t is " + t);
        }

        public void SinMove(BasicObject obj)
        {
            obj.Position = 100 * new Vector2((float)Math.Sin(t), (float)Math.Cos(t));
        }

        public override void Update(GameTime time)
        {
            t += .01f;
            base.Update(time);
        }
    }
}
