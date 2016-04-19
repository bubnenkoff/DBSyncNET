using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
//using System.Data;

using Npgsql;

namespace DBSync
{
    class Postgres
    {
        Config config;
       public Postgres(Config config)
        {
            this.config = config;
        }

       List<string> dbTablesList = new List<string>(); // LIST Of Tables in DB for processing

        struct UserData
        {
            public int id;
            public string guid;
            public string name;
            public byte[] userblob;
        };


        List<string> getListDBTablesForProcessing() // List of TABLES in DB
        {
            
            dbTablesList.AddRange(new List<string>() {"USERS"});
            return dbTablesList;
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
                Console.WriteLine("PG Connected");
            }

            catch(SocketException e)
            {
                Console.WriteLine(e.Message);

            }
            


            // NpgsqlCommand command = new NpgsqlCommand("SELECT city, state FROM cities", conn);
            string commandText = @"SELECT id, guid, username, userblob FROM ""USERS"" ";
          

            NpgsqlCommand command = new NpgsqlCommand(commandText, conn);

            try
            {
               
                NpgsqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                   // UserData ud = new UserData();
                    ud.id = Int32.Parse(dr[0].ToString());
                    ud.guid = (dr[1].ToString());
                    ud.name = (dr[2].ToString());
                    ud.userblob = (byte[])dr[3];
                    uds.Add(ud);
                    //File.WriteAllBytes("outputimg.jpg", ud.userblob);
                    //Console.ReadKey();

                }

            }

            finally
            {
                conn.Close();
            }


        }


    }
}
