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
using DBSync.Model;

namespace DBSync
{
    class Program
    {
        public static void Main(string[] args)
        {
//            IDBRepository repositoryOne;
//            IDBRepository repositoryTwo;


           // repositoryOne.Connection;

            Config config = new Config();
            config.parseConfig();

            //FireBird fb = new FireBird(config);

            SQLite sqllite = new SQLite(config);
            sqllite.Connect();
            

            Postgres pg = new Postgres(config, sqllite); // we should have access to sqlite instance
            pg.CheckIfPGDBFromConfigExists();
            pg.ListDBTablesForProcessing(); // список таблиц для обработки. Им заполняется List в поле класса.
            pg.GetListExistsTables();

            pg.ListDBTablesForProcessing();
            pg.Connect();
            pg.GetData();


            Console.ReadKey();
            Console.ReadKey();

   
        }



    }
}
