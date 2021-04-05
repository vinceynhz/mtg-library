using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;


using Nito.Mvvm;
using ReactiveUI;

using MtgData;

namespace MtgLibraryUI.ViewModels
{
    public class SearchPageViewModel : ViewModelBase
    {
        string searchName = string.Empty;
        public string SearchName
        {
            get => searchName;
            set => this.RaiseAndSetIfChanged(ref searchName, value);
        }

        string searchType = string.Empty;
        public string SearchType
        {
            get => searchType;
            set => this.RaiseAndSetIfChanged(ref searchType, value);
        }

        string searchSet = string.Empty;
        public string SearchSet
        {
            get => searchSet;
            set => this.RaiseAndSetIfChanged(ref searchSet, value);
        }

        bool loading = false;
        public bool Loading
        {
            get => loading;
            set => this.RaiseAndSetIfChanged(ref loading, value);
        }

        int resultCount = 0;
        public int ResultCount
        {
            get => resultCount;
            set => this.RaiseAndSetIfChanged(ref resultCount, value);
        }

        private Image Card;
        private Bitmap DefaultBitmap;
        private LoggingService Logging;
        public ObservableCollection<MtgData.Card> SearchResults { get; set; }

        public SearchPageViewModel(Image card)
        {
            this.Card = card;
            this.Logging = LoggingService.Instance;
            this.SearchResults = new ObservableCollection<Card>();

            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var url = "avares://MtgLibraryUI/Assets/card-back.png";
            this.DefaultBitmap = new Bitmap(assets.Open(new Uri(url)));
            this.Card.Source = this.DefaultBitmap;
        }

        public void Search()
        {
            if (!string.IsNullOrEmpty(this.SearchName))
            {
                var msg = $"Searching: {this.SearchName} - {this.SearchType} ({this.SearchSet})";
                this.Loading = true;
                this.Logging.Log(LogLevel.INFO, msg);
                NotifyTask.Create(SearchAsync());
            }
        }

        public void DoSomething(MtgData.Card card)
        {
            this.Card.Opacity = 0;
            this.Card.Source = this.DefaultBitmap;
            this.Card.Opacity = 1;
            if (!string.IsNullOrEmpty(card.ImageUrl))
            {
                this.Loading = true;
                NotifyTask.Create(LoadCardAsync(card.ImageUrl));
            }
            else
            {
                this.Logging.Warn($"No image URL available for {card.Name} - {card.SetCode}");
            }
        }

        public async Task SearchAsync()
        {
            List<MtgData.Card> result = await MtgLibrary.CardService.AsyncSearchCard(
                this.SearchName,
                this.SearchType,
                this.SearchSet
            );
            if (result.Count > 0)
            {
                this.ResultCount = result.Count;
                this.SearchResults.Clear();
                result.ForEach(card =>
                {
                    this.SearchResults.Add(card);
                });
            }
            this.Loading = false;
        }

        private async Task LoadCardAsync(string url)
        {
            Stream stream = await MtgLibrary.CardService.AsyncLoadImage(url);
            this.Card.Opacity = 0;
            this.Card.Source = new Bitmap(stream);
            this.Card.Opacity = 1;
            this.Loading = false;
            this.Logging.Info("Card updated async");
        }
    }
}