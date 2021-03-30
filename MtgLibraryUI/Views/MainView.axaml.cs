using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Nito.Mvvm;

namespace MtgLibraryUI.Views
{
    public class MainView : UserControl
    {
        // private Image Card { get; set; }
        // private NotifyTask InitializationNotifier { get; set; }

        public MainView()
        {
            InitializeComponent();
            // this.Card = this.Find<Image>("Card");
            // var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            // var uri = "avares://MtgLibraryUI/Assets/card-back.png";
            // Console.WriteLine("Setting card Sync");
            // this.Card.Source = new Bitmap(assets.Open(new Uri(uri)));
            // Console.WriteLine("Card set sync");
            // var uri2 = "https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=491714&type=card";
            // InitializationNotifier = NotifyTask.Create(InitializeAsync(uri2));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        // private async Task InitializeAsync(string uri)
        // {
        //     await Task.Delay(2000);
        //     Console.WriteLine("Updting card async");
        //     Stream stream = await MtgLibrary.CardService.AsyncLoadImage(uri);
        //     Console.WriteLine(stream);
        //     this.Card.Opacity = 0;
        //     this.Card.Source = new Bitmap(stream);
        //     this.Card.Opacity = 1;
        //     Console.WriteLine("Card updated async");
        // }
    }
}