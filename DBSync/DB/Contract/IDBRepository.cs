using System.Data.Common;
using DBSync.Model;

namespace DBSync.DB.Contract
{
    public interface IDBRepository
    {
        DbConnection Connection { get; set; }
        
        void Connect();
        void CloseConnect();
        UserData GetData();
        void InsertData(UserData ud);
        void GetListExistsTables();

    }
}