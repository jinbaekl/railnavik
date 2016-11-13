using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// 빈 페이지 항목 템플릿은 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 에 문서화되어 있습니다.

namespace uwpRNavi
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (PivotHeaderItem phItem in FindVisualChildren<PivotHeaderItem>(pvMain))
            {
                phItem.Height = 70;
                phItem.Margin = new Thickness(0);
                phItem.Padding = new Thickness(30, 0, 30, 0);
            }
        }

        //Find all children
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void pvMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(pvMain.SelectedIndex)
            {
                case 0:
                    if (frFirst.CurrentSourcePageType == null)
                    {
                        frFirst.Navigate(typeof(frameRealtime));
                    }
                    break;
                case 2:
                    if (wvLinemap.Source == null)
                    {
                        wvLinemap.Navigate(new Uri("ms-appx-web:///Map/subway.html"));
                    }
                    break;
            }
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }
    }

}
