using System;
using System.Collections.Generic;

namespace SmallGame.Services
{
    /// <summary>
    /// A GameService is fundamental part of the game architecture. It can be anything, and the IGameService is
    /// an empty interface, not enforcing any restrictions on what a game service can be, or do. 
    /// 
    /// Common examples of GameServices are the UpdateService, or RenderService. They are responsible for 
    /// controlling vast portions of the game logic.
    /// </summary>
    public interface IGameService
    {

    }

    /// <summary>
    /// GameServices is a collection of IGameService. It can hold one of each type of service. The GameServices class
    /// should be used to register and access services. 
    /// </summary>
    public class GameServices
    {

        private Dictionary<Type, IGameService> _services;

        // common accessors. Use extension methods to add more. 

        /// <summary>
        /// Gets the UpdateService attached to the GameServices collection. 
        /// </summary>
        public IUpdateService UpdateService { get { return RequestService<IUpdateService>(); } }

        /// <summary>
        /// Gets the ScriptService attached to the GameServices collection. 
        /// </summary>
        public IScriptService ScriptService { get { return RequestService<IScriptService>(); } }

        /// <summary>
        /// Gets the RenderService attached to the GameServices collection. 
        /// </summary>
        public IRenderService RenderService { get { return RequestService<IRenderService>(); } }

        /// <summary>
        /// Gets the ResourceService attached to the GameServices collection. 
        /// </summary>
        public IResourceService ResourceService { get { return RequestService<IResourceService>(); } }

        /// <summary>
        /// Gets the LevelService attached to the GameServices collection. 
        /// </summary>
        public ILevelService LevelService { get { return RequestService<ILevelService>(); } }

        public IStateUpdateService StateUpdator { get { return RequestService<IStateUpdateService>(); } }

        /// <summary>
        /// Constructs a new collection of IGameServices. 
        /// </summary>
        public GameServices()
        {
            _services = new Dictionary<Type, IGameService>();
        }

        /// <summary>
        /// Registering a service with the GameServices allows it to be retrieved later by its type. 
        /// The type of the service is how the service will be stored, and as such, each service type
        /// can only be registered once. Attempting to register two instances of the same service type
        /// will result in an exception. 
        /// </summary>
        /// <typeparam name="S">The type of service being registered. This must extend from IGameService</typeparam>
        /// <param name="service">An instance of S, the service being registered. </param>
        public void RegisterService<S>(S service) where S : IGameService
        {
            if (_services.ContainsKey(typeof(S)))
            {
                throw new Exception("That service has already been registered");
            }
            _services.Add(typeof(S), service);
        }

        /// <summary>
        /// Requesting a service will retrieve a service instance. However, if no service of the requested
        /// type has been registered, then an exception will be thrown. 
        /// </summary>
        /// <typeparam name="S">The type of service to request. This must extend from IGameService</typeparam>
        /// <returns>An instance of S, the requested service type</returns>
        public S RequestService<S>() where S : IGameService
        {
            if (_services.ContainsKey(typeof (S)))
            {
                return (S) _services[typeof (S)];
            }
            else throw new Exception("There is no service of type " + typeof(S).Name + ". Make sure you are requesting the EXACT same type used to register.");
        }

    }
}
