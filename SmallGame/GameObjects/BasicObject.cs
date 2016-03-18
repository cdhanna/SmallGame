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

    /// <summary>
    /// A BasicObject is a GameObject that subscribes to the Update event, and has basic motion capabilities
    /// </summary>
    public class BasicObject : GameObject
    {

        /// <summary>
        /// Gets or Sets the Position of the GameObject
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or Sets the Velocity of the GameObject
        /// </summary>
        public Vector2 Velocity { get; set; }

        /// <summary>
        /// Gets or Sets the Acceleration of the GameObject
        /// </summary>
        public Vector2 Acceleration { get; set; }

        /// <summary>
        /// Gets or Sets the ForceSum of the GameObject
        /// </summary>
        public Vector2 ForceSum { get; set; }


        /// <summary>
        /// Gets or Sets the Angle of the GameObject
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// Gets or Sets the Omega of the GameObject
        /// </summary>
        public float Omega { get; set; }

        /// <summary>
        /// Gets or Sets the Alpha of the GameObject
        /// </summary>
        public float Alpha { get; set; }

        /// <summary>
        /// Gets or Sets the TorqueSum of the GameObject
        /// </summary>
        public float TorqueSum { get; set; }


        /// <summary>
        /// Gets or Sets the Mass of the GameObject
        /// </summary>
        public float Mass { get; set; } // TODO enforce positive mass.

        /// <summary>
        /// Gets or Sets the Inertia of the GameObject
        /// </summary>
        public float Inertia { get; set; } // TODO enforce positive intertia.

        /// <summary>
        /// Constructs the BasicObject, and sets all fields to zero.
        /// </summary>
        public BasicObject()
        {
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            Acceleration = Vector2.Zero;
            ForceSum = Vector2.Zero;

            Angle = 0;
            Omega = 0;
            Alpha = 0;
            TorqueSum = 0;

            Mass = 1;
            Inertia = 1;
        }

        /// <summary>
        /// Inits the BasicObject, and subscribes to the UpdateService
        /// </summary>
        /// <param name="services">The GameServices used to configure the object.</param>
        protected override void OnInit(GameServices services)
        {
            services.UpdateService.OnUpdate += (s, a) => Update(a.GameTime, a.Services);
        }

        /// <summary>
        /// The Update function is run when the UpdateService invokes. 
        /// By default, the motion properties are updated by resolving new acceleraions from force
        /// and torque sums, and then propegated down through velocity and position.
        /// The force and torque sums are set to zero at the end of the function.
        /// </summary>
        /// <param name="time">The current GameTime</param>
        /// <param name="services">The current GameServices</param>
        public virtual void Update(GameTime time, GameServices services)
        {

            services.ScriptService.Run(Script, this);

            Acceleration = ForceSum/Mass;
            Velocity += Acceleration;
            Position += Velocity;
            ForceSum = Vector2.Zero;

            Alpha = TorqueSum/Inertia;
            Omega += Alpha;
            Angle += Omega;
            TorqueSum = 0;
        }

    }
}
