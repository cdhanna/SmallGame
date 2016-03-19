using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace SmallGame.Services
{
    public interface ILevelService: IGameService
    {
        GameLevel Level { get; }
        EventHandler OnLevelLoad { get; set; }
        EventHandler OnLevelEnd { get; set; }
        void Configure(GameServices services);
        T SetLevel<T>(T level) where T : GameLevel;
        void EndLevel();
        void Update();
    }

    internal class LevelService : ILevelService
    {
        public GameLevel Level { get; protected set; }
        public EventHandler OnLevelLoad { get; set; }
        public EventHandler OnLevelEnd { get; set; }
        private GameServices _services;
        public LevelService()
        {
            OnLevelEnd = (s, a) => { };
            OnLevelLoad = (s, a) => { };
        }

        public void Configure(GameServices services)
        {
            _services = services;
        }

        public T SetLevel<T>(T level) where T : GameLevel
        {
            EndLevel();
            Level = level;
            OnLevelLoad(this, new EventArgs());
            return level;
        }

        public void EndLevel()
        {
            if (Level == null) return;

            Level.Objects.Manage(_services, g => g.Kill());
            _services.RenderService.Empty();
            _services.UpdateService.Empty();
            //_services.KeyboardHelper.Empty();
            Level = null;

        }


        public void Update()
        {
            if (Level == null) return;
            Level.Objects.Manage(_services);
        }
    }
}
