using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;

namespace DBSync
{
    class FireBird
    {
        public static string connectionString =
                 "User=SYSDBA;" +
                 "Password=masterkey;" +
                 "Database=D:\\Project\\2016\\DBSync\\TEST.FDB;" +
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

        public static string testimg = "D:/Project/2016/DBSync/test.png";
        public static string outputimg = "D:/Project/2016/DBSync/output.png";

       

       public void dbDoSelect()
        {
            FbConnection myFBConnection = new FbConnection(connectionString); 
            try
            {
                Console.WriteLine("Open connections.");
                myFBConnection.Open();
                FbTransaction myTransaction = myFBConnection.BeginTransaction();
                FbCommand myCommand = new FbCommand();
                myCommand.Connection = myFBConnection;
                myCommand.Transaction = myTransaction;

                myCommand.CommandText = "SELECT id, userblob FROM USERS WHERE ID=1";
                myCommand.Connection = myFBConnection;
                myCommand.Transaction = myTransaction;

                byte[] userBlob;


                FbDataReader reader = myCommand.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(reader.GetString(0));
                        // MemoryStream memStream = new MemoryStream(userBlob);
                         //userBlob = Encoding.ASCII.GetBytes(reader.GetString(1));
                        int ordinal = 1;
                        userBlob = new byte[reader.GetBytes(ordinal, 0, null, 0, 0)];
                        reader.GetBytes(0, 0, userBlob, 0, userBlob.Length); 

                        File.WriteAllBytes(outputimg, userBlob);
                        //userBlob = (byte[])reader[0];
                        Console.WriteLine(userBlob);

                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Can't read data from DB");
                }

                finally
                {
                    reader.Close();
                }

            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            finally
            {
                myFBConnection.Close();
            }

            Console.ReadKey();

        }


      public void insertTestData()
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

          catch(Exception e)
           {
               Console.WriteLine(e.Message);
           }
           // Console.ReadKey();

       }


       public static byte[] readByteImage(string photoFilePath)
       {
           FileStream stream = new FileStream(
               photoFilePath, FileMode.Open, FileAccess.Read);
           BinaryReader reader = new BinaryReader(stream);

           byte[] photo = reader.ReadBytes((int)stream.Length);

           File.WriteAllBytes(outputimg, photo);


           reader.Close();
           stream.Close();
           return photo;


       }

        




    }

   


}
