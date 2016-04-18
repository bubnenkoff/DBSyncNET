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
                Console.WriteLine("SQLLite file do not exists, and will be created");
                try
                {
                    SQLiteConnection.CreateFile("MyDatabase.sqlite");
                    Console.WriteLine("Database created: {0}", config.SQLLitePath);
                }
                catch (Exception e)
                {
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
    }
}
