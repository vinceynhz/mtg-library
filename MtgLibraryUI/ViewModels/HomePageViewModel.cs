using Nito.Mvvm;
using ReactiveUI;

using MtgData;

namespace MtgLibraryUI.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        private string log = string.Empty;
        public string Log
        {
            get => this.log;
            set => this.RaiseAndSetIfChanged(ref this.log, value);
        }
        private LoggingService Logging { get; set; }

        public HomePageViewModel()
        {
            this.Logging = MtgData.LoggingService.Instance;
            LogDelegate logDelegate = (msg) => this.Log += msg;
            this.Logging.OnLog = logDelegate;
        }

    }
}