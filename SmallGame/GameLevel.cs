using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SmallGame
{

    /// <summary>
    /// A GameLevel is a collection of GameObjects and metaData for the level being used in CoreGame. 
    /// There is usually only level active at a time. Level is pretty much synomous with 'scene'. 
    /// </summary>
    public abstract class GameLevel
    {
        /// <summary>
        /// Gets or Sets the GameObjectCollection that holds all of the GameObjects in this Level.
        /// </summary>
        public GameObjectCollection Objects { get; set; }

        /// <summary>
        /// Gets or Sets the raw LevelData. LevelData is loaded from the JSON file. 
        /// </summary>
        public LevelData Data { get; set; }

        /// <summary>
        /// Gets the Name of the Level
        /// </summary>
        public string Name { get { return Data.Name; } }


        protected GameLevel()
        {
            Objects = new GameObjectCollection();
        }


    }
}
