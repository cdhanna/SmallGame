using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallGame
{
    public class IdFactory
    {
        static IdFactory ()
        {
            
        }

        public static string NewId
        {
            get { return Guid.NewGuid().ToString(); }
        }

    }
}
