using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SmallGame.Services
{

    /// <summary>
    /// An UpdateService can update objects every gameloop cycle.
    /// </summary>
    public interface IUpdateService : IGameService
    {
        /// <summary>
        /// Gets or Sets the eventhandler for recieveing updates
        /// </summary>
        EventHandler<UpdateEventArgs> OnUpdate { get; set; }

        /// <summary>
        /// Invokes the OnUpdate Event
        /// </summary>
        /// <param name="time">The GameTime that will be given to all subscribers</param>
        /// <param name="services">The GameServices that will be given to all subscribers</param>
        void Update(GameTime time, GameServices services);

        /// <summary>
        /// Clears all subscribers to the OnUpdate Event.
        /// </summary>
        void Empty();
    }

    public class UpdateEventArgs : EventArgs
    {
        public GameTime GameTime { get; set; }
        public GameServices Services { get; set; }

        public UpdateEventArgs(GameTime time, GameServices services)
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


        public void Update(GameTime time, GameServices services)
        {
            OnUpdate.Invoke(this, new UpdateEventArgs(time, services));
        }
    }
}
