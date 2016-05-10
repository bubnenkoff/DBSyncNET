using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using DBSync.DB.Contract;
using DBSync.Model;

namespace DBSync.DB
{
    public class SQLite : IDBRepository
    {
        Config config;

        public SQLite(Config config)
        {
            this.config = config;
        }

        public DbConnection Connection { get; set; }

        private UserData ud = new UserData();

        public void Connect()
        {
            Console.WriteLine("SQLLITE path {0}", config.SQLLitePath);
            if (!File.Exists(config.SQLLitePath))
            {
                Console.WriteLine("SQLite DataBase do not exists");
                try
                {
                    SQLiteConnection.CreateFile("MyDatabase.sqlite");
                    Console.WriteLine("Database created: {0}", config.SQLLitePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine("[ERROR] Can't create SQLite DataBase");
                    Console.WriteLine(e.Message);
                }

            }


            Connection = new SQLiteConnection("Data Source=" + config.SQLLitePath + ";Version=3;");
            Connection.Open();

            try
            {
                string sql = "create table if not exists USERS (id int, guid varchar(36), username varchar(20), userblob blob)";
                SQLiteCommand command = new SQLiteCommand(sql, (SQLiteConnection)Connection);
                command.ExecuteNonQuery();
                Console.WriteLine("Table in SQLLITE created with next request:!");
                Console.WriteLine("{0}", sql);
            }


            catch (Exception e)
            {
                Console.WriteLine("Can't connect to SQLLITE");
                Console.WriteLine(e.Message);
            }

        }

        public void liteCheckTablesExists(List<string> foundedInPGTables) // sqllite may do not have tables. Existen tables from PG
        {
            List<string> existInSQLLiteTables = new List<string>();
            List<string> existInSQLLiteAndPGTables = new List<string>();

            foreach (var table in foundedInPGTables)
            {

                // for each tables for Sync getting it's structure
                SQLiteCommand command = new SQLiteCommand((SQLiteConnection)Connection);
                command.CommandText = "SELECT name FROM sqlite_master where type='table'";
                command.Prepare();


                // check if every table in PG exists in SQLite
                SQLiteDataReader rdr = command.ExecuteReader();

                while (rdr.Read())
                {
                    existInSQLLiteTables.Add(rdr[0].ToString());
                }

                existInSQLLiteAndPGTables = existInSQLLiteTables.Intersect(foundedInPGTables).ToList();

                Console.WriteLine("Next tables exists in SQLite and PostgreSQL and will be sync:");
                foreach (var t in existInSQLLiteTables)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(t);
                    Console.ResetColor();

                }


            }
        }


        public void CloseConnect()
        {
            Connection.Close();
        }

        public UserData GetData()
        {
            SQLiteCommand command = new SQLiteCommand((SQLiteConnection)Connection);
            command.CommandText = "SELECT id, userblob FROM USERS WHERE ID=1";
            command.Prepare();

            // check if every table in PG exists in SQLite
            SQLiteDataReader rd = command.ExecuteReader();

            while (rd.Read())
            {
                ud.Id = rd[0].ToString();
                ud.Guid = (rd[1].ToString());
                ud.Name = (rd[2].ToString());
                ud.UserBlob = (byte[])rd[3];

                InsertData(ud); // insert data from sql lite to 
            }

            return ud;
        }



        public void InsertData(UserData ud)
        {
            Connection = new SQLiteConnection("Data Source=" + config.SQLLitePath + ";Version=3;");
            Connection.Open();

            SQLiteCommand insertSQL = new SQLiteCommand(@"INSERT INTO ""USERS"" (id, guid, username, userblob) VALUES (?,?,?,?)", (SQLiteConnection)Connection);
            try
            {
                insertSQL.Parameters.Add(new SQLiteParameter { Value = ud.Id });
                insertSQL.Parameters.Add(new SQLiteParameter { Value = ud.Guid });
                insertSQL.Parameters.Add(new SQLiteParameter { Value = ud.Name });
                insertSQL.Parameters.Add(new SQLiteParameter { Value = ud.UserBlob, DbType = DbType.Binary });

                insertSQL.ExecuteNonQuery();
                Console.WriteLine("Data Inserted in SQLLite");
            }

            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);

            }
        }

        public void GetListExistsTables()
        {
            throw new NotImplementedException();
        }
    }
}
