using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MtgLibraryUI.Pages
{
    public class SettingsPage : UserControl
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}