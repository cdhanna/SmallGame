using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;
using SmallGame.GameObjects;
using SmallGame.Render;
using SmallGame.Services;
using TestGame.Rendering;

namespace TestGame.GameObjects
{
    class Wall : GameObject
    {

        public Vector2[] Corners { get; private set; }
        public Color BaseColor { get; private set; }

        public Wall() // constructor for JSON parsing.
        {
            
        }

        public Wall(TestLevel lvl, Color baseColor, Vector2 TopLeft, Vector2 BottomRight)
        {
            Corners = new Vector2[4];
            Corners[0] = TopLeft;
            Corners[1] = new Vector2(BottomRight.X, TopLeft.Y);
            Corners[2] = BottomRight;
            Corners[3] = new Vector2(TopLeft.X, BottomRight.Y);
            BaseColor = baseColor;
            lvl.AddWall(this);
        }

        protected override void OnInit(GameServices services)
        {
            services.RenderService.Strategy.GetPass<ColorPass>().ColorActions[BaseColor].Add(RenderFunc);
            base.OnInit(services);
        }

        private void RenderFunc(RenderArgs<ColorPass> args)
        {
            args.Pass.PBatch.Begin(PrimitiveType.TriangleStrip);
            Corners.ToList().ForEach(v => 
                args.Pass.PBatch.AddVertex(v, BaseColor)
            );
            args.Pass.PBatch.End();
        }
    }
}
