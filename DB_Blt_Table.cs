using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinGo
{
    public class DB_Blt_Table
    {
        public DateTime date_time { get; set; }
        public string coin_name { get; set; }
        public double buy_price { get; set; }
        public double sell_price { get; set; }
        public int is_profit { get; set; }
        public double trade_pnl { get; set; }
        public DateTime buy_time { get; set; }
        public DateTime sell_time { get; set; }
        public DateTime update_time { get; set; }

        public DB_Blt_Table(DateTime _datetime, string _coinname, double _buyprice, double _sellprice, int _isprofit, double _trade_pnl, DateTime _buytime, DateTime _selltime, DateTime _updatetime)
        {
            this.date_time = _datetime;
            this.coin_name = _coinname;
            this.buy_price = _buyprice;
            this.sell_time = _selltime;
            this.is_profit = _isprofit;
            this.trade_pnl = _trade_pnl;
            this.buy_time = _buytime;
            this.sell_time = _selltime;
            this.update_time = _updatetime;
        }
    }
}
