using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBSync.DB;
using System.Net.Sockets;
//using System.Data;

using Npgsql;

namespace DBSync
{
    class Postgres
    {
        Config config;
        SQLLite sqllite;
        public Postgres(Config config, SQLLite sqllite)
       {
           this.config = config;
           this.sqllite = sqllite;

       }

        List<string> requireTablesList = new List<string>(); // LIST Of Tables in DB for processing
        List<string> existsInDBTables = new List<string>(); // this is getting from DB
        List<string> foundedTables = new List<string>(); // Tables that we will sync
        List<string> notFoundedTables = new List<string>(); // Not founded Tables!

        struct UserData
        {
            public int id;
            public string guid;
            public string name;
            public byte[] userblob;
        };


       public List<string> ListDBTablesForProcessing() // List of TABLES in DB
        {
            
            requireTablesList.AddRange(new List<string>() {"USERS", "sdf"});
            return requireTablesList;
        }


       public List<string> GetListDBsList() // selecting from pg_database and checking if DB from config is exists
        {

           NpgsqlConnection conn =
                new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=" + config.PGLogin + ";" +
                                     "Password=" + config.PGPass + ";");
            try
            {
                conn.Open();
                string getlistofDBs = @"SELECT datname FROM pg_database; ";

                NpgsqlCommand command = new NpgsqlCommand(getlistofDBs, conn);

                try
                {

                    List<string> DBList = new List<string>();
                    NpgsqlDataReader dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        DBList.Add(dr[0].ToString());
                    }
                    return DBList;
                }

                finally
                {
                    conn.Close();
                }

            }

            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();
            }

           return null;
        }


        public void PGConnect()
        {

          //  UserData [] uds;
            List<UserData> uds = new List<UserData>();
            UserData ud;

            List<string> dblist = GetListDBsList();
            if (dblist.Contains(config.PGdbName))
            {
                Console.WriteLine("Data Base exists: {0}", config.PGdbName);
            }

            else
            {
                Console.WriteLine("Data Base DO NOT exists: {0}", config.PGdbName);
                return;
            }

            NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=" + config.PGLogin + ";" +
               "Password=" + config.PGPass + ";Database=" + config.PGdbName + ";");

            //select datname from pg_database;

            try
            {
                conn.Open();
                
                Console.WriteLine("[STATUS] PG Connected");
            }

            catch(SocketException e)
            {
                Console.WriteLine(e.Message);

            }
            
            //
            try
            {


                string tablesListRequestSQL =
                    @"SELECT table_name FROM information_schema.tables WHERE table_schema='public'";
                NpgsqlCommand commandGetDBTables = new NpgsqlCommand(tablesListRequestSQL, conn);
                NpgsqlDataReader drGetDBTables = commandGetDBTables.ExecuteReader();

                while (drGetDBTables.Read())
                {
                    existsInDBTables.Add(drGetDBTables[0].ToString());
                }
                drGetDBTables.Dispose(); // complete request 

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

                else
                {
                    Console.WriteLine("[OK] All Tables that require Sync is exists in both DBs");
                }
            }

            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }
            
            string SQLrequest = @"SELECT id, guid, username, userblob FROM ""USERS""";
            NpgsqlCommand command = new NpgsqlCommand(SQLrequest, conn);

            try
            {

                NpgsqlDataReader dr = command.ExecuteReader(); // here exception
                while (dr.Read())
                {
                    // UserData ud = new UserData();
                    ud.id = Int32.Parse(dr[0].ToString());
                    ud.guid = (dr[1].ToString());
                    ud.name = (dr[2].ToString());
                    ud.userblob = (byte[]) dr[3];
                    uds.Add(ud);
                    //File.WriteAllBytes("outputimg.jpg", ud.userblob);
                    //Console.ReadKey();
                }
                dr.Dispose(); // releases conenction

            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            finally
            {
                conn.Close();
            }


            sqllite.liteCheckTablesExists(foundedTables);
           


        }


    }
}
