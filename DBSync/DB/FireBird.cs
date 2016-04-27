using System;
using System.IO;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBSync.DB.Contract;
using DBSync.Model;
using FirebirdSql.Data.FirebirdClient;
using IniParser;

namespace DBSync
{
    class FireBird : IDBRepository
    {
        Config config;
        string connectionString;

        private FbDataReader Reader { get; set; }

        public FireBird(Config config)
        {
            this.config = config;
            connectionString =
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

        private UserData ud = new UserData();

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
            FbConnection myFBConnection = new FbConnection(connectionString);
            try
            {
                Console.WriteLine("Open connections.");
                myFBConnection.Open();
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
          //  FbDataReader reader;
            try
            {

                FbTransaction myTransaction = (FbTransaction) Connection.BeginTransaction();
                FbCommand myCommand = new FbCommand();
                myCommand.Connection = (FbConnection) Connection;
                myCommand.Transaction = myTransaction;
                myCommand.CommandText = "SELECT id, userblob FROM USERS WHERE ID=1";
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
                return ud;
            }

            finally
            {
                Reader.Close();
            }

            return ud;
        }


        public void InsertData(UserData ud)
        {
            try
            {
                FbConnection myFBConnection = new FbConnection(connectionString);
                myFBConnection.Open();
                FbTransaction myTransaction = myFBConnection.BeginTransaction();
                FbCommand myCommand = new FbCommand();
                myCommand.CommandText = "UPDATE USERS SET USERBLOB = @userblob WHERE ID=1";
                myCommand.Connection = myFBConnection;
                myCommand.Transaction = myTransaction;
                myCommand.Parameters.Add("@userblob", readByteImage(testimg));
                // myCommand.Parameters.Add("@userblob", FbDbType.Binary).Value =  GetPhoto("D:/Project/2016/DBSync/test.png");
                myCommand.ExecuteNonQuery();

                myTransaction.Commit();
                myCommand.Dispose();
                myFBConnection.Close();
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
