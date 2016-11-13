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
        SimpleStation selected = null;

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
                        var newitem = new SimpleStation()
                        {
                            Station_CD = (string)stn["Station_CD"],
                            Station_NM_Kor = (string)stn["Station_NM_Kor"],
                            distance = (int)stn["distance"],
                            Station_Lineinfo = (string)stn["Station_Lineinfo"],
                            statnId = (string)stn["statnId"]
                        };
                        ocStation.Add(newitem);
                    }
                    asbStation.PlaceholderText = ocStation[0].Station_NM_Kor;
                    StaticStore.LastNearStation = ocStation.ToArray();
                    selected = ocStation[0];
                    RealtimePos(ocStation[0]);
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

        private void asbStation_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            
        }

        private void asbStation_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem != null)
            {
                RealtimePos(args.SelectedItem as SimpleStation);
            }
        }

        private async void RealtimePos(SimpleStation simpleStation)
        {
            if(simpleStation != null)
            {
                var cards = await Communication.GetRealtimeStnTrain(simpleStation.statnId);
                lvNow.ItemsSource = cards.ToList();
            }
        }

        private async void asbStation_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (asbStation.Text.Length == 0)
                {
                    sender.ItemsSource = ocStation;
                    selected = ocStation[0];
                }
                else
                {
                    var stns = await Communication.GetChosungOrKeywordStation(asbStation.Text);
                    if(stns == null)
                    {
                        return;
                    }
                    List<SimpleStation> lsSS = new List<SimpleStation>();
                    foreach (var stn in stns)
                    {
                        SimpleStation ss = new SimpleStation();
                        ss.Station_CD = (string)stn["Station_CD"];
                        ss.Station_NM_Kor = (string)stn["Station_NM_Kor"];
                        ss.Station_Lineinfo = (string)stn["Station_Lineinfo"];
                        ss.statnId = (string)stn["statnId"];
                        lsSS.Add(ss);
                    }
                    sender.ItemsSource = lsSS;
                }
            }
        }
    }
}

