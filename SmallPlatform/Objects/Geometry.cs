using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmallGame;
using SmallGame.Services;

namespace SmallPlatform.Objects
{
    class Geometry : GameObject
    {
        //public Vector2 Point { get; set; }
        public List<Vector2> Points { get; set; }

        public Geometry()
        {
            Points = new List<Vector2>();
        }

        protected override void OnInit(GameServices services)
        {
            services.RenderService.OnRender += (s, a) => Render(a.PrimitiveBatch);
            base.OnInit(services);
        }

        public void Render(PrimitiveBatch primitiveBatch)
        {
            primitiveBatch.Begin(PrimitiveType.TriangleStrip);
            Points.ForEach(p => primitiveBatch.AddVertex(p, Color.Black));
            primitiveBatch.End();
        }
    }
}
