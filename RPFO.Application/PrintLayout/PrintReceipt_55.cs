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
using System.Text;

namespace RPFO.Application.PrintLayout
{
    public partial class PrintReceipt_55 : DevExpress.XtraReports.UI.XtraReport
    {
        private SaleViewModel ViewModel;
        private string PrintStatus;
        Font NormalBold = new Font("arial, sans-serif", 6, FontStyle.Bold);
        Font Normal = new Font("arial, sans-serif", 6, FontStyle.Bold);

        public PrintReceipt_55()
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
            string saleType = "";
            if (ViewModel.SalesType == "Retail")
            {
                saleType = "SALES";
            }
            else
            {
                saleType = ViewModel.SalesType;
            }
            xrSaleType.Text = saleType + " " + PrintStatus;
            //xrPrintStatus.Text = PrintStatus;
            xrBarCode.Text = ViewModel.TransId;
            xrDateValue.Text = Convert.ToDateTime(ViewModel.CreatedOn).ToString("dd/MM/yyyy-hh:mm tt");
            //xrCardValue.Text = ViewModel.CusId;

            // set size
            xrDate.Font = NormalBold;
            xrDateValue.Font = Normal;
            xrMemCard.Font = NormalBold;
            xrCardValue.Font = Normal;
            xrMemName.Font = NormalBold;
            xrMemValue.Font = Normal;
            xrPointBlc.Font = NormalBold;
            xrPointValue.Font = Normal;
            xrCashier.Font = NormalBold;
            xrCashierValue.Font = Normal;

            xrTotalQty.Font = NormalBold;
            xrTotalQtyValue.Font = Normal;
            xrTotalBill.Font = NormalBold;
            xrTotalBillValue.Font = Normal;
            xrDiscountOnItemsLabel.Font = NormalBold;
            xrDiscountOnItemsValue.Font = Normal;
            xrTotalDiscountLabel.Font = NormalBold;
            xrTotalDiscountValue.Font = Normal;
            xrTotalPayable.Font = NormalBold;
            xrTotalPayableValue.Font = Normal;
            xrLabel1.Font = NormalBold;
            xrReceiptValue.Font = Normal;
            xrPaymentValue.Font = Normal;
            xrLabel3.Font = NormalBold;
            xrChangeValue.Font = Normal;
            xrContact.Font = Normal;

            xrDetailItem.Font = new Font("arial, sans-serif", 8);


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

            if (string.IsNullOrEmpty(ViewModel.RefTransId))
            {
                xrOriginalTransId.Text = string.Empty;
                xrOrginalValue.Text = string.Empty;
            }
            else
            {
                xrOriginalTransId.Text = "Original Trans ID: ";
                xrOrginalValue.Text = ViewModel.RefTransId;
            }
            if (!string.IsNullOrEmpty(ViewModel.CusGrpId) && ViewModel.CusGrpId == "1")
            {
                xrCustomerLabel.Text = string.Empty;
                xrCustomerValue.Text = string.Empty;

                xrMemCard.Text = "Member Card Number: ";
                xrCardValue.Text = ViewModel.CusId;
                xrMemName.Text = "Member Name: ";
                xrMemValue.Text = ViewModel.CusName;
                xrPointBlc.Text = "Point Balance: ";
                xrPointValue.Text = ViewModel.RewardPoints.ToString() + " - Member Expiry: " + Convert.ToDateTime(ViewModel.ExpiryDate).ToString("dd/MM/yyyy");
            }
            else
            {
                if (ViewModel.CusGrpId != "2")
                {
                    xrCustomerLabel.Text = "Customer: ";
                    string cusName = ViewModel.CusName;
                    if (!string.IsNullOrEmpty(ViewModel.Phone))
                    {
                        cusName += " - " + ViewModel.Phone;
                    }
                    xrCustomerValue.Text = cusName;
                }
                else
                {
                    xrCustomerLabel.Text = string.Empty;
                    xrCustomerValue.Text = string.Empty;
                }

                xrMemCard.Text = string.Empty;
                xrCardValue.Text = string.Empty;
                xrMemName.Text = string.Empty;
                xrMemValue.Text = string.Empty;
                xrPointBlc.Text = string.Empty;
                xrPointValue.Text = string.Empty;
            }
            //xrMemValue.Text = ViewModel.CusName;
            //xrPointValue.Text = ViewModel.RewardPoints.ToString();

