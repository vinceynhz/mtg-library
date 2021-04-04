using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MtgLibraryUI.Pages
{
    public class CardsPage : UserControl
    {
        public CardsPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}