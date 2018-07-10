using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using SmallGame;
using SmallGame.Services;

namespace SmallGame.GameObjects
{

    public class BasicObjectState : GameObjectState
    {
        private Vector2 _position, _velocity, _acceleration, _forceSum;
        private float _angle, _omega, _alpha, _torqueSum;
        private float _mass, _inertia;

        /// <summary>
        ///  no arg constructor
        /// </summary>
        public BasicObjectState()
        {
            
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="prev">state to clone</param>
        public BasicObjectState(BasicObjectState prev)
        {
            Position = prev.Position;
            Velocity = prev.Velocity;
            Acceleration = prev.Acceleration;
            ForceSum = prev.ForceSum;

            Angle = prev.Angle;
            Omega = prev.Omega;
            Alpha = prev.Alpha;
            TorqueSum = prev.TorqueSum;

            Mass = prev.Mass;
            Inertia = prev.Inertia;
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                FailIfLocked();
                _position = value;
            }
        }

        public Vector2 Velocity
        {
            get { return _velocity; }
            set
            {
                FailIfLocked();
                _velocity = value;
            }
        }

        public Vector2 Acceleration
        {
            get { return _acceleration; }
            set
            {
                FailIfLocked();
                _acceleration = value;
            }
        }

        public Vector2 ForceSum
        {
            get { return _forceSum; }
            set
            {
                FailIfLocked();
                _forceSum = value;
            }
        }

        public float Mass
        {
            get { return _mass; }
            set
            {
                FailIfLocked();
                _mass = value;
            }
        }

        public float Inertia
        {
            get { return _inertia; }
            set
            {
                FailIfLocked();
                _inertia = value;
            }
        }
        public float Angle
        {
            get { return _angle; }
            set
            {
                FailIfLocked();
                _angle = value;
            }
        }
        public float Omega
        {
            get { return _omega; }
            set
            {
                FailIfLocked();
                _omega = value;
            }
        }
        public float Alpha
        {
            get { return _alpha; }
            set
            {
                FailIfLocked();
                _alpha = value;
            }
        }
        public float TorqueSum
        {
            get { return _torqueSum; }
            set
            {
                FailIfLocked();
                _torqueSum = value;
            }
        }

    }

    /// <summary>
    /// A BasicObject is a GameObject that subscribes to the Update event, and has basic motion capabilities
    /// </summary>
    public class BasicObject : GameObject
    {
        public BasicObjectState InitialState { get; private set; }
        ///// <summary>
        ///// Gets or Sets the Position of the GameObject
        ///// </summary>
        //public Vector2 Position { get; set; }

        ///// <summary>
        ///// Gets or Sets the Velocity of the GameObject
        ///// </summary>
        //public Vector2 Velocity { get; set; }

        ///// <summary>
        ///// Gets or Sets the Acceleration of the GameObject
        ///// </summary>
        //public Vector2 Acceleration { get; set; }

        ///// <summary>
        ///// Gets or Sets the ForceSum of the GameObject
        ///// </summary>
        //public Vector2 ForceSum { get; set; }


        ///// <summary>
        ///// Gets or Sets the Angle of the GameObject
        ///// </summary>
        //public float Angle { get; set; }

        ///// <summary>
        ///// Gets or Sets the Omega of the GameObject
        ///// </summary>
        //public float Omega { get; set; }

        ///// <summary>
        ///// Gets or Sets the Alpha of the GameObject
        ///// </summary>
        //public float Alpha { get; set; }

        ///// <summary>
        ///// Gets or Sets the TorqueSum of the GameObject
        ///// </summary>
        //public float TorqueSum { get; set; }


        ///// <summary>
        ///// Gets or Sets the Mass of the GameObject
        ///// </summary>
        //public float Mass { get; set; } // TODO enforce positive mass.

        ///// <summary>
        ///// Gets or Sets the Inertia of the GameObject
        ///// </summary>
        //public float Inertia { get; set; } // TODO enforce positive intertia.

        /// <summary>
        /// Constructs the BasicObject, and sets all fields to zero.
        /// </summary>
        public BasicObject()
        {
            InitialState = new BasicObjectState()
            {
                Position = Vector2.Zero,
                Velocity = Vector2.Zero,
                Acceleration = Vector2.Zero,
                ForceSum = Vector2.Zero,

                Angle = 0,
                Omega = 0,
                Alpha = 0,
                TorqueSum = 0,

                Mass = 1,
                Inertia = 1
            };
        }

        //protected Vector2 AddForce(Vector2 force)
        //{
        //    ForceSum += force;
        //    return ForceSum;
        //}

        /// <summary>
        /// Inits the BasicObject, and subscribes to the UpdateService
        /// </summary>
        /// <param name="services">The GameServices used to configure the object.</param>
        protected override void OnInit(GameServices services)
        {
            //services.UpdateService.OnUpdate += (s, a) => Update(a.GameTime, a.Services);

            
            //services.StateUpdator.Add(this, InitialState, StateUpdate);
            GameObjectState.Chain(services.StateUpdator, this, InitialState, BasicUpdate);
            //services.RequestService<StateUpdater>().Subscribe(this, GetInitialState(), UpdateState);
        }


        protected BasicObjectState BasicUpdate(UpdateArgs args, BasicObjectState prev)
        {
            var next = new BasicObjectState(prev);

            next.Acceleration = prev.ForceSum / prev.Mass;
            next.Velocity += prev.Acceleration;
            next.Position += prev.Velocity;
            next.ForceSum = Vector2.Zero;

            next.Alpha = prev.TorqueSum / prev.Inertia;
            next.Omega += prev.Alpha;
            next.Angle += prev.Omega;
            next.TorqueSum = 0;

            return next;
        }

        /// <summary>
        /// The Update function is run when the UpdateService invokes. 
        /// By default, the motion properties are updated by resolving new acceleraions from force
        /// and torque sums, and then propegated down through velocity and position.
        /// The force and torque sums are set to zero at the end of the function.
        /// </summary>
        /// <param name="time">The current GameTime</param>
        /// <param name="services">The current GameServices</param>
        //protected virtual void Update(GameTime time, GameServices services)
        //{

        //    services.ScriptService.Run(Script, this);

        //    Acceleration = ForceSum/Mass;
        //    Velocity += Acceleration;
        //    Position += Velocity;
        //    ForceSum = Vector2.Zero;

        //    Alpha = TorqueSum/Inertia;
        //    Omega += Alpha;
        //    Angle += Omega;
        //    TorqueSum = 0;
        //}

    }
}
