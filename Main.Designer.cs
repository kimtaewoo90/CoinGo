
namespace CoinGo
{
    partial class Main
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.MarketUniverse = new System.Windows.Forms.TabPage();
            this.Orderbook_Code = new System.Windows.Forms.Label();
            this.OrderbookDataGrid = new System.Windows.Forms.DataGridView();
            this.Orderbook_AskPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Orderbook_Price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Orderbook_Bid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UniverseDataGrid = new System.Windows.Forms.DataGridView();
            this.ticker = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.curPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.change = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.volume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Blotter = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.BLT_OrderCoinCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BLT_OrderCoinName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BLT_OrderType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BLT_OrderQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BLT_OrderPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BLT_filledQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BLT_filledPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Position = new System.Windows.Forms.TabPage();
            this.positionDataGrid = new System.Windows.Forms.DataGridView();
            this.Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Quantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buyPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.curPrice_position = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tradingPnL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.LogBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.Candle_Chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.PnL_Change = new System.Windows.Forms.Label();
            this.PnL = new System.Windows.Forms.Label();
            this.Coin_Asset = new System.Windows.Forms.Label();
            this.Cash_Asset = new System.Windows.Forms.Label();
            this.Total_Asset = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Chart_check = new System.Windows.Forms.CheckBox();
            this.Orderbook_check = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.TabControl.SuspendLayout();
            this.MarketUniverse.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OrderbookDataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UniverseDataGrid)).BeginInit();
            this.Blotter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.Position.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.positionDataGrid)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Candle_Chart)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add(this.MarketUniverse);
            this.TabControl.Controls.Add(this.Blotter);
            this.TabControl.Controls.Add(this.Position);
            this.TabControl.Controls.Add(this.tabPage4);
            this.TabControl.Location = new System.Drawing.Point(22, 237);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(825, 706);
            this.TabControl.TabIndex = 0;
            // 
            // MarketUniverse
            // 
            this.MarketUniverse.Controls.Add(this.Orderbook_Code);
            this.MarketUniverse.Controls.Add(this.OrderbookDataGrid);
            this.MarketUniverse.Controls.Add(this.UniverseDataGrid);
            this.MarketUniverse.Location = new System.Drawing.Point(4, 22);
            this.MarketUniverse.Name = "MarketUniverse";
            this.MarketUniverse.Padding = new System.Windows.Forms.Padding(3);
            this.MarketUniverse.Size = new System.Drawing.Size(817, 680);
            this.MarketUniverse.TabIndex = 0;
            this.MarketUniverse.Text = "Market";
            this.MarketUniverse.UseVisualStyleBackColor = true;
            // 
            // Orderbook_Code
            // 
            this.Orderbook_Code.AutoSize = true;
            this.Orderbook_Code.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Orderbook_Code.Location = new System.Drawing.Point(465, 6);
            this.Orderbook_Code.Name = "Orderbook_Code";
            this.Orderbook_Code.Size = new System.Drawing.Size(154, 27);
            this.Orderbook_Code.TabIndex = 2;
            this.Orderbook_Code.Text = "Coin Code";
            // 
            // OrderbookDataGrid
            // 
            this.OrderbookDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.OrderbookDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Orderbook_AskPrice,
            this.Orderbook_Price,
            this.Orderbook_Bid});
            this.OrderbookDataGrid.Location = new System.Drawing.Point(465, 36);
            this.OrderbookDataGrid.Name = "OrderbookDataGrid";
            this.OrderbookDataGrid.RowTemplate.Height = 23;
            this.OrderbookDataGrid.Size = new System.Drawing.Size(343, 638);
            this.OrderbookDataGrid.TabIndex = 1;
            // 
            // Orderbook_AskPrice
            // 
            this.Orderbook_AskPrice.HeaderText = "Ask";
            this.Orderbook_AskPrice.Name = "Orderbook_AskPrice";
            // 
            // Orderbook_Price
            // 
            this.Orderbook_Price.HeaderText = "Price";
            this.Orderbook_Price.Name = "Orderbook_Price";
            // 
            // Orderbook_Bid
            // 
            this.Orderbook_Bid.HeaderText = "Bid";
            this.Orderbook_Bid.Name = "Orderbook_Bid";
            // 
            // UniverseDataGrid
            // 
            this.UniverseDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.UniverseDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ticker,
            this.curPrice,
            this.change,
            this.volume});
            this.UniverseDataGrid.Location = new System.Drawing.Point(6, 9);
            this.UniverseDataGrid.Name = "UniverseDataGrid";
            this.UniverseDataGrid.ReadOnly = true;
            this.UniverseDataGrid.RowTemplate.Height = 23;
            this.UniverseDataGrid.Size = new System.Drawing.Size(453, 665);
            this.UniverseDataGrid.TabIndex = 0;
            this.UniverseDataGrid.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.UniverseDataGrid_CellMouseDoubleClick);
            this.UniverseDataGrid.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.UniverseDataGrid_CellMouseClick);
            // 
            // ticker
            // 
            this.ticker.HeaderText = "코드";
            this.ticker.Name = "ticker";
            this.ticker.ReadOnly = true;
            // 
            // curPrice
            // 
            this.curPrice.HeaderText = "현재가";
            this.curPrice.Name = "curPrice";
            this.curPrice.ReadOnly = true;
            // 
            // change
            // 
            this.change.HeaderText = "등락률";
            this.change.Name = "change";
            this.change.ReadOnly = true;
            // 
            // volume
            // 
            this.volume.HeaderText = "거래량";
            this.volume.Name = "volume";
            this.volume.ReadOnly = true;
            // 
            // Blotter
            // 
            this.Blotter.Controls.Add(this.dataGridView1);
            this.Blotter.Location = new System.Drawing.Point(4, 22);
            this.Blotter.Name = "Blotter";
            this.Blotter.Padding = new System.Windows.Forms.Padding(3);
            this.Blotter.Size = new System.Drawing.Size(817, 680);
            this.Blotter.TabIndex = 1;
            this.Blotter.Text = "Blotter";
            this.Blotter.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.BLT_OrderCoinCode,
            this.BLT_OrderCoinName,
            this.BLT_OrderType,
            this.BLT_OrderQty,
            this.BLT_OrderPrice,
            this.BLT_filledQty,
            this.BLT_filledPrice});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(811, 674);
            this.dataGridView1.TabIndex = 0;
            // 
            // BLT_OrderCoinCode
            // 
            this.BLT_OrderCoinCode.HeaderText = "Code";
            this.BLT_OrderCoinCode.Name = "BLT_OrderCoinCode";
            // 
            // BLT_OrderCoinName
            // 
            this.BLT_OrderCoinName.HeaderText = "KrName";
            this.BLT_OrderCoinName.Name = "BLT_OrderCoinName";
            // 
            // BLT_OrderType
            // 
            this.BLT_OrderType.HeaderText = "OrderType";
            this.BLT_OrderType.Name = "BLT_OrderType";
            // 
            // BLT_OrderQty
            // 
            this.BLT_OrderQty.HeaderText = "Qty";
            this.BLT_OrderQty.Name = "BLT_OrderQty";
            // 
            // BLT_OrderPrice
            // 
            this.BLT_OrderPrice.HeaderText = "Price";
            this.BLT_OrderPrice.Name = "BLT_OrderPrice";
            // 
            // BLT_filledQty
            // 
            this.BLT_filledQty.HeaderText = "filledQty";
            this.BLT_filledQty.Name = "BLT_filledQty";
            // 
            // BLT_filledPrice
            // 
            this.BLT_filledPrice.HeaderText = "filledPrice";
            this.BLT_filledPrice.Name = "BLT_filledPrice";
            // 
            // Position
            // 
            this.Position.Controls.Add(this.positionDataGrid);
            this.Position.Location = new System.Drawing.Point(4, 22);
            this.Position.Name = "Position";
            this.Position.Padding = new System.Windows.Forms.Padding(3);
            this.Position.Size = new System.Drawing.Size(817, 680);
            this.Position.TabIndex = 2;
            this.Position.Text = "Position";
            this.Position.UseVisualStyleBackColor = true;
            // 
            // positionDataGrid
            // 
            this.positionDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.positionDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Code,
            this.Quantity,
            this.buyPrice,
            this.curPrice_position,
            this.rate,
            this.tradingPnL});
            this.positionDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.positionDataGrid.Location = new System.Drawing.Point(3, 3);
            this.positionDataGrid.Name = "positionDataGrid";
            this.positionDataGrid.RowTemplate.Height = 23;
            this.positionDataGrid.Size = new System.Drawing.Size(811, 674);
            this.positionDataGrid.TabIndex = 0;
            // 
            // Code
            // 
            this.Code.HeaderText = "코인명";
            this.Code.Name = "Code";
            // 
            // Quantity
            // 
            this.Quantity.HeaderText = "Qty";
            this.Quantity.Name = "Quantity";
            // 
            // buyPrice
            // 
            this.buyPrice.HeaderText = "매수가";
            this.buyPrice.Name = "buyPrice";
            // 
            // curPrice_position
            // 
            this.curPrice_position.HeaderText = "현재가";
            this.curPrice_position.Name = "curPrice_position";
            // 
            // rate
            // 
            this.rate.HeaderText = "수익(%)";
            this.rate.Name = "rate";
            // 
            // tradingPnL
            // 
            this.tradingPnL.HeaderText = "수익(\\)";
            this.tradingPnL.Name = "tradingPnL";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.LogBox);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(817, 680);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Logs";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // LogBox
            // 
            this.LogBox.BackColor = System.Drawing.SystemColors.InfoText;
            this.LogBox.ForeColor = System.Drawing.Color.Lime;
            this.LogBox.Location = new System.Drawing.Point(7, 7);
            this.LogBox.Multiline = true;
            this.LogBox.Name = "LogBox";
            this.LogBox.Size = new System.Drawing.Size(814, 633);
            this.LogBox.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chart2);
            this.groupBox1.Controls.Add(this.Candle_Chart);
            this.groupBox1.Location = new System.Drawing.Point(853, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(673, 902);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // chart2
            // 
            chartArea3.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea3);
            legend2.Name = "Legend1";
            this.chart2.Legends.Add(legend2);
            this.chart2.Location = new System.Drawing.Point(5, 697);
            this.chart2.Margin = new System.Windows.Forms.Padding(2);
            this.chart2.Name = "chart2";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart2.Series.Add(series2);
            this.chart2.Size = new System.Drawing.Size(654, 200);
            this.chart2.TabIndex = 1;
            this.chart2.Text = "chart2";
            // 
            // Candle_Chart
            // 
            chartArea4.CursorX.IsUserSelectionEnabled = true;
            chartArea4.Name = "ChartArea1";
            this.Candle_Chart.ChartAreas.Add(chartArea4);
            this.Candle_Chart.Location = new System.Drawing.Point(5, 19);
            this.Candle_Chart.Margin = new System.Windows.Forms.Padding(2);
            this.Candle_Chart.Name = "Candle_Chart";
            this.Candle_Chart.Size = new System.Drawing.Size(654, 663);
            this.Candle_Chart.TabIndex = 0;
            this.Candle_Chart.Text = "chart1";
            this.Candle_Chart.AxisViewChanged += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ViewEventArgs>(this.ChartAxisChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(37, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "총 자산";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(17, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 24);
            this.label2.TabIndex = 4;
            this.label2.Text = "코인 자산";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(17, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 24);
            this.label3.TabIndex = 5;
            this.label3.Text = "현금 자산";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(374, 84);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 24);
            this.label4.TabIndex = 6;
            this.label4.Text = "수익금";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(374, 123);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 24);
            this.label5.TabIndex = 7;
            this.label5.Text = "수익률";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.groupBox2.Controls.Add(this.PnL_Change);
            this.groupBox2.Controls.Add(this.PnL);
            this.groupBox2.Controls.Add(this.Coin_Asset);
            this.groupBox2.Controls.Add(this.Cash_Asset);
            this.groupBox2.Controls.Add(this.Total_Asset);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox2.Location = new System.Drawing.Point(22, 77);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(818, 154);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "자산현황";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // PnL_Change
            // 
            this.PnL_Change.AutoSize = true;
            this.PnL_Change.Font = new System.Drawing.Font("굴림", 10F);
            this.PnL_Change.Location = new System.Drawing.Point(485, 133);
            this.PnL_Change.Name = "PnL_Change";
            this.PnL_Change.Size = new System.Drawing.Size(15, 14);
            this.PnL_Change.TabIndex = 12;
            this.PnL_Change.Text = "0";
            // 
            // PnL
            // 
            this.PnL.AutoSize = true;
            this.PnL.Font = new System.Drawing.Font("굴림", 10F);
            this.PnL.Location = new System.Drawing.Point(485, 94);
            this.PnL.Name = "PnL";
            this.PnL.Size = new System.Drawing.Size(15, 14);
            this.PnL.TabIndex = 11;
            this.PnL.Text = "0";
            // 
            // Coin_Asset
            // 
            this.Coin_Asset.AutoSize = true;
            this.Coin_Asset.Font = new System.Drawing.Font("굴림", 10F);
            this.Coin_Asset.Location = new System.Drawing.Point(149, 94);
            this.Coin_Asset.Name = "Coin_Asset";
            this.Coin_Asset.Size = new System.Drawing.Size(15, 14);
            this.Coin_Asset.TabIndex = 10;
            this.Coin_Asset.Text = "0";
            this.Coin_Asset.Click += new System.EventHandler(this.label8_Click);
            // 
            // Cash_Asset
            // 
            this.Cash_Asset.AutoSize = true;
            this.Cash_Asset.Font = new System.Drawing.Font("굴림", 10F);
            this.Cash_Asset.Location = new System.Drawing.Point(149, 133);
            this.Cash_Asset.Name = "Cash_Asset";
            this.Cash_Asset.Size = new System.Drawing.Size(15, 14);
            this.Cash_Asset.TabIndex = 9;
            this.Cash_Asset.Text = "0";
            // 
            // Total_Asset
            // 
            this.Total_Asset.AutoSize = true;
            this.Total_Asset.Font = new System.Drawing.Font("굴림", 10F);
            this.Total_Asset.Location = new System.Drawing.Point(149, 55);
            this.Total_Asset.Name = "Total_Asset";
            this.Total_Asset.Size = new System.Drawing.Size(15, 14);
            this.Total_Asset.TabIndex = 8;
            this.Total_Asset.Text = "0";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBox3);
            this.groupBox3.Controls.Add(this.Orderbook_check);
            this.groupBox3.Controls.Add(this.Chart_check);
            this.groupBox3.Location = new System.Drawing.Point(26, 23);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(817, 48);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "groupBox3";
            // 
            // Chart_check
            // 
            this.Chart_check.AutoSize = true;
            this.Chart_check.Location = new System.Drawing.Point(17, 18);
            this.Chart_check.Name = "Chart_check";
            this.Chart_check.Size = new System.Drawing.Size(72, 16);
            this.Chart_check.TabIndex = 0;
            this.Chart_check.Text = "차트보기";
            this.Chart_check.UseVisualStyleBackColor = true;
            // 
            // Orderbook_check
            // 
            this.Orderbook_check.AutoSize = true;
            this.Orderbook_check.Location = new System.Drawing.Point(132, 18);
            this.Orderbook_check.Name = "Orderbook_check";
            this.Orderbook_check.Size = new System.Drawing.Size(72, 16);
            this.Orderbook_check.TabIndex = 1;
            this.Orderbook_check.Text = "호가보기";
            this.Orderbook_check.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(248, 18);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(86, 16);
            this.checkBox3.TabIndex = 2;
            this.checkBox3.Text = "checkBox3";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1538, 955);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.TabControl);
            this.Name = "Main";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Main_Load);
            this.TabControl.ResumeLayout(false);
            this.MarketUniverse.ResumeLayout(false);
            this.MarketUniverse.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OrderbookDataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UniverseDataGrid)).EndInit();
            this.Blotter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.Position.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.positionDataGrid)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Candle_Chart)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage MarketUniverse;
        private System.Windows.Forms.TabPage Blotter;
        private System.Windows.Forms.TabPage Position;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.DataGridView UniverseDataGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn ticker;
        private System.Windows.Forms.DataGridViewTextBoxColumn curPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn change;
        private System.Windows.Forms.DataGridViewTextBoxColumn volume;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView positionDataGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn Quantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn buyPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn curPrice_position;
        private System.Windows.Forms.DataGridViewTextBoxColumn rate;
        private System.Windows.Forms.DataGridViewTextBoxColumn tradingPnL;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn BLT_OrderCoinCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn BLT_OrderCoinName;
        private System.Windows.Forms.DataGridViewTextBoxColumn BLT_OrderType;
        private System.Windows.Forms.DataGridViewTextBoxColumn BLT_OrderQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn BLT_OrderPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn BLT_filledQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn BLT_filledPrice;
        private System.Windows.Forms.DataGridView OrderbookDataGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Orderbook_AskPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn Orderbook_Price;
        private System.Windows.Forms.DataGridViewTextBoxColumn Orderbook_Bid;
        private System.Windows.Forms.Label Orderbook_Code;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private System.Windows.Forms.DataVisualization.Charting.Chart Candle_Chart;
        private System.Windows.Forms.TextBox LogBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label PnL_Change;
        private System.Windows.Forms.Label PnL;
        private System.Windows.Forms.Label Coin_Asset;
        private System.Windows.Forms.Label Cash_Asset;
        private System.Windows.Forms.Label Total_Asset;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox Orderbook_check;
        private System.Windows.Forms.CheckBox Chart_check;
    }
}

