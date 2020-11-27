using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExcelDataReader;
using log4net;
using PinFun.Core.DataBase;
using PinFun.Wpf;

namespace ExcelToDb
{
    public class ImportDialogVm : ViewModelBase
    {
        private int _total, _finished, _error, _rowCount;
        private readonly List<TableMap> _allMaps = new List<TableMap>();
        private readonly object _mapLock = new object();

        public ImportDialogVm()
        {
            Threads = new ObservableCollection<WorkThread>();
        }

        public void Setup(int threadCount, TableMap[] maps)
        {
            var th = Threads.ToArray();
            foreach (var workThread in th)
            {
                workThread.Stop();
            }
            Threads.Clear();

            _allMaps.Clear();
            _allMaps.AddRange(maps);
            _total = maps.Length;
            _finished = 0;
            UpdateProcess();

            for (var i = 0; i < threadCount; i++)
            {
                Threads.Add(new WorkThread(GetNexTable, Report));
            }
        }

        public ObservableCollection<WorkThread> Threads { get; }

        public string Process
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        void UpdateProcess()
        {
            Process = _total == 0 ? $"共计：{_total}个，已完成文件：100%" : $"共计：{_total}个，已完成文件：{(_finished * 100.0 / _total):F}%，错误数量：{_error}，成功插入：{_rowCount}";
        }

        void Report(bool error)
        {
            if (error)
                Interlocked.Increment(ref _error);
            else
                Interlocked.Increment(ref _rowCount);
            UpdateProcess();
        }

        TableMap GetNexTable()
        {
            lock (_mapLock)
            {
                var exist = _allMaps.FirstOrDefault();
                if (exist == null)
                {
                    UpdateProcess();
                    return null;
                }

                _allMaps.Remove(exist);
                _finished++;
                if (_finished > _total) _finished = _total;
                UpdateProcess();
                return exist;
            }
        }
    }

    public class WorkThread : ViewModelBase
    {
        private readonly Func<TableMap> _getNextTableFunc;
        private bool _stop;
        private readonly ILog _log = LogManager.GetLogger(typeof(WorkThread));
        private readonly Action<bool> _reportError;
        private readonly Db _db;

        public WorkThread(Func<TableMap> getNextTable, Action<bool> reportError)
        {
            _reportError = reportError;
            _getNextTableFunc = getNextTable;
            _db = GlobalInfo.Instance.GetDb();
            State = "运行中";
            Task.Run(Start);
        }

        public string CurrentFile
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string State
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public int Total
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public int Finished
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public int Progress
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        void Start()
        {
            var map = _getNextTableFunc();
            try
            {
                ProcessMap(map);
            }
            catch (Exception ex)
            {
                State = $"出错";
                _log.Warn("处理出错", ex);
            }
        }

        void ProcessMap(TableMap map)
        {
            if (map == null)
            {
                State = "已完成";
                return;
            }

            CurrentFile = map.FileName;

            DataSet ds;
            using (var stream = File.Open(map.FullPath, FileMode.Open, FileAccess.Read))
            {
                var reader = ExcelReaderFactory.CreateReader(stream);
                ds = reader.AsDataSet();
                reader.Close();
                reader.Dispose();
                stream.Close();
            }

            if (ds != null && ds.Tables.Count >= 1)
            {
                Total = ds.Tables[0].Rows.Count - 1;
                Finished = 0;
                Progress = 0;
                var cols = new Dictionary<string, string>();
                foreach (DataColumn column in ds.Tables[0].Columns)
                {
                    cols[ds.Tables[0].Rows[0][column].ToString()] = column.ColumnName;
                }

                for (var i = 1; i <= Total; i++)
                {
                    var sql = GlobalInfo.Instance.BuildInsert(map.TableName, ds.Tables[0].Rows[i], cols);
                    try
                    {
                        _db.Execute(sql);
                        _reportError(false);
                    }
                    catch (Exception ex)
                    {
                        _reportError(true);
                        if (GlobalInfo.Instance.SwitchConfig.IgnoreError)
                        {
                            _log.Debug($"插入错误({map.FileName}：{i})：{sql}", ex);
                        }
                        else
                        {
                            if (ex.Message.Contains("将截断字符串或二进制数据"))
                            {
                                _log.Debug($"字符超长({map.FileName}：{i})：{sql}", ex);
                            }
                            else if (ex.Message.Contains("PRIMARY KEY"))
                            {
                                _log.Debug($"主键冲突({map.FileName}：{i})：{sql}", ex);
                            }
                            else if (ex.Message.Contains("列不允许有 Null 值"))
                            {
                                _log.Debug($"主键为空({map.FileName}：{i})：{sql}", ex);
                            }
                            else
                            {
                                _log.Debug($"其他错误({map.FileName}：{i})：{sql}");
                                throw;
                            }
                        }
                    }

                    if (_stop) break;
                    //模拟处理
                    Finished++;
                    Progress = Finished * 300 / Total;
                }
            }

            if (_stop)
            {
                State = "已停止";
                return;
            }

            ProcessMap(_getNextTableFunc());
        }

        public void Stop()
        {
            State = "正在停止";
            _stop = true;
        }
    }
}
