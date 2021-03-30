using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MtgLibraryUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<MtgData.Player> Players { get; }
        public MainViewModel(IEnumerable<MtgData.Player> players)
        {
            Console.WriteLine("Setting up players");
            Players = new ObservableCollection<MtgData.Player>(players);
            Console.WriteLine("Players set");
        }
    }
}