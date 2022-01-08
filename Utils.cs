using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace CoinGo
{
    public class Utils
    {
        public string get_cur_tm()
        {
            DateTime cur_time;
            string cur_tm;

            cur_time = DateTime.Now;
            cur_tm = cur_time.ToString("HH:mm:ss");

            return cur_tm;
        }


        // 지연함수 구현
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public DateTime delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                try
                {
                    unsafe
                    {
                        System.Windows.Forms.Application.DoEvents();
                    }
                }
                catch (AccessViolationException ex)
                {
                    //logs.write_sys_log("delay() ex.Message : [" + ex.Message + "]\r\n", 0);
                }

                ThisMoment = DateTime.Now;
            }

            return DateTime.Now;
        }

       
    }
}
