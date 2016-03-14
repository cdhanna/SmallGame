using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SmallGame.Services
{

    public interface IUpdateService : CoreGameService
    {
        EventHandler<UpdateEventArgs> OnUpdate { get; set; }
        void Update(GameTime time, CoreGameServices services);
        void Empty();
    }

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
    public class UpdateService : IUpdateService
    {
        public EventHandler<UpdateEventArgs> OnUpdate { get; set; }

        public UpdateService()
        {
            Empty();
        }

        public void Empty()
        {
            OnUpdate = (sender, args) => { };
        }


        public void Update(GameTime time, CoreGameServices services)
        {
            OnUpdate.Invoke(this, new UpdateEventArgs(time, services));
        }
    }
}
