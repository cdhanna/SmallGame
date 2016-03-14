using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using SmallGame.Services;

namespace SmallGame.Input
{

    public class KeyboardEventArgs
    {
        public Keys[] Keys { get; private set; }
        public Keys[] OldKeys { get; private set; }

        public KeyboardState State { get; private set; }
        public KeyboardState OldState { get; private set; }

        public KeyboardEventArgs(Keys[] keys, Keys[] oldKeys, KeyboardState state, KeyboardState oldState)
        {
            Keys = keys;
            OldKeys = oldKeys;
            State = state;
            OldState = oldState;
        }
    }

    public class KeyboardHelper : IGameService
    {

        private KeyboardState _kState;
        private KeyboardState _oldkState;

        private Dictionary<Keys, List<Action<KeyboardEventArgs>>> _onDownListeners;
        private Dictionary<Keys, List<Action<KeyboardEventArgs>>> _onNewDownListeners;
        private Dictionary<Keys, List<Action<KeyboardEventArgs>>> _onNewUpListeners; 

        public KeyboardHelper()
        {
            _onDownListeners = new Dictionary<Keys, List<Action<KeyboardEventArgs>>>();
            _onNewDownListeners = new Dictionary<Keys, List<Action<KeyboardEventArgs>>>();
            _onNewUpListeners = new Dictionary<Keys, List<Action<KeyboardEventArgs>>>();
        }

        private bool KState(Keys key, KeyboardState state)
        {
            return state.IsKeyDown(key);
        }

        private List<Action<KeyboardEventArgs>> GetActions(Keys key, Dictionary<Keys, List<Action<KeyboardEventArgs>>> table)
        {
            if (!table.ContainsKey(key))
            {
                table.Add(key, new List<Action<KeyboardEventArgs>>());
            }
            return table[key];
        } 

        /// <summary>
        /// Adds a listener that will be invoked when the given key is down.
        /// </summary>
        /// <param name="key">the key to listen for</param>
        /// <param name="listener">the listener to invoke</param>
        public void OnDown(Keys key, Action<KeyboardEventArgs> listener)
        {
            GetActions(key, _onDownListeners).Add(listener);
        }

        /// <summary>
        /// Adds a listener that will be invoked when the given key is newly down.
        /// </summary>
        /// <param name="key">the key to listen for</param>
        /// <param name="listener">the listener to invoke</param>
        public void OnNewDown(Keys key, Action<KeyboardEventArgs> listener)
        {
            GetActions(key, _onNewDownListeners).Add(listener);
        }

        /// <summary>
        /// Adds a listener that will be invoked when the given key is newly up.
        /// </summary>
        /// <param name="key">the key to listen for</param>
        /// <param name="listener">the listener to invoke</param>
        public void OnNewUp(Keys key, Action<KeyboardEventArgs> listener)
        {
            GetActions(key, _onNewUpListeners).Add(listener);
        }

        /// <summary>
        /// Remove a listener that was being invoked when the given key is down
        /// </summary>
        /// <param name="key">the key to listen for</param>
        /// <param name="listener">the listener to invoke</param>
        public void RemoveOnDown(Keys key, Action<KeyboardEventArgs> listener)
        {
            GetActions(key, _onDownListeners).Remove(listener);
        }

        /// <summary>
        /// Remove a listener that was being invoked when the given key is newly down
        /// </summary>
        /// <param name="key">the key to listen for</param>
        /// <param name="listener">the listener to invoke</param>
        public void RemoveOnNewDown(Keys key, Action<KeyboardEventArgs> listener)
        {
            GetActions(key, _onNewDownListeners).Remove(listener);
        }

        /// <summary>
        /// Remove a listener that was being invoked when the given key is newly up
        /// </summary>
        /// <param name="key">the key to listen for</param>
        /// <param name="listener">the listener to invoke</param>
        public void RemoveOnNewUp(Keys key, Action<KeyboardEventArgs> listener)
        {
            GetActions(key, _onNewUpListeners).Remove(listener);
        }

        /// <summary>
        /// Checks if the given key is down
        /// </summary>
        /// <param name="key">the key to watch for</param>
        /// <returns>True if the key is down, false otherwise</returns>
        public bool IsDown(Keys key)
        {
            return KState(key, _kState);
        }

        /// <summary>
        /// Checks if the given key is up
        /// </summary>
        /// <param name="key">the key to watch for</param>
        /// <returns>True if the key is up, false otherwise</returns>
        public bool IsUp(Keys key)
        {
            return !KState(key, _kState);
        }

        /// <summary>
        /// Checks if the given key is newly down
        /// </summary>
        /// <param name="key">the key to watch for</param>
        /// <returns>True if the key is newly down, false otherwise</returns>
        public bool IsNewDown(Keys key)
        {
            return KState(key, _kState) && !KState(key, _oldkState);
        }

        /// <summary>
        /// Checks if the given key is newly up
        /// </summary>
        /// <param name="key">the key to watch for</param>
        /// <returns>True if the key is newly up, false otherwise</returns>
        public bool IsNewUp(Keys key)
        {
            return !KState(key, _kState) && KState(key, _oldkState);
        }

        /// <summary>
        /// Removes all listeners
        /// </summary>
        public void Empty()
        {
            _onDownListeners = new Dictionary<Keys, List<Action<KeyboardEventArgs>>>();
            _onNewDownListeners = new Dictionary<Keys, List<Action<KeyboardEventArgs>>>();
            _onNewUpListeners = new Dictionary<Keys, List<Action<KeyboardEventArgs>>>();
        }

        /// <summary>
        /// Updates the keyboard state, and invokes any valid listeners
        /// </summary>
        public void Update()
        {
            _kState = Keyboard.GetState();

           
            var args = new KeyboardEventArgs(_kState.GetPressedKeys(), _oldkState.GetPressedKeys(), _kState, _oldkState);


            _kState.GetPressedKeys().ToList()
                .ForEach(k =>
                {
                    // k is being pushed, notify.
                    GetActions(k, _onDownListeners).ForEach(l => l(args));

                    if (_oldkState[k] == KeyState.Up)
                    {
                        // k wasn't be pushed last time, notify.
                        GetActions(k, _onNewDownListeners).ForEach(l => l(args));
                    }
                });

            _oldkState.GetPressedKeys().ToList()
                .ForEach(k =>
                {
                    if (_kState[k] == KeyState.Up)
                    {
                        // k was being pushed, but isnt anymore, notify.
                        GetActions(k, _onNewUpListeners).ForEach(l => l(args)); 
                    }
                });
            

            _oldkState = _kState;
        }

    }
}
