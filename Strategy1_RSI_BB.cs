using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinGo
{
    public class Strategy1_RSI_BB
    {
        public string ticker { get; set; }

        public Strategy1_RSI_BB(string code)
        {
            ticker = code;
        }

        public bool Is_Signal()
        {
            var signal = false;
            if (!Params.Signal_1.ContainsKey(ticker)) Params.Signal_1[ticker] = false;
            if (!Params.Signal_2.ContainsKey(ticker)) Params.Signal_2[ticker] = false;
            var first_signal = Params.Signal_1[ticker];
            var second_signal = Params.Signal_2[ticker];


            if (Params.RSI_List[ticker][Params.RSI_List[ticker].Count - 1] < 30) first_signal = true;
            if (first_signal is true && Params.RSI_List[ticker][Params.RSI_List[ticker].Count - 1] > 30) second_signal = true;

            if (first_signal && second_signal) signal = true;
            
            return signal;
        }
    }
}