            string CashierValue = ViewModel.SalesPersonName;
            if (!string.IsNullOrEmpty(ViewModel.TerminalId))
            {
                CashierValue += " - " + ViewModel.TerminalId;
            }
            xrCashierValue.Text = CashierValue;

            DataTable dtLine = new DataTable();
            dtLine.Columns.Add("ItemLine");

            int SumQty = 0;
            decimal discountOnItems = 0;
            StringBuilder tempText = new StringBuilder();
            var html = "";
            for (int i = 0; i < ViewModel.Lines.Count; i++)
            {
                var salesLineView = ViewModel.Lines[i];
                string ItemName = "";
                string Qty = "";
                string Price = "";
                string LineTotal = "";
                string ItemLine = "";

                // bùa nhẹ
                string trimItem0 = "";
                string trimItem1 = "";

                string discountItemLine = "";


                Qty = Convert.ToDouble(salesLineView.Quantity).ToString().PadLeft(5, ' ');
                Price = Convert.ToDouble(salesLineView.Price).ToString(formatByCurrency).PadLeft(6, ' ');
                LineTotal = Convert.ToDouble(salesLineView.LineTotal).ToString(formatByCurrency).PadLeft(10, ' ');
                decimal a = salesLineView.ItemName.Length;
                ItemName = salesLineView.ItemName;
                discountOnItems += salesLineView.DiscountAmt.HasValue ? salesLineView.DiscountAmt.Value : 0;
                if (salesLineView.DiscountAmt > 0)
                {
                    discountItemLine += ("Disc: - " + Convert.ToDouble(salesLineView.DiscountAmt).ToString(formatByCurrency)).PadLeft(20, ' ');
                }

                if (a > 15)
                {
                    ItemName = salesLineView.ItemName.Insert(15, " \r\n ");
                    trimItem0 = ItemName.Split("\r\n")[0];
                    trimItem1 = ItemName.Split("\r\n")[1];
                    html += trimItem0;
                }
                else
                {
                    ItemName = salesLineView.ItemName;
                    html += ItemName;
                }
                //html = trimItem0;
                html += Qty + "x" + Price + LineTotal + Environment.NewLine;
                if (a > 15)
                {
                    html += trimItem1 + Environment.NewLine;
                }
                if (salesLineView.Custom1 != null)
                {
                    html += "Card No: " + salesLineView.Custom1 + Environment.NewLine;
                }
                if (salesLineView.Custom3 != null)
                {
                    html += "Ref Number: " + salesLineView.Custom3.Split("|")[0] + Environment.NewLine;
                }
                //if (salesLineView.Custom4 != null)
                //{
                //    html += salesLineView.Custom4 + Environment.NewLine;
                //}

                html += salesLineView.BarCode + discountItemLine;
                if ((ViewModel.Lines.Count - 1) != i)
                {
                    html += Environment.NewLine;
                    if (salesLineView.IsSerial == false)
                    {
                        html += Environment.NewLine;
                    }
                }

                if (((ViewModel.Lines.Count - 1) == i) && salesLineView.IsSerial == true)
                {
                    html += Environment.NewLine;
                    html += "Serials:" + Environment.NewLine;
                    foreach (TSalesLineSerialViewModel itemSerial in ViewModel.SerialLines)
                    {
                        html += "- " + itemSerial.SerialNum + " Exp Date: " + Convert.ToDateTime(itemSerial.ExpDate).ToString("dd/MM/yyyy");
                    }
                }
                else if (salesLineView.IsSerial == true)
                {
                    html += "Serials:" + Environment.NewLine;
                    foreach (TSalesLineSerialViewModel itemSerial in ViewModel.SerialLines)
                    {
                        html += "- " + itemSerial.SerialNum + " Exp Date: " + Convert.ToDateTime(itemSerial.ExpDate).ToString("dd/MM/yyyy");
                    }
                    html += Environment.NewLine;
                    html += Environment.NewLine;
                }

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
            //foreach (TSalesLineViewModel salesLineView in ViewModel.Lines)
            //{

            //    string ItemLine = "";
            //    string ItemName = "";
            //    string Qty = "".PadLeft(4, ' ');
            //    string Price = "".PadLeft(4, ' ');
            //    string LineTotal = "".PadLeft(4, ' ');

            //    ItemName = salesLineView.ItemName + Environment.NewLine;

            //    if (salesLineView.Quantity != null && salesLineView.Quantity > 0)
            //    {
            //        if (salesLineView.Quantity > 1)
            //            SumQty += Convert.ToInt32(salesLineView.Quantity);
            //        else
            //            SumQty += 1;
            //    }
            //    if (salesLineView.Quantity != null)
            //        Qty = Convert.ToDouble(salesLineView.Quantity).ToString().PadLeft(6, ' ');

            //    if (salesLineView.Price != null)
            //        Price = Convert.ToDouble(salesLineView.Price).ToString("#,##0.##").PadLeft(6, ' ');

            //    if (salesLineView.LineTotal != null)
            //        LineTotal = Convert.ToDouble(salesLineView.LineTotal).ToString("#,##0.##").PadLeft(7, ' ');

            //    ItemLine = ItemName;
            //    ItemLine += salesLineView.BarCode.PadRight(16, ' ');
            //    ItemLine += Qty + "x" + Price + " " + LineTotal;

            //    DataRow newLine = dtLine.NewRow();
            //    newLine["ItemLine"] = ItemLine;
            //    dtLine.Rows.Add(ItemLine);
            //    discountOnItems += salesLineView.DiscountAmt.HasValue ? salesLineView.DiscountAmt.Value : 0;
            //}
            //DataSource = dtLine;
            //xrTableCell.DataBindings.Add("Text", DataSource, "ItemLine");

            xrTotalQtyValue.Text = Convert.ToInt32(SumQty).ToString();
            xrTotalBillValue.Text = Convert.ToDouble(ViewModel.TotalAmount).ToString(formatByCurrency);

            if (ViewModel.DiscountAmount != null && ViewModel.DiscountAmount != 0)
            {
                xrDiscountOnTotalValue.Text = " - " + Convert.ToDouble(ViewModel.DiscountAmount).ToString(formatByCurrency);
            }
            else
            {
                xrDiscountOnTotalLabel.Text = string.Empty;
                xrDiscountOnTotalValue.Text = string.Empty;
            }
            if (discountOnItems != 0)
            {
                xrDiscountOnItemsValue.Text = " - " + Convert.ToDouble(discountOnItems).ToString(formatByCurrency);
            }
            else
            {
                xrDiscountOnItemsLabel.Text = string.Empty;
                xrDiscountOnItemsValue.Text = string.Empty;
            }
            if (ViewModel.TotalDiscountAmt != 0)
            {
                xrTotalDiscountValue.Text = " - " + Convert.ToDouble(ViewModel.TotalDiscountAmt).ToString(formatByCurrency);
            }
            else
            {
                xrTotalDiscountLabel.Text = string.Empty;
                xrTotalDiscountValue.Text = string.Empty;
            }
            if (ViewModel.RoundingOff != 0)
            {
                xrRoundingValue.Text = Convert.ToDouble(ViewModel.RoundingOff).ToString(formatByCurrency);
            }
            else
            {
                xrRounding.Text = string.Empty;
                xrRoundingValue.Text = string.Empty;
            }

            xrTotalPayableValue.Text = Convert.ToDouble(ViewModel.TotalPayable).ToString(formatByCurrency);

            string PaymentLine = "";
            foreach (TSalesPayment salesPaymentView in ViewModel.Payments)
            {
                PaymentLine += salesPaymentView.ShortName.PadRight(28, ' ') + Convert.ToDouble(salesPaymentView.CollectedAmount).ToString(formatByCurrency).PadLeft(7, ' ') + Environment.NewLine;
                if (!string.IsNullOrEmpty(salesPaymentView.RefNumber))
                {
                    if (salesPaymentView.RefNumber.Length >= 4)
                    {
                        PaymentLine += "Ref1 : **" + salesPaymentView.RefNumber.Substring(salesPaymentView.RefNumber.Length - 4, 4);
                    }
                    else
                    {
                        PaymentLine += "Ref1 : **" + salesPaymentView.RefNumber.Substring(salesPaymentView.RefNumber.Length - 1, 1);
                    }
                    PaymentLine += Environment.NewLine;
                    //PaymentLine += salesPaymentView.CustomF1 + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(salesPaymentView.CustomF1))
                {
                    if (salesPaymentView.CustomF1.Length >= 4)
                    {
                        PaymentLine += "Ref2 : **" + salesPaymentView.CustomF1.Substring(salesPaymentView.CustomF1.Length - 4, 4);
                    }
                    else
                    {
                        PaymentLine += "Ref2 : **" + salesPaymentView.CustomF1.Substring(salesPaymentView.CustomF1.Length - 1, 1);
                    }
                    PaymentLine += Environment.NewLine;
                    //PaymentLine += salesPaymentView.CustomF1 + Environment.NewLine;
                }
            }

            xrPaymentValue.Text = PaymentLine;
            xrReceiptValue.Text = Convert.ToDouble(ViewModel.TotalReceipt).ToString(formatByCurrency);
            xrChangeValue.Text = Convert.ToDouble(ViewModel.AmountChange).ToString(formatByCurrency);
            if (string.IsNullOrEmpty(ViewModel.ApprovalId))
                xrApproval.Text = ViewModel.ApprovalId;
            xrContact.Text = ViewModel.CompanyPhone + " " + ViewModel.Email + " - " + ViewModel.StoreAddress;

            // EPay
            foreach (TSalesLineViewModel itemLine in ViewModel.Lines)
            {
                if (itemLine.ItemType?.ToLower() == "pn" || itemLine.ItemType.ToLower() == "pin")
                {
                    foreach (TSalesLineSerialViewModel arrSerial in ViewModel.SerialLines)
                    {
                        xrItemNameEpay.Text = arrSerial.ItemName;
                        xrBaseTransId.Text = arrSerial.BaseTransId.Split("|")[0];
                        xrSerialNumEpayValue.Text = arrSerial.SerialNum;
                        xrTerminalIdEpayValue.Text = arrSerial.TransId;
                        xrCashierEpayValue.Text = arrSerial.CreatedBy;
                        xrTerminalIdEpayValue.Text = ViewModel.TerminalId;
                        if (arrSerial.ExpDate != null)
                        {
                            xrExpDateEpayValue.Text = Convert.ToDateTime(arrSerial.ExpDate).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            xrExpDateEpay.Text = String.Empty;
                            xrExpDateEpayValue.Text = String.Empty;
                        }
                    }
                }
                else
                {
                    xrItemNameEpay.Text = String.Empty;
                    xrBaseTransId.Text = String.Empty;

                    xrSerialNumEpay.Text = String.Empty;
                    xrSerialNumEpayValue.Text = String.Empty;

                    xrTerminalIdEpay.Text = String.Empty;
                    xrTerminalIdEpayValue.Text = String.Empty;


                    xrTransationIdEpay.Text = String.Empty;
                    xrTransationIdEpayValue.Text = String.Empty;

                    xrCashierEpay.Text = String.Empty;
                    xrCashierEpayValue.Text = String.Empty;

                    xrExpDateEpay.Text = String.Empty;
                    xrExpDateEpayValue.Text = String.Empty;

                }
            }
        }
    }
}
