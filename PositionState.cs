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
        public string currency { get; set; }
        public string balance { get; set; }
        public string locked { get; set; }
        public string avg_buy_price { get; set; }
        public string unit_currency { get; set; }

        public PositionState(string _currency, string _balance, string _locked, string _avg_buy_price, string _unit_currency)
        {
            currency = _currency;
            balance = _balance;
            locked = _locked;
            avg_buy_price = _avg_buy_price;
            unit_currency = _unit_currency;
        }

    }
}
