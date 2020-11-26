using System.Windows;
using PinFun.Wpf.Controls;

namespace ExcelToDb
{
    /// <summary>
    /// ImportDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ImportDialog : IUseViewModel<ImportDialogVm>
    {
        private readonly int _threadCount;
        private readonly TableMap[] _maps;

        public ImportDialog(int threadCount, TableMap[] maps)
        {
            _threadCount = threadCount;
            _maps = maps;
            InitializeComponent();
            Loaded += ImportDialog_Loaded;
        }

        private void ImportDialog_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Setup(_threadCount, _maps);
        }

        public ImportDialogVm ViewModel => GetViewModel<ImportDialogVm>();
    }
}
