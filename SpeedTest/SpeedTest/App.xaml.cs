using SpeedTest.Views.Test;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SpeedTest
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new TestGeoMatchSpeedPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
