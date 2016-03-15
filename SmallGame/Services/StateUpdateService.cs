using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmallGame.GameObjects;

namespace SmallGame.Services
{

    abstract class State
    {
        public abstract State Clone();
    }
    abstract class State<T> : State where T : State
    {
        public override State Clone()
        {
            return CloneHack();
        }

        public abstract T CloneHack();
    }

    class SampleState : State<SampleState>
    {
        public int X { get; set; }

        public override SampleState CloneHack()
        {
            return new SampleState() {X = this.X};
        }
    }

    class StateUpdateService 
    {

        // probably turn into fields later. 
        private List<GameObject> AllGobs { get; set; }
        private Dictionary<GameObject, Action<State>> FunctionTable { get; set; }
        private Dictionary<GameObject, State> GobStateTable { get; set; }
        private Dictionary<GameObject, State> OldGobStateTable { get; set; } 
        
        public StateUpdateService()
        {
            AllGobs = new List<GameObject>();
            FunctionTable = new Dictionary<GameObject, Action<State>>();
            GobStateTable = new Dictionary<GameObject, State>();
            OldGobStateTable = new Dictionary<GameObject, State>();

        }

        public void Subscribe<T>(GameObject gob, Action<T> updater, T initial) where T : State
        {
            AllGobs.Add(gob);
            FunctionTable.Add(gob, s=> updater((T)s) );
            OldGobStateTable.Add(gob, initial);
            GobStateTable.Add(gob, null); // we have nothing yet.
        }

        public State GetState(GameObject gob)
        {
            // access into the OldState table.
            // TODO there is no way to know that gob as a subclass of State without *knowing*
            return OldGobStateTable[gob]; // TODO error case if gob doesn't exist.
        }



        public void Update()
        {
            // TODO make async instead of sync. 
            AllGobs.ForEach(gob =>
            {
                var oldState = OldGobStateTable[gob];
                var nextState = oldState.Clone();
                FunctionTable[gob](nextState);
                GobStateTable[gob] = nextState;
            });

            AllGobs.ForEach(gob =>
            {
                OldGobStateTable[gob] = GobStateTable[gob];
            });
        }
    }
}
