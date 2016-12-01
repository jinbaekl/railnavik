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
        ObservableCollection<SimpleRealtimeStationCard> ocRS = new ObservableCollection<SimpleRealtimeStationCard>();
        SimpleStation selected = null;
        DispatcherTimer dtTimer = new DispatcherTimer();

        public frameRealtime()
        {
            this.InitializeComponent();
            dtTimer.Interval = new TimeSpan(0, 0, 10);
            dtTimer.Tick += DtTimer_Tick;
            dtTimer.Start();
        }

        private void DtTimer_Tick(object sender, object e)
        {
            if(selected != null)
            {
                RealtimePos(selected);
            }
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
                if(!pos.Coordinate.Point.Position.Latitude.InRange(36.602917, 38.242152) || !pos.Coordinate.Point.Position.Longitude.InRange(126.301692, 128.164636))
                {
                    prLoading.IsActive = false;
                    prLoading.Visibility = Visibility.Collapsed;
                    StaticStore.ShowToastNotification("RailNavi 위치 특정 불가", "위치를 특정할 수 없습니다. 직접 역을 지정하세요.");
                    return;
                }
                StaticStore.LastPos = pos.Coordinate.Point.Position;
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
            else
            {
                prLoading.IsActive = false;
                prLoading.Visibility = Visibility.Collapsed;
                StaticStore.ShowToastNotification("RailNavi 위치 특정 불가", "위치를 특정할 수 없습니다. 직접 역을 지정하세요.");
            }
            lvNow.ItemsSource = ocRS;
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
                selected = args.SelectedItem as SimpleStation;
                RealtimePos(args.SelectedItem as SimpleStation);
            }
        }

        private async void RealtimePos(SimpleStation simpleStation)
        {
            prLoading.IsActive = true;
            prLoading.Visibility = Visibility.Visible;
            if (simpleStation != null)
            {
                var cards = await Communication.GetRealtimeStnTrain(simpleStation.statnId);
                while (ocRS.Count > cards.Length)
                {
                    ocRS.RemoveAt(ocRS.Count - 1);
                }
                for (int i = 0; i < cards.Length; i++)
                {
                    if (i < ocRS.Count)
                    {
                        ocRS[i].cStatnNm = cards[i].cStatnNm;
                        ocRS[i].bStatnNm = cards[i].bStatnNm;
                        ocRS[i].arvlMsg2 = cards[i].arvlMsg2;
                        ocRS[i].arvlMsg3 = cards[i].arvlMsg3;
                    }
                    else
                    {
                        ocRS.Add(cards[i]);
                    }
                }
            }
            prLoading.IsActive = false;
            prLoading.Visibility = Visibility.Collapsed;
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

