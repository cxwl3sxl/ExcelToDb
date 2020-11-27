using System;
using System.Collections.Generic;
using System.Data;
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

        public string BuildInsert(string tableName, DataRow row, Dictionary<string, string> colMap)
        {
            var values = new List<string>();
            var columns = new List<string>();
            foreach (var kv in colMap)
            {
                columns.Add($"[{kv.Key}]");
                var v = row[kv.Value];
                if (v == null || v == DBNull.Value || "NULL".Equals(v.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    values.Add("null");
                }
                else
                {
                    if (v is DateTime dt)
                    {
                        values.Add($"'{dt:yyyy-MM-dd HH:mm:ss}'");
                    }
                    else if (DateTime.TryParse(v.ToString(), out var dt2))
                    {
                        values.Add($"'{dt2:yyyy-MM-dd HH:mm:ss}'");
                    }
                    else
                    {
                        values.Add($"'{v.ToString()?.Replace("'", "''")}'");
                    }
                }
            }

            return $"insert into [{tableName}]({string.Join(",", columns)}) values({string.Join(",", values)})";
        }
    }
}
