using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MtgLibraryUI.Pages
{
    public class TokensPage : UserControl
    {
        public TokensPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}