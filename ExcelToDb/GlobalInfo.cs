using ExcelToDb.DbDiscover;
using PinFun.Core.DataBase;
using PinFun.Core.Utils;

namespace ExcelToDb
{
    class GlobalInfo : Singleton<GlobalInfo>
    {
        private string _server, _db, _uid, _pwd;
        private IDbDiscover _discover;

        public void SetDbInfo(IDbDiscover discover, string server, string db, string uid, string pwd)
        {
            _discover = discover;
            _server = server;
            _db = db;
            _uid = uid;
            _pwd = pwd;
        }

        public Db GetDb()
        {
            return new Db(_discover.DbType, _discover.GetConnectionString(_server, _db, _uid, _pwd), false);
        }
    }
}
