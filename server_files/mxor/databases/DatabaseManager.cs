using System;
using System.Collections.Generic;
using System.Text;

using mxor.databases.interfaces;

namespace mxor.databases{
    
    public class DatabaseManager{

        public IAuthDBHandler AuthDbHandler { get; set; }
        public IMarginDBHandler MarginDbHandler { get; set; }
        public IWorldDBHandler WorldDbHandler { get; set; }

    }
}
