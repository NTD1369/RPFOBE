using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit;
using RPFO.Data.ViewModels;
using System;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using RPFO.Data.Entities;


namespace RPFO.Application.PrintLayout
{
    public partial class PrintLucky : DevExpress.XtraReports.UI.XtraReport
    {
        private SaleViewModel ViewModel;
        private string PrintStatus;

        public PrintLucky()
        {
            InitializeComponent();
        }

        public void SetModel(SaleViewModel model, string printstatus)
        {
            this.ViewModel = model;
            this.PrintStatus = printstatus;
        }

        protected override void OnBeforePrint(PrintEventArgs e)
        {
            BindingData();

            base.OnBeforePrint(e);
        }

        private void BindingData()
        {
            //  input code here
            xrStoreName.Text = ViewModel.StoreName;
            xrReceiptValue.Text = ViewModel.TransId;

            // format  Currency
            string formatByCurrency = "#,##0.00";
            if (ViewModel.Payments[0].Currency == "VND")
            {
                formatByCurrency = "#,##0.##";
            }
            else
            {
                formatByCurrency = "#,##0.00";
            }


            if (!string.IsNullOrEmpty(ViewModel.CusGrpId) && ViewModel.CusGrpId != "2")
            {
                xrMemberCardValue.Text = ViewModel.CusId;
                xrMemberNameValue.Text = ViewModel.CusName;
                xrPointBalanceValue.Text = ViewModel.RewardPoints.ToString() + " - Member Expiry: " + Convert.ToDateTime(ViewModel.ExpiryDate).ToString("dd/MM/yyyy");

                if (!string.IsNullOrEmpty(ViewModel.Phone))
                {
                    xrTelNoValue.Text = ViewModel.Phone;
                }
                else
                {
                    xrTelNoValue.Text = String.Empty;
                    xrTelNo.Text = String.Empty;
                }
            }
            else
            {
                xrMemberCard.Text = String.Empty;
                xrMemberCardValue.Text = String.Empty;

                xrMemberName.Text = String.Empty;
                xrMemberNameValue.Text= String.Empty;

                xrTelNo.Text = String.Empty;
                xrTelNoValue.Text = String.Empty;

                xrPointBalance.Text = String.Empty;
                xrPointBalanceValue.Text = String.Empty;
            }
            xrTotalReceipt.Text = Convert.ToDouble(ViewModel.TotalAmount).ToString(formatByCurrency);
            xrReceiptDateValue.Text = Convert.ToDateTime(ViewModel.CreatedOn).ToString("dd/MM/yyyy HH:ss");
            xrLuckyCodeValue.Text = ViewModel.LuckyNo;
            xrContact.Text = "Contact: " + ViewModel.CompanyPhone + " " + ViewModel.Email + " - " + ViewModel.StoreAddress;

        }

        private void PrintLucky_AfterPrint(object sender, EventArgs e)
        {



        }
    }
}
