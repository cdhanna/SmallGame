using Microsoft.Xna.Framework;
using SmallGame.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallGame.Services
{
    public class GameObjectState
    {
        public static void Chain<S, G>(IStateUpdateService service, G gob, S initialState, Func<UpdateArgs, S, S> newFunc) where G : GameObject where S : GameObjectState
        {
            var old = service.GetFunc<S>(gob);
            service.Add(gob, initialState, (args, prev) =>
            {
                var next = old(args, prev);
                var next2 = newFunc(args, next);
                return next2;
            });
        }

        public static void Friction(IStateUpdateService service, BasicObject gob)
        {
            Chain(service, gob, gob.InitialState, (args, prev) =>
            {
                var next = new BasicObjectState(prev);
                next.Velocity -= prev.Velocity*.1f;

                return next;
            });
        }

        private bool _isLocked = false;

        /// <summary>
        /// Once an object has been 'Locked' with the Lock() method,
        /// any invocation of the FailIfLocked function will cause a runtime exception.
        /// 
        /// The Lock function will be called after the state updator function has returned a fresh state.
        /// DO NOT CALL THIS MANUALLY UNLESS YOU MUST.
        /// </summary>
        public void Lock()
        {
            _isLocked = true;
        }

        /// <summary>
        /// Each state should call this on every 'SET' 
        /// </summary>
        protected void FailIfLocked()
        {
            if (_isLocked)
            {
                throw new Exception("The object has been locked, and cannot be modified");
            }
        }

        /// <summary>
        /// Gets called when the state is about to gotten rid of. This is happens when the next state is inbound
        /// </summary>
        public virtual void OnDispose()
        {
            
        }

    }

    public class StateUpdateService
    {
        private Dictionary<GameObject, Tuple<Action<UpdateArgs>, Action, Dictionary<Type, GameObjectState>>> _actions;

        public StateUpdateService()
        {
            
        }

        private void Add(
            GameObject gob, 
            Func<UpdateArgs, Dictionary<Type, GameObjectState>, Dictionary<Type, GameObjectState>> func,
            params GameObjectState[] initalStates)
        {
            var currs = new Dictionary<Type, GameObjectState>();
            initalStates.ToList().ForEach(s => currs.Add(s.GetType(), s));

            var nexts = new Dictionary<Type, GameObjectState>();
            var update = new Action<UpdateArgs>(args =>
            {
                nexts = func(args, currs);
                nexts.Keys.ToList().ForEach(k => nexts[k].Lock());
            });
            var finish = new Action(() =>
            {
                currs = nexts;
            });

            _actions.Add(gob,
                new Tuple<Action<UpdateArgs>, Action, Dictionary<Type, GameObjectState>>(update, finish, currs));
        }

        public void Add<G, T>(G gob, T init, Func<UpdateArgs, T, T> transform)
            where G : GameObject
            where T : GameObjectState
        {
            Add(gob, (args, currs) =>
            {
                var outs = new Dictionary<Type, GameObjectState>();
                outs.Add(typeof(T), transform(args, currs[typeof(T)] as T));
                return outs;
            }, new GameObjectState[]{init});
        }

        public void Add<G, T1, T2>(G gob, T1 initT1, T2 initT2, Func<UpdateArgs, Tuple<T1, T2>, Tuple<T1, T2>> transform)
            where G : GameObject
            where T1 : GameObjectState
            where T2 : GameObjectState
        {
            Add(gob, (args, currs) =>
            {
                var outs = new Dictionary<Type, GameObjectState>();
                var output = transform(args, new Tuple<T1, T2>(currs[typeof (T1)] as T1, currs[typeof (T2)] as T2));
                outs.Add(typeof (T1), output.Item1);
                outs.Add(typeof (T2), output.Item2);
                return outs;
            }, new GameObjectState[]{initT1, initT2});
        }

        public void GetState<G, T>(G gob)
            where G : GameObject
            where T : GameObjectState
        {
            
        }

        //public void Add<G, T1, T2>(G gob, Tuple<T1, T2> inits, Func<Tuple<T1, T2>, Tuple<T1, T2>> funcs)
        //{
            
        //}

        public void Update(GameTime time, GameServices services)
        {
            var args = new UpdateArgs(time, services);

            var kList = _actions.Keys.ToList();
            kList.ForEach(k => _actions[k].Item1(args));
            kList.ForEach(k => _actions[k].Item2());

        }
    }

   
    //public interface IStateBundle<T> where T : GameObjectState
    //{
    //    void Update(UpdateArgs args);
    //    void Finish();
    //    void Clear();
    //    T State { get; }
    //    Func<UpdateArgs, T, T> Function { get; }
    //    //Func<UpdateArgs, GameObjectState, T> GetFunction();
    //}

    //public class StateBundle<T> : IStateBundle<T> where T : GameObjectState
    //{
    //    public T InitialState { get; set; }
    //    public Func<UpdateArgs, T, T> Function { get; set; }

    //    public T State { get; set; }
    //    public T NextState { get; set; }

    //    public StateBundle(T initState, Func<UpdateArgs, T, T> func )
    //    {
    //        InitialState = initState;
    //        Function = func;
    //        State = InitialState;
    //    }

    //    //public Func<UpdateArgs, GameObjectState, T> GetFunction()
    //    //{
           
    //    //} 

    //    public void Update(UpdateArgs args)
    //    {
    //        NextState = Function(args, State);
    //        NextState.Lock();
    //    }

    //    public void Finish()
    //    {
    //        NextState.OnDispose();
    //        State = NextState;
    //    }

    //    public void Clear()
    //    {
    //        State.OnDispose();
    //    }
    //}

    //public interface IStateUpdateService : IGameService
    //{
    //    void Clear();
    //    void Add<T>(GameObject gob, T initState, Func<UpdateArgs, T, T> updateFunc) where T : GameObjectState;
    //    void Remove<T>(GameObject gob) where T : GameObjectState;
    //    void Update(GameTime time, GameServices services);

    //    T Get<T>(GameObject gob) where T : GameObjectState;
    //    Func<UpdateArgs, T, T> GetFunc<T>(GameObject gob) where T : GameObjectState;
    //}

    //public class GameObjectStateService : IStateUpdateService
    //{
    //    private Dictionary<GameObject, Dictionary<Type, IStateBundle<GameObjectState>>> _gobTable;

    //    public GameObjectStateService()
    //    {
    //        Clear();
    //    }

    //    /// <summary>
    //    /// Removes all subscribers.
    //    /// All subscribers' state will get a disposal event notice.
    //    /// </summary>
    //    public void Clear()
    //    {
    //        Forall( b => b.Clear() );
    //        _gobTable = new Dictionary<GameObject, Dictionary<Type, IStateBundle<GameObjectState>>>();
    //    }

    //    /// <summary>
    //    /// Add a gob object state and modifier function. 
    //    /// A gob can only have one of each TYPE of state/func. If an attempt is made to add a TYPE that already exists, the old one will be overwritten with the new
    //    /// </summary>
    //    /// <typeparam name="T">The type of gameobjectstate </typeparam>
    //    /// <param name="gob">The game object</param>
    //    /// <param name="init">The intial state</param>
    //    /// <param name="func">The function to transform state</param>
    //    public void Add<T>(GameObject gob, T init, Func<UpdateArgs, T, T> func ) where T : GameObjectState
    //    {
    //        if (!_gobTable.ContainsKey(gob))
    //            _gobTable.Add(gob, new Dictionary<Type, IStateBundle<GameObjectState>>());

    //        var bundle = new StateBundle<T>(init, func);

    //        if (!_gobTable[gob].ContainsKey(typeof (T)))
    //            _gobTable[gob].Add(typeof (T), (IStateBundle<GameObjectState>) bundle);
    //        else _gobTable[gob][typeof(T)] = (IStateBundle<GameObjectState>) bundle;
    //    }

    //    /// <summary>
    //    /// Remove a gob and state/func. If it doesn't exist, an exception will be thrown
    //    /// </summary>
    //    /// <typeparam name="T">The type of gameobjectstate</typeparam>
    //    /// <param name="gob">the gob</param>
    //    public void Remove<T>(GameObject gob) where T : GameObjectState
    //    {
    //        if (!_gobTable.ContainsKey(gob) || !_gobTable[gob].ContainsKey(typeof (T)))
    //            throw new Exception("No gob or state exists for this request. ");
            
    //        _gobTable[gob][typeof(T)].Clear();
    //        _gobTable[gob].Remove(typeof (T));
    //    }

    //    /// <summary>
    //    /// Fetch a state. The returned state will be the state from the previous update loop
    //    /// </summary>
    //    /// <typeparam name="T">The type of gameobject state. </typeparam>
    //    /// <param name="gob">the gob</param>
    //    /// <returns>The state of the gameobject in the requested type</returns>
    //    public T Get<T>(GameObject gob) where T : GameObjectState
    //    {
    //        if (!_gobTable.ContainsKey(gob) || !_gobTable[gob].ContainsKey(typeof(T)))
    //            throw new Exception("No gob or state exists for this request. " + gob.Id + typeof(T));

    //        return (T)_gobTable[gob][typeof (T)].State;
    //    }

    //    public Func<UpdateArgs, T, T> GetFunc<T>(GameObject gob) where T : GameObjectState
    //    {
    //        if (!_gobTable.ContainsKey(gob) || !_gobTable[gob].ContainsKey(typeof(T)))
    //            return new Func<UpdateArgs, T, T>( (a, init) => init);

    //        return (Func<UpdateArgs, T, T>) _gobTable[gob][typeof (T)].Function;
    //    }

    //    /// <summary>
    //    /// Run an update on all the gob state/funcs
    //    /// </summary>
    //    /// <param name="time"></param>
    //    /// <param name="services"></param>
    //    public void Update(GameTime time, GameServices services)
    //    {
    //        var args = new UpdateArgs(time, services);

    //        Forall(b => b.Update(args));
    //        Forall(b => b.Finish());
    //    }

    //    private void Forall( Action< IStateBundle<GameObjectState>> action )
    //    {
    //        _gobTable.Keys.AsParallel()
    //            .ForAll(
    //                g => _gobTable[g].Keys.AsParallel()
    //                        .ForAll( t => action( _gobTable[g][t] ) ));

    //    }
    //}




    //public abstract class GobState
    //{

    //}

    //public class OtherState : GobState
    //{
        
    //}

    public class UpdateArgs
    {

        public GameTime Time { get; private set; }
        public GameServices Services { get; private set; }
        public UpdateArgs(GameTime time, GameServices services)
        {
            Time = time;
            Services = services;
        }
    }

    ////public class StateBundle<T> where T : GobState
    ////{
    ////    public StateUpdateFunc<T> UpdateFunc { get; set; }
    ////    public Func<T> InitStateGenerator { get; set; }

    ////    public StateBundle(Func<T> generator, StateUpdateFunc<T> updator)
    ////    {
    ////        InitStateGenerator = generator;
    ////        UpdateFunc = updator;
    ////    } 
    ////}
    //public class StateBundle<S> where S : GobState
    //{
        

    //    private StateBundle(S init, StateUpdateFunc<S> updater )
    //    {
            
    //    }

    //    public StateBundle<S> Create<S>(S init, StateUpdateFunc<S> updater ) where S : GobState
    //    {
            
    //    }
    //}

    //public delegate T StateUpdateFunc<out T>(UpdateArgs args) where T : GobState;

    //class StateUpdater : IStateUpdateService
    //{

    //    private List<GameObject> _gobs;
    //    private Dictionary<GameObject, GobState> _previousStateTable;
    //    private Dictionary<GameObject, StateUpdateFunc<GobState>> _updateFuncs;

    //    public StateUpdater()
    //    {
    //        _gobs = new List<GameObject>();
    //        _previousStateTable = new Dictionary<GameObject, GobState>();
    //        _updateFuncs = new Dictionary<GameObject, StateUpdateFunc<GobState>>();
            
    //    }

    //    public void Clear()
    //    {
    //        _gobs.Clear();
    //        _previousStateTable.Clear();
    //        _updateFuncs.Clear();
    //    }

    //    public T GetState<T>(GameObject gob) where T : GobState
    //    {
    //        if (!_previousStateTable.ContainsKey(gob)) throw new Exception("gob does not exist");
    //        return _previousStateTable[gob] as T;
    //    }

    //    public void Subscribe<T>(GameObject gob, T initState, Func<UpdateArgs, T> updateFunc) where T : GobState
    //    {
    //        if (_previousStateTable.ContainsKey(gob)) throw new Exception("The same Gob cannot be added twice.");

    //        _gobs.Add(gob);
    //        _previousStateTable.Add(gob, initState);
    //        _updateFuncs.Add(gob, new StateUpdateFunc<T>(updateFunc));
    //    }

    //    public void Unsubscribe(GameObject gob)
    //    {
    //        if (!_previousStateTable.ContainsKey(gob)) throw new Exception("The gob was never subscribed");

    //        _gobs.Remove(gob);
    //        _previousStateTable.Remove(gob);
    //        _updateFuncs.Remove(gob);
    //    }

    //    public void Update(GameTime time, GameServices services)
    //    {

    //        var nextStateTable = new Dictionary<GameObject, GobState>();
    //        var updateArgs = new UpdateArgs(time, services);
    //        _gobs.ForEach(g =>
    //        {
    //            var prevState = _previousStateTable[g];
    //            var nextState = _updateFuncs[g](updateArgs);
    //            nextStateTable.Add(g, nextState);
    //        });

    //        _previousStateTable = nextStateTable;
    //    }

    //}
}
