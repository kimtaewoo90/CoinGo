using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CoinGo
{
    public class CandleState
    {

        //public List<string> unit = new List<string>();
        public List<string> code = new List<string>();
        public List<string> date_time = new List<string>();
        public List<string> opening_price = new List<string>();
        public List<string> high_price = new List<string>();
        public List<string> low_price = new List<string>();
        public List<string> trade_price = new List<string>();
        public List<string> total_trading_price = new List<string>();
        public List<string> total_trading_volume = new List<string>();

        public CandleState(string data)
        {
            List<string> data_list = data.Split(new char[] { '{' }).ToList();
            //data_list = data_list[1..];
            int count = data_list.Count;

            for (int i=1; i < count; i++)
            {
                code.Add(data_list[i].Split(new char[] { ',' })[0].Split(new char[] { ':' })[1]);
                date_time.Add(data_list[i].Split(new char[] { ',' })[2].Substring(24, 19));
                opening_price.Add(data_list[i].Split(new char[] { ',' })[3].Split(new char[] { ':' })[1]);
                high_price.Add(data_list[i].Split(new char[] { ',' })[4].Split(new char[] { ':' })[1]);
                low_price.Add(data_list[i].Split(new char[] { ',' })[5].Split(new char[] { ':' })[1]);
                trade_price.Add(data_list[i].Split(new char[] { ',' })[6].Split(new char[] { ':' })[1]);
                total_trading_price.Add(data_list[i].Split(new char[] { ',' })[8].Split(new char[] { ':' })[1]);
                total_trading_volume.Add(data_list[i].Split(new char[] { ',' })[9].Split(new char[] { ':' })[1]);
                //unit.Add(data_list[i].Split(new char[] { ',' })[10].Split(new char[] { ':' })[1]);
            }
            
        }

        public bool IsSameTime(string time)
        {
            //var compared_time = Convert.ToDateTime(Params.date_time[Params.date_time.Count - 1]);
            var rtd_time = Convert.ToDateTime(time);

            //if (compared_time == rtd_time) return true;
            //else return false;
            return false;
        }
    }


}
