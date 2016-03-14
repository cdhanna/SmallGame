using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using SmallGame;
using SmallGame.Services;

namespace SmallPlatform.Objects
{
    public class BasicObject : GameObject
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }
        public Vector2 ForceSum { get; set; }

        public float Angle { get; set; }
        public float Omega { get; set; }
        public float Alpha { get; set; }
        public float TorqueSum { get; set; }

        public float Mass { get; set; }
        public float Inertia { get; set; }

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

        protected override void OnInit(CoreGameServices services)
        {
            services.UpdateService.OnUpdate += (s, a) => Update(a.GameTime, a.Services);
        }

        public virtual void Update(GameTime time, CoreGameServices services)
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
