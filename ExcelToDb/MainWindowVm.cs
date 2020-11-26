using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ExcelToDb.DbDiscover;
using PinFun.Core.DataBase;
using PinFun.Core.Utils;
using PinFun.Wpf;
using PinFun.Wpf.Controls;

namespace ExcelToDb
{
    public class MainWindowVm : ViewModelBase
    {
        public MainWindowVm()
        {
            DbNames = new ObservableCollection<string>();
            ReloadDbNamesCommand = new RelayCommand(ReloadDbNames);
            DbType = LocalSettings.Instance.Get("DbType", DbType);
            ServerIp = LocalSettings.Instance.Get("ServerIp", ServerIp);
            UserName = LocalSettings.Instance.Get("UserName", UserName);
            Password = LocalSettings.Instance.Get("Password", Password);
            DbName = LocalSettings.Instance.Get("DbName", DbName);
        }

        public DbTypes DbType
        {
            get => GetValue<DbTypes>();
            set => SetValue(value);
        }
        public string ServerIp
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string UserName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string Password
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string DbName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public ICommand ReloadDbNamesCommand { get; set; }

        public ObservableCollection<string> DbNames { get; set; }

        async void ReloadDbNames()
        {
            if (!DiscoverManager.Instance.GetDiscover(DbType, out var dbDiscover))
            {
                ShowToast("指定的数据库类型尚未支持！", MessageLevel.Waring);
                return;
            }

            try
            {
                SetBusy(true, "正在刷新数据库");
                var dbNames = await dbDiscover.GetDbNames(ServerIp, UserName, Password);
                DbNames.Clear();
                foreach (var name in dbNames)
                {
                    DbNames.Add(name);
                }
            }
            catch (Exception e)
            {
                ShowToast(e.Message, MessageLevel.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        public bool CanNext()
        {
            if (!DiscoverManager.Instance.GetDiscover(DbType, out var dbDiscover))
            {
                ShowToast("指定的数据库类型尚未支持！", MessageLevel.Waring);
                return false;
            }

            if (string.IsNullOrWhiteSpace(ServerIp))
            {
                ShowToast("请输入服务器地址！", MessageLevel.Waring);
                return false;
            }
            if (string.IsNullOrWhiteSpace(UserName))
            {
                ShowToast("请输入登录名！", MessageLevel.Waring);
                return false;
            }
            if (string.IsNullOrWhiteSpace(DbName))
            {
                ShowToast("请选择数据库！", MessageLevel.Waring);
                return false;
            }
            LocalSettings.Instance.Set("DbType", DbType);
            LocalSettings.Instance.Set("ServerIp", ServerIp);
            LocalSettings.Instance.Set("UserName", UserName);
            LocalSettings.Instance.Set("Password", Password);
            LocalSettings.Instance.Set("DbName", DbName);
            GlobalInfo.Instance.SetDbInfo(dbDiscover, ServerIp, DbName, UserName, Password);
            return true;
        }
    }
}
