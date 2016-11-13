using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using uwpRNavi.Model;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 http://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace uwpRNavi
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class frameRealtime : Page
    {
        ObservableCollection<SimpleStation> ocStation = new ObservableCollection<SimpleStation>();

        public frameRealtime()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            asbStation.ItemsSource = ocStation;
            var accessStatus = await Geolocator.RequestAccessAsync();
            if(accessStatus == GeolocationAccessStatus.Allowed)
            {
                // If DesiredAccuracy or DesiredAccuracyInMeters are not set (or value is 0), DesiredAccuracy.Default is used.
                Geolocator geolocator = new Geolocator();

                // Carry out the operation.
                Geoposition pos = await geolocator.GetGeopositionAsync();

                var stations = await Communication.GetNearestStation(pos.Coordinate.Point.Position);
                if (stations != null)
                {
                    ocStation.Clear();
                    foreach(var stn in stations.Children())
                    {
                        ocStation.Add(new SimpleStation()
                        {
                            Station_CD=(string)((Newtonsoft.Json.Linq.JProperty)stn).Value["Station_CD"],
                            Station_NM_Kor =(string)((Newtonsoft.Json.Linq.JProperty)stn).Value["Station_NM_Kor"],
                            distance=(int)((Newtonsoft.Json.Linq.JProperty)stn).Value["distance"]
                        });
                    }
                }
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Communication.Register();
        }

        private void asbStation_GotFocus(object sender, RoutedEventArgs e)
        {
            if(asbStation.Text.Length == 0)
            {
                asbStation.IsSuggestionListOpen = true;
            }
        }
    }
}

