using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using MtgLibraryUI.ViewModels;

namespace MtgLibraryUI.Controls
{
    public class SearchResult : UserControl
    {
        public SearchResult()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}