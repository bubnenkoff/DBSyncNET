using DBSync.DB.Contract;

namespace DBSync.Model

{
   public struct UserData
    {
        public string Id;
        public string Guid { get; set; }
        public string Name { get; set; }
        public byte[] UserBlob { get; set; }
        public string FL { get; set; }

     
    };
}