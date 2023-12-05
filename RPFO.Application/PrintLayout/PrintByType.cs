using RPFO.Data.ViewModel;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;

namespace RPFO.Application.PrintLayout
{
    public partial class PrintByType : DevExpress.XtraReports.UI.XtraReport
    {
        private GroupItemByPrint ViewModel;

        Font NormalBold = new Font("arial, sans-serif", 9, FontStyle.Bold);
        Font Normal = new Font("arial, sans-serif", 9);

        public PrintByType()
        {
            InitializeComponent();
        }

        public void SetModel(GroupItemByPrint model)
        {
            this.ViewModel = model;

        }

        protected override void OnBeforePrint(PrintEventArgs e)
        {
            BindingData();
            base.OnBeforePrint(e);
        }

        private void BindingData()
        {
            xrDate.Font = NormalBold;
            xrDateValue.Font = Normal;
            xrCashier.Font = NormalBold;
            xrCashierValue.Font = Normal;
            xrTotalQty.Font = NormalBold;
            xrTotalQtyValue.Font = Normal;
            xrDetailItem.Font = new Font("arial, sans-serif", 10);

            xrStoreName.Text = ViewModel.StoreName;
            xrSaleType.Text = $"Table {ViewModel.TableName.ToLower()}";

            xrDateValue.Text = Convert.ToDateTime(ViewModel.CreatedOn ?? DateTime.Now).ToString("dd/MM/yyyy-hh:mm tt");

            xrPlaceValue.Text = ViewModel.PlaceName.ToLower();
            string formatByCurrency = "#,##0.00";
            string CashierValue = ViewModel.SalesPersonName;

            if (!string.IsNullOrEmpty(ViewModel.TerminalId))
            {
                CashierValue += " - " + ViewModel.TerminalId;
            }

            xrCashierValue.Text = CashierValue;

            DataTable dtLine = new DataTable();
            dtLine.Columns.Add("ItemLine");

            double SumQty = 0;
            StringBuilder tempText = new StringBuilder();
            var html = string.Empty;

            for (int i = 0; i < ViewModel.Items.Count; i++)
            {
                int setPositionRow = 1;
                int setPositionItemNameRow = 35;
                var salesLineView = ViewModel.Items[i];

                string Qty = Convert.ToDouble(salesLineView.Quantity).ToString();
                var name = i + " " + salesLineView.ItemName;

                if (salesLineView.ItemName.Length == setPositionItemNameRow)
                {
                    html += name;
                }
                else if (salesLineView.ItemName.Length < setPositionItemNameRow)
                {
                    var maxlenght = setPositionItemNameRow - salesLineView.ItemName.Length;
                    var addSpace = "";

                    for (int x = 0; x < maxlenght; x++)
                    {
                        addSpace += "";
                    }
                    html += name += addSpace;
                }
                else
                {
                    html += name;
                }

                for (int x = 0; x < setPositionRow; x++)
                {
                    html += " ";
                }

                html += "X" + Qty + Environment.NewLine;

                if (salesLineView.Quantity != null && salesLineView.Quantity > 0)
                {
                    if (salesLineView.Quantity > 1)
                        SumQty += Convert.ToInt32(salesLineView.Quantity);
                    else
                        SumQty += 1;
                }

                tempText.Append(html);
            }
            xrDetailItem.Text = html.ToString();
            xrTotalQtyValue.Text = Convert.ToInt32(SumQty).ToString(formatByCurrency);
        }

        private void PrintReceipt_AfterPrint(object sender, EventArgs e)
        {
        }
    }
}
