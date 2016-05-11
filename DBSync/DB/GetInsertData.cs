using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBSync.DB.Contract;
using DBSync.Model;

namespace DBSync.DB
{
    class GetInsertData
    {
        private Config config;
        private SQLite sqllite;
        private FireBird fb;
        private Postgres pg;

        public GetInsertData(Config config, SQLite sqllite, FireBird fb, Postgres pg)
        {
            this.config = config;
            this.sqllite = sqllite;
            this.fb = fb;
            this.pg = pg;
        }

        public void ConnectSQLite()
        {
            throw new NotImplementedException();
        }

        public void CloseConnectSQLite()
        {
            throw new NotImplementedException();
        }

        public UserData GetDataSQLite()
        {
            throw new NotImplementedException();
        }

        public void InsertDataSQLite(UserData ud)
        {
            throw new NotImplementedException();
        }

        public void GetListExistsTablesSQLite()
        {
            throw new NotImplementedException();
        }

        //-------------------------

        public void ConnectPG()
        {
            throw new NotImplementedException();
        }

        public void CloseConnectPG()
        {
            throw new NotImplementedException();
        }

        public UserData GetDataPG()
        {
            throw new NotImplementedException();
        }

        public void InsertDataPG(UserData ud)
        {
            throw new NotImplementedException();
        }

        public void GetListExistsTablesPG()
        {
            throw new NotImplementedException();
        }
        //-----------------------

        public void ConnectFB()
        {
            throw new NotImplementedException();
        }

        public void CloseConnectFB()
        {
            throw new NotImplementedException();
        }

        public UserData GetDataFB()
        {
            throw new NotImplementedException();
        }

        public void InsertDataFB(UserData ud)
        {
            throw new NotImplementedException();
        }

        public void GetListExistsTablesFB()
        {
            throw new NotImplementedException();
        }


    }
}
