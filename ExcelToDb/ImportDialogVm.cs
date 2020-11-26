using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PinFun.Wpf;

namespace ExcelToDb
{
    public class ImportDialogVm : ViewModelBase
    {
        private int _total, _finished;
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
                Threads.Add(new WorkThread(GetNexTable));
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
            Process = _total == 0 ? $"共计：{_total}个，已完成：100%" : $"共计：{_total}个，已完成：{(_finished * 100.0 / _total):F}%";
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
        private bool _stop = false;

        public WorkThread(Func<TableMap> getNextTable)
        {
            _getNextTableFunc = getNextTable;
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
            ProcessMap(map);
        }

        void ProcessMap(TableMap map)
        {
            if (map == null)
            {
                State = "已完成";
                return;
            }

            CurrentFile = map.FileName;

            Total = 10;
            Finished = 0;
            Progress = 0;

            for (var i = 0; i < 10; i++)
            {
                Thread.Sleep(2000);
                if (_stop) break;
                //模拟处理
                Finished++;
                Progress = Finished * 300 / Total;
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
