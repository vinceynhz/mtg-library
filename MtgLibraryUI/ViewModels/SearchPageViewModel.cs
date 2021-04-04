using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Nito.Mvvm;
using ReactiveUI;

using MtgData;

namespace MtgLibraryUI.ViewModels
{
    public class SearchPageViewModel : ViewModelBase
    {
        string searchName = string.Empty;
        string searchType = string.Empty;
        string searchSet = string.Empty;
        bool searching = false;
        int resultCount = 0;
        public string SearchName
        {
            get => searchName;
            set => this.RaiseAndSetIfChanged(ref searchName, value);
        }

        public string SearchType
        {
            get => searchType;
            set => this.RaiseAndSetIfChanged(ref searchType, value);
        }

        public string SearchSet
        {
            get => searchSet;
            set => this.RaiseAndSetIfChanged(ref searchSet, value);
        }

        public bool Searching
        {
            get => searching;
            set => this.RaiseAndSetIfChanged(ref searching, value);
        }

        public int ResultCount
        {
            get => resultCount;
            set => this.RaiseAndSetIfChanged(ref resultCount, value);
        }
        private LoggingService Logging;
        public ObservableCollection<MtgData.Card> SearchResults { get; set; }

        public SearchPageViewModel()
        {
            this.Logging = LoggingService.Instance;
            this.SearchResults = new ObservableCollection<Card>();
        }

        public void Search()
        {
            if (!string.IsNullOrEmpty(this.SearchName))
            {
                var msg = $"Searching: {this.SearchName} - {this.SearchType} ({this.SearchSet})";
                this.Searching = true;
                this.Logging.Log(LogLevel.INFO, msg);
                NotifyTask.Create(SearchAsync());
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
                    this.Logging.Log(LogLevel.INFO, card.ToString());
                });
            }
            this.Searching = false;
        }
    }
}