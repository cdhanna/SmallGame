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

    public class Camera2DState : GameObjectState
    {
        private Matrix _transform;
        private float _zoom, _zoomAcc, _zoomVel, _zoomForceSum;

        public Camera2DState()
        {
            
        }

        public Camera2DState(Camera2DState clone)
        {
            Transform = clone.Transform;
            Zoom = clone.Zoom;
            ZoomVelocity = clone.ZoomVelocity;
            ZoomAcceleration = clone.ZoomAcceleration;
            ZoomForceSum = clone.ZoomForceSum;
        }

        public Matrix Transform
        {
            get { return _transform; }
            set
            {
                FailIfLocked();
                _transform = value;
            }
        }

        public float Zoom
        {
            get { return _zoom; }
            set
            {
                FailIfLocked();
                _zoom = value;
            }
        }

        public float ZoomVelocity
        {
            get { return _zoomVel; }
            set
            {
                FailIfLocked();
                _zoomVel = value;
            }
        }

        public float ZoomAcceleration
        {
            get { return _zoomAcc; }
            set
            {
                FailIfLocked();
                _zoomAcc = value;
            }
        }

        public float ZoomForceSum
        {
            get { return _zoomForceSum; }
            set
            {
                FailIfLocked();
                _zoomForceSum = value;
            }
        }
    }

    public class Camera2DStrategy
    {
        public Action<Camera2DState, BasicObjectState> Action;

        public Camera2DStrategy()
        {
            Action = (state, objectState) => { };
        }


        public static class Common
        {
            public static Camera2DStrategy Follow(GameServices services, BasicObject gob)
            {
                return new Camera2DStrategy()
                {
                    Action = (camState, basicState) =>
                    {
                        var gobState = services.StateUpdator.Get<BasicObjectState>(gob);
                        basicState.Position = gobState.Position;
                    }
                };
            }

            public static Camera2DStrategy Idle()
            {
                return new Camera2DStrategy();
            }
        }
    }

    public class Camera2D : BasicObject
    {
        public Camera2DState InitialState { get; private set; } 
        //public Matrix Transform { get; private set; }

        private Vector2 _screenSize;

        /// <summary>
        /// The fixture is the position the camera rotates around. Built to be center of screen.
        /// </summary>
        public Vector2 Fixture { get; private set; }

        public Camera2DStrategy Strategy { get; set; }

        //public float Zoom { get; set; }
        //public float ZoomVelocity { get; set; }
        //public float ZoomAcceleration { get; set; }
        //public float ZoomForceSum { get; set; }

        public Camera2D() 
        {
            InitialState = new Camera2DState()
            {
                Zoom = 1,
                Transform = Matrix.Identity
            };
        }

        //public void Move(Vector2 inc)
        //{
        //    // add inc to position such that we always move 'right', iff inc is unitx. 
        //    // this means we need to not just add the inc to the position, but convert it into a vector
        //    // that points to the right, currently. 
        //    // rotate the vector around the Angle. 

        //    var rotation = Matrix.CreateRotationZ(-Angle);
        //    var inc3 = Vector3.Transform(new Vector3(inc.X, inc.Y, 0), rotation);
        //    inc = new Vector2(inc3.X, inc3.Y);
        //    ForceSum += inc;

        //}

        protected override void OnInit(GameServices services)
        {
            //Transform = Matrix.Identity;
            _screenSize = new Vector2(
                services.RenderService.Graphics.PresentationParameters.BackBufferWidth,
                services.RenderService.Graphics.PresentationParameters.BackBufferHeight
                    );

            Fixture = _screenSize / 2;
            //Zoom = 1;


            GameObjectState.Chain(services.StateUpdator, this, InitialState, CameraUpdate);
            GameObjectState.Chain(services.StateUpdator, this, base.InitialState, FrictionUpdate);
            base.OnInit(services);
        }

        private BasicObjectState FrictionUpdate(UpdateArgs args, BasicObjectState prev)
        {
            var next = new BasicObjectState(prev);
            next.ForceSum -= prev.Velocity*.1f;
            next.TorqueSum -= prev.Omega*.1f;
            return next;
        }

        //private Tuple<BasicObjectState, Camera2DState> Update(UpdateArgs args,
        //    Tuple<BasicObjectState, Camera2DState> prev)
        //{
            
        //}

        private Camera2DState CameraUpdate(UpdateArgs args, Camera2DState prev)
        {
            var next = new Camera2DState(prev);
            var basic = args.Services.StateUpdator.Get<BasicObjectState>(this);
           
            next.ZoomForceSum += -prev.ZoomVelocity * .1f;

            // f = ma. a = f/m

            next.ZoomAcceleration = prev.ZoomForceSum /  basic.Mass;
            next.ZoomVelocity += prev.ZoomAcceleration;
            next.Zoom += prev.ZoomVelocity;
            next.ZoomForceSum = 0;

            next.Transform = Matrix.Identity;

            Fixture = -basic.Position - _screenSize / 2;
            //Fixture = Position;

            var zoomAdj = prev.Zoom < .1f ? .1f : prev.Zoom;
            next.Zoom = zoomAdj;

            next.Transform *= MatrixExtensions.CreateTranslation(Fixture);
            next.Transform *= Matrix.CreateScale(zoomAdj);
            next.Transform *= Matrix.CreateRotationZ(basic.Angle);
            next.Transform *= MatrixExtensions.CreateTranslation(-Fixture);

            next.Transform *= MatrixExtensions.CreateTranslation(-basic.Position.X, -basic.Position.Y);

            return next;
        }

        //protected override void Update(GameTime time, GameServices services)
        //{

        //    // friction up.
        //    ForceSum += -Velocity*.1f;
        //    TorqueSum += -Omega*.1f;
        //    ZoomForceSum += -ZoomVelocity*.1f;

        //    // f = ma. a = f/m

        //    ZoomAcceleration = ZoomForceSum/Mass;
        //    ZoomVelocity += ZoomAcceleration;
        //    Zoom += ZoomVelocity;
        //    ZoomForceSum = 0;

        //    Transform = Matrix.Identity;

        //    Fixture = -Position - _screenSize/2;
        //    //Fixture = Position;

        //    var zoomAdj = Zoom < .1f ? .1f : Zoom;
        //    Zoom = zoomAdj;

        //    Transform *= MatrixExtensions.CreateTranslation(Fixture);
        //    Transform *= Matrix.CreateScale(zoomAdj);
        //    Transform *= Matrix.CreateRotationZ(Angle);
        //    Transform *= MatrixExtensions.CreateTranslation(-Fixture);

        //    Transform *= MatrixExtensions.CreateTranslation(-Position.X, -Position.Y);



        //    base.Update(time, services);
        //}
    }
}
