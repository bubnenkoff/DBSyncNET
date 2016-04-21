using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using System.Data.SQLite;
using DBSync.DB;

namespace DBSync
{
    class Program
    {
        public static void Main(string[] args)
        {
            Config config = new Config();
            config.parseConfig();

            FireBird fb = new FireBird(config);
          //  fb.dbDoSelect();
            fb.insertTestData();

            SQLLite sqllite = new SQLLite(config);
            sqllite.liteConnect();

            Postgres pg = new Postgres(config, sqllite); // we should have acces to sqlite instance
            pg.ListDBTablesForProcessing();
            pg.PGConnect();




            Console.ReadKey();

   
        }



    }
}
