using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;

namespace DBSync
{
   public class Config
    {
      public string fbLogin;
      public string fbPass;
      public string fbPath;
      public string fbdbName;

      public string PGLogin;
      public string PGPass;
      public string PGdbName;

      public string SQLLitePath;



       public void parseConfig()
        {
            if (!File.Exists("config.ini"))
            {
                Console.WriteLine("Can't find config.ini");
                throw new Exception("Can't find config.ini");
            }

            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile("config.ini");

            fbLogin = data["firebird"]["login"];
            fbPass = data["firebird"]["pass"];
            fbPath = data["firebird"]["path"];
            fbdbName = data["firebird"]["dbname"];

            PGLogin = data["postgres"]["login"];
            PGPass = data["postgres"]["pass"];
            PGdbName = data["postgres"]["dbname"];

            SQLLitePath = data["sqllite"]["path"];

            if (!File.Exists(fbPath))
            {
                Console.WriteLine("Wrong Path to FireBird DataBase");
               // throw new Exception("Wrong Path to FireBird DataBase");
            }

            if (!File.Exists(SQLLitePath))
            {
                Console.WriteLine("Wrong Path to SQLLite DataBase");
               // throw new Exception("Wrong Path to SQLLite DataBase");
            }

        }

    }
}
