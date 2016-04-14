using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;

namespace DBSync
{
    class Program
    {
        public static void Main(string[] args)
        {
            FireBird fb = new FireBird();
          //  fb.dbDoSelect();
            fb.insertTestData();

   
        }



    }
}
