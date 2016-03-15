using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallGame.GameObjects
{
    class XFetcher : GameObjectFetcher<BasicObject, float>
    {
        public XFetcher()
        {
            SetComputeFunction(gob => gob.Position.X);
        }
    }
}
