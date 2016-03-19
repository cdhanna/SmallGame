using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SmallGame.Services;

namespace SmallGame.GameObjects
{

    /// <summary>
    /// A collection of string constants for representing GameObject state.
    /// Note that by default, a GameObject does not subscribe to update or render events. 
    /// </summary>
    public static class GameObjectState
    {
        public const string New = "new";
        public const string Ok = "ok";
        public const string Dead = "dead";
    }

    /// <summary>
    /// A GameObject is the primary object for all SmallGame Games. 
    /// Pretty much every object in game should inherit from this to get the benefits of SG. 
    /// </summary>
    public abstract class GameObject
    {

        /// <summary>
        /// Gets the type of this object. This is the Name of the class name of this object.
        /// </summary>
        public string Type { get; internal set; }

        /// <summary>
        /// Gets the Id of this object. This is either set from the level data, or is a random Guid.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the state of this object. "New", "Ok", or "Dead"
        /// </summary>
        public string State { get; private set; }

        /// <summary>
        /// Gets or Sets the Script attached to this GameObject.
        /// </summary>
        public string Script { get; set; }

        private GameServices _services;

        /// <summary>
        /// Constructs the GameObject. Sets State to New.
        /// </summary>
        protected GameObject()
        {
            State = "new";
            Type = GetType().Name;
            Id = IdFactory.NewId;
        }

        /// <summary>
        /// Initializes the GameObject. Sets State to Ok
        /// </summary>
        /// <param name="services">The GameServices used to configure the object.</param>
        public void Init(GameServices services)
        {
            _services = services;

            OnInit(services);
            State = "ok";
        }

        /// <summary>
        /// Kills the GameObject. Sets State to Dead.
        /// </summary>
        public void Kill()
        {
            OnKill(_services);
            State = "dead";
        }

        /// <summary>
        /// Invoked when the GameObject is Init'd.
        /// </summary>
        /// <param name="services">The GameServices used to configure the object</param>
        protected virtual void OnInit(GameServices services)
        {
            // do something?
        }

        /// <summary>
        /// Invoked when the GameObject is Killed
        /// </summary>
        /// <param name="services">The GameServices used to configure the object</param>
        protected virtual void OnKill(GameServices services)
        {
            // meh...
        }

    }
}
