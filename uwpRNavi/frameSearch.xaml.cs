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
                if (cbOption.SelectedIndex > 1)
                {
                    tpTime.Visibility = Visibility.Collapsed;
                }
                else
                {
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
    }
}
