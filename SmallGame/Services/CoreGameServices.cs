using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SmallGame.Services
{

    public interface CoreGameService
    {

    }

    public class CoreGameServices
    {

        //public EventHandler<UpdateEventArgs> OnUpdate { get; set; }

        //public CoreGameServices()
        //{
        //    OnUpdate = (sender, args) => { };
        //}

        private Dictionary<Type, CoreGameService> _services;

        // common accessors. Use extension methods to add more. 
        public IUpdateService UpdateService { get { return RequestService<IUpdateService>(); } }
        public IScriptService ScriptService { get { return RequestService<IScriptService>(); } }
        public IRenderService RenderService { get { return RequestService<IRenderService>(); } }
        public IResourceService ResourceService { get { return RequestService<IResourceService>(); } }

        public CoreGameServices()
        {
            _services = new Dictionary<Type, CoreGameService>();
        }

        public void RegisterService<S>(S service) where S : CoreGameService
        {
            if (_services.ContainsKey(typeof(S)))
            {
                throw new Exception("That service has already been registered");
            }
            _services.Add(typeof(S), service);
        }

        //public void RegisterService(CoreGameService service)
        //{
        //    if (_services.ContainsKey(service.GetType()))
        //    {
        //        throw new Exception("That service has already been registered");
        //    }
        //    _services.Add(service.GetType(), service);
        //}

        public S RequestService<S>() where S : CoreGameService
        {
            if (_services.ContainsKey(typeof (S)))
            {
                return (S) _services[typeof (S)];
            }
            else return default(S);
        }

    }
}
