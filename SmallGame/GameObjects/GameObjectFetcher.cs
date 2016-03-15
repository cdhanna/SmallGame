using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallGame.GameObjects
{

    public class GameObjectFetcher
    {
        public Func<GameObject, object> ComputeFunc { get; protected set; }

        public GameObjectFetcher()
        {
            
        }

        public void SetComputeFunction(Func<GameObject, object> computationFunc)
        {
            ComputeFunc = computationFunc;
        }

        public void ManageSet(List<GameObject> gobs)
        {
            
        }

    }
    
    public class GameObjectFetcher<T> : GameObjectFetcher
    {
        public void SetComputeFunction(Func<GameObject, T> computationFunc)
        {
            ComputeFunc = (gob) => computationFunc(gob);
        }
    }

    public class GameObjectFetcher<G, T> : GameObjectFetcher<T> where G : GameObject
    {

        public void SetComputeFunction(Func<G, T> computationFunc)
        {
            ComputeFunc = (gob) => computationFunc((G) gob);
        }
    }

    
}
