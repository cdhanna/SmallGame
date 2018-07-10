using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmallGame;
using SmallGame.Render;
using SmallGame.Services;

namespace SmallGame.GameObjects
{
    public class SpriteObjectState : GameObjectState
    {
        private Color _color;
        private Vector2 _scale, _offset;

        public SpriteObjectState()
        {
            
        }

        public SpriteObjectState(SpriteObjectState clone)
        {
            Color = clone.Color;
            Scale = clone.Scale;
            Offset = clone.Offset;
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                FailIfLocked();
                _color = value;
            }
        }

        public Vector2 Scale
        {
            get { return _scale; }
            set
            {
                FailIfLocked();
                _scale = value;
            }
        }

        public Vector2 Offset
        {
            get { return _offset; }
            set
            {
                FailIfLocked();
                _offset = value;
            }
        }
    }

    /// <summary>
    /// A SpriteObject is a BasicObject that has a Texture2D and subscribes to the Render service.
    /// </summary>
    public class SpriteObject : BasicObject
    {
        /// <summary>
        /// Gets or Sets the path to the image file used as the sprite
        /// </summary>
        public string MediaPath { get; set; }

        ///// <summary>
        ///// Gets or Sets the Color used to tint the sprite
        ///// </summary>
        //public Color Color { get; set; }

        ///// <summary>
        ///// Gets or Sets the Scale of the sprite.
        ///// </summary>
        //public Vector2 Scale { get; set; }

        ///// <summary>
        ///// Gets or Sets the Offset amount for the sprite. 
        ///// </summary>
        //public Vector2 Offset
        //{
        //    get { return _offset; }
        //    set
        //    {
        //        _offsetChanged = true;
        //        _offset = value;
        //    }
        //}

        public SpriteObjectState InitialState { get; private set; }

        public bool OnCamera { get; set; }


        private Texture2D _tex;
        private bool _offsetChanged = false;
        private Vector2 _offset;

        /// <summary>
        /// Constructs the SpriteObject. By default, the Color is white, the Scale is Vector.One, and the offset is zero.
        /// </summary>
        public SpriteObject()
        {
            InitialState = new SpriteObjectState()
            {
                Color = Color.White,
                Scale = Vector2.One,
                Offset = Vector2.Zero
            };
          
            OnCamera = false;
        }

        /// <summary>
        /// Inits the SpriteObject. Unless set in JSON data or earlier, the Offset property will be set to the center of the image
        /// </summary>
        /// <param name="services">The GameServices used to configure this object</param>
        protected override void OnInit(GameServices services)
        {
            _tex = services.ResourceService.Load<Texture2D>(MediaPath);
            //if (!_offsetChanged)
            //{
            InitialState.Offset = new Vector2(_tex.Width, _tex.Height) / 2;    
            //}
            
            //services.RenderService.OnRender += Render;

            //services.RenderService.Strategy.GetPass<SimpleSpritePass>()

            if (OnCamera)
            {
                services.RenderService.GetPass<SimpleSpritePass>()
                    .AddCameraAction(OnRender);
            }
            else
            {
                services.RenderService.GetPass<SimpleSpritePass>()
                    .AddAction(OnRender);    
            }
            

            base.OnInit(services);
        }

        protected override void OnKill(GameServices services)
        {
            //services.RequestService<RenderService>().OnRender -= Render;
            base.OnKill(services);
        }

        protected virtual void OnRender(RenderArgs<SimpleSpritePass> args)
        {

            var objState = args.Services.StateUpdator.Get<BasicObjectState>(this);
            var sprState = args.Services.StateUpdator.Get<SpriteObjectState>(this);

            args.Pass.SpriteBatch.Draw(_tex, 
                objState.Position,
                null,
                sprState.Color,
                objState.Angle,
                sprState.Offset,
                sprState.Scale,
                SpriteEffects.None,
                0f);  
        }

    }
}
