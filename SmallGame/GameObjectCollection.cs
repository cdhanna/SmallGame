using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using SmallGame.GameObjects;
using SmallGame.Services;

namespace SmallGame
{

    public class GameObjectCollection
    {

        public Dictionary<string, List<GameObject>> Objects { get; set; }

        private Dictionary<string, GameObject> _objectMap; 

        public GameObjectCollection()
        {
            Objects = new Dictionary<string, List<GameObject>>();
            _objectMap = new Dictionary<string, GameObject>();
        }

        public void Add(GameObject obj)
        {
            if (obj == null) return; // do nothing with a null object.

            if (!Objects.ContainsKey(obj.Type))
            {
                Objects.Add(obj.Type, new List<GameObject>());
            }

            _objectMap.Add(obj.Id, obj);
            Objects[obj.Type].Add(obj);
        }


        //public List<GameObject> GetAll()
        //{
        //    var gobs = new List<GameObject>();
        //    Objects.Values.ToList().ForEach(g => gobs.Add(g));
        //    return gobs;
        //} 

        public List<T> GetObjects<T>() where T : GameObject
        {
            if (Objects.ContainsKey(typeof (T).Name))
            {
                return Objects[typeof (T).Name].ConvertAll(g => (T) g);
            }
            else
            {
                return new List<T>();
            }
        }

        public void Manage(GameServices services, Action<GameObject> action=null)
        {

            Objects.Values.ToList().ForEach(list =>
            {
                var deadList = new List<GameObject>();
                list.ForEach(obj =>
                {
                    if (action != null)
                    {
                        action(obj);
                    }
                    if (obj.State.Equals("new"))
                    {
                        obj.Init(services);
                    }
                    if (obj.State.Equals("dead"))
                    {
                        deadList.Add(obj);
                    }
                });
                deadList.ForEach(g => list.Remove(g));
                deadList.Clear();
            });

        }

    }
}
