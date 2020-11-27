using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using PinFun.Wpf;
using PinFun.Wpf.Controls;

namespace ExcelToDb
{
    public class BrowseExcelVm : ViewModelBase
    {
        public BrowseExcelVm()
        {
            Files = new ObservableCollection<TableMap>();
            RefreshTableNameCommand = new RelayCommand(RefreshTableName);
            ThreadCount = Environment.ProcessorCount;
            TableNameReplace = @"dbo.;_\d+";
            IgnoreError = true;
        }

        public string Dir
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string TableNameReplace
        {
            get => GetValue<string>();
            set
            {
                SetValue(value);
                RefreshTableName();
            }
        }

        public bool AutoCreateTable
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public bool IgnoreError
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public int ThreadCount
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public ObservableCollection<TableMap> Files { get; set; }

        public void Browse(string dir)
        {
            Dir = dir;
            Files.Clear();
            ProcessDir(dir, "*.xls");
            ProcessDir(dir, "*.xlsx");
        }

        void ProcessDir(string dir, string ext)
        {
            var files = Directory.GetFiles(dir, ext);
            foreach (var file in files)
            {
                Files.Add(new TableMap(file));
            }
        }

        public ICommand RefreshTableNameCommand { get; set; }

        void RefreshTableName()
        {
            try
            {
                var regexString = TableNameReplace.Split(';');

                var regexs = (from reg in regexString
                              where !string.IsNullOrWhiteSpace(reg)
                              select new Regex(reg)).ToArray();

                foreach (var file in Files)
                {
                    file.UpdateFileName(regexs);
                }
            }
            catch (Exception ex)
            {
                ShowToast($"刷新表名称出错：{ex.Message}", MessageLevel.Waring);
            }
        }
    }

    public class TableMap : ViewModelBase
    {
        public TableMap(string file)
        {
            FullPath = file;
            FileName = Path.GetFileNameWithoutExtension(file);
            TableName = FileName;
        }

        public string FullPath { get; }

        public string FileName { get; }

        public string TableName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public void UpdateFileName(Regex[] replace)
        {
            if (replace == null) return;
            TableName = FileName;
            foreach (var regex in replace)
            {
                foreach (var match in regex.Matches(FileName))
                {
                    TableName = TableName.Replace(match.ToString(), "");
                }
            }
        }
    }
}
