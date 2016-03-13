using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SmallGame
{
    public class UpdateEventArgs : EventArgs
    {
        public GameTime GameTime { get; set; }
        public CoreGameServices Services { get; set; }

        public UpdateEventArgs(GameTime time, CoreGameServices services)
        {
            GameTime = time;
            Services = services;
        }
    }
    public class UpdateService : CoreGameService
    {
        public EventHandler<UpdateEventArgs> OnUpdate { get; set; }

        public void Empty()
        {
            OnUpdate = (sender, args) => { };
        }

        public UpdateService()
        {
            OnUpdate = (sender, args) => { };
        }
    }
}
