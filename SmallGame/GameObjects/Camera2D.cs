using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SmallGame.Services;

namespace SmallGame.GameObjects
{

    static class MatrixExtensions
    {
        public static Matrix CreateTranslation( Vector2 position)
        {
            return Matrix.CreateTranslation(position.X, position.Y, 0);
        }

        public static Matrix CreateTranslation(float x, float y)
        {
            return CreateTranslation(new Vector2(x, y));
        }
    }

    public class Camera2D : BasicObject
    {

        public Matrix Transform { get; private set; }

        private Vector2 _screenSize;

        /// <summary>
        /// The fixture is the position the camera rotates around. Built to be center of screen.
        /// </summary>
        public Vector2 Fixture { get; private set; }

        public float Zoom { get; set; }
        public float ZoomVelocity { get; set; }
        public float ZoomAcceleration { get; set; }
        public float ZoomForceSum { get; set; }

        public Camera2D() 
        {
            
        }


        protected override void OnInit(GameServices services)
        {
            Transform = Matrix.Identity;
            _screenSize = new Vector2(
                services.RenderService.Graphics.PresentationParameters.BackBufferWidth,
                services.RenderService.Graphics.PresentationParameters.BackBufferHeight
                    );

            Fixture = _screenSize / 2;
            Zoom = 1;

            base.OnInit(services);
        }

        public override void Update(GameTime time, GameServices services)
        {

            // friction up.
            ForceSum += -Velocity*.1f;
            TorqueSum += -Omega*.1f;
            ZoomForceSum += -ZoomVelocity*.1f;

            // f = ma. a = f/m

            ZoomAcceleration = ZoomForceSum/Mass;
            ZoomVelocity += ZoomAcceleration;
            Zoom += ZoomVelocity;
            ZoomForceSum = 0;

            Transform = Matrix.Identity;

            Fixture = -Position - _screenSize/2;
            //Fixture = Position;

            var zoomAdj = Zoom < .1f ? .1f : Zoom;
            Zoom = zoomAdj;

            Transform *= MatrixExtensions.CreateTranslation(Fixture);
            Transform *= Matrix.CreateScale(zoomAdj);
            Transform *= Matrix.CreateRotationZ(Angle);
            Transform *= MatrixExtensions.CreateTranslation(-Fixture);

            Transform *= MatrixExtensions.CreateTranslation(-Position.X, -Position.Y);



            base.Update(time, services);
        }
    }
}
