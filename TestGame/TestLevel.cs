using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmallGame;
using TestGame.GameObjects;

namespace TestGame
{
    class TestLevel : GameLevel
    {
        public List<Wall> Walls
        {
            get { return Objects.GetObjects<Wall>(); }
        }


        public Wall AddWall(Wall wall)
        {
            Objects.Add(wall);
            return wall;
        }



        public void SetPlayer(Player player)
        {
            Objects.GetObjects<Player>().ForEach(p => p.Kill());
            Objects.Add(player);
        }
    }
}
