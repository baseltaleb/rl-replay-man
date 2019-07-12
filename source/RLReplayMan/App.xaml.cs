using CefSharp;
using CefSharp.Wpf;
using System.Windows;

namespace RLReplayMan
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var cefSettings = new CefSettings();
            cefSettings.SetOffScreenRenderingBestPerformanceArgs();
            Cef.Initialize(cefSettings);
        }
    }
}
