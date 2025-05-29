using System;
using System.Collections.Generic;
using System.Text;

namespace mxor{
    
    public class ServerConfig{

        private string filename;

        private DbParams _dbParams;
        private ServerParams _serverParams;

        public DbParams DBParams { get { return _dbParams; } }
        public ServerParams ServerParams { get { return _serverParams; } }


        public ServerConfig(string _filename){
            filename = _filename;
        }

        public void LoadDbParams() {
            XmlParser.LoadDBParams(filename, out _dbParams);
        }

        public void LoadServerParams() {
            XmlParser.LoadServerParams(filename, out _serverParams);
        }
    
    }


    public class DbParams{

        public string Host { get; set; }
        public int Port {get; set;}
        public string DatabaseName {get;set;}
        public string Username {get;set;}
        public string Password {get;set;}
        public string Motd {get;set;}
        public string DbType {get;set;}

    }

    public class ServerParams{
        public bool AdminConsoleEnabled { get; set; }
        public string ServerType { get; set; }
    }
}
