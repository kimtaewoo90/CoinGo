using System;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
//using System.Net.WebSockets;

namespace CoinGo
{
    class UpbitWebSocket
    {
        //Logs logs = new Logs();
        //Main main = new Main();

        public string MakeSendMsg(List<string> Tickers, string Type)
        {
            var array = JArray.FromObject(Tickers);

            JObject obj1 = new JObject();
            obj1["ticket"] = Guid.NewGuid();//UUID

            JObject obj2 = new JObject();
            obj2["type"] = Type;
            obj2["codes"] = array;

            return string.Format("[{0},{1}]", obj1.ToString(), obj2.ToString());
        }



    }
}
