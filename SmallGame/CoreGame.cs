using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SmallGame
{
    public class CoreGame : Game
    {
        public DataLoader DataLoader { get; private set; }
        public GameLevel Level { get; private set; }
        public GraphicsDeviceManager Graphics { get; private set; }

        public PrimitiveBatch PrimitiveBatch { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }

        public CoreGameServices Services { get; private set; }
        public GameTime Time { get; private set; }

        public ScriptService ScriptService { get { return Services.RequestService<ScriptService>(); } }

        public CoreGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            DataLoader = new DataLoader();
            Services = new CoreGameServices();


            Services.RegisterService(new ResourceService());
            Services.RegisterService(new RenderService());
            Services.RegisterService(new UpdateService());
            Services.RegisterService(new ScriptService());

            ScriptService.RegisterParameterHandler(() => Time);
        }

        public T SetLevel<T>(T level) where T: GameLevel
        {
            EndLevel();
            Level = level;
            return level;
        }

        public void EndLevel()
        {
            if (Level == null) return;

            
            Level.Objects.Manage(Services, g => g.Kill());
            Services.RequestService<RenderService>().Empty();
            Services.RequestService<UpdateService>().Empty();
            Level = null;
        }

        protected override void Initialize()
        {
            PrimitiveBatch = new PrimitiveBatch(GraphicsDevice);
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            
            Services.RequestService<RenderService>().Configure(SpriteBatch, PrimitiveBatch);
            Services.RequestService<ResourceService>().Configure(Content);
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            try
            {
                Time = gameTime;
                ScriptService.Update(Time);

                Services.RequestService<UpdateService>().OnUpdate(this, new UpdateEventArgs(gameTime, Services));
                Level.Objects.Manage(Services);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Services.RequestService<RenderService>().Render();
            base.Draw(gameTime);
        }
    }
}
