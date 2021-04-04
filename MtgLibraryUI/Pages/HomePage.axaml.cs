using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using MtgLibraryUI.ViewModels;

namespace MtgLibraryUI.Pages
{
    public class HomePage : UserControl
    {
        public HomePage()
        {
            InitializeComponent();
            this.DataContext = new HomePageViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}