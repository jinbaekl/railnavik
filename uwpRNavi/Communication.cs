using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using uwpRNavi.Model;
using System.Net;
using System.Xml.Linq;

namespace uwpRNavi
{
    public class MyResponse
    {
        private bool _success;

        public bool Success
        {
            get { return _success; }
        }

        private string _content;

        public string Content
        {
            get { return _content; }
        }

        private bool _isjson;

        public bool IsJson
        {
            get { return _isjson; }
            set { _isjson = value; }
        }


        private JObject _jarray;

        public JObject Json
        {
            get { return _jarray; }
        }

        public MyResponse(bool success, string content)
        {
            _success = success;
            _content = content;
            try
            {
                _jarray = JObject.Parse(content);
                _isjson = true;
            }
            catch
            {
                _isjson = false;
            }
        }
    }

    public static class Communication
    {
        private const string proc = "http://projflag.cafe24.com/railnavik/request.php";

        public static string regkey = null;

        private static async Task<MyResponse> Get(string uri)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage responseGet = await client.GetAsync(uri);
                string response = await responseGet.Content.ReadAsStringAsync();

                return new MyResponse(responseGet.IsSuccessStatusCode, response);
            }
            catch(Exception ex)
            {
                return new MyResponse(false, ex.GetType().ToString());
            }
        }

        public static async void Register()
        {
            var resp = await Get(proc + "?do=register");
            if (resp.IsJson)
            {
                var token = resp.Json["result"];
                regkey = (string)token;
                System.Diagnostics.Debug.WriteLine(regkey);
            }
        }

        public static async Task<JToken> GetNearestStation(Windows.Devices.Geolocation.BasicGeoposition pos)
        {
            var resp = await Get(proc + string.Format("?do=gpsstn&regkey={0}&lat={1}&long={2}", regkey, pos.Latitude, pos.Longitude));
            if(resp.IsJson)
            {
                return resp.Json["result"];
            }
            return null;
        }

        public static async Task<JToken> GetChosungOrKeywordStation(string chokey)
        {
            var resp = await Get(proc + string.Format("?do=findstn&regkey={0}&word={1}", regkey, WebUtility.UrlEncode(chokey)));
            if (resp.IsJson)
            {
                return resp.Json["result"];
            }
            return null;
        }

        public static async Task<SimpleLine[]> GetLineByStnCode(string code)
        {
            var resp = await Get(proc + string.Format("?do=stnline&regkey={0}&sid={1}", regkey, code));
            if (resp.IsJson)
            {
                var result = resp.Json["result"];
                List<SimpleLine> lsSL = new List<SimpleLine>();
                foreach(var line in result.Children())
                {
                    SimpleLine sl = new SimpleLine();
                    sl.LineChar = (string)line;
                    lsSL.Add(sl);
                }
                return lsSL.ToArray();
            }
            return null;
        }

        public static async Task<SimpleRealtimeStationCard[]> GetRealtimeStnTrain(string code)
        {
            var resp = await Get(string.Format("http://210.96.13.82:8099/api/rest/subwayInfo/getArvlByInfo?statnId={0}&subwayId={1}", code, code.Substring(0,4)));
            var xdoc = XDocument.Parse(resp.Content);
            var data = from query in xdoc.Descendants("itemList")
                       select new SimpleRealtimeStationCard
                       {
                           arvlMsg2 = (string)query.Element("arvlMsg2"),
                           arvlMsg3 = (string)query.Element("arvlMsg3"),
                           bStatnNm = (string)query.Element("bStatnNm"),
                           bTrainNo = (string)query.Element("bTrainNo"),
                           cStatnNm = (string)query.Element("cStatnNm"),
                           updnLine = (string)query.Element("updnLine")
                       };
            return data.ToArray();
        }

        public static async Task<string> GetNotice()
        {
            var resp = await Get(proc + "?do=alert");
            return resp.Content;
        }

        public static async Task<JToken> GetToiletStation(Windows.Devices.Geolocation.BasicGeoposition pos)
        {
            var resp = await Get(proc + string.Format("?do=toilet&regkey={0}&lat={1}&long={2}", regkey, pos.Latitude, pos.Longitude));
            if (resp.IsJson)
            {
                return resp.Json["result"];
            }
            return null;
        }

        public static async Task<List<NavTodo>> GetSimpleRoute(string start, string end, DateTime dt, int type)
        {
            List<NavTodo> lsnt = new List<NavTodo>();
            var resp = await Get(proc + string.Format("?do=simpleroute&start={0}&end={1}&day={2}&type={3}&when={4}", start, end, StaticStore.GetWeek(dt), type, dt.ToString("yyyyMMddHHmm")+"00"));
            if (resp.IsJson)
            {
                try
                {
                    var paths = resp.Json["result"]["subwayPaths"][0]["path"]["routes"].Children();
                    foreach(var p in paths)
                    {
                        lsnt.Add(new NavTodo() { Label = string.Format("{0} 탑승",(string)p["logicalLine"]["name"]), Sublabel = string.Format("{0} 방면", (string)p["logicalLine"]["direction"]), Type = NavEnumTodo.Transfer });
                        foreach (var q in p["stations"])
                        {
                            lsnt.Add(new NavTodo() { Label = (string)q["name"], Sublabel = string.Format("{0}:{1}", ((string)q["departureTime"]).Substring(0, 2), ((string)q["departureTime"]).Substring(2, 2)), Type = NavEnumTodo.Ride });
                        }
                    }
                    return lsnt;
                }
                catch
                {
                    StaticStore.ShowToastNotification("올바르지 않은 탐색", "탐색 결과가 없습니다.");
                    return null;
                }
            }
            return null;
        }
    }
}
