using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace DBSync.DB
{
    class SQLLite
    {
        Config config;
        public SQLLite(Config config)
        {
            this.config = config;
        }

       public void liteConnect()
        {
            if(!File.Exists(config.SQLLitePath))
            {
                Console.WriteLine("SQLLite DataBase do not exists");
                try
                {
                    SQLiteConnection.CreateFile("MyDatabase.sqlite");
                    Console.WriteLine("Database created: {0}", config.SQLLitePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine("[ERROR] Can't create SQLLite DataBase");
                    Console.WriteLine(e.Message);
                }

            }
            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + config.SQLLitePath + ";Version=3;");
            m_dbConnection.Open();

            try
            {
                string sql = "create table if not exists USERS (id int, guid varchar(36), username varchar(20), userdata blob)";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
          

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            finally
            {
                m_dbConnection.Close();
            }
            

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


                // check if every table in PG exists in SQLLite
                SQLiteDataReader rdr = command.ExecuteReader();

                while (rdr.Read())
                {
                    existInSQLLiteTables.Add(rdr[0].ToString());
                }

                existInSQLLiteAndPGTables = existInSQLLiteTables.Intersect(foundedInPGTables).ToList();

               Console.WriteLine("Next tables exists in SQLLite and PostgreSQL and will be sync:"); 
               foreach (var t in existInSQLLiteTables)
                {
                   Console.ForegroundColor = ConsoleColor.Green; 
                   Console.WriteLine(t);
                   Console.ResetColor();
                }


                /*
                    SQLiteCommand cmd = new SQLiteCommand(m_dbConnection);
                    cmd.CommandText = "INSERT INTO Cars(Name, Price) VALUES(@Name, @Price)";
                    cmd.Prepare();

                    cmd.Parameters.AddWithValue("@Name", "BMW");
                    cmd.Parameters.AddWithValue("@Price", 36600);
                    cmd.ExecuteNonQuery();
                 */


                //  SQLiteCommand command = new SQLiteCommand("SELECT 1 FROM USERS LIMIT1", m_dbConnection); // getting list of all 

//                {
//                    db.query("SELECT * FROM " + table);
//                    return true;
//                }
//                catch (SQLException e)
//                {
//                    return false;
//                }

            }
        }


    }
}
