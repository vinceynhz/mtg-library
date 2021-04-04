using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using MtgLibraryUI.ViewModels;

namespace MtgLibraryUI.Views
{
    public class MainView : UserControl
    {
        // private Image Card { get; set; }
        public MainView()
        {
            InitializeComponent();
            // Set the data context
            this.DataContext = new MainViewModel();

            this.FindControl<Button>("Settings").Click += delegate
            {
                var window = new Window
                {
                    Height = 400,
                    Width = 600,
                    Content = new MtgLibraryUI.Pages.SettingsPage(),
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    ShowInTaskbar = false,
                    Title = "Magic: The Gathering - Settings",
                    FontFamily = "Ubuntu, Segoe UI"
                };
                window.ShowDialog(GetWindow());
            };
            // this.Card = this.Find<Image>("Card");
            // var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            // var uri = "avares://MtgLibraryUI/Assets/card-back.png";
            // Console.WriteLine("Setting card Sync");
            // this.Card.Source = new Bitmap(assets.Open(new Uri(uri)));
            // Console.WriteLine("Card set sync");
            // var uri2 = "https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=491714&type=card";
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private Window GetWindow() => (Window)this.VisualRoot;
    }
}