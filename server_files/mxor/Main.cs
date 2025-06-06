using mxor.auth;
using mxor.console;
using mxor.databases.Entities;
using mxor.margin;
using mxor.shared;
using mxor.world.scripting;
using System;


namespace mxor
{

    public static class MainClass
    {

        public static void Main(string[] args)
        {
            var customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            HealthCheck hc = new HealthCheck();

            if (hc.doTests())
            {
                Output.WriteLine("\nHealth checks OK. Proceeding.\n");
                // Create
                Store.auth = new AuthServer();
                Store.margin = new MarginServer();
                Store.world = new WorldServer();
                Store.worldThreads = new WorldThreads();

                Store.config = new ServerConfig("Config.xml");
                Store.config.LoadDbParams();
                Store.config.LoadServerParams();

                Store.worldConfig = new WorldConfig("WorldConfig.xml");

                /* Load Game Data */
                DataLoader.getInstance();

                /* Initialize DB Stuff */

                Store.dbManager = new databases.DatabaseManager();
                Store.matrixDbContext = new MatrixDbContext(Store.config.DBParams);

                if (Store.config.DBParams.DbType == "mysql")
                {
                    Store.dbManager.AuthDbHandler = new databases.MyAuthDBAccess();
                    Store.dbManager.MarginDbHandler = new databases.MyMarginDBAccess();
                    Store.dbManager.WorldDbHandler = new databases.MyWorldDbAccess();
                }

                /* Initialize the MPM object */
                Store.dbManager.WorldDbHandler.ResetOnlineStatus();
                Store.Mpm = new MultiProtocolManager();

                /* Initialize the scripting server */
                Store.rpcScriptManager = new ScriptManager();
                var scrLoader = new ScriptLoader();
                scrLoader.LoadScripts(); //<<-- This does


                /* External console */
                if (Store.config.ServerParams.AdminConsoleEnabled)
                {
                    ConsoleSocket adminConsole = new ConsoleSocket();
                    adminConsole.StartServer();
                }


                // Now everything should be loaded - START THE ENGINES!!!
                Store.auth.StartServer();
                Store.margin.StartServer();
                Store.world.StartServer();

                // Check if execution keeps going after starting
                Output.WriteLine("Im'running :D");


                // Capture Ctrl C key to clean and then end the program
                Console.CancelKeyPress += delegate
                {
                    Output.WriteLine("Closing Auth server and threads");
                    Store.auth.StopServer();

                    Output.WriteLine("Closing Margin server and threads");
                    Store.margin.StopServer();

                    Output.WriteLine("Closing World server and threads");
                    Store.world.StopServer();

                    Output.WriteLine("Server exited");
                };

            }
            else
            {
                Output.WriteLine("\nHealth checks not passed. Aborting launch.");
                Output.WriteLine("Please check the errors above or press Enter to close the window.");
                Console.ReadLine();
            }
        }
    }
}