using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using PinFun.Core.DataBase;
using PinFun.Core.Utils;

namespace ExcelToDb.DbDiscover
{
    class DiscoverManager : Singleton<DiscoverManager>
    {
        readonly List<IDbDiscover> _dbDiscovers = new List<IDbDiscover>();
        private readonly ILog _log = LogManager.GetLogger(typeof(DiscoverManager));

        public DiscoverManager()
        {
            var discovers = AppAssemblyManager.Instance.FindAllInterfaceImpl<IDbDiscover>(true);
            foreach (var discover in discovers)
            {
                try
                {
                    if (discover.GetNewObject<IDbDiscover>() is { } dbDiscover)
                    {
                        _dbDiscovers.Add(dbDiscover);
                    }
                }
                catch (Exception ex)
                {
                    _log.Warn("创建数据库发现程序出错", ex);
                }
            }
        }

        public void Init()
        {

        }

        public bool GetDiscover(DbTypes dbType, out IDbDiscover dbDiscover)
        {
            dbDiscover = _dbDiscovers.FirstOrDefault(a => a.DbType == dbType);
            return dbDiscover != null;
        }
    }
}
