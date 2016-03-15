using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
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

    class SampleObj : GameObject
    {
        private Vector2 Position { get; set; }

        protected override void OnInit(GameServices services)
        {
            var initial = new SampleState() {X = Position.X}; // converts class data into initial value.

            SampleService.Instance.Tracker.Subscribe(this, Update, initial);

            base.OnInit(services);
        }

        private void Update(SampleState nextState)
        {
            Position = new Vector2(Position.X + 1, 0);

            nextState.X = Position.X; // converts class data into updated value.
        }

    }

    class SampleState : State<SampleState>
    {
        public float X { get; set; }

        public override SampleState CloneHack()
        {
            return new SampleState() {X = this.X};
        }
    }

    class SampleService
    {
        public static SampleService Instance { get; private set; } // todo make not null.

        public StateTracker<SampleState> Tracker { get; set; }

        public void Update()
        {
            Tracker.Update();
        }
    }

    class StateTracker<S> where S : State <S>
    {

        // probably turn into fields later. 
        private List<GameObject> AllGobs { get; set; }
        private Dictionary<GameObject, Action<S>> FunctionTable { get; set; }
        private Dictionary<GameObject, S> GobStateTable { get; set; }
        private Dictionary<GameObject, S> OldGobStateTable { get; set; } 
        
        public StateTracker()
        {
            AllGobs = new List<GameObject>();
            FunctionTable = new Dictionary<GameObject, Action<S>>();
            GobStateTable = new Dictionary<GameObject, S>();
            OldGobStateTable = new Dictionary<GameObject, S>();

        }

        public void Subscribe(GameObject gob, Action<S> updater, S initial)
        {
            AllGobs.Add(gob);
            FunctionTable.Add(gob, updater);
            OldGobStateTable.Add(gob, initial);
            GobStateTable.Add(gob, null); // we have nothing yet.
        }

        public S GetState(GameObject gob)
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
                var nextState = oldState.CloneHack();

                FunctionTable[gob].Invoke(oldState);
                GobStateTable[gob] = nextState;
            });

            AllGobs.ForEach(gob =>
            {
                OldGobStateTable[gob] = GobStateTable[gob];
            });
        }
    }
}
