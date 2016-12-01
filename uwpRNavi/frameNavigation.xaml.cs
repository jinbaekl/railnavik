using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using uwpRNavi.Model;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;

// 빈 페이지 항목 템플릿에 대한 설명은 http://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace uwpRNavi
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class frameNavigation : Page
    {
        public frameNavigation()
        {
            this.InitializeComponent();

            lvToilet.ItemsSource = ocToilet;
        }

        ObservableCollection<SimpleStation> ocToilet = new ObservableCollection<SimpleStation>();

        private async void Flyout_Opened(object sender, object e)
        {            
            tbErrorToilet.Visibility = Visibility.Collapsed;
            if (StaticStore.LastPos.Latitude.InRange(36.602917,38.242152))
            {
                var stations = await Communication.GetToiletStation(StaticStore.LastPos);
                if (stations != null)
                {
                    ocToilet.Clear();
                    foreach (var stn in stations.Children())
                    {
                        var newitem = new SimpleStation()
                        {
                            Station_CD = (string)stn["Station_CD"],
                            Station_NM_Kor = (string)stn["Station_NM_Kor"],
                            distance = (int)stn["distance"],
                            Station_Lineinfo = (string)stn["Station_Lineinfo"],
                            statnId = (string)stn["statnId"]
                        };
                        ocToilet.Add(newitem);
                    }                    
                }
                lvToilet.Visibility = Visibility.Visible;
            }
            else
            {
                ocToilet.Clear();
                lvToilet.Visibility = Visibility.Collapsed;
                tbErrorToilet.Visibility = Visibility.Visible;
            }
        }
    }
}
