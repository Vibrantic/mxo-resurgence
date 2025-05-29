using System;
using System.Collections.Generic;
using System.Text;
using mxor.auth;
using mxor.databases;
using mxor.databases.Entities;
using mxor.margin;
using mxor.world.scripting;

namespace mxor.shared{
    public class Store{

        /* Configuration */

        public static ServerConfig config { get; set; }
        public static WorldConfig worldConfig { get; set; }

        /* Servers */
        public static AuthServer auth {get;set;}
        public static MarginServer margin {get;set;}
        public static WorldServer world {get;set;}

        /* Threading */
        public static WorldThreads worldThreads { get; set; }

        /* Database Handling */
        public static DatabaseManager dbManager { get; set; }
        public static MatrixDbContext matrixDbContext { get; set; }

        /* Protocol Handling */
        public static WorldClient currentClient { get; set; }
        public static MultiProtocolManager Mpm { get; set; }

        /* Scripting Handling */

        public static ScriptManager rpcScriptManager { get; set; }

    }
}
