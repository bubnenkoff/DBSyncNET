using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Data.SQLite;
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


        public void liteCheckTablesExists(List<string> foundedInPGTables) // sqllite may do not have tables. Existen tables from PG
        {
            List<string> existInSQLLiteTables = new List<string>();
            List<string> existInSQLLiteAndPGTables = new List<string>();

            foreach (var table in foundedInPGTables)
            {


                SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + config.SQLLitePath + ";Version=3;");
                m_dbConnection.Open();


                // for each tables for Sync getting it's structure
                SQLiteCommand command = new SQLiteCommand(m_dbConnection);
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



        public void insertDataFromPGToSQLLite(UserData ud)
        {

            Connection = new SQLiteConnection("Data Source=" + config.SQLLitePath + ";Version=3;");
            Connection.Open();

            SQLiteCommand insertSQL = new SQLiteCommand(@"INSERT INTO ""USERS"" (id, guid, username, userblob) VALUES (?,?,?,?)", (SQLiteConnection)Connection);
            try
            {
                Console.WriteLine("------------------------");
                insertSQL.Parameters.Add(new SQLiteParameter { Value = ud.Id });
                insertSQL.Parameters.Add(new SQLiteParameter { Value = ud.Guid });
                insertSQL.Parameters.Add(new SQLiteParameter { Value = ud.Name });
                insertSQL.Parameters.Add(new SQLiteParameter { Value = ud.UserBlob, DbType = DbType.Binary});

                Console.WriteLine("!!!");
                insertSQL.ExecuteNonQuery();
                Console.WriteLine("DONE");
            }

            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);

            }

            finally
            {
                Connection.Close();
            }

        }


        public DbConnection Connection { get; set; }

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

            finally
            {
                Connection.Close();
            }
        }

        public void CloseConnect()
        {
            Connection.Close();
        }

        public UserData GetData()
        {
            throw new NotImplementedException();
        }

        public void InsertData()
        {
            throw new NotImplementedException();
        }
    }
}
