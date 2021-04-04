using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MtgLibraryUI.Pages
{
    public class PlayersPage : UserControl
    {
        public PlayersPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}