using Avalonia;
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
            this.DataContext = new SearchPageViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}