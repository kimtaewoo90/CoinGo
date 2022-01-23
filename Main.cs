﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;

namespace CoinGo
{
    public partial class Main : Form
    {

        Thread PositionThread = null;
        Thread OrderbookThread = null;
        Thread MarketThread = null;
        Thread LogThread = null;
        Thread Strategy2 = null;

        Utils util = new Utils();
        delegate void Ctr_Involk(Control ctr, string text);

        public string Orderbook_ShortCode { get; set; } = "KRW-BTC";

        public Dictionary<string, bool> Is_get_200_candle_data = new Dictionary<string, bool>();

        public Main()
        {
            InitializeComponent();

            ExitProgramBtn.Click += ExitProgramBtnClicked;
            CoinStart();
        }

        public void CoinStart()
        {

            for (int i = 0; i < 20; i++)
            {
                OrderbookDataGrid.Rows.Add("", "", "");
            }

            UpbitWebSocket WS = new UpbitWebSocket();

            var Markets = Params.upbit.GetMarkets();
            var MarketTickers = Params.upbit.GetMarketTicker(Markets).Item1;
            var MarketTickersKrName = Params.upbit.GetMarketTicker(Markets).Item2;

            for (int i = 0; i < MarketTickers.Count; i++)
            {
                Is_get_200_candle_data[MarketTickers[i]] = true;
                Params.Candle_Time[MarketTickers[i]] = "0";
                Params.Is_Start_Strategy2[MarketTickers[i]] = true;
                Params.ForcedSell[MarketTickers[i]] = false;
                Params.Avg_Volume_Now_Candle[MarketTickers[i]] = new List<double>();
                Params.Avg_Price_Now_Candle[MarketTickers[i]] = new List<double>();
                Params.HistoricalTickSpeed[MarketTickers[i]] = new List<double>();
                Params.TradeVolume[MarketTickers[i]] = new List<double>();

                // strategy1
                Params.RSI_list[MarketTickers[i]] = new List<double>();
            }

            // 제외 코인
            Params.ExceptCoinList.Add("KRW-BTC");
            Params.ExceptCoinList.Add("KRW-ETH");
            Params.ExceptCoinList.Add("KRW-BORA");
            Params.ExceptCoinList.Add("KRW-HUM");
            Params.ExceptCoinList.Add("KRW-BTT");


            Params.ForcedSell["KRW-KRW"] = false;
            Params.ForcedSell["KRW-APENFT"] = false;

            // 포지션 초기화
            UpdatePosition();

            // Make SendMsg for WebSocket
            var sendMsg = WS.MakeSendMsg(MarketTickers, "ticker");
            SendWebSocket(sendMsg);

            #region Thread

            PositionThread = new Thread(new ThreadStart(DisplayPosition));
            PositionThread.Start();

            OrderbookThread = new Thread(new ThreadStart(UpdateOrderbook));
            OrderbookThread.Start();

            //MarketThread = new Thread(new ThreadStart(DisplayUniverseMarket));
            //MarketThread.Start();

            LogThread = new Thread(new ThreadStart(UpdateLog));
            LogThread.Start();

            #endregion Thread
        }

        /* WebSocket Thread for Coin Information */
        public void SendWebSocket(string sendMsg)
        {
            WebSocket webSocket = new WebSocket("wss://api.upbit.com/websocket/v1");
            webSocket.OnMessage += Ws_TickerMessage;
            webSocket.Connect();
            if (webSocket.ReadyState == WebSocketState.Open)
            {
                webSocket.Send(sendMsg);
            }
        }

