using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SmallGame.Input;
using SmallGame.Render;
using SmallGame.Services;

namespace SmallGame
{
    public class CoreGame : Game
    {
        public DataLoader DataLoader { get; private set; }
        public GameLevel Level { get; private set; }
        public GraphicsDeviceManager Graphics { get; private set; }

        //public PrimitiveBatch PrimitiveBatch { get; private set; }
        //public SpriteBatch SpriteBatch { get; private set; }

        public GameServices Services { get; private set; }
        public GameTime Time { get; private set; }

        // ease of use accessors. 
        public IScriptService ScriptService { get { return Services.ScriptService; } }
        public IUpdateService UpdateService { get { return Services.UpdateService; } }
        public IRenderService RenderService { get { return Services.RenderService; } }
        public IResourceService ResourceService { get { return Services.ResourceService; } }

        public KeyboardHelper KeyboardHelper { get { return Services.RequestService<KeyboardHelper>(); } }

        public CoreGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            DataLoader = new DataLoader();
            Services = new GameServices();


            Services.RegisterService<IResourceService>(new ResourceService());
            Services.RegisterService<IRenderService>(new RenderService());
            Services.RegisterService<IUpdateService>(new UpdateService());
            Services.RegisterService<IScriptService>(new ScriptService());

            Services.RegisterService(new KeyboardHelper());

            ScriptService.RegisterParameterHandler(() => Time);


            //RenderService.Strategy.AddPass(new DefaultRenderPass("default"));

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
            RenderService.Empty();
            UpdateService.Empty();
            KeyboardHelper.Empty();

            Level = null;
        }

        protected override void Initialize()
        {
            //PrimitiveBatch = new PrimitiveBatch(GraphicsDevice);
            //SpriteBatch = new SpriteBatch(GraphicsDevice);

            ResourceService.Configure(Content);

            RenderService.Configure(Services, GraphicsDevice);
            //RenderService.Strategy.AddPass(new SimpleSpritePass());
            //RenderService.Strategy.AddPass(new SimplePrimtivePass());
            RenderService.Init();

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            try
            {
                Time = gameTime;
                KeyboardHelper.Update();
                ScriptService.Update(Time);

                UpdateService.Update(gameTime, Services);
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
            RenderService.Render();
            base.Draw(gameTime);
        }
    }
}
