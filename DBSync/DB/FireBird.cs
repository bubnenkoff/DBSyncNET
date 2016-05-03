using System;
using System.IO;
using System.Data.Common;
using DBSync.DB.Contract;
using DBSync.Model;
using FirebirdSql.Data.FirebirdClient;

namespace DBSync
{
    class FireBird : IDBRepository
    {
   
        private string ConnectionString { get; set; }

        private FbDataReader Reader { get; set; }

        public FireBird(Config config)
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
        }


        public static string testimg = "D:/Project/2016/DBSync/test.png";
        public static string outputimg = "D:/Project/2016/DBSync/output.png";

        

        public static byte[] readByteImage(string photoFilePath)
        {
            FileStream stream = new FileStream(
                photoFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);

            byte[] photo = reader.ReadBytes((int) stream.Length);

            File.WriteAllBytes(outputimg, photo);


            reader.Close();
            stream.Close();
            return photo;
        }


        public DbConnection Connection { get; set; }

        public void Connect()
        {
            Connection = new FbConnection(ConnectionString);
            try
            {
                Console.WriteLine("Open connections.");
                Connection.Open();
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void CloseConnect()
        {
            Connection.Close();
        }

        public UserData GetData()
        {

          var ud = new UserData(); 
          //  FbDataReader reader;
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
                    ud.Id = Reader[0].ToString(); // берем хотя бы один
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
                Connection.Close();
            }

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
                myCommand.Parameters.Add("@userblob", readByteImage(testimg));
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
