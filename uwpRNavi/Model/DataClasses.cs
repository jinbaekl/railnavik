using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace uwpRNavi.Model
{
    public class SimpleStation
    {
        public string Station_CD { get; set; }
        public string Station_NM_Kor { get; set; }
        public int distance { get; set; }
        public string Distance { get
            {
                if(distance >= 1000)
                {
                    return string.Format("{0}km", Math.Round(distance/1000.0,1));
                }
                else
                {
                    return string.Format("{0}m", distance);
                }
            } }

        public override string ToString()
        {
            return Station_NM_Kor;
        }

        public string Station_Lineinfo { get; set; }
        public int LineIndex { get
            {
                return StaticStore.LineCharToInt(Station_Lineinfo);
            }
        }
        public string LineChar { get
            {
                return StaticStore.lineCharacterReal[LineIndex];
            } }
        public SolidColorBrush LineColor
        {
            get
            {
                Windows.UI.Color wc = (Windows.UI.Color)XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color),StaticStore.lineCharacterColor[LineIndex]);
                return new SolidColorBrush(wc);
            }
        }
        public string LineName
        {
            get
            {
                return StaticStore.lineName[LineIndex];
            }
        }

        public string statnId { get; set; }

        public double DistanceVisibility { get
            {
                return distance > 0 ? 0.5: 0;
            }
        }
    }

    public class SimpleLine
    {
        public string LineChar { get; set; }
        public string Name { get
            {
                int test = Array.FindIndex(StaticStore.lineCharacter,(s) => { return s == LineChar; });
                if(test >= 0)
                {
                    return StaticStore.lineName[test];
                }
                return "";
            } }
    }

    public class SimpleRealtimeStationCard : INotifyPropertyChanged
    {
        private string _arvlMsg2;
        public string arvlMsg2 { get
            {
                return _arvlMsg2;
            }
            set
            {
                _arvlMsg2 = value;
                RaisePropertyChanged("arvlMsg2"); 
            }
        }
        private string _arvlMsg3;
        public string arvlMsg3
        {
            get
            {
                return _arvlMsg3;
            }
            set
            {
                _arvlMsg3 = value;
                RaisePropertyChanged("arvlMsg3");
            }
        }
        private string _bStatnNm;
        public string bStatnNm { get
            {
                return _bStatnNm;
            } set
            {
                _bStatnNm = value;
                RaisePropertyChanged("toTrain");
            }
        }
        private string _bTrainNo;
        public string bTrainNo { get
            {
                return _bTrainNo;
            }
                set
            {
                _bTrainNo = value;
                RaisePropertyChanged("toTrain");
            }
        }
        public string cStatnNm { get; set; }
        public string updnLine { get; set; }
        public string toTrain { get
            {
                return string.Format("{0} - {1}", cStatnNm, bStatnNm);
            } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    enum RealtimeType
    {
        Station,
        Line
    }

    public class RealtimeList
    {
        private int myVar;

        public int MyProperty
        {
            get { return myVar; }
            set { myVar = value; }
        }

    }

    public enum NavEnumTodo
    {
        Start,
        Walk,
        Ride,
        Takeoff,
        Transfer,
        End
    }

    public class NavTodo
    {
        public NavEnumTodo Type { get; set; }
        public Symbol Symbol { get
            {
                switch(Type)
                {
                    case NavEnumTodo.Start: return Symbol.Play;
                    case NavEnumTodo.Ride: return Symbol.Forward;
                    case NavEnumTodo.Takeoff: return Symbol.Back;
                    case NavEnumTodo.Walk: return Symbol.Street;
                    case NavEnumTodo.Transfer: return Symbol.Refresh;
                    case NavEnumTodo.End: return Symbol.Accept;
                }
                return Symbol.More;
            }
        } 
        public string Label { get; set; }
        public string Sublabel { get; set; }
    }
    
    class Train
    {
    }

    public class Journey
    {
        public int overallTravelTimeInSec { get; set; }
        public int overallTravelTimeInSecAvg { get; set; }
        public int overallTravelDistance { get; set; }
        public int overallNumStations { get; set; }
        public int overallNumTransfers { get; set; }
        public int overallTransferTimeSec { get; set; }

        public List<JourneyStation> Stations { get; set; }
    }

    public class JourneyStation
    {
        
    }
}
