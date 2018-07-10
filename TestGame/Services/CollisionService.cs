using Microsoft.Xna.Framework;
using SmallCommons;
using SmallGame;
using SmallGame.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestGame.Services
{

    //public delegate bool ObjectFilter(GameObject g);
    //public delegate bool ObjectFilter<T>(T g) where T : GameObject;

    //public static class MakeObjectFilter
    //{
    //    public static ObjectFilter ByType<T>()
    //    {
    //        return new ObjectFilter(new Func<GameObject, bool>(g => g.GetType().Equals(typeof(T))));
    //    }

    //    public static ObjectFilter ById(string name)
    //    {
    //        return new ObjectFilter(new Func<GameObject, bool>(g => g.Id.Equals(name)));
    //    }

    //    public static ObjectFilter ByCustom(Func<GameObject, bool> test)
    //    {
    //        return new ObjectFilter(test);
    //    }
    //}

    public class GobGroupGenerator
    {
        public Func<GameObjectCollection, List<GameObject>> Generator { get; private set; }

        public GobGroupGenerator(Func<GameObjectCollection, List<GameObject>> generator)
        {
            Generator = generator;
        }

        public static class Build
        {
            public static GobGroupGenerator ByType<T>() where T :GameObject
            {
                return new GobGroupGenerator(goc => goc.GetObjects<T>().ConvertAll(g => (GameObject)g));
            }
   
        }
    }


    public class CollisionService
    {


 

        private List<GameObject> _gobs;
        private Dictionary<GameObject, Func<Vector2[]>> _generators;

        private Dictionary<GameObject, List<GobGroupGenerator>> _listeners;

        private Dictionary<int, Dictionary<int, List<GameObject>>> _grid;
        private Dictionary<GameObject, List<Point>> _gridRef;
        private int _gridSize = 200;

        private SATHelper sat;

        public void Declare(GameObject gob, Func<Vector2[]> pointGenerator)
        {
            if (_generators.ContainsKey(gob)) throw new Exception("A generator has already been added");
            _generators.Add(gob, pointGenerator);


            _gobs.Add(gob);
        }

        public void ListenFor(GameObject gob, GobGroupGenerator filter)
        {

            if (!_gobs.Contains(gob)) throw new Exception("A gob must be registered to listen for collisions");

            if (!_listeners.ContainsKey(gob)) _listeners.Add(gob, new List<GobGroupGenerator>());
            _listeners[gob].Add(filter);

        }

        public void Update(GameObjectCollection goc)
        {
            // need to resolve all listeners. 

            // worst case scenario, we need to check everything against everything else. 

            // place all objects in the grid.
            _grid = new Dictionary<int, Dictionary<int, List<GameObject>>>();
            _gridRef = new Dictionary<GameObject, List<Point>>();
            _gobs.ForEach(g =>
            {
                var pts = _generators[g]();
                //var center = pts.Aggregate((agg, curr) => agg + curr) / pts.Count();

                pts.ToList().ForEach(v =>
                {
                    SetGrid(Snap(v.X), Snap(v.Y), g);
                });
            });

            // make groups for listeners
            _listeners.Keys.ToList().ForEach(lGob =>
            {
                var group = _listeners[lGob]
                    .Select(g => g.Generator(goc))
                    .Aggregate((agg, curr) => { agg.AddRange(curr); return agg; });

                var inGridGroup = _gridRef[lGob]
                    .Select(p => GetObjectsInCell(p.X, p.Y))
                    .Aggregate((agg, curr) => { agg.AddRange(curr); return agg; });

                group.ForEach(g =>
               {
                    //if (_gridRef[lGob].Contains( new Point(Snap())))
                    if (inGridGroup.Contains(g))
                   {
                       // BAM, we got some collision to do. 
                       var info = sat.Check(_generators[lGob](), _generators[g]());
                   }
               });

            });

            // check all objects in each grid cell. 
            //_grid.Keys.ToList().ForEach(x =>
            //{
            //    _grid[x].Keys.ToList().ForEach(y =>
            //    {
            //        _grid[x][y].ForEach(g =>
            //        {
            //            var gPts = _generators[g]();
            //            _grid[x][y].ForEach(g2 =>
            //            {
            //                if (g2 == g) return;
            //                var g2Pts = _generators[g2]();

            //                sat.Check(gPts, g2Pts);

            //            });
            //        });
            //    });
            //});
        }

        private void SetGrid(int x, int y, GameObject gob)
        {
            if (!_grid.ContainsKey(x)) _grid.Add(x, new Dictionary<int, List<GameObject>>());
            if (!_grid[x].ContainsKey(y)) _grid[x].Add(y, new List<GameObject>());
            _grid[x][y].Add(gob);

            if (!_gridRef.ContainsKey(gob)) _gridRef.Add(gob, new List<Point>());
            _gridRef[gob].Add(new Point(x, y));
        }

        private List<GameObject> GetObjectsInCell(int x, int y)
        {
            if (!_grid.ContainsKey(x)) return new List<GameObject>();
            if (!_grid[x].ContainsKey(y)) return new List<GameObject>();
            return _grid[x][y];
        }

        private int Snap(float coord)
        {
            // 1 -> 1, 2-> 1, 3->1, 3->2, 
            return ((int)coord) / _gridSize;
        }

    }
}
