using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoinGo
{
    public partial class PositionScreen : Form
    {
        BindingSource bindingSource = new BindingSource();


        public PositionScreen ()
        {
            InitializeComponent();
            PositionState state = new PositionState("-", "-", "-", "-", "-");
            Params.CoinPositionDict["Start"] = state;

            bindingSource.DataSource = Params.CoinPositionDict["Start"];
            PositionDataGrid.DataSource = bindingSource;
        }

        public void DisplayTargetCoins(string currency)
        {


            if (PositionDataGrid.InvokeRequired)
            {
                PositionDataGrid.Invoke(new MethodInvoker(delegate ()
                {
                    bindingSource.Add(Params.CoinPositionDict[currency]);
                }));
            }

            else
            {
                bindingSource.Add(Params.CoinPositionDict[currency]);
            }
        }
    }

}