        private void Ws_TickerMessage(object sender, MessageEventArgs e)
        {
            string requestMsg = Encoding.UTF8.GetString(e.RawData);
            JObject res = JObject.Parse(requestMsg);

            Params.cur_time = DateTime.Now;

            // Market Rtd data.
            if (res["type"].ToString() == "ticker")
            {
                // 로스컷 x번이면 종료
                if (Params.LosscutTimes == 3)
                {
                    if (MessageBox.Show("정말 종료합니까?", "YesOrNo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        ExitProgram();
                    }
                    else
                    {
                        Params.LosscutTimes = 0;
                    }
                }

                var code = res["code"].ToString();

                CoinState state = new CoinState(res);
                Params.CoinInfoDict[code] = state;

                // Except 1000원 미만 코인
                if(double.Parse(res["trade_price"].ToString()) < 1000 &&
                    !Params.ExceptCoinList.Contains(code))
                {
                    Params.ExceptCoinList.Add(code);
                }

                if (Params.ExceptCoinList.Contains(code)) return;

                // Max Ratio display
                if (Params.Avg_Price_Now_Candle.ContainsKey(code))
                {
                    if (maxRatio.InvokeRequired)
                    {
                        maxRatio.Invoke(new MethodInvoker(delegate ()
                        {
                            var max_ratio = Params.Avg_Price_Now_Candle.Aggregate((x, y) => x.Value.Sum() > y.Value.Sum() ? x : y).Key;
                            maxRatio.Text = max_ratio;
                            profitTimes.Text = Params.ProfitTimes.ToString();
                            LosscutTimes.Text = Params.LosscutTimes.ToString();
                        }));
                    }
                    else
                    {
                        maxRatio.Text = code;
                    }
                }

                // update position coin's cur_price
                if (Params.CoinPositionDict.ContainsKey(code))
                    Params.CoinPositionDict[code].cur_price = Params.CoinInfoDict[code].curPrice;

                /*
                if (tickspeed.Checked)
                {
                    // TickSpeed
                    if (res["ask_bid"].ToString() == "BID")
                    {
                        Params.TradeVolume[code].Add(double.Parse(res["trade_volume"].ToString()));                             // Change Candle 에서 초기화
                        Params.CurrentTickSpeed[code] = Params.TradeVolume[code].Sum();// / Params.TradeVolume[code].Count;        // 현재 Candle의 스피드 

                        if (Params.HistoricalTickSpeed.ContainsKey(code) && Params.HistoricalTickSpeed[code].Count > 0)
                            Params.SpeedRatio[code] = Params.CurrentTickSpeed[code] / Params.HistoricalTickSpeed[code].Average();
                    }
                }
                else
                {
                    Params.SpeedRatio[code] = 0;
                }
                */

                #region 코인 골라내기

                #endregion

                //if (code != "KRW-BTC") return;
                //if(Params.CoinPositionDict.ContainsKey(code))
                DisplayUniverseMarket(res);

                if (strategy1_check.Checked)
                {
                    Strategy1_RSI_BB strategy1 = new Strategy1_RSI_BB(code);
                    strategy1.GetCandleData();      // test

                    var curTime = Params.cur_time.ToString("yyyyMMddHHmmss");
                    DateTime temp = DateTime.ParseExact(curTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                    var tradedTimeDouble = double.Parse(temp.ToString("yyyyMMddHHmmss"));

                    // visualizing Times
                    if (curTimeText.InvokeRequired)
                    {
                        curTimeText.Invoke(new MethodInvoker(delegate ()
                        {
                            curTimeText.Text = DateTime.Now.ToString("MM/dd HH:mm:ss");
                            CandleCode.Text = code;
                        }));
                    }

                    strategy1.GetCandleData();

                    // Candle Changed
                    if (tradedTimeDouble - double.Parse(Params.Candle_Time[code].ToString()) > 300)
                    {
                        // Get new candle data
                        strategy1.GetCandleData();
                    }
                }

                if (strategy2_check.Checked)
                {

                    try
                    {
                        Strategy2 strategy2 = new Strategy2(code, res);

                        #region Main Logic

                        // Initialize Candle Chart
                        if (Params.Is_Start_Strategy2[code] is true)
                        {
                            // Request candle data
                            strategy2.Get_Avg_Volume_Before_Candle();
                            Params.Is_Start_Strategy2[code] = false;
                        }

                        else
                        {
                            // 로스컷 한 코인은 한시간 동안 Pass
                            if (Params.LosscutCode.ContainsKey(code))
                            {
                                // 1시간 지나면 LosscutCode Dictionary 에서 삭제
                                if (DateTime.Compare(Params.LosscutCode[code].AddHours(1), DateTime.Now) < 0)
                                {
                                    Params.LosscutCode.Remove(code);
                                }

                                // 1시간 안지났으면 그냥 pass
                                else
                                {
                                    return;
                                }
                            }

                            var curTime = Params.cur_time.ToString("yyyyMMddHHmmss");
                            DateTime temp = DateTime.ParseExact(curTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                            var tradedTimeDouble = double.Parse(temp.ToString("yyyyMMddHHmmss"));

                            // visualizing Times
                            if (curTimeText.InvokeRequired)
                            {
                                curTimeText.Invoke(new MethodInvoker(delegate ()
                                {
                                    curTimeText.Text = DateTime.Now.ToString("MM/dd HH:mm:ss");
                                    candleTimeText.Text = DateTime.ParseExact(Params.Candle_Time[code], "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).ToString("MM/dd HH:mm:ss");
                                    CandleCode.Text = code;
                                }));
                            }

                            // Candle Changed
                            if (tradedTimeDouble - double.Parse(Params.Candle_Time[code].ToString()) > 300)
                            {
                                if(Params.CurrentTickSpeed.ContainsKey(code))
                                    Params.HistoricalTickSpeed[code].Add(Params.CurrentTickSpeed[code]);    // Tick speed

                                // 초기화
                                Params.TradeVolume[code] = new List<double>();                              //Tick speed   
                                Params.Avg_Volume_Now_Candle[code] = new List<double>();
                                Params.Avg_Price_Now_Candle[code] = new List<double>();

                                Params.BuySignalRatio[code] = 0.0;

                                // 새로운 캔들봉 요청하기
                                strategy2.Get_Avg_Volume_Before_Candle();

                                return;
                            }

                            // Candle Continue
                            else
                            {
                                if (Params.Avg_Volume_Now_Candle.ContainsKey(code) && Params.Avg_Volume_Before_20_Candle.ContainsKey(code))
                                {
                                    Params.Avg_Volume_Now_Candle[code].Add(Math.Abs(double.Parse(res["trade_volume"].ToString())));
                                    Params.Avg_Price_Now_Candle[code].Add(
                                        Math.Abs(double.Parse(res["trade_price"].ToString())) *
                                        Math.Abs(double.Parse(res["trade_volume"].ToString())));

                                    Params.BuySignalRatio[code] = Params.Avg_Volume_Now_Candle[code].Sum() / Params.Avg_Volume_Before_20_Candle[code];
                                }        
                            }
                        }


                        #endregion Main Logic
                        // Except Coin 은 건너뜀


                        if (!Params.CoinPositionDict.ContainsKey(code) &&    // 현재 보유중인 코인은 매수 안함
                            !Params.LosscutCode.ContainsKey(code))           // 로스컷 한 코인은 1시간동안 매매 안함.
                        {
                            var buy_signal = false;
                            buy_signal = strategy2.RequestLongSignal();

                            if (buy_signal is true)
                            {
                                // BuyRatio 최댓값일 때 매수 시도
                                if (code == Params.Avg_Price_Now_Candle.Aggregate((x, y) => x.Value.Sum() > y.Value.Sum() ? x : y).Key || true)
                                {
                                    var result = strategy2.SendLongOrder();
                                    if (result != "failed")
                                    {
                                        write_sys_log($"[{code}] is sending Long Order, wait 5 sec", 0);
                                        util.delay(5000);        // 매수 후 5초간 대기
                                        write_sys_log($"[{code}] passed 5 sec ", 0);
                                    }
                                    else
                                    {
                                        write_sys_log($"{code}'s Not enough Money", 0);
                                    }
                                }
                                return;
                            }
                        }

                        else
                        {
                            var sell_signal = false;
                            sell_signal = strategy2.RequestShortSignal();

                            if (sell_signal is true)
                            {           
                                var result = strategy2.SendShortOrder();
                                write_sys_log($"[{code}] is sending Short Order, wait 5 sec", 0);
                                util.delay(5000);        // 매도 후 5초간 대기
                                write_sys_log($"[{code}] passed 5 sec ", 0);

                                JObject sellResult = JObject.Parse(result);

                                if (result != null && result.Substring(2, 5) != "error" && sellResult["avg_price"] != null)                                
                                {
                                    Params.CoinPositionDict.Remove(code);
                                    DeletePositionGrid(code);
                                    return;
                                }

                                else return;
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        write_sys_log($"RTD Data : {ex.Message.ToString()}", 0);
                    }
                }
            }

            // Orderbook of each coins.
            else if (res["type"].ToString() == "orderbook")
            {
                var code = res["code"].ToString();

                OrderbookState state = new OrderbookState(res);
                Params.CoinOrderbookDict[code] = state;

                if (Orderbook_check.Checked)
                    DisplayOrderbook(res);
            }

            else if (res["type"].ToString() == "trades")
            {
                var code = res["code"].ToString();
            }
        }

        public void DisplayUniverseMarket(JObject res)
        {
            var code = res["code"].ToString();
            var avgPrice = 0.0;
            if (Params.Avg_Closed_Price.ContainsKey(code))
                avgPrice = Params.Avg_Closed_Price[code];
            var curPrice = double.Parse(res["trade_price"].ToString());
            var openingPrice = double.Parse(res["opening_price"].ToString());
            var change = ((curPrice / openingPrice) - 1) * 100;
            //var volume = Math.Round(double.Parse(res["acc_trade_price"].ToString()), 2);
            var volume = 0.0;
            if (Params.Avg_Price_Now_Candle.ContainsKey(code)) volume = Params.Avg_Price_Now_Candle[code].Sum();

            var buy_ratio = 0.0;
            if (Params.BuySignalRatio.ContainsKey(code)) buy_ratio = Params.BuySignalRatio[code];

            var sell_ratio = 0.0;
            if (Params.SellSignalRatio.ContainsKey(code)) sell_ratio = Params.SellSignalRatio[code];

            var losscutTime = "";
            if (Params.LosscutCode.ContainsKey(code)) losscutTime = Params.LosscutCode[code].ToString();

            var speed_ratio = "";
            if(Params.SpeedRatio.ContainsKey(code))  speed_ratio = Params.SpeedRatio[code].ToString();

            // Sorting by buy_ratio
            //UniverseDataGrid.Columns["buyRatio"].ValueType = typeof(double);


            if (UniverseDataGrid.InvokeRequired)
            {
                UniverseDataGrid.Invoke(new MethodInvoker(delegate ()
                {
                    if (UniverseDataGrid.Rows.Count > 1)
                    {
                        //UniverseDataGrid.Sort(UniverseDataGrid.Columns["buyRatio"], ListSortDirection.Descending);
                        //UniverseDataGrid.Columns["buyRatio"].ValueType = typeof(string);

                        foreach (DataGridViewRow row in UniverseDataGrid.Rows)
                        {
                            if (row.Cells["ticker"].Value == null)
                                break;


                            if (row.Cells["ticker"].Value.ToString() == code || row.Cells["ticker"].Value == null)
                            {
                                row.Cells["ticker"].Value = code;
                                row.Cells["avgPrice"].Value = String.Format("{0:0,0}", avgPrice);
                                row.Cells["curPrice"].Value = String.Format("{0:0,0}", curPrice);
                                if (change > 0) row.Cells["change"].Style.ForeColor = Color.Red;
                                else row.Cells["change"].Style.ForeColor = Color.Blue;
                                row.Cells["change"].Value = String.Format("{0:0.#}", change) + " %";
                                row.Cells["volume"].Value = String.Format("{0:0,0}", volume);
                                row.Cells["buyRatio"].Value = String.Format("{0:0.##}", buy_ratio);
                                row.Cells["speedRatio"].Value = String.Format("{0:0.##}", speed_ratio);

                                row.Cells["losscutTime"].Value = losscutTime;
                                return;
                            }
                        }

                        UniverseDataGrid.Rows.Add(code,
                                                  String.Format("{0:0,0}", curPrice),
                                                  String.Format("{0:0,0}", avgPrice),
                                                  String.Format("{0:0.#}", change) + " %",
                                                  String.Format("{0:0,0}", volume),
                                                  String.Format("{0:0.##}", buy_ratio),
                                                  String.Format("{0:0.##}", speed_ratio),

                                                  losscutTime);
                    }

                    else UniverseDataGrid.Rows.Add(code,
                                                    String.Format("{0:0,0}", curPrice),
                                                    String.Format("{0:0,0}", avgPrice),
                                                    String.Format("{0:0.#}", change) + " %",
                                                    String.Format("{0:0,0}", volume),
                                                    String.Format("{0:0.##}", buy_ratio),
                                                    String.Format("{0:0.##}", speed_ratio),

                                                    losscutTime);

                }));
            }

            else
            {
                if (UniverseDataGrid.Rows.Count > 1)
                {
                    // UniverseDataGrid.Sort(UniverseDataGrid.Columns["buyRatio"], ListSortDirection.Ascending);
                    // UniverseDataGrid.Columns["buyRatio"].ValueType = typeof(string);

                    foreach (DataGridViewRow row in UniverseDataGrid.Rows)
                    {
                        if (row.Cells["ticker"].Value == null)
                            break;


                        if (row.Cells["ticker"].Value.ToString() == code || row.Cells["ticker"].Value == null)
                        {
                            row.Cells["ticker"].Value = code;
                            row.Cells["avgPrice"].Value = String.Format("{0:0,0}", avgPrice);
                            row.Cells["curPrice"].Value = String.Format("{0:0,0}", curPrice);
                            if (change > 0) row.Cells["change"].Style.ForeColor = Color.Red;
                            else row.Cells["change"].Style.ForeColor = Color.Blue;
                            row.Cells["change"].Value = String.Format("{0:0.#}", change) + " %";
                            row.Cells["volume"].Value = String.Format("{0:0,0}", volume);
                            row.Cells["buyRatio"].Value = String.Format("{0:0,##}", buy_ratio);
                            row.Cells["speedRatio"].Value = String.Format("{0:0.##}", speed_ratio);

                            row.Cells["losscutTime"].Value = losscutTime;
                            return;
                        }
                    }

                    UniverseDataGrid.Rows.Add(code,
                                              String.Format("{0:0,0}", curPrice),
                                              String.Format("{0:0,0}", avgPrice),
                                              String.Format("{0:0.#}", change) + " %",
                                              String.Format("{0:0,0}", volume),
                                              String.Format("{0:0,##}", buy_ratio),
                                              String.Format("{0:0.##}", speed_ratio),
                                                                        losscutTime);
                }

                else UniverseDataGrid.Rows.Add(code,
                          String.Format("{0:0,0}", curPrice),
                          String.Format("{0:0,0}", avgPrice),
                          String.Format("{0:0.#}", change) + " %",
                          String.Format("{0:0,0}", volume),
                          String.Format("{0:0,##}", buy_ratio),
                          String.Format("{0:0.##}", speed_ratio),
                                                    losscutTime);
            }
        }

        public void DisplayOrderbook(JObject orderbook)
        {
            var code = orderbook["code"].ToString();

            if (code == Orderbook_ShortCode)
            {
                if (OrderbookDataGrid.InvokeRequired)
                {
                    OrderbookDataGrid.Invoke(new MethodInvoker(delegate ()
                    {
                        Orderbook_Code.Text = code;

                        OrderbookDataGrid.Rows[0].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][9]["ask_price"]);
                        OrderbookDataGrid.Rows[0].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][9]["ask_size"]);

                        OrderbookDataGrid.Rows[1].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][8]["ask_price"]);
                        OrderbookDataGrid.Rows[1].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][8]["ask_size"]);

                        OrderbookDataGrid.Rows[2].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][7]["ask_price"]);
                        OrderbookDataGrid.Rows[2].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][7]["ask_size"]);

