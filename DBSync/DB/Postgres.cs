using System;
using System.IO;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBSync.DB;
using System.Net.Sockets;
using DBSync.DB.Contract;
using DBSync.Model;
using Npgsql;

namespace DBSync
{
    class Postgres : IDBRepository
    {
        Config config;
        SQLite sqllite;
        public Postgres(Config config, SQLite sqllite)
       {
           this.config = config;
           this.sqllite = sqllite;
       }

        private List<UserData> PGuds = new List<UserData>();

        public DbConnection Connection { get; set; }

        List<string> requireTablesList = new List<string>(); // LIST Of Tables in DB for processing
        List<string> existsInDBTables = new List<string>(); // this is getting from DB
        List<string> foundedTables = new List<string>(); // Tables that we will sync
        List<string> notFoundedTables = new List<string>(); // Not founded Tables!



       public List<string> ListDBTablesForProcessing() // List of TABLES in DB
        {
            requireTablesList.AddRange(new List<string>() {"USERS", "sdf"});
            return requireTablesList;
        }


        public void CheckIfPGDBFromConfigExists() // selecting from pg_database and checking if DB from config is exists
        {
            // Тут подключаемся не стандартно -- без указания целевой БД, поэтому нельзя использовать метод Connect 
            NpgsqlConnection conn =
                new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=" + config.PGLogin + ";" +
                                     "Password=" + config.PGPass + ";");

            conn.Open();
            string getlistofDBs = @"SELECT datname FROM pg_database; ";

            NpgsqlCommand command = new NpgsqlCommand(getlistofDBs, conn);

            List<string> DBList = new List<string>();
            NpgsqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                DBList.Add(dr[0].ToString());
            }
            if (DBList.Contains(config.PGdbName))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Postgres DataBase from config is Exists");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] Postgres DataBase from config DO NOT Exists");
                Console.ResetColor();
            }

        }

   
        public void Connect()
        {
            Connection = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=" + config.PGLogin + ";" +
               "Password=" + config.PGPass + ";Database=" + config.PGdbName + ";");
            try
            {
                Connection.Open();
                Console.WriteLine("[STATUS] PG Connected");
            }

            catch (SocketException e)
            {
                Console.WriteLine(e.Message);

            }
        }

        public void CloseConnect()
        {
            Connection.Close();
        }

        public List<UserData> GetData()
        {
            try
            {
                string sql = @"SELECT id, guid, username, userblob, ""FL"" FROM ""USERS"" WHERE ""FL""=10;";

                NpgsqlCommand command = new NpgsqlCommand(sql, (NpgsqlConnection)Connection);

                NpgsqlDataReader dr = command.ExecuteReader(); // here exception
                if (!dr.HasRows)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("[INFO] PostgreSQL do not have data with FL=10");
                    Console.ResetColor();
                }
                int i = 0;
                while (dr.Read())
                {
                    UserData ud = new UserData();
                    ud.Id = dr[0].ToString();
                    ud.Guid = (dr[1].ToString());
                    ud.Name = (dr[2].ToString());
                    ud.UserBlob = (byte[])dr[3];
                    ud.FL = dr[4].ToString();
                    PGuds.Add(ud);
                    i++;
                    Console.WriteLine("Get PG data: {0}", i);

                }
                Console.WriteLine("Total PG rows: {0}", PGuds.Count);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[INFO] Syncronisated PG->SQLite rows: {0}", i);
                Console.ResetColor();
                dr.Dispose(); // releases connection
                try
                {
                    sqllite.Connect();
                    Console.WriteLine("we are here");
                    sqllite.InsertData(PGuds);
                    string sqlUpdate = @"UPDATE ""USERS"" SET ""FL""=11 WHERE ""FL""=10;";
                    //NpgsqlCommand commandUpdate = new NpgsqlCommand(sql, (NpgsqlConnection)Connection);
                    NpgsqlCommand cmd = new NpgsqlCommand(sqlUpdate, (NpgsqlConnection)Connection);
                    cmd.ExecuteNonQuery();

                    if (i > 0)
                    {
                        Console.WriteLine("Flags in PostgreSQL set to 11 (waiting for sync)");
                    }
                        
                    sqllite.CloseConnect();
                    Console.ReadKey();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Can't insert data to SQLite");
                }
                

            }

            catch (Exception e)
            {
                Console.WriteLine("SelectDataForSync function");
                Console.WriteLine(e.Message);
            }

            return PGuds;
        }


  

        public void InsertData(List<UserData> ud)
        {
            throw new NotImplementedException();
        }

        public void GetListExistsTables()
        {
            try
            {
                Connect();
                string tablesListRequestSQL = @"SELECT table_name FROM information_schema.tables WHERE table_schema='public'";
                NpgsqlCommand command = new NpgsqlCommand(tablesListRequestSQL, (NpgsqlConnection)Connection);
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    existsInDBTables.Add(reader[0].ToString());
                }
                reader.Dispose(); // complete request 

                foreach (string table in requireTablesList) // 
                {
                    if (!existsInDBTables.Contains(table))
                    // if element from requireTablesList do not found -> DB have not similar Tables!
                    {
                        notFoundedTables.Add(table);
                    }

                    else
                    {
                        foundedTables.Add(table); // this Tables we will sync
                    }
                }

                if (notFoundedTables.Count != 0) // if not empty
                {
                    Console.WriteLine("[WARNING] Next tables are marked as reqired for Sync, but can't be found in DataBase: ");

                    foreach (var table in notFoundedTables)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(table);
                        Console.ResetColor();
                    }
                }

                Console.WriteLine("Next tables will be Sync:");
                foreach (var table in foundedTables)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(table);
                    Console.ResetColor();
                }
                
            }

            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }

            Connection.Close();
        }

    }
}
