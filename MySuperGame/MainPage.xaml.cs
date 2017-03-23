using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MySuperGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Butterfly Perhonen
        private Butterfly butterfly;

        // Flower, Flowers, Kukkia
        private List<Flower> flowers;

        // Which keys are pressed,  mitkä näppäimet ovat alaspainettuina
        private bool UpPressed;     // Onko Ylöspäin nuolta painetty
        private bool LeftPressed;
        private bool RightPressed;

        // Game Loop Timer
        private DispatcherTimer timer;

        // Audio, ääni
        private MediaElement mediaElement;




        public MainPage()
        {
            this.InitializeComponent();

            // add butterfly
            butterfly = new Butterfly
            {
                LocationX = MyCanvas.Width / 2,
                LocationY= MyCanvas.Height / 2
            };
            // Add butterfly to Canvas, lisätään perhonen Canvakselle
            MyCanvas.Children.Add(butterfly);

            // Init list of Flowers, alustetaan kukkalista
            flowers = new List<Flower>();

            // Key Listeners,   näppäimien kuuntelua, onko jokin painettuna vai ei?
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;  // Onko näppäimi alhaalla
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;      //Onko näppäimiä "ylhäällä"

            // Mouse Listeners,  hiiren kuuntelu
            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;

            // Load Audio, ladataan ääni
            LoadAudio();
            
            // Start game loop,     PELI LÄHTEE HETI KÄYNTIIN
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        // Load audio when colliding has happened, ladataan audio kun törmäys on tapahtunut
        private async void LoadAudio()
        {
            StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            StorageFile file = await folder.GetFileAsync("ding.wav");
            var stream = await file.OpenAsync(FileAccessMode.Read);

            mediaElement = new MediaElement();      // Ladataan audio valmiiksi muistiin, mutta ei vielä soiteta
            mediaElement.AutoPlay = false;
            mediaElement.SetSource(stream, file.ContentType);
        }

        private void CoreWindow_PointerPressed(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            Flower flower = new Flower();
            flower.LocationX = args.CurrentPoint.Position.X - flower.Width / 2;     // Kukka tulee juuri hiiren kursorin kohdalle
            flower.LocationY = args.CurrentPoint.Position.Y - flower.Width / 2;
            // Add to Canvas, lisätään kukka näytölle
            MyCanvas.Children.Add(flower);
            flower.SetLocation();
            // Add to flowers list, lisätään kukkalistaan
            flowers.Add(flower);
        }

        //game loop
        private void Timer_Tick(object sender, object e)
        {
            // Move butterfly if up pressed
            if (UpPressed) butterfly.Move();

            // Rotate butterfly if left/right pressed
            if (LeftPressed) butterfly.Rotate(-1);   // -1 == left
            if (RightPressed) butterfly.Rotate(1);   //  1 == right

            // Update butterfly , Perhosen paikka Canvaksella päivitetään
            butterfly.SetLocation();

            // Collision...
            CheckCollision();
        }
        private void CheckCollision()
        {
            // Loop flower list, Käydään läpi kukkalista
            foreach(Flower flower in flowers)
            {
                // Get Rects, katsotaan osuuko mikään kukkalistan kukista perhoseen
                Rect BRect = new Rect(                                               // Perhosen sijainti ja koko
                    butterfly.LocationX, butterfly.LocationY, butterfly.ActualWidth, butterfly.ActualHeight
                    );
                Rect FRect = new Rect(                                               // Kukan sijainti ja koko
                    flower.LocationX, flower.LocationY, flower.ActualWidth, flower.ActualHeight
                    );
                // Does objects intersects, törmääkö objektit
                BRect.Intersect(FRect);
                if (!BRect.IsEmpty) // Jos palautettu arvo EI OLE TYHJÄ
                {
                    // Collision! Area isn't empty, törmäys - alue ei ole tyhjä
                    // Remove flower from Canvas, poistetaan Kukka Canvakselta
                    MyCanvas.Children.Remove(flower);
                    // Remove from list, poistetaan myös listasta kukka
                    flowers.Remove(flower);
                    // Play audio
                    mediaElement.Play();
                    
                    break;
                }

            }
        }

        private void CoreWindow_KeyUp(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)    // Tutkii tableteilta, puhelimilta, koneilta
            {
                case VirtualKey.Up:
                    UpPressed = false;
                    break;
                case VirtualKey.Left:
                    LeftPressed = false;
                    break;
                case VirtualKey.Right:
                    RightPressed = false;
                    break;

            }
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)    // Tutkii tableteilta, puhelimilta, koneilta
            {
                case VirtualKey.Up:
                    UpPressed = true;
                    break;
                case VirtualKey.Left:
                    LeftPressed = true;
                    break;
                case VirtualKey.Right:
                    RightPressed = true;
                    break;

            }
        }
    }
}
