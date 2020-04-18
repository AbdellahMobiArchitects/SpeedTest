using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SpeedTest.Views.Test
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestGeoMatchSpeedPage : ContentPage
    {
        #region private: const
        private const double _radiusEarthMiles = 3959;
        private const double _radiusEarthKM = 6371;
        private const double _m2km = 1.60934;
        private const double _toRad = Math.PI / 180;
        #endregion

        #region HASSAN2 => TECHNOPARK

        double line1_StartPoint_lat = 33.608160;
        double line1_StartPoint_lon = -7.632696;

        double line1_EndPoint_lat = 33.542660;
        double line1_EndPoint_lon = -7.639656;

        #endregion

        #region SINDIBAD => GOYAVE

        double line2_StartPoint_lat = 33.582771;
        double line2_StartPoint_lon = -7.693986;

        double line2_EndPoint_lat = 33.554333;
        double line2_EndPoint_lon = -7.624169;

        #endregion

        public TestGeoMatchSpeedPage()
        {
            InitializeComponent();
        }

        public bool TestRunning { get; set; }
        private async void StartTest_Clicked(object sender, EventArgs e)
        {
            if (TestRunning)
            {
                return;
            }
            else
            {
                TestRunning = true;
            }

            lblStatus.Text = "Running";
            await Task.Delay(1000);

            try
            {
                var unitePasLine1_lat = (Math.Abs(line1_StartPoint_lat) - Math.Abs(line1_EndPoint_lat)) / 10000;
                var unitePasLine1_lon = (Math.Abs(line1_StartPoint_lon) - Math.Abs(line1_EndPoint_lon)) / 10000;

                var unitePasLine2_lat = (Math.Abs(line2_StartPoint_lat) - Math.Abs(line2_EndPoint_lat)) / 10000;
                var unitePasLine2_lon = (Math.Abs(line2_StartPoint_lon) - Math.Abs(line2_EndPoint_lon)) / 10000;

                var Line1Points = new List<LatLon>();
                var Line2Points = new List<LatLon>();

                for (int i = 0; i < 10000; i++)
                {
                    Line1Points.Add(new LatLon()
                    {
                        Lat = line1_EndPoint_lat + i * unitePasLine1_lat,
                        Lon = line1_EndPoint_lon + i * unitePasLine1_lon,
                    });

                    Line2Points.Add(new LatLon()
                    {
                        Lat = line2_EndPoint_lat + i * unitePasLine2_lat,
                        Lon = line2_StartPoint_lon + i * unitePasLine2_lon,
                    });
                }

                var startTime = new Stopwatch();
                startTime.Start();

                int matchCount = 0;
                int loopCount = 0;


                Parallel.ForEach(Line1Points, (item) =>
                {
                    foreach (var item2 in Line2Points)
                    {
                        if (DistanceMetresSEP(item.Lat, item.Lon, item2.Lat, item2.Lon) < 5)
                        {
                            matchCount++;
                        }
                        loopCount++;


                        if (loopCount % 1000000 == 0)
                            Console.WriteLine($"{loopCount / 1000000} % => {startTime.Elapsed.TotalSeconds} s");
                    }
                });

                startTime.Stop();

                var info = $"total loops => {loopCount}" +
                    $"{Environment.NewLine}" +
                    $"total match => {matchCount}" +
                    $"{Environment.NewLine}" +
                    $"total seconds => {startTime.Elapsed.TotalSeconds}";

                await DisplayAlert("info!", info, "OK");
            }
            finally
            {
                TestRunning = false;
                lblStatus.Text = "Standby";
            }
        }

        public static double DistanceMetresSEP(double Lat1,
                               double Lon1,
                               double Lat2,
                               double Lon2)
        {
            try
            {
                double _radLat1 = Lat1 * _toRad;
                double _radLat2 = Lat2 * _toRad;
                double _dLat = (_radLat2 - _radLat1);
                double _dLon = (Lon2 - Lon1) * _toRad;

                double _a = (_dLon) * Math.Cos((_radLat1 + _radLat2) / 2);

                // central angle, aka arc segment angular distance
                double _centralAngle = Math.Sqrt(_a * _a + _dLat * _dLat);

                // great-circle (orthodromic) distance on Earth between 2 points
                return _radiusEarthMiles * _centralAngle * 1609;
            }
            catch { throw; }
        }


    }

    public class LatLon
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public override string ToString()
        {
            return $"{Lat},{Lon}"; ;
        }
    }
}