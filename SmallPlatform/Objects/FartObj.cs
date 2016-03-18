using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmallGame.GameObjects;
using SmallGame.Render;
using SmallGame.Services;

namespace SmallPlatform.Objects
{
    class FartObj : BasicObject
    {
        protected override void OnInit(GameServices services)
        {

            var strategy = services.RenderService.Strategy;
       


            base.OnInit(services);
        }
    }
}
