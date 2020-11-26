using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using PinFun.Core.DataBase;

namespace ExcelToDb.DbDiscover
{
    public interface IDbDiscover
    {
        DbTypes DbType { get; }

        Task<string[]> GetDbNames(string server, string uid, string pwd);

        string GetConnectionString(string server, string db, string uid, string pwd);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="row"></param>
        /// <param name="colMap">key为数据库的列名称，value为excel的列名称</param>
        /// <returns></returns>
        string BuildInsert(string tableName, DataRow row, Dictionary<string, string> colMap);
    }
}
