
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
            this.TabControl = new System.Windows.Forms.TabControl();
            this.MarketUniverse = new System.Windows.Forms.TabPage();
            this.UniverseDataGrid = new System.Windows.Forms.DataGridView();
            this.ticker = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.curPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.change = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.volume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Blotter = new System.Windows.Forms.TabPage();
            this.Position = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.LogBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.positionDataGrid = new System.Windows.Forms.DataGridView();
            this.Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Quantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buyPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.curPrice_position = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tradingPnL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TabControl.SuspendLayout();
            this.MarketUniverse.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UniverseDataGrid)).BeginInit();
            this.Position.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.positionDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add(this.MarketUniverse);
            this.TabControl.Controls.Add(this.Blotter);
            this.TabControl.Controls.Add(this.Position);
            this.TabControl.Controls.Add(this.tabPage4);
            this.TabControl.Location = new System.Drawing.Point(12, 21);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(808, 672);
            this.TabControl.TabIndex = 0;
            // 
            // MarketUniverse
            // 
            this.MarketUniverse.Controls.Add(this.UniverseDataGrid);
            this.MarketUniverse.Location = new System.Drawing.Point(4, 22);
            this.MarketUniverse.Name = "MarketUniverse";
            this.MarketUniverse.Padding = new System.Windows.Forms.Padding(3);
            this.MarketUniverse.Size = new System.Drawing.Size(477, 646);
            this.MarketUniverse.TabIndex = 0;
            this.MarketUniverse.Text = "Market";
            this.MarketUniverse.UseVisualStyleBackColor = true;
            // 
            // UniverseDataGrid
            // 
            this.UniverseDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.UniverseDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ticker,
            this.curPrice,
            this.change,
            this.volume});
            this.UniverseDataGrid.Location = new System.Drawing.Point(6, 6);
            this.UniverseDataGrid.Name = "UniverseDataGrid";
            this.UniverseDataGrid.RowTemplate.Height = 23;
            this.UniverseDataGrid.Size = new System.Drawing.Size(465, 637);
            this.UniverseDataGrid.TabIndex = 0;
            // 
            // ticker
            // 
            this.ticker.HeaderText = "코드";
            this.ticker.Name = "ticker";
            // 
            // curPrice
            // 
            this.curPrice.HeaderText = "현재가";
            this.curPrice.Name = "curPrice";
            // 
            // change
            // 
            this.change.HeaderText = "등락률";
            this.change.Name = "change";
            // 
            // volume
            // 
            this.volume.HeaderText = "거래량";
            this.volume.Name = "volume";
            // 
            // Blotter
            // 
            this.Blotter.Location = new System.Drawing.Point(4, 22);
            this.Blotter.Name = "Blotter";
            this.Blotter.Padding = new System.Windows.Forms.Padding(3);
            this.Blotter.Size = new System.Drawing.Size(477, 646);
            this.Blotter.TabIndex = 1;
            this.Blotter.Text = "Blotter";
            this.Blotter.UseVisualStyleBackColor = true;
            // 
            // Position
            // 
            this.Position.Controls.Add(this.positionDataGrid);
            this.Position.Location = new System.Drawing.Point(4, 22);
            this.Position.Name = "Position";
            this.Position.Padding = new System.Windows.Forms.Padding(3);
            this.Position.Size = new System.Drawing.Size(800, 646);
            this.Position.TabIndex = 2;
            this.Position.Text = "Position";
            this.Position.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.LogBox);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(800, 646);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // LogBox
            // 
            this.LogBox.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.LogBox.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.LogBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.LogBox.Location = new System.Drawing.Point(3, 6);
            this.LogBox.Multiline = true;
            this.LogBox.Name = "LogBox";
            this.LogBox.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.LogBox.Size = new System.Drawing.Size(791, 638);
            this.LogBox.TabIndex = 8;
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(826, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(340, 652);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
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
            this.positionDataGrid.Location = new System.Drawing.Point(7, 7);
            this.positionDataGrid.Name = "positionDataGrid";
            this.positionDataGrid.RowTemplate.Height = 23;
            this.positionDataGrid.Size = new System.Drawing.Size(787, 633);
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
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1170, 699);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.TabControl);
            this.Name = "Main";
            this.Text = "Form1";
            this.TabControl.ResumeLayout(false);
            this.MarketUniverse.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UniverseDataGrid)).EndInit();
            this.Position.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.positionDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage MarketUniverse;
        private System.Windows.Forms.TabPage Blotter;
        public System.Windows.Forms.TextBox LogBox;
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
    }
}

