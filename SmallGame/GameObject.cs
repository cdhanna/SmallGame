using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SmallGame.Services;

namespace SmallGame
{
    public abstract class GameObject
    {
        public string Type { get; internal set; }
        public string Id { get; internal set; }
        public string State { get; private set; }

        public string Script { get; set; }

        private CoreGameServices _services;

        protected GameObject()
        {
            State = "new";
        }

        public void Init(CoreGameServices services)
        {
            _services = services;

            

            OnInit(services);
            State = "ok";
        }

        public void Kill()
        {
            OnKill(_services);
           // _services = null;
            State = "dead";
        }

        protected virtual void OnInit(CoreGameServices services)
        {
            // do something?
        }

        protected virtual void OnKill(CoreGameServices services)
        {
            // meh...
        }

    }
}
