using System.Threading.Tasks;
using PinFun.Core.DataBase;

namespace ExcelToDb.DbDiscover
{
    public interface IDbDiscover
    {
        DbTypes DbType { get; }

        Task<string[]> GetDbNames(string server, string uid, string pwd);

        string GetConnectionString(string server, string db, string uid, string pwd);
    }
}
