using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmallGame;
using SmallGame.GameObjects;
using SmallGame.Render;
using SmallGame.Services;

namespace SmallPlatform.Objects
{

    struct Particle
    {
        public Vector2 Position;
        public Color Color;

        public Particle(Vector2 _position)
        {
            Position = _position;
            Color = Color.White;
        }
    }

    class ParticleSystem : BasicObject
    {

        private Texture2D _tex;
        private List<Particle> _particles;
        private Effect _effect;

        private VertexPositionColor[] buffer;

        private GraphicsDevice gfx;

        private Random r = new Random();

        private Texture2D _particleTex;

        protected override void OnInit(GameServices services)
        {
            _particles = new List<Particle>();

            _particles.Add(new Particle(new Vector2(100, 100)));

            //_particles.Add(new Particle(new Vector2(100, 100)));
            //_particles.Add(new Particle(new Vector2(10, 130)));

            //for (var ic = 0; ic < 3000000; ic++)
            //    NewParticle();


            buffer = new VertexPositionColor[_particles.Count * 3];
            var i = 0;
            _particles.ForEach(p =>
            {
                buffer[i++] = new VertexPositionColor(new Vector3(0, 0, 0), Color.White);
                buffer[i++] = new VertexPositionColor(new Vector3(0, 0, 1), Color.White);
                buffer[i++] = new VertexPositionColor(new Vector3(0, 0, 2), Color.White);
                //buffer[i++] = new VertexPositionColor(new Vector3(p.Position.X - 10, p.Position.Y - 10, 0), Color.Green);
                //buffer[i++] = new VertexPositionColor(new Vector3(p.Position.X + 10, p.Position.Y - 10, 0), Color.White);
                //buffer[i++] = new VertexPositionColor(new Vector3(p.Position.X, p.Position.Y, 0), Color.White);
            });


            _tex = services.ResourceService.Load<Texture2D>("Content\\bullet.png");
            _effect = services.ResourceService.Load<Effect>("Content\\Shaders\\test.fx.mgfxo");

            gfx = services.RenderService.Graphics;

            _particleTex = new Texture2D(gfx, 10, 10, false, SurfaceFormat.Color);
            _particleTex.SetData(new Color[]{new Color(100, 0, 0)}, 0, 1);
        


            services.RenderService.GetPass<SimpleSpritePass>().AddStandaloneAction(Render);
      

            Matrix world = Matrix.CreateTranslation(0, 0, 0);
            Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.Forward, Vector3.Up);
            Matrix projection = Matrix.CreateOrthographicOffCenter
                (0, gfx.Viewport.Width,
                gfx.Viewport.Height, 0,
                0, 1);

            _effect.Parameters["World"].SetValue(world);
            _effect.Parameters["View"].SetValue(view);
            _effect.Parameters["Projection"].SetValue(projection);

            services.UpdateService.OnUpdate += OnUpdate;

            base.OnInit(services);
        }

        private void OnUpdate(object sender, UpdateEventArgs updateEventArgs)
        {
            //_particles = _particles.Select(p => new Particle(p.Position + Vector2.UnitX * (float)(r.NextDouble()-.5f))).ToList();
        }


        private void NewParticle()
        {
            _particles.Add(new Particle(new Vector2(r.Next(8000), r.Next(8000))));
        }

        private void Render(RenderArgs<SimpleSpritePass> args)
        {
            //var pass = args.Pass;
            //pb.Begin(PrimitiveType.TriangleStrip);
            //pb.Transform = Matrix.Identity;
            //_particles.ForEach(p => pb.AddVertex(p.Position, Color.White));
            //pb.End();
            //pass.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, _effect,
            //    args.Pass.Camera.Transform);

            _effect.CurrentTechnique = _effect.Techniques[0];
            _effect.Techniques[0].Passes[0].Apply();
            
            //var vertexBuffer = new VertexBuffer(args.Graphics, typeof(VertexPositionColor), 8, BufferUsage.None);


            //foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            //{
                //pass.Apply();
                //gfx.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, vertices, 0, 1);

            //}

            //_basicEffect.CurrentTechnique.Passes[0].Apply();
            _effect.Parameters["World"].SetValue(args.Pass.Camera.Transform);
            
            _effect.Parameters["ParticleData"].SetValue(_particleTex);

            //buffer = _particles.Select(p => {new VertexPositionColor(new Vector3(p.Position.X, p.Position.Y, 0), Color.Red)})
            //    .ToArray();



            gfx.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, buffer, 0, _particles.Count);


            //vertexBuffer.SetData(buffer);
            //var lineListIndexBuffer = new IndexBuffer(
            //    args.Graphics,
            //    IndexElementSize.SixteenBits,
            //    sizeof(short) * lineListIndices.Length,
            //    BufferUsage.None);


            //args.Graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, vertices, 0, 1);

            //_particles.ForEach(p =>
            //{
            //    pass.SpriteBatch.Draw(_tex, p.Position, Color.White);
            //});

            //pass.SpriteBatch.End();
        }
    }
}
