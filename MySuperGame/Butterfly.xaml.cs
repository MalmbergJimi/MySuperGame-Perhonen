using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MySuperGame
{
    public sealed partial class Butterfly : UserControl
    {
        // Animate Butterfly timer, ajastin milloin "kuvaa" vaihdetaan
        private DispatcherTimer timer;

        // Offset to show, eli mikä kohta perhosen kuvasta näytetään suorakulmion sisällä
        private int currentFrame = 0;
        private int direction = 1; // 1 or -1 Vaihtuuko frame "isommaksi" vai "pienemmäksi"
        private int frameheight = 132;

        // Speed, nopeus
        private readonly double MaxSpeed = 10;      // Maxnopeus perhoselle
        private readonly double Accelerate = 0.5;   // Kiihtyvyys
        private double speed;

        // Angel, perhosen kulma
        private readonly double AngleStep = 5;
        private double Angle = 0;

        // Perhosen sijainti Canvaksella, collision detectionia varten julkisia.
        public double LocationX { get; set; }
        public double LocationY { get; set; }


        public Butterfly()
        {
            this.InitializeComponent();

            // Animate 
            timer = new DispatcherTimer();
            // 125ms,   Aikaväli päivittämiselle (fps)
            timer.Interval = new TimeSpan(0, 0, 0, 0, 125);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            // Current frame 0,1,2,3,4   nykyinen frame perhosesta
            if (direction == 1) currentFrame++;
            else currentFrame--;
            if (currentFrame == 0 || currentFrame == 4)
            {
                direction = -1 * direction; // 1 tai -1
            }
            // Set offset
            SpriteSheetOffset.Y = currentFrame * -frameheight;
        }

        // Move -metodi
        public void Move()
        {
            // More speed
            speed += Accelerate;
            if (speed > MaxSpeed) speed = MaxSpeed;

            // Set location values (angle, speed)
            LocationX -= (Math.Cos(Math.PI / 180 * (Angle + 90))) * speed;
            LocationY -= (Math.Sin(Math.PI / 180 * (Angle + 90))) * speed;
        }

        public void Rotate(int direction)
        {
            Angle += direction * AngleStep; // -1 * 5 tai 1 * 5
            ButterflyRotateAngle.Angle = Angle;
        }

        // Update location, päivitetään sijainti Canvaksella
        public void SetLocation()
        {
            SetValue(Canvas.LeftProperty, LocationX);
            SetValue(Canvas.TopProperty, LocationY);
        }
    }
}
