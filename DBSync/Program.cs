using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using System.Data.SQLite;
using DBSync.DB;
using DBSync.DB.Contract;

namespace DBSync
{
    class Program
    {
        public static void Main(string[] args)
        {
            IDBRepository repositoryOne;
            IDBRepository repositoryTwo;

            Config config = new Config();
            config.parseConfig();

            FireBird fb = new FireBird(config);
        //  fb.dbDoSelect();
            fb.insertTestData();

            SQLite sqllite = new SQLite(config);
            sqllite.liteConnect();
            

            Postgres pg = new Postgres(config, sqllite); // we should have access to sqlite instance
            pg.ListDBTablesForProcessing();
            pg.PGConnect();
            pg.selectDataForSync();


            Console.ReadKey();
            Console.ReadKey();

   
        }



    }
}
