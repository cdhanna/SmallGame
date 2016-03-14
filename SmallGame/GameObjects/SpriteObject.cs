using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmallGame;
using SmallGame.Services;

namespace SmallGame.GameObjects
{
    /// <summary>
    /// A SpriteObject is a BasicObject that has a Texture2D and subscribes to the Render service.
    /// </summary>
    public class SpriteObject : BasicObject
    {
        /// <summary>
        /// Gets or Sets the path to the image file used as the sprite
        /// </summary>
        public string MediaPath { get; set; }

        /// <summary>
        /// Gets or Sets the Color used to tint the sprite
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or Sets the Scale of the sprite.
        /// </summary>
        public Vector2 Scale { get; set; }

        /// <summary>
        /// Gets or Sets the Offset amount for the sprite. 
        /// </summary>
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

        /// <summary>
        /// Constructs the SpriteObject. By default, the Color is white, the Scale is Vector.One, and the offset is zero.
        /// </summary>
        public SpriteObject()
        {
            Color = Color.White;
            Scale = Vector2.One;
            _offset = Vector2.Zero;
        }

        /// <summary>
        /// Inits the SpriteObject. Unless set in JSON data or earlier, the Offset property will be set to the center of the image
        /// </summary>
        /// <param name="services">The GameServices used to configure this object</param>
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

        /// <summary>
        /// Renders the sprite
        /// </summary>
        /// <param name="sender">idk, probably the renderService</param>
        /// <param name="args">The render arguments</param>
        protected virtual void Render(object renderService, RenderEventArgs args)
        {
          
            args.SpriteBatch.Draw(_tex, Position, null, Color, Angle, Offset, Scale, SpriteEffects.None, 1f);
        }


    }
}
