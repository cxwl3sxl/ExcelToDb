using System.Text;

namespace ExcelToDb
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Init();
        }

        async void Init()
        {
            DbDiscover.DiscoverManager.Instance.Init();
            PinFun.Wpf.Theme.ThemeManager.Instance.EnableTheme();
            await PinFun.Core.Startup.Init();
        }
    }
}
