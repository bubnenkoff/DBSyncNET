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

        struct UserData
        {
            public int id;
            public string guid;
            public string name;
            public byte[] userblob;
        };


        public void PGConnect()
        {

          //  UserData [] uds;
            List<UserData> uds = new List<UserData>();
            UserData ud;

            NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5433;User Id=" + config.PGLogin + ";" +
               "Password=" + config.PGPass + ";Database=" + config.PGdbName + ";");

            try
            {
                conn.Open();
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
