using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallGame
{
    /// <summary>
    /// The IdFactory can generate unique ids. 
    /// </summary>
    public class IdFactory
    {
        static IdFactory ()
        {
            
        }

        /// <summary>
        /// Get a new, and never before used ID. This is a Guid string. 
        /// </summary>
        public static string NewId
        {
            get { return Guid.NewGuid().ToString(); }
        }

    }
}
