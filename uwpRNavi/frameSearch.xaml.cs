using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using uwpRNavi.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class frameSearch : Page
    {
        public SimpleStation From { get; set; }
        public SimpleStation To { get; set; }

        public frameSearch()
        {
            this.InitializeComponent();
            
        }

        private void cbOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tpTime != null)
            {
                if (cbOption.SelectedIndex == 0)
                {
                    dpDate.Visibility = Visibility.Collapsed;
                    tpTime.Visibility = Visibility.Collapsed;
                }
                else if (cbOption.SelectedIndex > 2)
                {
                    dpDate.Visibility = Visibility.Visible;
                    tpTime.Visibility = Visibility.Collapsed;
                }
                else
                {
                    dpDate.Visibility = Visibility.Visible;
                    tpTime.Visibility = Visibility.Visible;
                }
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(StaticStore.LastNearStation != null && StaticStore.LastNearStation.Length > 0)
            {
                asbFrom.PlaceholderText = StaticStore.LastNearStation[0].Station_NM_Kor;
            }
            string[] alert = (await Communication.GetNotice()).Split(';');
            if(alert.Length >= 3)
            {
                hbButton.Content = alert[2];
            }
        }

        private void asbFrom_GotFocus(object sender, RoutedEventArgs e)
        {
            if (asbFrom.Text.Length == 0 && StaticStore.LastNearStation != null)
            {
                asbFrom.IsSuggestionListOpen = true;
            }
        }

        private async void asbTo_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (sender.Text.Length == 0 && StaticStore.LastNearStation != null)
                {
                    sender.ItemsSource = StaticStore.LastNearStation.ToList();
                }
                else
                {
                    var stns = await Communication.GetChosungOrKeywordStation(sender.Text);
                    if (stns == null)
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
