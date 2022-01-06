using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinGo
{
    public class PositionState
    {
        //currency, balance, locked, avg_buy_price, unit_currency
        public string code { get; set; }
        public string balance { get; set; }
        public string locked { get; set; }
        public string avg_buy_price { get; set; }
        public string cur_price { get; set; }

        public PositionState(string _code, string _balance, string _locked, string _avg_buy_price, string _cur_price)
        {
            code = _code;
            balance = _balance;
            locked = _locked;
            avg_buy_price = _avg_buy_price;
            cur_price = _cur_price;
        }

    }
}
