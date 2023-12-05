
namespace RPFO.Application.PrintLayout
{
    partial class PrintReceipt
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

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintReceipt));
            DevExpress.XtraPrinting.BarCode.Code128Generator code128Generator1 = new DevExpress.XtraPrinting.BarCode.Code128Generator();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrDetailItem = new DevExpress.XtraReports.UI.XRRichText();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrLine3 = new DevExpress.XtraReports.UI.XRLine();
            this.xrCustomerLabel = new DevExpress.XtraReports.UI.XRLabel();
            this.xrCustomerValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrOriginalTransId = new DevExpress.XtraReports.UI.XRLabel();
            this.xrOrginalValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrCashierValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPointValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrMemValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrCardValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrDateValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrCashier = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPointBlc = new DevExpress.XtraReports.UI.XRLabel();
            this.xrMemName = new DevExpress.XtraReports.UI.XRLabel();
            this.xrMemCard = new DevExpress.XtraReports.UI.XRLabel();
            this.xrDate = new DevExpress.XtraReports.UI.XRLabel();
            this.xrSaleType = new DevExpress.XtraReports.UI.XRLabel();
            this.xrStoreName = new DevExpress.XtraReports.UI.XRLabel();
            this.xrBarCode = new DevExpress.XtraReports.UI.XRBarCode();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.xrExpDateEpayValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrCashierEpay = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTerminalIdEpayValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTerminalIdEpay = new DevExpress.XtraReports.UI.XRLabel();
            this.xrCashierEpayValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTransactionIdEpayValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrExpDateEpay = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTransactionIdEpay = new DevExpress.XtraReports.UI.XRLabel();
            this.xrSerialNumEpayValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrSerialNumEpay = new DevExpress.XtraReports.UI.XRLabel();
            this.xrBaseTransId = new DevExpress.XtraReports.UI.XRLabel();
            this.xrItemNameEpay = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine5 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLine4 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLine2 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.xrTotalDiscountLabel = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTotalDiscountValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrDiscountOnTotalLabel = new DevExpress.XtraReports.UI.XRLabel();
            this.xrDiscountOnTotalValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrDiscountOnItemsValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrDiscountOnItemsLabel = new DevExpress.XtraReports.UI.XRLabel();
            this.xrApproval = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTotalPayableValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrRoundingValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTotalBillValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTotalQtyValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTotalPayable = new DevExpress.XtraReports.UI.XRLabel();
            this.xrRounding = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTotalBill = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTotalQty = new DevExpress.XtraReports.UI.XRLabel();
            this.xrChangeValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrReceiptValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrContact = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPaymentValue = new DevExpress.XtraReports.UI.XRRichText();
            ((System.ComponentModel.ISupportInitialize)(this.xrDetailItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrPaymentValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrDetailItem});
            this.Detail.Dpi = 254F;
            this.Detail.HeightF = 58.42F;
            this.Detail.HierarchyPrintOptions.Indent = 50.8F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrDetailItem
            // 
            this.xrDetailItem.Dpi = 254F;
            this.xrDetailItem.Font = new System.Drawing.Font("Times New Roman", 9.75F);
            this.xrDetailItem.LocationFloat = new DevExpress.Utils.PointFloat(25.00001F, 0F);
            this.xrDetailItem.Name = "xrDetailItem";
            this.xrDetailItem.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrDetailItem.SerializableRtfString = resources.GetString("xrDetailItem.SerializableRtfString");
            this.xrDetailItem.SizeF = new System.Drawing.SizeF(689.3552F, 58.42F);
            // 
            // TopMargin
            // 
            this.TopMargin.Dpi = 254F;
            this.TopMargin.HeightF = 0F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.Dpi = 254F;
            this.BottomMargin.HeightF = 4.304163F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLine3,
            this.xrCustomerLabel,
            this.xrCustomerValue,
            this.xrOriginalTransId,
            this.xrOrginalValue,
            this.xrCashierValue,
            this.xrPointValue,
            this.xrMemValue,
            this.xrCardValue,
            this.xrDateValue,
            this.xrCashier,
            this.xrPointBlc,
            this.xrMemName,
            this.xrMemCard,
            this.xrDate,
            this.xrSaleType,
            this.xrStoreName,
            this.xrBarCode});
            this.ReportHeader.Dpi = 254F;
            this.ReportHeader.HeightF = 570.9919F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // xrLine3
            // 
            this.xrLine3.Dpi = 254F;
            this.xrLine3.LineStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            this.xrLine3.LocationFloat = new DevExpress.Utils.PointFloat(37.06524F, 545.9919F);
            this.xrLine3.Name = "xrLine3";
            this.xrLine3.SizeF = new System.Drawing.SizeF(656.9347F, 25F);
            // 
            // xrCustomerLabel
            // 
            this.xrCustomerLabel.CanShrink = true;
            this.xrCustomerLabel.Dpi = 254F;
            this.xrCustomerLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrCustomerLabel.LocationFloat = new DevExpress.Utils.PointFloat(37.064F, 355.9919F);
            this.xrCustomerLabel.Multiline = true;
            this.xrCustomerLabel.Name = "xrCustomerLabel";
            this.xrCustomerLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrCustomerLabel.SizeF = new System.Drawing.SizeF(169.6003F, 37.99997F);
            this.xrCustomerLabel.StylePriority.UseFont = false;
            this.xrCustomerLabel.StylePriority.UseTextAlignment = false;
            this.xrCustomerLabel.Text = "Customer: ";
            this.xrCustomerLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrCustomerValue
            // 
            this.xrCustomerValue.CanShrink = true;
            this.xrCustomerValue.Dpi = 254F;
            this.xrCustomerValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrCustomerValue.LocationFloat = new DevExpress.Utils.PointFloat(206.6642F, 355.9919F);
            this.xrCustomerValue.Name = "xrCustomerValue";
            this.xrCustomerValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrCustomerValue.SizeF = new System.Drawing.SizeF(508.4155F, 38.00003F);
            this.xrCustomerValue.StylePriority.UseFont = false;
            this.xrCustomerValue.StylePriority.UseTextAlignment = false;
            this.xrCustomerValue.Text = "Customer Value";
            this.xrCustomerValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrOriginalTransId
            // 
            this.xrOriginalTransId.CanShrink = true;
            this.xrOriginalTransId.Dpi = 254F;
            this.xrOriginalTransId.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrOriginalTransId.LocationFloat = new DevExpress.Utils.PointFloat(37.064F, 317.9919F);
            this.xrOriginalTransId.Multiline = true;
            this.xrOriginalTransId.Name = "xrOriginalTransId";
            this.xrOriginalTransId.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrOriginalTransId.SizeF = new System.Drawing.SizeF(298.3642F, 38F);
            this.xrOriginalTransId.StylePriority.UseFont = false;
            this.xrOriginalTransId.StylePriority.UseTextAlignment = false;
            this.xrOriginalTransId.Text = "Original Trans ID: ";
            this.xrOriginalTransId.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrOrginalValue
            // 
            this.xrOrginalValue.CanShrink = true;
            this.xrOrginalValue.Dpi = 254F;
            this.xrOrginalValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrOrginalValue.LocationFloat = new DevExpress.Utils.PointFloat(335.428F, 317.992F);
            this.xrOrginalValue.Name = "xrOrginalValue";
            this.xrOrginalValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrOrginalValue.SizeF = new System.Drawing.SizeF(379.6516F, 38.00003F);
            this.xrOrginalValue.StylePriority.UseFont = false;
            this.xrOrginalValue.StylePriority.UseTextAlignment = false;
            this.xrOrginalValue.Text = "Original Value";
            this.xrOrginalValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrCashierValue
            // 
            this.xrCashierValue.Dpi = 254F;
            this.xrCashierValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrCashierValue.LocationFloat = new DevExpress.Utils.PointFloat(175.6422F, 507.9919F);
            this.xrCashierValue.Name = "xrCashierValue";
            this.xrCashierValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrCashierValue.SizeF = new System.Drawing.SizeF(539.4376F, 38.00006F);
            this.xrCashierValue.StylePriority.UseFont = false;
            this.xrCashierValue.StylePriority.UseTextAlignment = false;
            this.xrCashierValue.Text = "Cahier Value";
            this.xrCashierValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrPointValue
            // 
            this.xrPointValue.CanShrink = true;
            this.xrPointValue.Dpi = 254F;
            this.xrPointValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrPointValue.LocationFloat = new DevExpress.Utils.PointFloat(285.8329F, 469.9919F);
            this.xrPointValue.Name = "xrPointValue";
            this.xrPointValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrPointValue.SizeF = new System.Drawing.SizeF(429.2467F, 38.00006F);
            this.xrPointValue.StylePriority.UseFont = false;
            this.xrPointValue.StylePriority.UseTextAlignment = false;
            this.xrPointValue.Text = "Point Value";
            this.xrPointValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrMemValue
            // 
            this.xrMemValue.CanShrink = true;
            this.xrMemValue.Dpi = 254F;
            this.xrMemValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrMemValue.LocationFloat = new DevExpress.Utils.PointFloat(285.8334F, 431.9918F);
            this.xrMemValue.Name = "xrMemValue";
            this.xrMemValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrMemValue.SizeF = new System.Drawing.SizeF(429.2464F, 38.00003F);
            this.xrMemValue.StylePriority.UseFont = false;
            this.xrMemValue.StylePriority.UseTextAlignment = false;
            this.xrMemValue.Text = "Member Value";
            this.xrMemValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrCardValue
            // 
            this.xrCardValue.CanShrink = true;
            this.xrCardValue.Dpi = 254F;
            this.xrCardValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrCardValue.LocationFloat = new DevExpress.Utils.PointFloat(403.4951F, 393.9919F);
            this.xrCardValue.Name = "xrCardValue";
            this.xrCardValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrCardValue.SizeF = new System.Drawing.SizeF(311.5847F, 38.00003F);
            this.xrCardValue.StylePriority.UseFont = false;
            this.xrCardValue.StylePriority.UseTextAlignment = false;
            this.xrCardValue.Text = "Card Value";
            this.xrCardValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrDateValue
            // 
            this.xrDateValue.Dpi = 254F;
            this.xrDateValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrDateValue.LocationFloat = new DevExpress.Utils.PointFloat(132.3156F, 279.9919F);
            this.xrDateValue.Name = "xrDateValue";
            this.xrDateValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrDateValue.SizeF = new System.Drawing.SizeF(582.764F, 38.00006F);
            this.xrDateValue.StylePriority.UseFont = false;
            this.xrDateValue.StylePriority.UseTextAlignment = false;
            this.xrDateValue.Text = "Date Value";
            this.xrDateValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrCashier
            // 
            this.xrCashier.Dpi = 254F;
            this.xrCashier.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrCashier.LocationFloat = new DevExpress.Utils.PointFloat(37.064F, 507.9919F);
            this.xrCashier.Multiline = true;
            this.xrCashier.Name = "xrCashier";
            this.xrCashier.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrCashier.SizeF = new System.Drawing.SizeF(138.5782F, 37.99997F);
            this.xrCashier.StylePriority.UseFont = false;
            this.xrCashier.StylePriority.UseTextAlignment = false;
            this.xrCashier.Text = "Cashier:";
            this.xrCashier.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrPointBlc
            // 
            this.xrPointBlc.CanShrink = true;
            this.xrPointBlc.Dpi = 254F;
            this.xrPointBlc.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrPointBlc.LocationFloat = new DevExpress.Utils.PointFloat(37.064F, 469.9919F);
            this.xrPointBlc.Multiline = true;
            this.xrPointBlc.Name = "xrPointBlc";
            this.xrPointBlc.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrPointBlc.SizeF = new System.Drawing.SizeF(248.7664F, 38F);
            this.xrPointBlc.StylePriority.UseFont = false;
            this.xrPointBlc.StylePriority.UseTextAlignment = false;
            this.xrPointBlc.Text = "Point Balance:";
            this.xrPointBlc.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrMemName
            // 
            this.xrMemName.CanShrink = true;
            this.xrMemName.Dpi = 254F;
            this.xrMemName.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrMemName.LocationFloat = new DevExpress.Utils.PointFloat(37.064F, 431.9918F);
            this.xrMemName.Multiline = true;
            this.xrMemName.Name = "xrMemName";
            this.xrMemName.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrMemName.SizeF = new System.Drawing.SizeF(248.7694F, 38F);
            this.xrMemName.StylePriority.UseFont = false;
            this.xrMemName.StylePriority.UseTextAlignment = false;
            this.xrMemName.Text = "Member Name:";
            this.xrMemName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrMemCard
            // 
            this.xrMemCard.CanShrink = true;
            this.xrMemCard.Dpi = 254F;
            this.xrMemCard.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrMemCard.LocationFloat = new DevExpress.Utils.PointFloat(37.064F, 393.9919F);
            this.xrMemCard.Multiline = true;
            this.xrMemCard.Name = "xrMemCard";
            this.xrMemCard.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrMemCard.SizeF = new System.Drawing.SizeF(366.4312F, 38F);
            this.xrMemCard.StylePriority.UseFont = false;
            this.xrMemCard.StylePriority.UseTextAlignment = false;
            this.xrMemCard.Text = "Member Card Number:";
            this.xrMemCard.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrDate
            // 
            this.xrDate.Dpi = 254F;
            this.xrDate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrDate.LocationFloat = new DevExpress.Utils.PointFloat(37.064F, 279.992F);
            this.xrDate.Multiline = true;
            this.xrDate.Name = "xrDate";
            this.xrDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrDate.SizeF = new System.Drawing.SizeF(95.25176F, 38F);
            this.xrDate.StylePriority.UseFont = false;
            this.xrDate.StylePriority.UseTextAlignment = false;
            this.xrDate.Text = "Date:";
            this.xrDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrSaleType
            // 
            this.xrSaleType.Dpi = 254F;
            this.xrSaleType.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrSaleType.LocationFloat = new DevExpress.Utils.PointFloat(37.06395F, 58.42001F);
            this.xrSaleType.Multiline = true;
            this.xrSaleType.Name = "xrSaleType";
            this.xrSaleType.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrSaleType.SizeF = new System.Drawing.SizeF(681.9364F, 58.41998F);
            this.xrSaleType.StylePriority.UseFont = false;
            this.xrSaleType.StylePriority.UseTextAlignment = false;
            this.xrSaleType.Text = "xrSaleType";
            this.xrSaleType.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrStoreName
            // 
            this.xrStoreName.CanGrow = false;
            this.xrStoreName.Dpi = 254F;
            this.xrStoreName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrStoreName.LocationFloat = new DevExpress.Utils.PointFloat(37.064F, 0F);
            this.xrStoreName.Multiline = true;
            this.xrStoreName.Name = "xrStoreName";
            this.xrStoreName.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrStoreName.SizeF = new System.Drawing.SizeF(678.0158F, 58.42F);
            this.xrStoreName.StylePriority.UseFont = false;
            this.xrStoreName.StylePriority.UseTextAlignment = false;
            this.xrStoreName.Text = "xrStoreName";
            this.xrStoreName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrBarCode
            // 
            this.xrBarCode.AutoModule = true;
            this.xrBarCode.Dpi = 254F;
            this.xrBarCode.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrBarCode.LocationFloat = new DevExpress.Utils.PointFloat(36.33937F, 130.0691F);
            this.xrBarCode.Module = 5.08F;
            this.xrBarCode.Name = "xrBarCode";
            this.xrBarCode.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
            this.xrBarCode.SizeF = new System.Drawing.SizeF(678.0158F, 136.2597F);
            this.xrBarCode.StylePriority.UseFont = false;
            this.xrBarCode.StylePriority.UsePadding = false;
            this.xrBarCode.Symbology = code128Generator1;
            this.xrBarCode.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPaymentValue,
            this.xrExpDateEpayValue,
            this.xrCashierEpay,
            this.xrTerminalIdEpayValue,
            this.xrTerminalIdEpay,
            this.xrCashierEpayValue,
            this.xrTransactionIdEpayValue,
            this.xrExpDateEpay,
            this.xrTransactionIdEpay,
            this.xrSerialNumEpayValue,
            this.xrSerialNumEpay,
            this.xrBaseTransId,
            this.xrItemNameEpay,
            this.xrLine5,
            this.xrLine4,
            this.xrLine2,
            this.xrLine1,
            this.xrTotalDiscountLabel,
            this.xrTotalDiscountValue,
            this.xrDiscountOnTotalLabel,
            this.xrDiscountOnTotalValue,
            this.xrDiscountOnItemsValue,
            this.xrDiscountOnItemsLabel,
            this.xrApproval,
            this.xrTotalPayableValue,
            this.xrRoundingValue,
            this.xrTotalBillValue,
            this.xrTotalQtyValue,
            this.xrTotalPayable,
            this.xrRounding,
            this.xrTotalBill,
            this.xrTotalQty,
            this.xrChangeValue,
            this.xrReceiptValue,
            this.xrContact,
            this.xrLabel3,
            this.xrLabel1});
            this.ReportFooter.Dpi = 254F;
            this.ReportFooter.HeightF = 904.2706F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // xrExpDateEpayValue
            // 
            this.xrExpDateEpayValue.Dpi = 254F;
            this.xrExpDateEpayValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrExpDateEpayValue.LocationFloat = new DevExpress.Utils.PointFloat(361.3679F, 854.6148F);
            this.xrExpDateEpayValue.Name = "xrExpDateEpayValue";
            this.xrExpDateEpayValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrExpDateEpayValue.SizeF = new System.Drawing.SizeF(340.9225F, 38F);
            this.xrExpDateEpayValue.StylePriority.UseFont = false;
            this.xrExpDateEpayValue.StylePriority.UseTextAlignment = false;
            this.xrExpDateEpayValue.Text = "Serial Value";
            this.xrExpDateEpayValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrCashierEpay
            // 
            this.xrCashierEpay.CanShrink = true;
            this.xrCashierEpay.Dpi = 254F;
            this.xrCashierEpay.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrCashierEpay.LocationFloat = new DevExpress.Utils.PointFloat(35.61475F, 816.6149F);
            this.xrCashierEpay.Multiline = true;
            this.xrCashierEpay.Name = "xrCashierEpay";
            this.xrCashierEpay.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrCashierEpay.SizeF = new System.Drawing.SizeF(325.7532F, 37.99994F);
            this.xrCashierEpay.StylePriority.UseFont = false;
            this.xrCashierEpay.StylePriority.UseTextAlignment = false;
            this.xrCashierEpay.Text = "Cashier :";
            this.xrCashierEpay.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrTerminalIdEpayValue
            // 
            this.xrTerminalIdEpayValue.Dpi = 254F;
            this.xrTerminalIdEpayValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTerminalIdEpayValue.LocationFloat = new DevExpress.Utils.PointFloat(364.2707F, 740.615F);
            this.xrTerminalIdEpayValue.Name = "xrTerminalIdEpayValue";
            this.xrTerminalIdEpayValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrTerminalIdEpayValue.SizeF = new System.Drawing.SizeF(339.4703F, 38F);
            this.xrTerminalIdEpayValue.StylePriority.UseFont = false;
            this.xrTerminalIdEpayValue.StylePriority.UseTextAlignment = false;
            this.xrTerminalIdEpayValue.Text = "Serial Value";
            this.xrTerminalIdEpayValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrTerminalIdEpay
            // 
            this.xrTerminalIdEpay.CanShrink = true;
            this.xrTerminalIdEpay.Dpi = 254F;
            this.xrTerminalIdEpay.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTerminalIdEpay.LocationFloat = new DevExpress.Utils.PointFloat(37.06524F, 740.615F);
            this.xrTerminalIdEpay.Multiline = true;
            this.xrTerminalIdEpay.Name = "xrTerminalIdEpay";
            this.xrTerminalIdEpay.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrTerminalIdEpay.SizeF = new System.Drawing.SizeF(327.2037F, 38F);
            this.xrTerminalIdEpay.StylePriority.UseFont = false;
            this.xrTerminalIdEpay.StylePriority.UseTextAlignment = false;
            this.xrTerminalIdEpay.Text = "TerminalId :";
            this.xrTerminalIdEpay.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrCashierEpayValue
            // 
            this.xrCashierEpayValue.Dpi = 254F;
            this.xrCashierEpayValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrCashierEpayValue.LocationFloat = new DevExpress.Utils.PointFloat(361.368F, 816.6149F);
            this.xrCashierEpayValue.Name = "xrCashierEpayValue";
            this.xrCashierEpayValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrCashierEpayValue.SizeF = new System.Drawing.SizeF(340.9225F, 38F);
            this.xrCashierEpayValue.StylePriority.UseFont = false;
            this.xrCashierEpayValue.StylePriority.UseTextAlignment = false;
            this.xrCashierEpayValue.Text = "Serial Value";
            this.xrCashierEpayValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrTransactionIdEpayValue
            // 
            this.xrTransactionIdEpayValue.Dpi = 254F;
            this.xrTransactionIdEpayValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTransactionIdEpayValue.LocationFloat = new DevExpress.Utils.PointFloat(364.2689F, 778.6149F);
            this.xrTransactionIdEpayValue.Name = "xrTransactionIdEpayValue";
            this.xrTransactionIdEpayValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrTransactionIdEpayValue.SizeF = new System.Drawing.SizeF(340.1979F, 38F);
            this.xrTransactionIdEpayValue.StylePriority.UseFont = false;
            this.xrTransactionIdEpayValue.StylePriority.UseTextAlignment = false;
            this.xrTransactionIdEpayValue.Text = "Serial Value";
            this.xrTransactionIdEpayValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrExpDateEpay
            // 
            this.xrExpDateEpay.CanShrink = true;
            this.xrExpDateEpay.Dpi = 254F;
            this.xrExpDateEpay.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrExpDateEpay.LocationFloat = new DevExpress.Utils.PointFloat(35.61475F, 854.6148F);
            this.xrExpDateEpay.Multiline = true;
            this.xrExpDateEpay.Name = "xrExpDateEpay";
            this.xrExpDateEpay.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrExpDateEpay.SizeF = new System.Drawing.SizeF(325.7532F, 37.99994F);
            this.xrExpDateEpay.StylePriority.UseFont = false;
            this.xrExpDateEpay.StylePriority.UseTextAlignment = false;
            this.xrExpDateEpay.Text = "Exp Date :";
            this.xrExpDateEpay.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrTransactionIdEpay
            // 
            this.xrTransactionIdEpay.CanShrink = true;
            this.xrTransactionIdEpay.Dpi = 254F;
            this.xrTransactionIdEpay.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTransactionIdEpay.LocationFloat = new DevExpress.Utils.PointFloat(35.61475F, 778.6149F);
            this.xrTransactionIdEpay.Multiline = true;
            this.xrTransactionIdEpay.Name = "xrTransactionIdEpay";
            this.xrTransactionIdEpay.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrTransactionIdEpay.SizeF = new System.Drawing.SizeF(327.2037F, 38F);
            this.xrTransactionIdEpay.StylePriority.UseFont = false;
            this.xrTransactionIdEpay.StylePriority.UseTextAlignment = false;
            this.xrTransactionIdEpay.Text = "Transaction Id :";
            this.xrTransactionIdEpay.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrSerialNumEpayValue
            // 
            this.xrSerialNumEpayValue.Dpi = 254F;
            this.xrSerialNumEpayValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrSerialNumEpayValue.LocationFloat = new DevExpress.Utils.PointFloat(364.2689F, 702.6151F);
            this.xrSerialNumEpayValue.Name = "xrSerialNumEpayValue";
            this.xrSerialNumEpayValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrSerialNumEpayValue.SizeF = new System.Drawing.SizeF(339.4704F, 38.00006F);
            this.xrSerialNumEpayValue.StylePriority.UseFont = false;
            this.xrSerialNumEpayValue.StylePriority.UseTextAlignment = false;
            this.xrSerialNumEpayValue.Text = "Serial Value";
            this.xrSerialNumEpayValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrSerialNumEpay
            // 
            this.xrSerialNumEpay.CanShrink = true;
            this.xrSerialNumEpay.Dpi = 254F;
            this.xrSerialNumEpay.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrSerialNumEpay.LocationFloat = new DevExpress.Utils.PointFloat(36.33935F, 702.6149F);
            this.xrSerialNumEpay.Multiline = true;
            this.xrSerialNumEpay.Name = "xrSerialNumEpay";
            this.xrSerialNumEpay.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrSerialNumEpay.SizeF = new System.Drawing.SizeF(326.4791F, 38F);
            this.xrSerialNumEpay.StylePriority.UseFont = false;
            this.xrSerialNumEpay.StylePriority.UseTextAlignment = false;
            this.xrSerialNumEpay.Text = "Serial Number :";
            this.xrSerialNumEpay.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrBaseTransId
            // 
            this.xrBaseTransId.Dpi = 254F;
            this.xrBaseTransId.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrBaseTransId.LocationFloat = new DevExpress.Utils.PointFloat(37.06524F, 644.195F);
            this.xrBaseTransId.Multiline = true;
            this.xrBaseTransId.Name = "xrBaseTransId";
            this.xrBaseTransId.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrBaseTransId.SizeF = new System.Drawing.SizeF(666.6741F, 58.41998F);
            this.xrBaseTransId.StylePriority.UseFont = false;
            this.xrBaseTransId.StylePriority.UseTextAlignment = false;
            this.xrBaseTransId.Text = "xrBaseTransId";
            this.xrBaseTransId.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrItemNameEpay
            // 
            this.xrItemNameEpay.CanGrow = false;
            this.xrItemNameEpay.Dpi = 254F;
            this.xrItemNameEpay.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrItemNameEpay.LocationFloat = new DevExpress.Utils.PointFloat(35.61475F, 585.775F);
            this.xrItemNameEpay.Multiline = true;
            this.xrItemNameEpay.Name = "xrItemNameEpay";
            this.xrItemNameEpay.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrItemNameEpay.SizeF = new System.Drawing.SizeF(668.1246F, 58.41998F);
            this.xrItemNameEpay.StylePriority.UseFont = false;
            this.xrItemNameEpay.StylePriority.UseTextAlignment = false;
            this.xrItemNameEpay.Text = "xrItemNameEpay";
            this.xrItemNameEpay.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLine5
            // 
            this.xrLine5.Dpi = 254F;
            this.xrLine5.LineStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            this.xrLine5.LocationFloat = new DevExpress.Utils.PointFloat(37.06524F, 560.7751F);
            this.xrLine5.Name = "xrLine5";
            this.xrLine5.SizeF = new System.Drawing.SizeF(656.9349F, 24.99994F);
            // 
            // xrLine4
            // 
            this.xrLine4.Dpi = 254F;
            this.xrLine4.LineStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            this.xrLine4.LocationFloat = new DevExpress.Utils.PointFloat(37.06524F, 290.9997F);
            this.xrLine4.Name = "xrLine4";
            this.xrLine4.SizeF = new System.Drawing.SizeF(666.674F, 24.99994F);
            // 
            // xrLine2
            // 
            this.xrLine2.Dpi = 254F;
            this.xrLine2.LineStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            this.xrLine2.LocationFloat = new DevExpress.Utils.PointFloat(37.06399F, 353.9995F);
            this.xrLine2.Name = "xrLine2";
            this.xrLine2.SizeF = new System.Drawing.SizeF(666.6763F, 25F);
            // 
            // xrLine1
            // 
            this.xrLine1.Dpi = 254F;
            this.xrLine1.LineStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(37.06524F, 0F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(656.9347F, 25F);
            // 
            // xrTotalDiscountLabel
            // 
            this.xrTotalDiscountLabel.CanShrink = true;
            this.xrTotalDiscountLabel.Dpi = 254F;
            this.xrTotalDiscountLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrTotalDiscountLabel.LocationFloat = new DevExpress.Utils.PointFloat(37.064F, 176.9999F);
            this.xrTotalDiscountLabel.Multiline = true;
            this.xrTotalDiscountLabel.Name = "xrTotalDiscountLabel";
            this.xrTotalDiscountLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrTotalDiscountLabel.SizeF = new System.Drawing.SizeF(264.791F, 37.99997F);
            this.xrTotalDiscountLabel.StylePriority.UseFont = false;
            this.xrTotalDiscountLabel.StylePriority.UseTextAlignment = false;
            this.xrTotalDiscountLabel.Text = "Total Discount: ";
            this.xrTotalDiscountLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrTotalDiscountValue
            // 
            this.xrTotalDiscountValue.CanShrink = true;
            this.xrTotalDiscountValue.Dpi = 254F;
            this.xrTotalDiscountValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTotalDiscountValue.LocationFloat = new DevExpress.Utils.PointFloat(322.2985F, 176.9999F);
            this.xrTotalDiscountValue.Name = "xrTotalDiscountValue";
            this.xrTotalDiscountValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrTotalDiscountValue.SizeF = new System.Drawing.SizeF(381.4416F, 38.00005F);
            this.xrTotalDiscountValue.StylePriority.UseFont = false;
            this.xrTotalDiscountValue.StylePriority.UseTextAlignment = false;
            this.xrTotalDiscountValue.Text = "Receipt Value";
            this.xrTotalDiscountValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrDiscountOnTotalLabel
            // 
            this.xrDiscountOnTotalLabel.CanShrink = true;
            this.xrDiscountOnTotalLabel.Dpi = 254F;
            this.xrDiscountOnTotalLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrDiscountOnTotalLabel.LocationFloat = new DevExpress.Utils.PointFloat(37.064F, 138.9999F);
            this.xrDiscountOnTotalLabel.Multiline = true;
            this.xrDiscountOnTotalLabel.Name = "xrDiscountOnTotalLabel";
            this.xrDiscountOnTotalLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrDiscountOnTotalLabel.SizeF = new System.Drawing.SizeF(389.1451F, 37.99997F);
            this.xrDiscountOnTotalLabel.StylePriority.UseFont = false;
            this.xrDiscountOnTotalLabel.StylePriority.UseTextAlignment = false;
            this.xrDiscountOnTotalLabel.Text = "Discount On Total Bill: ";
            this.xrDiscountOnTotalLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrDiscountOnTotalValue
            // 
            this.xrDiscountOnTotalValue.CanShrink = true;
            this.xrDiscountOnTotalValue.Dpi = 254F;
            this.xrDiscountOnTotalValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrDiscountOnTotalValue.LocationFloat = new DevExpress.Utils.PointFloat(437.0313F, 138.9999F);
            this.xrDiscountOnTotalValue.Name = "xrDiscountOnTotalValue";
            this.xrDiscountOnTotalValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrDiscountOnTotalValue.SizeF = new System.Drawing.SizeF(266.7086F, 38.00005F);
            this.xrDiscountOnTotalValue.StylePriority.UseFont = false;
            this.xrDiscountOnTotalValue.StylePriority.UseTextAlignment = false;
            this.xrDiscountOnTotalValue.Text = "Receipt Value";
            this.xrDiscountOnTotalValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrDiscountOnItemsValue
            // 
            this.xrDiscountOnItemsValue.CanShrink = true;
            this.xrDiscountOnItemsValue.Dpi = 254F;
            this.xrDiscountOnItemsValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrDiscountOnItemsValue.LocationFloat = new DevExpress.Utils.PointFloat(362.8184F, 101F);
            this.xrDiscountOnItemsValue.Name = "xrDiscountOnItemsValue";
            this.xrDiscountOnItemsValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrDiscountOnItemsValue.SizeF = new System.Drawing.SizeF(340.9211F, 38.00002F);
            this.xrDiscountOnItemsValue.StylePriority.UseFont = false;
            this.xrDiscountOnItemsValue.StylePriority.UseTextAlignment = false;
            this.xrDiscountOnItemsValue.Text = "Receipt Value";
            this.xrDiscountOnItemsValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrDiscountOnItemsLabel
            // 
            this.xrDiscountOnItemsLabel.CanShrink = true;
            this.xrDiscountOnItemsLabel.Dpi = 254F;
            this.xrDiscountOnItemsLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrDiscountOnItemsLabel.LocationFloat = new DevExpress.Utils.PointFloat(37.064F, 101F);
            this.xrDiscountOnItemsLabel.Multiline = true;
            this.xrDiscountOnItemsLabel.Name = "xrDiscountOnItemsLabel";
            this.xrDiscountOnItemsLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrDiscountOnItemsLabel.SizeF = new System.Drawing.SizeF(325.7552F, 37.99997F);
            this.xrDiscountOnItemsLabel.StylePriority.UseFont = false;
            this.xrDiscountOnItemsLabel.StylePriority.UseTextAlignment = false;
            this.xrDiscountOnItemsLabel.Text = "Discount On Items: ";
            this.xrDiscountOnItemsLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrApproval
            // 
            this.xrApproval.CanShrink = true;
            this.xrApproval.Dpi = 254F;
            this.xrApproval.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrApproval.LocationFloat = new DevExpress.Utils.PointFloat(37.06395F, 456.7603F);
            this.xrApproval.Multiline = true;
            this.xrApproval.Name = "xrApproval";
            this.xrApproval.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrApproval.SizeF = new System.Drawing.SizeF(666.6761F, 37.99991F);
            this.xrApproval.StylePriority.UseFont = false;
            this.xrApproval.StylePriority.UseTextAlignment = false;
            this.xrApproval.Text = "Approved by:";
            this.xrApproval.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrTotalPayableValue
            // 
            this.xrTotalPayableValue.Dpi = 254F;
            this.xrTotalPayableValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTotalPayableValue.LocationFloat = new DevExpress.Utils.PointFloat(296.2914F, 252.9999F);
            this.xrTotalPayableValue.Name = "xrTotalPayableValue";
            this.xrTotalPayableValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrTotalPayableValue.SizeF = new System.Drawing.SizeF(407.4485F, 38.00002F);
            this.xrTotalPayableValue.StylePriority.UseFont = false;
            this.xrTotalPayableValue.StylePriority.UseTextAlignment = false;
            this.xrTotalPayableValue.Text = "Receipt Value";
            this.xrTotalPayableValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrRoundingValue
            // 
            this.xrRoundingValue.CanShrink = true;
            this.xrRoundingValue.Dpi = 254F;
            this.xrRoundingValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrRoundingValue.LocationFloat = new DevExpress.Utils.PointFloat(296.2914F, 214.9998F);
            this.xrRoundingValue.Name = "xrRoundingValue";
            this.xrRoundingValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrRoundingValue.SizeF = new System.Drawing.SizeF(407.4485F, 37.99998F);
            this.xrRoundingValue.StylePriority.UseFont = false;
            this.xrRoundingValue.StylePriority.UseTextAlignment = false;
            this.xrRoundingValue.Text = "Receipt Value";
            this.xrRoundingValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrTotalBillValue
            // 
            this.xrTotalBillValue.Dpi = 254F;
            this.xrTotalBillValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTotalBillValue.LocationFloat = new DevExpress.Utils.PointFloat(484.1456F, 63.00003F);
            this.xrTotalBillValue.Name = "xrTotalBillValue";
            this.xrTotalBillValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrTotalBillValue.SizeF = new System.Drawing.SizeF(219.5942F, 38.00002F);
            this.xrTotalBillValue.StylePriority.UseFont = false;
            this.xrTotalBillValue.StylePriority.UseTextAlignment = false;
            this.xrTotalBillValue.Text = "Receipt Value";
            this.xrTotalBillValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrTotalQtyValue
            // 
            this.xrTotalQtyValue.Dpi = 254F;
            this.xrTotalQtyValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTotalQtyValue.LocationFloat = new DevExpress.Utils.PointFloat(403.496F, 25F);
            this.xrTotalQtyValue.Name = "xrTotalQtyValue";
            this.xrTotalQtyValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrTotalQtyValue.SizeF = new System.Drawing.SizeF(300.2433F, 38.00001F);
            this.xrTotalQtyValue.StylePriority.UseFont = false;
            this.xrTotalQtyValue.StylePriority.UseTextAlignment = false;
            this.xrTotalQtyValue.Text = "Receipt Value";
            this.xrTotalQtyValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrTotalPayable
            // 
            this.xrTotalPayable.Dpi = 254F;
            this.xrTotalPayable.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrTotalPayable.LocationFloat = new DevExpress.Utils.PointFloat(37.064F, 252.9999F);
            this.xrTotalPayable.Multiline = true;
            this.xrTotalPayable.Name = "xrTotalPayable";
            this.xrTotalPayable.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrTotalPayable.SizeF = new System.Drawing.SizeF(248.7694F, 37.99997F);
            this.xrTotalPayable.StylePriority.UseFont = false;
            this.xrTotalPayable.StylePriority.UseTextAlignment = false;
            this.xrTotalPayable.Text = "Total Payable :";
            this.xrTotalPayable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrRounding
            // 
            this.xrRounding.CanShrink = true;
            this.xrRounding.Dpi = 254F;
            this.xrRounding.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrRounding.LocationFloat = new DevExpress.Utils.PointFloat(37.064F, 214.9999F);
            this.xrRounding.Multiline = true;
            this.xrRounding.Name = "xrRounding";
            this.xrRounding.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrRounding.SizeF = new System.Drawing.SizeF(248.7694F, 37.99997F);
            this.xrRounding.StylePriority.UseFont = false;
            this.xrRounding.StylePriority.UseTextAlignment = false;
            this.xrRounding.Text = "Rounding:";
            this.xrRounding.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrTotalBill
            // 
            this.xrTotalBill.Dpi = 254F;
            this.xrTotalBill.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrTotalBill.LocationFloat = new DevExpress.Utils.PointFloat(37.06399F, 63.00003F);
            this.xrTotalBill.Multiline = true;
            this.xrTotalBill.Name = "xrTotalBill";
            this.xrTotalBill.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrTotalBill.SizeF = new System.Drawing.SizeF(447.0816F, 37.99996F);
            this.xrTotalBill.StylePriority.UseFont = false;
            this.xrTotalBill.StylePriority.UseTextAlignment = false;
            this.xrTotalBill.Text = "Total Bill Before Discount :";
            this.xrTotalBill.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrTotalQty
            // 
            this.xrTotalQty.Dpi = 254F;
            this.xrTotalQty.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrTotalQty.LocationFloat = new DevExpress.Utils.PointFloat(37.064F, 25F);
            this.xrTotalQty.Multiline = true;
            this.xrTotalQty.Name = "xrTotalQty";
            this.xrTotalQty.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrTotalQty.SizeF = new System.Drawing.SizeF(349.311F, 37.99997F);
            this.xrTotalQty.StylePriority.UseFont = false;
            this.xrTotalQty.StylePriority.UseTextAlignment = false;
            this.xrTotalQty.Text = "Total Quantity :";
            this.xrTotalQty.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrChangeValue
            // 
            this.xrChangeValue.Dpi = 254F;
            this.xrChangeValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrChangeValue.LocationFloat = new DevExpress.Utils.PointFloat(296.2923F, 418.76F);
            this.xrChangeValue.Name = "xrChangeValue";
            this.xrChangeValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrChangeValue.SizeF = new System.Drawing.SizeF(407.4471F, 38.00003F);
            this.xrChangeValue.StylePriority.UseFont = false;
            this.xrChangeValue.StylePriority.UseTextAlignment = false;
            this.xrChangeValue.Text = "Change Value";
            this.xrChangeValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrReceiptValue
            // 
            this.xrReceiptValue.Dpi = 254F;
            this.xrReceiptValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrReceiptValue.LocationFloat = new DevExpress.Utils.PointFloat(296.2923F, 380.7604F);
            this.xrReceiptValue.Name = "xrReceiptValue";
            this.xrReceiptValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrReceiptValue.SizeF = new System.Drawing.SizeF(407.4476F, 38F);
            this.xrReceiptValue.StylePriority.UseFont = false;
            this.xrReceiptValue.StylePriority.UseTextAlignment = false;
            this.xrReceiptValue.Text = "Receipt Value";
            this.xrReceiptValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrContact
            // 
            this.xrContact.Dpi = 254F;
            this.xrContact.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrContact.LocationFloat = new DevExpress.Utils.PointFloat(37.06395F, 494.7603F);
            this.xrContact.Multiline = true;
            this.xrContact.Name = "xrContact";
            this.xrContact.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrContact.SizeF = new System.Drawing.SizeF(666.6764F, 66.01474F);
            this.xrContact.StylePriority.UseFont = false;
            this.xrContact.StylePriority.UseTextAlignment = false;
            this.xrContact.Text = "Contact";
            this.xrContact.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel3
            // 
            this.xrLabel3.CanShrink = true;
            this.xrLabel3.Dpi = 254F;
            this.xrLabel3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(37.064F, 418.7602F);
            this.xrLabel3.Multiline = true;
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(248.7694F, 37.99997F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.Text = "Change :";
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel1
            // 
            this.xrLabel1.Dpi = 254F;
            this.xrLabel1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(37.064F, 380.7604F);
            this.xrLabel1.Multiline = true;
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(248.7694F, 37.99997F);
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.Text = "Total Receipt :";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrPaymentValue
            // 
            this.xrPaymentValue.Dpi = 254F;
            this.xrPaymentValue.Font = new System.Drawing.Font("Times New Roman", 9.75F);
            this.xrPaymentValue.LocationFloat = new DevExpress.Utils.PointFloat(25.00001F, 315.9996F);
            this.xrPaymentValue.Name = "xrPaymentValue";
            this.xrPaymentValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrPaymentValue.SerializableRtfString = resources.GetString("xrPaymentValue.SerializableRtfString");
            this.xrPaymentValue.SizeF = new System.Drawing.SizeF(690.0798F, 37.99991F);
            // 
            // PrintReceipt
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.ReportHeader,
            this.ReportFooter});
            this.Dpi = 254F;
            this.Margins = new System.Drawing.Printing.Margins(0, 1, 0, 4);
            this.PageHeight = 32969;
            this.PageWidth = 720;
            this.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.ReportUnit = DevExpress.XtraReports.UI.ReportUnit.TenthsOfAMillimeter;
            this.RollPaper = true;
            this.ShowPrintMarginsWarning = false;
            this.ShowPrintStatusDialog = false;
            this.SnapGridSize = 25F;
            this.Version = "21.2";
            this.AfterPrint += new System.EventHandler(this.PrintReceipt_AfterPrint);
            ((System.ComponentModel.ISupportInitialize)(this.xrDetailItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrPaymentValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.XRLabel xrCashierValue;
        private DevExpress.XtraReports.UI.XRLabel xrPointValue;
        private DevExpress.XtraReports.UI.XRLabel xrMemValue;
        private DevExpress.XtraReports.UI.XRLabel xrCardValue;
        private DevExpress.XtraReports.UI.XRLabel xrDateValue;
        private DevExpress.XtraReports.UI.XRLabel xrCashier;
        private DevExpress.XtraReports.UI.XRLabel xrPointBlc;
        private DevExpress.XtraReports.UI.XRLabel xrMemName;
        private DevExpress.XtraReports.UI.XRLabel xrMemCard;
        private DevExpress.XtraReports.UI.XRLabel xrDate;
        private DevExpress.XtraReports.UI.XRLabel xrStoreName;
        private DevExpress.XtraReports.UI.XRBarCode xrBarCode;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRLabel xrContact;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRLabel xrTotalPayableValue;
        private DevExpress.XtraReports.UI.XRLabel xrRoundingValue;
        private DevExpress.XtraReports.UI.XRLabel xrTotalBillValue;
        private DevExpress.XtraReports.UI.XRLabel xrTotalQtyValue;
        private DevExpress.XtraReports.UI.XRLabel xrTotalPayable;
        private DevExpress.XtraReports.UI.XRLabel xrRounding;
        private DevExpress.XtraReports.UI.XRLabel xrTotalBill;
        private DevExpress.XtraReports.UI.XRLabel xrTotalQty;
        private DevExpress.XtraReports.UI.XRLabel xrChangeValue;
        private DevExpress.XtraReports.UI.XRLabel xrReceiptValue;
        private DevExpress.XtraReports.UI.XRLabel xrApproval;
        private DevExpress.XtraReports.UI.XRLabel xrCustomerLabel;
        private DevExpress.XtraReports.UI.XRLabel xrCustomerValue;
        private DevExpress.XtraReports.UI.XRLabel xrOriginalTransId;
        private DevExpress.XtraReports.UI.XRLabel xrOrginalValue;
        private DevExpress.XtraReports.UI.XRLabel xrTotalDiscountLabel;
        private DevExpress.XtraReports.UI.XRLabel xrTotalDiscountValue;
        private DevExpress.XtraReports.UI.XRLabel xrDiscountOnTotalLabel;
        private DevExpress.XtraReports.UI.XRLabel xrDiscountOnTotalValue;
        private DevExpress.XtraReports.UI.XRLabel xrDiscountOnItemsValue;
        private DevExpress.XtraReports.UI.XRLabel xrDiscountOnItemsLabel;
        private DevExpress.XtraReports.UI.XRLine xrLine2;
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        private DevExpress.XtraReports.UI.XRLine xrLine3;
        private DevExpress.XtraReports.UI.XRLine xrLine4;
        private DevExpress.XtraReports.UI.XRLine xrLine5;
        private DevExpress.XtraReports.UI.XRLabel xrSaleType;
        private DevExpress.XtraReports.UI.XRLabel xrBaseTransId;
        private DevExpress.XtraReports.UI.XRLabel xrItemNameEpay;
        private DevExpress.XtraReports.UI.XRLabel xrSerialNumEpayValue;
        private DevExpress.XtraReports.UI.XRLabel xrSerialNumEpay;
        private DevExpress.XtraReports.UI.XRLabel xrCashierEpayValue;
        private DevExpress.XtraReports.UI.XRLabel xrTransactionIdEpayValue;
        private DevExpress.XtraReports.UI.XRLabel xrExpDateEpay;
        private DevExpress.XtraReports.UI.XRLabel xrTransactionIdEpay;
        private DevExpress.XtraReports.UI.XRRichText xrDetailItem;
        private DevExpress.XtraReports.UI.XRLabel xrTerminalIdEpayValue;
        private DevExpress.XtraReports.UI.XRLabel xrTerminalIdEpay;
        private DevExpress.XtraReports.UI.XRLabel xrCashierEpay;
        private DevExpress.XtraReports.UI.XRLabel xrExpDateEpayValue;
        private DevExpress.XtraReports.UI.XRRichText xrPaymentValue;
    }
}
