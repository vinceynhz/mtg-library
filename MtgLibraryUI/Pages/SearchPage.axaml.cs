using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using MtgLibraryUI.ViewModels;

namespace MtgLibraryUI.Pages
{
    public class SearchPage : UserControl
    {
        public SearchPage()
        {
            InitializeComponent();
            var card = this.Find<Image>("Card");
            this.DataContext = new SearchPageViewModel(card);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}