                        OrderbookDataGrid.Rows[3].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][6]["ask_price"]);
                        OrderbookDataGrid.Rows[3].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][6]["ask_size"]);

                        OrderbookDataGrid.Rows[4].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][5]["ask_price"]);
                        OrderbookDataGrid.Rows[4].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][5]["ask_size"]);

                        OrderbookDataGrid.Rows[5].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][4]["ask_price"]);
                        OrderbookDataGrid.Rows[5].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][4]["ask_size"]);

                        OrderbookDataGrid.Rows[6].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][3]["ask_price"]);
                        OrderbookDataGrid.Rows[6].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][3]["ask_size"]);

                        OrderbookDataGrid.Rows[7].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][2]["ask_price"]);
                        OrderbookDataGrid.Rows[7].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][2]["ask_size"]);

                        OrderbookDataGrid.Rows[8].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][1]["ask_price"]);
                        OrderbookDataGrid.Rows[8].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][1]["ask_size"]);

                        OrderbookDataGrid.Rows[9].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][0]["ask_price"]);
                        OrderbookDataGrid.Rows[9].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][0]["ask_size"]);

                        OrderbookDataGrid.Rows[10].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][0]["bid_price"]);
                        OrderbookDataGrid.Rows[10].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][0]["bid_size"]);

                        OrderbookDataGrid.Rows[11].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][1]["bid_price"]);
                        OrderbookDataGrid.Rows[11].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][1]["bid_size"]);

                        OrderbookDataGrid.Rows[12].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][2]["bid_price"]);
                        OrderbookDataGrid.Rows[12].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][2]["bid_size"]);

                        OrderbookDataGrid.Rows[13].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][3]["bid_price"]);
                        OrderbookDataGrid.Rows[13].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][3]["bid_size"]);

                        OrderbookDataGrid.Rows[14].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][4]["bid_price"]);
                        OrderbookDataGrid.Rows[14].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][4]["bid_size"]);

                        OrderbookDataGrid.Rows[15].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][5]["bid_price"]);
                        OrderbookDataGrid.Rows[15].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][5]["bid_size"]);

                        OrderbookDataGrid.Rows[16].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][6]["bid_price"]);
                        OrderbookDataGrid.Rows[16].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][6]["bid_size"]);

                        OrderbookDataGrid.Rows[17].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][7]["bid_price"]);
                        OrderbookDataGrid.Rows[17].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][7]["bid_size"]);

                        OrderbookDataGrid.Rows[18].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][8]["bid_price"]);
                        OrderbookDataGrid.Rows[18].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][8]["bid_size"]);

                        OrderbookDataGrid.Rows[19].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][9]["bid_price"]);
                        OrderbookDataGrid.Rows[19].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][9]["bid_size"]);

                    }));
                }
                else
                {
                    Orderbook_Code.Text = code;

                    OrderbookDataGrid.Rows[0].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][9]["ask_price"]);
                    OrderbookDataGrid.Rows[0].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][9]["ask_size"]);

                    OrderbookDataGrid.Rows[1].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][8]["ask_price"]);
                    OrderbookDataGrid.Rows[1].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][8]["ask_size"]);

                    OrderbookDataGrid.Rows[2].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][7]["ask_price"]);
                    OrderbookDataGrid.Rows[2].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][7]["ask_size"]);

                    OrderbookDataGrid.Rows[3].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][6]["ask_price"]);
                    OrderbookDataGrid.Rows[3].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][6]["ask_size"]);

                    OrderbookDataGrid.Rows[4].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][5]["ask_price"]);
                    OrderbookDataGrid.Rows[4].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][5]["ask_size"]);

                    OrderbookDataGrid.Rows[5].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][4]["ask_price"]);
                    OrderbookDataGrid.Rows[5].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][4]["ask_size"]);

                    OrderbookDataGrid.Rows[6].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][3]["ask_price"]);
                    OrderbookDataGrid.Rows[6].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][3]["ask_size"]);

                    OrderbookDataGrid.Rows[7].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][2]["ask_price"]);
                    OrderbookDataGrid.Rows[7].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][2]["ask_size"]);

                    OrderbookDataGrid.Rows[8].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][1]["ask_price"]);
                    OrderbookDataGrid.Rows[8].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][1]["ask_size"]);

                    OrderbookDataGrid.Rows[9].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][0]["ask_price"]);
                    OrderbookDataGrid.Rows[9].Cells[0].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][0]["ask_size"]);

                    OrderbookDataGrid.Rows[10].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][0]["bid_price"]);
                    OrderbookDataGrid.Rows[10].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][0]["bid_size"]);

                    OrderbookDataGrid.Rows[11].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][1]["bid_price"]);
                    OrderbookDataGrid.Rows[11].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][1]["bid_size"]);

                    OrderbookDataGrid.Rows[12].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][2]["bid_price"]);
                    OrderbookDataGrid.Rows[12].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][2]["bid_size"]);

                    OrderbookDataGrid.Rows[13].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][3]["bid_price"]);
                    OrderbookDataGrid.Rows[13].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][3]["bid_size"]);

                    OrderbookDataGrid.Rows[14].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][4]["bid_price"]);
                    OrderbookDataGrid.Rows[14].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][4]["bid_size"]);

                    OrderbookDataGrid.Rows[15].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][5]["bid_price"]);
                    OrderbookDataGrid.Rows[15].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][5]["bid_size"]);

                    OrderbookDataGrid.Rows[16].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][6]["bid_price"]);
                    OrderbookDataGrid.Rows[16].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][6]["bid_size"]);

                    OrderbookDataGrid.Rows[17].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][7]["bid_price"]);
                    OrderbookDataGrid.Rows[17].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][7]["bid_size"]);

                    OrderbookDataGrid.Rows[18].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][8]["bid_price"]);
                    OrderbookDataGrid.Rows[18].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][8]["bid_size"]);

                    OrderbookDataGrid.Rows[19].Cells[1].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][9]["bid_price"]);
                    OrderbookDataGrid.Rows[19].Cells[2].Value = String.Format("{0:0,0}", orderbook["orderbook_units"][9]["bid_size"]);

                }
            }
        }

        public void DisplayTargetCoins(string currency, string balance, string buy_price, string cur_price, string rate, string pnl, string filledTime)
        {


            if (positionDataGrid.InvokeRequired)
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
                                row.Cells["code"].Value = String.Format("{0:0,0}", currency);
                                row.Cells["Quantity"].Value = String.Format("{0:0,0}", balance);
                                row.Cells["buyPrice"].Value = String.Format("{0:0,0}", buy_price);
                                row.Cells["curPrice_position"].Value = String.Format("{0:0,0}", cur_price);
                                row.Cells["rate"].Value = rate;
                                row.Cells["tradingPnL"].Value = String.Format("{0:0,0}", pnl);
                                row.Cells["filledTime"].Value = filledTime;
                                return;
                            }
                        }

                        positionDataGrid.Rows.Add(String.Format("{0:0,0}", currency),
                                                  String.Format("{0:0,0}", balance),
                                                  String.Format("{0:0,0}", buy_price),
                                                  String.Format("{0:0,0}", cur_price),
                                                  rate,
                                                  String.Format("{0:0,0}", pnl),
                                                  filledTime);
                    }

                    else positionDataGrid.Rows.Add(String.Format("{0:0,0}", currency),
                                                    String.Format("{0:0,0}", balance),
                                                    String.Format("{0:0,0}", buy_price),
                                                    String.Format("{0:0,0}", cur_price),
                                                    rate,
                                                    String.Format("{0:0,0}", pnl),
                                                    filledTime
                                                    );


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
                            row.Cells["code"].Value = String.Format("{0:0,0}", currency);
                            row.Cells["Quantity"].Value = String.Format("{0:0,0}", balance);
                            row.Cells["buyPrice"].Value = String.Format("{0:0,0}", buy_price);
                            row.Cells["curPrice_position"].Value = String.Format("{0:0,0}", cur_price);
                            row.Cells["rate"].Value = rate;
                            row.Cells["tradingPnL"].Value = String.Format("{0:0,0}", pnl);
                            row.Cells["filledTime"].Value = filledTime;

                            return;
                        }
                    }

                    positionDataGrid.Rows.Add(String.Format("{0:0,0}", currency),
                                              String.Format("{0:0,0}", balance),
                                              String.Format("{0:0,0}", buy_price),
                                              String.Format("{0:0,0}", cur_price),
                                              rate,
                                              String.Format("{0:0,0}", pnl),
                                              filledTime);
                }

                else positionDataGrid.Rows.Add(String.Format("{0:0,0}", currency),
                                                String.Format("{0:0,0}", balance),
                                                String.Format("{0:0,0}", buy_price),
                                                String.Format("{0:0,0}", cur_price),
                                                rate,
                                                String.Format("{0:0,0}", pnl),
                                                filledTime
                                                );
            }
        }

        public void DeletePositionGrid(string currency)
        {

            if (positionDataGrid.InvokeRequired)
            {

                positionDataGrid.Invoke(new MethodInvoker(delegate ()
                {
                    if (positionDataGrid.Rows.Count > 1)
                    {
                        foreach (DataGridViewRow row in positionDataGrid.Rows)
                        {
                            if (row.Cells["code"].Value != null && row.Cells["code"].Value.ToString() == currency)
                            {
                                positionDataGrid.Rows.Remove(row);
                                break;
                            }
                        }
                    }
                }));
            }

            else
            {
                if (positionDataGrid.Rows.Count > 1)
                {
                    foreach (DataGridViewRow row in positionDataGrid.Rows)
                    {
                        if (row.Cells["code"].Value.ToString() == currency)
                        {
                            positionDataGrid.Rows.Remove(row);
                            break;
                        }
                    }
                }
            }

        }

        // LogThread
        public void UpdateLog()
        {

        }

        public void UpdatePosition()
        {
            List<JObject> Result = Params.upbit.GetAccount();

            Params.CoinPositionDict = new Dictionary<string, PositionState>();
            util.delay(200);

            for (int i = 0; i < Result.Count; i++)
            {
                var currency = Result[i].GetValue("currency").ToString().Trim();
                var balance = Result[i].GetValue("balance").ToString().Trim();
                var locked = Result[i].GetValue("locked").ToString().Trim();
                var avg_buy_price = Result[i].GetValue("avg_buy_price").ToString().Trim();
                var cur_price = "0.0";

                string code = $"KRW-{currency}";

                if (Params.CoinInfoDict.Count != 0)
                {
                    if (Params.CoinInfoDict.ContainsKey(code))
                        cur_price = Params.CoinInfoDict[code].curPrice;
                }

                PositionState position = new PositionState(code, balance, locked, avg_buy_price, cur_price);
                Params.CoinPositionDict[code] = position;
            }
        }

        // PositionThread
        public void DisplayPosition()
        {
            while (true)
            {
                try
                {
                    List<JObject> Result = Params.upbit.GetAccount();
                    util.delay(200);

                    // Initialize Position
                    Params.TotalAsset = 0.0;
                    Params.CoinAsset = 0.0;
                    Params.CashAsset = 0.0;
                    Params.PnL = 500000.0;
                    Params.PnLChange = 0.0;
                    var cash = 0.0;

                    // Display Position
                    for (int i = 0; i < Result.Count; i++)
                    {
                        var currency = Result[i].GetValue("currency").ToString().Trim();
                        var balance = Result[i].GetValue("balance").ToString().Trim();
                        var locked = Result[i].GetValue("locked").ToString().Trim();
                        var avg_buy_price = Result[i].GetValue("avg_buy_price").ToString().Trim();
                        var cur_price = "0.0";
                        var filledTime = "";

                        string code = $"KRW-{currency}";

                        if (currency == "KRW")
                        {
                            cash = double.Parse(balance);
                            code = "Cash";
                        }

                        if (Params.FilledTime.ContainsKey(code)) filledTime = Params.FilledTime[code].ToString();

                        if (Params.CoinInfoDict.Count != 0)
                        {
                            if (Params.CoinInfoDict.ContainsKey(code))
                                cur_price = Params.CoinInfoDict[code].curPrice;
                        }

                        var rate = $"{Math.Round(((double.Parse(cur_price) / double.Parse(avg_buy_price)) - 1) * 100, 2)} %";
                        var tradingPnL = $"{Math.Round((double.Parse(cur_price) - double.Parse(avg_buy_price)) * double.Parse(balance), 2)} 원";

                        if (!Params.CoinPositionDict.ContainsKey(code))
                        {
                            PositionState position = new PositionState(code, balance, locked, avg_buy_price, cur_price);
                            Params.CoinPositionDict[code] = position;
                        }

                        // Display Position Func
                        DisplayTargetCoins(code, balance, avg_buy_price, cur_price, rate, tradingPnL, filledTime);


                        Params.TotalAsset = Params.TotalAsset + double.Parse(balance) * double.Parse(cur_price);
                        Params.CoinAsset = Params.CoinAsset + double.Parse(balance) * double.Parse(cur_price);
                        Params.PnL = Params.PnL + (double.Parse(cur_price) - double.Parse(avg_buy_price)) * double.Parse(balance);

                    }

                    Params.TotalAsset = Params.TotalAsset + cash;
                    Params.CashAsset = cash;

                    // Display Total Position display
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            Total_Asset.Text = String.Format("{0:0,0}", Math.Round(Params.TotalAsset));
                            Cash_Asset.Text = String.Format("{0:0,0}", Params.CashAsset);
                            Coin_Asset.Text = String.Format("{0:0,0}", Params.CoinAsset);
                            PnL.Text = String.Format("{0:0,0}", Params.PnL);
                            Strategy_PnL.Text = Params.Strategy_PnL.ToString();
                        }));
                    }
                    else
                    {
                        Total_Asset.Text = String.Format("{0:0,0}", Params.TotalAsset);
                        Cash_Asset.Text = String.Format("{0:0,0}", Params.CashAsset);
                        Coin_Asset.Text = String.Format("{0:0,0}", Params.CoinAsset);
                        PnL.Text = String.Format("{0:0,0}", Params.PnL);
                        Strategy_PnL.Text = Params.Strategy_PnL.ToString();
                    }

                }

                catch (Exception ex)
                {
                    write_sys_log($"In Position Thread : {ex.Message.ToString()}", 0);
                }
            }


        }


        // OrderbookThread
        public void UpdateOrderbook()
        {
            var Markets = Params.upbit.GetMarkets();
            var MarketTickers = Params.upbit.GetMarketTicker(Markets).Item1;

            UpbitWebSocket WS_Orderbook = new UpbitWebSocket();

            var sendOrderbookMsg = WS_Orderbook.MakeSendMsg(MarketTickers, "orderbook");
            SendWebSocket(sendOrderbookMsg);

        }

        public void write_sys_log(String text, int is_Clear)
        {
            DateTime cur_time;
            String cur_dt;
            String cur_tm;
            String cur_dtm;

            cur_dt = "";
            cur_tm = "";

            cur_time = DateTime.Now;
            cur_dt = cur_time.ToString("yyyy-") + cur_time.ToString("MM-") + cur_time.ToString("dd");
            cur_tm = Params.CurTime;

            cur_dtm = "[" + cur_dt + " / " + cur_tm + "] ";

            if (is_Clear == 1)
            {
                if (LogBox.InvokeRequired)
                {
                    LogBox.BeginInvoke(new Action(() => LogBox.Clear()));
                }
                else
                {
                    this.LogBox.Clear();
                }
            }

            else
            {
                if (this.LogBox.InvokeRequired)
                {
                    LogBox.BeginInvoke(new Action(() => LogBox.AppendText("\n" + cur_dtm + text + Environment.NewLine)));
                }

                else
                {
                    this.LogBox.AppendText("\n" + cur_dtm + text + Environment.NewLine);
                    // log 기록

                    //File.AppendAllText(BotParams.Path + BotParams.LogFileName, cur_dtm + text + Environment.NewLine, Encoding.Default);
                }
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void UniverseDataGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                Orderbook_ShortCode = UniverseDataGrid.Rows[e.RowIndex].Cells[0].Value.ToString();

                if (Chart_check.Checked)
                {
                    //CreateCandleChart();
                }
                else
                {
                    //MessageBox.Show("차트보기가 체크되어있지 않습니다");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void UniverseDataGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                Orderbook_ShortCode = UniverseDataGrid.Rows[e.RowIndex].Cells[0].Value.ToString();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void UniverseDataGrid_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {

        }

        public void ExitProgram()
        {
            Application.ExitThread();
            Environment.Exit(0);
        }

        public void ExitProgramBtnClicked(object sender, EventArgs e)
        {
            if (MessageBox.Show("정말 종료하시겠습니까?", "YesOrNo", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ExitProgram();
            }
        }
    }
}
