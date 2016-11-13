using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    return string.Format("{0}km", distance);
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
    }

    class Line
    {
    }

    class Direction
    {
        public int Value { get; set; }
        public string Label { get; set; }
    }
    
    class Train
    {
    }
}
