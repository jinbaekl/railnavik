using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uwpRNavi.Model
{
    public static class StaticStore
    {
        public static bool ContainsAny(this string haystack, params string[] needles)
        {
            foreach (string needle in needles)
            {
                if (haystack.Contains(needle))
                    return true;
            }

            return false;
        }

        public static string[] holidays = new string[]
        {
            "01.01","03.01","04.08","05.05","06.06","08.15","10.03","10.09","12.25",
            "2017.01.28","2017.05.03","2017.10.04","2018.02.16","2018.05.22","2018.09.24"
        };
        public static int Week { get
            {
                if(DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.ToString(@"yyyy\.MM\.dd").ContainsAny(holidays))
                {
                    return 3;
                }
                else if(DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
        }
        public static string[] lineCharacter = new string[] { "1","2","3","4","5","6","7","8","9","A","B","E","G","I","J","K","O","S","U","W","Y" };
        public static string[] lineName = new string[] { "1호선", "2호선", "3호선", "4호선", "5호선", "6호선", "7호선", "8호선", "9호선", "공항철도", "분당선", "인천2호선", "경춘선", "인천1호선", "J", "경의중앙선", "경강선", "신분당선", "의정부경천절", "수인선", "용인경전철" };

        public static SimpleStation[] LastNearStation { get; set; }
    }
}
