using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using SmallGame.Render;
using Microsoft.Xna.Framework;

namespace TestGame.Rendering
{
    class BackgroundCircle
    {
        public Vector2 Position;
        public Color Color;
        public float Scale;
    }

    class BackgroundPass : RenderPass<BackgroundPass>
    {
        public SpriteBatch SpriteBatch { get; private set; }
        public Texture2D CircleTex { get; private set; }

        public Vector2 Offset { get; private set; }
        public List<BackgroundCircle> Circles { get; private set; }

        public Color Color { get { return Circles.Last().Color; } }

        protected override void OnInit(GraphicsDevice graphics)
        {
            CircleTex = Services.ResourceService.Load<Texture2D>("Content\\circle.png");
            SpriteBatch = new SpriteBatch(graphics);
            Offset = new Vector2(CircleTex.Width, CircleTex.Height) / 2;
            Circles = new List<BackgroundCircle>();
        }

        protected override void OnRender(RenderStrategy strategy, List<Action<RenderArgs<BackgroundPass>>> Actions, RenderArgs<BackgroundPass> args)
        {
            Graphics.Clear(Color.White);
            SpriteBatch.Begin();
            Circles.ForEach(c =>
            {
                c.Scale += 1;
               SpriteBatch.Draw(CircleTex, c.Position, null, c.Color, 0f, Offset, c.Scale, SpriteEffects.None, 1);
            });
            if (Circles.Count > 15)
            {
                Circles.RemoveAt(0);
            }
            
            SpriteBatch.End();
        }

        public void ChangeColor(Vector2 position, Color color)
        {
            Circles.Add(new BackgroundCircle() {
                Position = position,
                Color = color,
                Scale = 0
            });
        }

    }
}
