using System.Windows;
using System.Windows.Forms;
using PinFun.Wpf.Controls;

namespace ExcelToDb
{
    /// <summary>
    /// BrowseExcel.xaml 的交互逻辑
    /// </summary>
    public partial class BrowseExcel : IUseViewModel<BrowseExcelVm>
    {
        public BrowseExcel()
        {
            InitializeComponent();
        }

        public BrowseExcelVm ViewModel => GetViewModel<BrowseExcelVm>();

        private void BrowseExcel_OnClick(object sender, RoutedEventArgs e)
        {
            var ofd = new FolderBrowserDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ViewModel.Browse(ofd.SelectedPath);
            }
        }
    }
}
