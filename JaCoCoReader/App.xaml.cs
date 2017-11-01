using System.Windows;
using JaCoCoReader.Core.Threading;

namespace JaCoCoReader
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ThreadDispatcher.Dispatcher = new WpfDispatcher();

            base.OnStartup(e);
        }
    }
}
