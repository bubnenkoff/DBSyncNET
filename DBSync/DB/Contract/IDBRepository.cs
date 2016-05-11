using System.Collections.Generic;
using System.Data.Common;
using DBSync.Model;

namespace DBSync.DB.Contract
{
    public interface IDBRepository
    {
        DbConnection Connection { get; set; }
        
        void Connect();
        void CloseConnect();
        List<UserData> GetData();
        void InsertData(List<UserData> ud);
        void GetListExistsTables();

    }
}