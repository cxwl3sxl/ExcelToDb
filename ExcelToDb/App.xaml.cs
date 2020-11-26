namespace ExcelToDb
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            DbDiscover.DiscoverManager.Instance.Init();
            PinFun.Wpf.Theme.ThemeManager.Instance.EnableTheme();
        }
    }
}
