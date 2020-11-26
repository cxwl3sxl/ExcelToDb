using System.Linq;
using System.Threading.Tasks;
using PinFun.Core.DataBase;

namespace ExcelToDb.DbDiscover
{
    class SqlServer : IDbDiscover
    {
        public DbTypes DbType => DbTypes.MsSql;

        public async Task<string[]> GetDbNames(string server, string uid, string pwd)
        {
            using var db = new Db(DbType, $"Password={pwd};User ID={uid};Initial Catalog=master;Data Source={server}",
                false);
            var dbs = await db.QueryAsync<string>("SELECT name FROM [sys].[databases] ORDER BY database_id asc");
            return dbs.ToArray();
        }

        public string GetConnectionString(string server, string db, string uid, string pwd)
        {
            return $"Password={pwd};User ID={uid};Initial Catalog={db};Data Source={server}";
        }
    }
}
