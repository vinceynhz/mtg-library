using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

using Nito.Mvvm;
using ReactiveUI;

using MtgData;

namespace MtgLibraryUI.ViewModels
{

    public class MainViewModel : ViewModelBase
    {


        public ObservableCollection<MtgData.Player> Players { get; set; }
        private NotifyTask InitializationNotifier { get; set; }
        private DataAccess Db { get; set; }
        private LoggingService Logging { get; set; }

        public MainViewModel()
        {
            this.Logging = MtgData.LoggingService.Instance;
            this.Db = new DataAccess();
            this.Players = new ObservableCollection<MtgData.Player>();
            InitializationNotifier = NotifyTask.Create(InitializeAsync());
        }

        // private void Logger(LogLevel level, string msg)
        // {
        //     var toLog = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {level.ToString()} - {msg}";
        //     Console.WriteLine(toLog);
        //     this.Log += toLog + "\n";
        //     this.OnLog();
        // }

        private async Task InitializeAsync()
        {
            await Task.Delay(2000);
            // Starting up the database
            this.Logging.Log(LogLevel.INFO, "Loading Players from DB");
            IEnumerable<MtgData.Player> players = await this.Db.AsyncGetPlayers();
            this.Logging.Log(LogLevel.INFO, "Players Loaded");
            this.Logging.Log(LogLevel.INFO, "Adding Players to View");
            this.Players.Clear();
            foreach (var p in players)
            {
                this.Players.Add(p);
            }
            this.Logging.Log(LogLevel.INFO, "Players Added to View");
            //     Stream stream = await MtgLibrary.CardService.AsyncLoadImage(uri);
            //     Console.WriteLine(stream);
            //     this.Card.Opacity = 0;
            //     this.Card.Source = new Bitmap(stream);
            //     this.Card.Opacity = 1;
            //     Console.WriteLine("Card updated async");
        }
    }
}