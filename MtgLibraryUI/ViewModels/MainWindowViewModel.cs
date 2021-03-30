using System;

namespace MtgLibraryUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainViewModel MtgViewModel { get; }
        public MainWindowViewModel(MtgData.DataAccess db)
        {
            Console.WriteLine("Loading DB Data...");
            MtgViewModel = new MainViewModel(db.GetPlayers());
            Console.WriteLine("DB Data loaded");
        }
        public string Greeting => "Little library tool for Magic: The Gathering players";
    }
}
