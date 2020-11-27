using System.Collections.Generic;
using System.Data;
using ExcelToDb.DbDiscover;
using PinFun.Core.DataBase;
using PinFun.Core.Utils;

namespace ExcelToDb
{
    class GlobalInfo : Singleton<GlobalInfo>
    {
        private string _server, _db, _uid, _pwd;
        private IDbDiscover _discover;

        public GlobalInfo()
        {
            SwitchConfig = new SwitchConfig();
        }

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

        public string BuildInsert(string tableName, DataRow row, Dictionary<string, string> colMap)
        {
            return _discover.BuildInsert(tableName, row, colMap);
        }

        public SwitchConfig SwitchConfig { get; }
    }

    public class SwitchConfig
    {
        public bool IgnoreError { get; set; }
    }
}
