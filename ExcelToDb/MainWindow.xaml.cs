using System.Windows;

namespace ExcelToDb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Next_OnClick(object sender, RoutedEventArgs e)
        {
            if (!GetViewModel<MainWindowVm>().CanNext()) return;

            new BrowseExcel().Show();
            Close();
        }
    }
}
