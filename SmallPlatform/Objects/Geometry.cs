using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmallGame;
using SmallGame.GameObjects;
using SmallGame.Render;
using SmallGame.Services;

namespace SmallPlatform.Objects
{
    class Geometry : GameObject
    {
        //public Vector2 Point { get; set; }
        public List<Vector2> Points { get; set; }
        public Color Color { get; set; }
        public bool OnCamera { get; set; }

        public Geometry()
        {
            Points = new List<Vector2>();
            Color = Color.Blue;
            OnCamera = true;
        }

        protected override void OnInit(GameServices services)
        {

            if (OnCamera)
            {
                services.RenderService.GetPass<SimplePrimtivePass>()
                    .AddCameraAction(Render);
            }
            else
            {
                services.RenderService.GetPass<SimplePrimtivePass>()
                    .AddAction(Render);
            }
            base.OnInit(services);
        }

        public void Render(RenderArgs<SimplePrimtivePass> args)
        {
            var primitiveBatch = args.Pass.PrimitiveBatch;
            primitiveBatch.Begin(PrimitiveType.TriangleStrip);
            Points.ForEach(p => primitiveBatch.AddVertex(p, Color));
            primitiveBatch.End();
        }
    }
}
