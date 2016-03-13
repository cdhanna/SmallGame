using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SmallGame
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

        public CoreGameServices()
        {
            _services = new Dictionary<Type, CoreGameService>();
        }

        public void RegisterService(CoreGameService service)
        {
            if (_services.ContainsKey(service.GetType()))
            {
                throw new Exception("That service has already been registered");
            }
            _services.Add(service.GetType(), service);
        }

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
