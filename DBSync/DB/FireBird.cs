using System;
using System.Collections.Generic;
using System.IO;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using DBSync.DB;
using DBSync.DB.Contract;
using DBSync.Model;
using FirebirdSql.Data.FirebirdClient;

namespace DBSync
{
    class FireBird : IDBRepository
    {
   
        private string ConnectionString { get; set; }

        private FbDataReader Reader { get; set; }

        Config config;
        SQLite sqllite;

        public FireBird(Config config, SQLite sqllite)
        {
            ConnectionString =
                "User=" + config.fbLogin + ";" +
                "Password=" + config.fbPass + ";" +
                "Database=" + config.fbPath + ";" +
                "DataSource=localhost;" +
                "Port=3050;" +
                "Dialect=3;" +
                "Charset=NONE;" +
                "Role=;" +
                "Connection lifetime=15;" +
                "Pooling=true;" +
                "MinPoolSize=0;" +
                "MaxPoolSize=50;" +
                "Packet Size=8192;" +
                "ServerType=0";

            this.config = config;
            this.sqllite = sqllite;
        }

 
        private UserData ud = new UserData();

        List<string> TablesForSyncFromFB = new List<string>(new string[]{"USERS"});


        public DbConnection Connection { get; set; }

       

        public void Connect()
        {
            Connection = new FbConnection(ConnectionString);
            try
            {
               // Console.WriteLine("Open connections with FireBird");
                Connection.Open();
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Сan't connect to FireBird");
            }
        }

        public void CloseConnect()
        {
            Connection.Close();
        }

     //  SQLite sqllite = new SQLite();
        
//        public void GetDataFromSQLite()
//        {
//            SQLiteCommand command = new SQLiteCommand((SQLiteConnection)Connection);
//            command.CommandText = "SELECT id, userblob FROM USERS WHERE ID=1";
//            command.Prepare();
//
//            // check if every table in PG exists in SQLite
//            SQLiteDataReader rd = command.ExecuteReader();
//
//            while (rd.Read())
//            {
//                ud.Id = rd[0].ToString();
//                ud.Guid = (rd[1].ToString());
//                ud.Name = (rd[2].ToString());
//                ud.UserBlob = (byte[])rd[3];
//                
//                InsertData(ud); // insert data from sql lite to 
//            }
//
//        }

        // Нужно взять все ГУИДы из FB и потом пересечь их
        public List<string> GetGUIDsFromFB()
        {
            
            List<string> FBguids = new List<string>();
            Connect();
            FbTransaction myTransaction = (FbTransaction)Connection.BeginTransaction();
            FbCommand myCommand = new FbCommand
            {
                Connection = (FbConnection)Connection,
                Transaction = myTransaction,
                CommandText = @"SELECT guid, id FROM ""USERS"";"
            };
            myCommand.Connection = (FbConnection)Connection;
            myCommand.Transaction = myTransaction;

            Reader = myCommand.ExecuteReader();

            while (Reader.Read())
            {
                FBguids.Add(Reader[0].ToString());
                Console.WriteLine(Reader[0].ToString());
              
            }

            Reader.Close();
            CloseConnect();

            return FBguids;
        }

        List<string> DataExistInSQLiteButNotInFB = new List<string>();

        public void IntersectPGandSQLite()
        {
            //sqllite.GetGUIDsFromSQLite();
            Console.WriteLine("---------");
            DataExistInSQLiteButNotInFB = sqllite.GetGUIDsFromSQLite().Intersect(GetGUIDsFromFB()).ToList();
            Console.WriteLine(string.Join(", ", DataExistInSQLiteButNotInFB));
            Console.WriteLine("^^^^^^^^^^");

        }
        
        public UserData GetData()
        {

            try
            {
                FbTransaction myTransaction = (FbTransaction) Connection.BeginTransaction();
                FbCommand myCommand = new FbCommand
                {
                    Connection = (FbConnection) Connection,
                    Transaction = myTransaction,
                    CommandText = "SELECT id, userblob FROM USERS WHERE ID=1"
                };
                myCommand.Connection = (FbConnection) Connection;
                myCommand.Transaction = myTransaction;


                Reader = myCommand.ExecuteReader();

                while (Reader.Read())
                {
                  //  ud.Id = Reader[0].ToString(); // берем хотя бы один
                }
                
                Reader.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Can't read data from DB");
            }

            finally
            {
                Reader.Close();
               
            }
            Connection.Close();
            return ud;
        }


        public void InsertData(UserData ud)
        {
            try
            {
                Connect();
                FbTransaction myTransaction = ((FbConnection)Connection).BeginTransaction();
                FbCommand myCommand = new FbCommand
                {
                    CommandText = "UPDATE USERS SET USERBLOB = @userblob WHERE ID=1",
                    Connection = (FbConnection) Connection,
                    Transaction = myTransaction
                };
            //    myCommand.Parameters.Add("@userblob", readByteImage(testimg));
                // myCommand.Parameters.Add("@userblob", FbDbType.Binary).Value =  GetPhoto("D:/Project/2016/DBSync/test.png");
                myCommand.ExecuteNonQuery();

                myTransaction.Commit();
                myCommand.Dispose();
                Connection.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void GetListExistsTables()
        {
            throw new NotImplementedException();
        }
    }
}
