
namespace CoinGo
{
    partial class PositionScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PositionDataGrid = new System.Windows.Forms.DataGridView();
            this.Currency = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Balance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Locked = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Avg_Buy_Price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Unit_Currency = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.PositionDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // PositionDataGrid
            // 
            this.PositionDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PositionDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Currency,
            this.Balance,
            this.Locked,
            this.Avg_Buy_Price,
            this.Unit_Currency});
            this.PositionDataGrid.Location = new System.Drawing.Point(12, 7);
            this.PositionDataGrid.Name = "PositionDataGrid";
            this.PositionDataGrid.RowTemplate.Height = 23;
            this.PositionDataGrid.Size = new System.Drawing.Size(548, 431);
            this.PositionDataGrid.TabIndex = 1;
            // 
            // Currency
            // 
            this.Currency.HeaderText = "Currency";
            this.Currency.Name = "Currency";
            // 
            // Balance
            // 
            this.Balance.HeaderText = "Balance";
            this.Balance.Name = "Balance";
            // 
            // Locked
            // 
            this.Locked.HeaderText = "Locked";
            this.Locked.Name = "Locked";
            // 
            // Avg_Buy_Price
            // 
            this.Avg_Buy_Price.HeaderText = "Avg_Buy_Price";
            this.Avg_Buy_Price.Name = "Avg_Buy_Price";
            // 
            // Unit_Currency
            // 
            this.Unit_Currency.HeaderText = "Unit_Currency";
            this.Unit_Currency.Name = "Unit_Currency";
            // 
            // PositionScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 456);
            this.Controls.Add(this.PositionDataGrid);
            this.Name = "PositionScreen";
            this.Text = "PositionScreen";
            ((System.ComponentModel.ISupportInitialize)(this.PositionDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView PositionDataGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Currency;
        private System.Windows.Forms.DataGridViewTextBoxColumn Balance;
        private System.Windows.Forms.DataGridViewTextBoxColumn Locked;
        private System.Windows.Forms.DataGridViewTextBoxColumn Avg_Buy_Price;
        private System.Windows.Forms.DataGridViewTextBoxColumn Unit_Currency;
    }
}