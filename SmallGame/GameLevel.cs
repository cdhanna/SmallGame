using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SmallGame
{
    public abstract class GameLevel
    {

        public GameObjectCollection Objects { get; set; }
        public LevelData Data { get; set; }
        public string Name { get { return Data.Name; } }

        protected GameLevel()
        {
            Objects = new GameObjectCollection();
        }


    }
}
