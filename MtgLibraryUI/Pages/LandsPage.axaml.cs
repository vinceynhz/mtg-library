using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MtgLibraryUI.Pages
{
    public class LandsPage : UserControl
    {
        public LandsPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}