using System;
using System.Collections.Generic;
using System.Linq;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoinGo
{
    public partial class Main
    {
        public void DisplayTargetCoins(string currency, string balance, string buy_price, string cur_price, string rate, string pnl)
        {


            if (positionDataGrid.InvokeRequired)
            {
                positionDataGrid.Invoke(new MethodInvoker(delegate ()
                {
                    positionDataGrid.Invoke(new MethodInvoker(delegate ()
                    {
                        if (positionDataGrid.Rows.Count > 1)
                        {
                            foreach (DataGridViewRow row in positionDataGrid.Rows)
                            {
                                if (row.Cells["code"].Value == null)
                                    break;


                                if (row.Cells["code"].Value.ToString() == currency || row.Cells["code"].Value == null)
                                {
                                    row.Cells["code"].Value = currency;
                                    row.Cells["Quantity"].Value = balance;
                                    row.Cells["buyPrice"].Value = buy_price;
                                    row.Cells["curPrice_position"].Value = cur_price;
                                    row.Cells["rate"].Value = rate;
                                    row.Cells["tradingPnL"].Value = pnl;
                                    return;
                                }
                            }

                            positionDataGrid.Rows.Add(currency, balance, buy_price, cur_price, rate, pnl);
                        }

                        else positionDataGrid.Rows.Add(currency, balance, buy_price, cur_price, rate, pnl);

                    }));
                }));
            }

            else
            {
                if (positionDataGrid.Rows.Count > 1)
                {
                    foreach (DataGridViewRow row in positionDataGrid.Rows)
                    {
                        if (row.Cells["code"].Value == null)
                            break;


                        if (row.Cells["code"].Value.ToString() == currency || row.Cells["code"].Value == null)
                        {
                            row.Cells["code"].Value = currency;
                            row.Cells["Quantity"].Value = balance;
                            row.Cells["buyPrice"].Value = buy_price;
                            row.Cells["curPrice_position"].Value = cur_price;
                            row.Cells["rate"].Value = rate;
                            row.Cells["tradingPnL"].Value = pnl;
                            return;
                        }
                    }

                    positionDataGrid.Rows.Add(currency, balance, buy_price, cur_price, rate, pnl);
                }

                else positionDataGrid.Rows.Add(currency, balance, buy_price, cur_price, rate, pnl);
            }
        }
    }
}


