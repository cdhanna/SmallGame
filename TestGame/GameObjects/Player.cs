using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpenTK.Input;
using SmallCommons;
using SmallGame.GameObjects;
using SmallGame.Input;
using SmallGame.Render;
using SmallGame.Services;
using TestGame.Rendering;

namespace TestGame.GameObjects
{
    class Player : BasicObject
    {

        public int Width { get; private set; }
        public int Height { get; private set; }

        private Vector2[] _corners;

        public Player()
        {
            _corners = new Vector2[0];
            Width = 40;
            Height = 40;
            
        }

        public Player(TestLevel lvl) : this()
        {
            lvl.SetPlayer(this);

        }

        protected override void OnInit(GameServices services)
        {
            services.RenderService.Strategy.GetPass<ColorPass>().ColorActions[Color.White].Add(Render);

            services.RenderService.Strategy.GetPass<BackgroundPass>().ChangeColor(Position, Color.Blue);
            base.OnInit(services);
        }

        protected override void Update(GameTime time, GameServices services)
        {
            var sizeVec = new Vector2(Width, Height);
            var halfSizeVec = sizeVec/2;
            var lvl = services.LevelService.GetLevel<TestLevel>();

            // add gravity
            var gravity = new Vector2(0, 1f);
            AddForce(gravity);

            // add friction

            var drag = -Velocity.Scale(.12f, .05f);
            AddForce(drag);

            // add controls
            var direction = Vector2.Zero;
            var keyboard = services.RequestService<KeyboardHelper>();
            if (keyboard.IsDown(Keys.A))
            {
                direction -= Vector2.UnitX;
            }
            if (keyboard.IsDown(Keys.D))
            {
                direction += Vector2.UnitX;
            }
            if (keyboard.IsNewDown(Keys.W))
            {
                direction -= gravity*25;
            }
            AddForce(direction);

            var sat = new SATHelper();
            // check for collisions. 
            if (_corners.Length > 0)
            lvl.Walls.ForEach(wall =>
            {
                var info = sat.Check(_corners, wall.Corners);
                var normal = info.Normal;

                if (info.IsOverlapping && wall.BaseColor != services.RenderService.Strategy.GetPass<BackgroundPass>().Color)
                {

                    // should we flip the normal ? The normal should always point TOWARDS the center of the player.
                    // C is center of player (position)
                    // 
                    var normalFlipper = 1;
                    if (normal.Dot(wall.Corners[0] - Position) < 0) normalFlipper = -1;

                    var penetration = info.Penetration * normalFlipper;
                    Position -= penetration;

                    var coef = 1f;
                    if (Velocity.Length() < 5)
                    {
                        coef = 1;
                    }

                    //AddForce(coef * Vector2.Reflect(Velocity, info.Normal));
                    var bounceForce = -coef * Velocity.Dot(normal) * normal;
                    AddForce( bounceForce );
                }
            });


            base.Update(time, services);

            // generate corners.
            _corners = new Vector2[]
            {
                Position + -halfSizeVec,
                Position + new Vector2(halfSizeVec.X, -halfSizeVec.Y),
                Position + halfSizeVec,
                Position + new Vector2(-halfSizeVec.X, halfSizeVec.Y)
            };

            
            if (keyboard.IsNewDown(Keys.D1))
            {
                services.RenderService.Strategy.GetPass<BackgroundPass>().ChangeColor(Position, Color.Red);
            }
            if (keyboard.IsNewDown(Keys.D2))
            {
                services.RenderService.Strategy.GetPass<BackgroundPass>().ChangeColor(Position, Color.Green);
            }
            if (keyboard.IsNewDown(Keys.D3))
            {
                services.RenderService.Strategy.GetPass<BackgroundPass>().ChangeColor(Position, Color.Blue);
            }
        }

        private void Render(RenderArgs<ColorPass> args)
        {
            var p = args.Pass.PBatch;
            p.Begin(PrimitiveType.TriangleStrip);
            _corners.ToList().ForEach(c => p.AddVertex(c, Color.White));
            p.End();
        }
    }
}
