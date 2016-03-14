using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmallGame;
using SmallGame.Services;

namespace SmallPlatform.Objects
{
    class SpriteObject : BasicObject
    {

        public string MediaPath { get; set; }
        public Color Color { get; set; }
        public Vector2 Scale { get; set; }

        public Vector2 Offset
        {
            get { return _offset; }
            set
            {
                _offsetChanged = true;
                _offset = value;
            }
        }


        private Texture2D _tex;
        private bool _offsetChanged = false;
        private Vector2 _offset;


        public SpriteObject()
        {
            Color = Color.White;
            Scale = Vector2.One;
            _offset = Vector2.Zero;
        }

        protected override void OnInit(GameServices services)
        {
            _tex = services.ResourceService.Load<Texture2D>(MediaPath);
            if (!_offsetChanged)
            {
                Offset = new Vector2(_tex.Width, _tex.Height) / 2;    
            }
            services.RenderService.OnRender += Render;

            base.OnInit(services);
        }

        protected override void OnKill(GameServices services)
        {
            //services.RequestService<RenderService>().OnRender -= Render;
            base.OnKill(services);
        }

        protected void Render(object sender, RenderEventArgs args)
        {
          
            args.SpriteBatch.Draw(_tex, Position, null, Color, Angle, Offset, Scale, SpriteEffects.None, 1f);
        }


    }
}
