using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Markup;

namespace uwpRNavi.Model
{
    public static class StaticStore
    {

        public static Windows.Devices.Geolocation.BasicGeoposition LastPos = new Windows.Devices.Geolocation.BasicGeoposition();

        public static bool ContainsAny(this string haystack, params string[] needles)
        {
            foreach (string needle in needles)
            {
                if (haystack.Contains(needle))
                    return true;
            }

            return false;
        }

        public static int LineCharToInt(string linechar)
        {
            int i = Array.IndexOf(lineCharacter, linechar);
            return Math.Max(0,i);
        }

        public static string[] holidays = new string[]
        {
            "01.01","03.01","04.08","05.05","06.06","08.15","10.03","10.09","12.25",
            "2017.01.28","2017.05.03","2017.10.04","2018.02.16","2018.05.22","2018.09.24"
        };
        public static int Week { get
            {
                return GetWeek(DateTime.Now);
            }
        }
        public static string[] lineCharacter = new string[] { "1","2","3","4","5","6","7","8","9","A","B","E","G","I","J","K","O","S","U","W","Y" };
        public static string[] lineCharacterReal = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "I2", "G", "I1", "J", "K", "K", "S", "U", "S", "Y" };
        public static string[] lineCharacterColor = new string[] { "#00498B","#009246","#F36630","#00A2D1","#A064A3","#9E4510","#5D6519","#D6406A","#8E764B","#006D9D","#E0A134","#ED8B00","#2ABFD0","#6E98BB","","#72C7A6","#0065B3","#BB1833","#FF850D","#E0A134","#509F22"};
        public static string[] lineName = new string[] { "1호선", "2호선", "3호선", "4호선", "5호선", "6호선", "7호선", "8호선", "9호선", "공항철도", "분당선", "인천2호선", "경춘선", "인천1호선", "J", "경의중앙선", "경강선", "신분당선", "의정부경천절", "수인선", "용인경전철" };

        public static SimpleStation[] LastNearStation { get; set; }


        public static int GetWeek(DateTime targetDate)
        {
            DayOfWeek dof = targetDate.DayOfWeek;
            if (targetDate.Hour < 4)
            {
                dof = (targetDate - new TimeSpan(1, 0, 0, 0)).DayOfWeek;
            }
            if (dof == DayOfWeek.Sunday || targetDate.ToString(@"yyyy\.MM\.dd").ContainsAny(holidays))
            {
                return 3;
            }
            else if (dof == DayOfWeek.Saturday)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        public static void ShowToastNotification(string title, string stringContent)
        {
            ToastNotifier ToastNotifier = ToastNotificationManager.CreateToastNotifier();
            Windows.Data.Xml.Dom.XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            Windows.Data.Xml.Dom.XmlNodeList toastNodeList = toastXml.GetElementsByTagName("text");
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode(title));
            toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(stringContent));
            Windows.Data.Xml.Dom.IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            Windows.Data.Xml.Dom.XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.SMS");

            ToastNotification toast = new ToastNotification(toastXml);
            toast.ExpirationTime = DateTime.Now.AddSeconds(4);
            ToastNotifier.Show(toast);
        }
    }

    public static class IComparableExtension
    {
        public static bool InRange<T>(this T value, T from, T to) where T : IComparable<T>
        {
            return value.CompareTo(from) >= 1 && value.CompareTo(to) <= -1;
        }
    }
}
