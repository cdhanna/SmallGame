using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmallGame;
using SmallPlatform.Objects;

namespace SmallPlatform.Levels
{
    internal class TestLevel : GameLevel
    {

        public List<Geometry> Geometrys
        {
            get { return Objects.GetObjects<Geometry>(); }
        }
    }

}
