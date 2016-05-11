using DBSync.DB.Contract;

namespace DBSync.Model

{
   public  class UserData
    {
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public byte[] UserBlob { get; set; }
        public string FL { get; set; }

     
    };
}