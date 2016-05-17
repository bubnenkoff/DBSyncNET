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

        private List<UserData> uds = new List<UserData>();

        private List<UserData> uds_forFB = new List<UserData>(); // Данные после выборки по ГУИДам которые мы будем вставлять в FB

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
                string sql = "create table if not exists USERS (id int, guid varchar(36), username varchar(20), userblob blob, FL int)";
                SQLiteCommand command = new SQLiteCommand(sql, (SQLiteConnection)Connection);
                command.ExecuteNonQuery();
                Console.WriteLine("Table in SQLLITE created with next request:");
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

        public List<UserData> GetData()
        {
            SQLiteCommand command = new SQLiteCommand((SQLiteConnection)Connection);
            command.CommandText = "SELECT id, userblob FROM USERS WHERE ID=1";
            command.Prepare();

            // check if every table in PG exists in SQLite
            SQLiteDataReader rd = command.ExecuteReader();

            while (rd.Read())
            {
                UserData ud = new UserData(); //создаем один экземпляр
                ud.Id = rd[0].ToString();
                ud.Guid = (rd[1].ToString());
                ud.Name = (rd[2].ToString());
                ud.UserBlob = (byte[])rd[3];
                uds.Add(ud);
                InsertData(uds); // insert data from sql lite to 
            }

            return uds;
        }



        public void InsertData(List<UserData> PGuds) // Вставляем данные из ПГ
        {
            Connection = new SQLiteConnection("Data Source=" + config.SQLLitePath + ";Version=3;");
            Connection.Open();
            
            try
            {
                Console.WriteLine(PGuds.Count);

                foreach (UserData d in PGuds)
                {
                    SQLiteCommand insertSQL = new SQLiteCommand(@"INSERT INTO ""USERS"" (id, guid, username, userblob, ""FL"") VALUES (?,?,?,?,?)", (SQLiteConnection)Connection);

                    Console.WriteLine(d.Id);
                    Console.WriteLine(d.Guid);
                    Console.WriteLine(d.Name);
                    insertSQL.Parameters.Add(new SQLiteParameter { Value = d.Id });
                    insertSQL.Parameters.Add(new SQLiteParameter { Value = d.Guid });
                    insertSQL.Parameters.Add(new SQLiteParameter { Value = d.Name });
                    insertSQL.Parameters.Add(new SQLiteParameter { Value = d.UserBlob, DbType = DbType.Binary });
                    insertSQL.Parameters.Add(new SQLiteParameter { Value = d.FL });
                    
                insertSQL.ExecuteNonQuery();
                Console.WriteLine("Data Inserted in SQLLite");
                }

            }

            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);

            }
        }



        public List<string> GetGUIDsFromSQLite()
        {
            Connect();
            List<string> liteguids = new List<string>();
            SQLiteCommand command = new SQLiteCommand((SQLiteConnection)Connection);
            command.CommandText = "SELECT guid FROM USERS";
            command.Prepare();

            // check if every table in PG exists in SQLite
            SQLiteDataReader rdr = command.ExecuteReader();
       
            while (rdr.Read())
            {
                liteguids.Add(rdr[0].ToString());
               // Console.WriteLine(rdr[0].ToString());
            }
            CloseConnect();
            return liteguids;
        }



        public void GetAllDataWithSelectedGUIDs(List<string> DataExistInSQLiteButNOTINFB) // Забираем только данные с нужными ГУИДами (получили их путем пересечения ГУИДов в FB и PG
        {
            Connect();
            List<string> liteguids = new List<string>();
            SQLiteCommand command = new SQLiteCommand((SQLiteConnection)Connection);

            foreach (var el in DataExistInSQLiteButNOTINFB)
            {
                string sql = "SELECT id, guid, username, userblob, FL FROM USERS WHERE GUID = '" + el.ToString()  + "'";

                command.CommandText = sql;
                command.Prepare();

                // check if every table in PG exists in SQLite
                SQLiteDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    UserData ud = new UserData();
                    ud.Id = dr[0].ToString();
                    ud.Guid = (dr[1].ToString());
                    ud.Name = (dr[2].ToString());
                    ud.UserBlob = (byte[])dr[3];
                    ud.FL = dr[4].ToString();
                    uds_forFB.Add(ud); 
                    // Закинули данные из SQLite в структуру данных для FB. 
                    // Теперь эти данные нужно обратно в FB вставить.
                    Console.WriteLine("33333333--------------------333333333333");
                    Console.WriteLine(dr[0].ToString());
                }
            }

            CloseConnect();
            
        }




        public void GetListExistsTables()
        {
            throw new NotImplementedException();
        }
    }
}
