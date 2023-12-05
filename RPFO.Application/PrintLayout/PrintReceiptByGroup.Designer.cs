namespace RPFO.Application.PrintLayout
{
    partial class PrintReceiptByGroup
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
            DevExpress.XtraPrinting.BarCode.Code128Generator code128Generator1 = new DevExpress.XtraPrinting.BarCode.Code128Generator();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintReceiptByGroup));
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.xrStoreName = new DevExpress.XtraReports.UI.XRLabel();
            this.xrSaleType = new DevExpress.XtraReports.UI.XRLabel();
            this.xrBarCode = new DevExpress.XtraReports.UI.XRBarCode();
            this.xrDateValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrDate = new DevExpress.XtraReports.UI.XRLabel();
            this.xrOriginalTransId = new DevExpress.XtraReports.UI.XRLabel();
            this.xrOrginalValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrCustomerLabel = new DevExpress.XtraReports.UI.XRLabel();
            this.xrCustomerValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrCashierValue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrCashier = new DevExpress.XtraReports.UI.XRLabel();
            this.xrDetailItem = new DevExpress.XtraReports.UI.XRRichText();
            this.xrTotalQty = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTotalQtyValue = new DevExpress.XtraReports.UI.XRLabel();
            ((System.ComponentModel.ISupportInitialize)(this.xrDetailItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrDetailItem});
            this.Detail.Dpi = 254F;
            this.Detail.HeightF = 61.29655F;
            this.Detail.HierarchyPrintOptions.Indent = 50.8F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // TopMargin
            // 
            this.TopMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTotalQtyValue,
            this.xrTotalQty,
            this.xrStoreName,
            this.xrSaleType,
            this.xrBarCode,
            this.xrDateValue,
            this.xrDate,
            this.xrOriginalTransId,
            this.xrOrginalValue,
            this.xrCustomerLabel,
            this.xrCustomerValue,
            this.xrCashierValue,
            this.xrCashier});
            this.TopMargin.Dpi = 254F;
            this.TopMargin.HeightF = 471.2941F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.Dpi = 254F;
            this.BottomMargin.HeightF = 78.55396F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrStoreName
            // 
            this.xrStoreName.CanGrow = false;
            this.xrStoreName.Dpi = 254F;
            this.xrStoreName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrStoreName.LocationFloat = new DevExpress.Utils.PointFloat(0.7246284F, 0F);
            this.xrStoreName.Multiline = true;
            this.xrStoreName.Name = "xrStoreName";
            this.xrStoreName.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrStoreName.SizeF = new System.Drawing.SizeF(678.0158F, 58.42F);
            this.xrStoreName.StylePriority.UseFont = false;
            this.xrStoreName.StylePriority.UseTextAlignment = false;
            this.xrStoreName.Text = "xrStoreName";
            this.xrStoreName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrSaleType
            // 
            this.xrSaleType.Dpi = 254F;
            this.xrSaleType.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrSaleType.LocationFloat = new DevExpress.Utils.PointFloat(0.7245789F, 58.42001F);
            this.xrSaleType.Multiline = true;
            this.xrSaleType.Name = "xrSaleType";
            this.xrSaleType.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrSaleType.SizeF = new System.Drawing.SizeF(681.9365F, 58.41998F);
            this.xrSaleType.StylePriority.UseFont = false;
            this.xrSaleType.StylePriority.UseTextAlignment = false;
            this.xrSaleType.Text = "xrSaleType";
            this.xrSaleType.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrBarCode
            // 
            this.xrBarCode.AutoModule = true;
            this.xrBarCode.Dpi = 254F;
            this.xrBarCode.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrBarCode.LocationFloat = new DevExpress.Utils.PointFloat(0F, 130.0691F);
            this.xrBarCode.Module = 5.08F;
            this.xrBarCode.Name = "xrBarCode";
            this.xrBarCode.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
            this.xrBarCode.SizeF = new System.Drawing.SizeF(678.0158F, 136.2597F);
            this.xrBarCode.StylePriority.UseFont = false;
            this.xrBarCode.StylePriority.UsePadding = false;
            this.xrBarCode.Symbology = code128Generator1;
            this.xrBarCode.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
            // 
            // xrDateValue
            // 
            this.xrDateValue.Dpi = 254F;
            this.xrDateValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrDateValue.LocationFloat = new DevExpress.Utils.PointFloat(95.97623F, 279.9919F);
            this.xrDateValue.Name = "xrDateValue";
            this.xrDateValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrDateValue.SizeF = new System.Drawing.SizeF(582.7639F, 38.00006F);
            this.xrDateValue.StylePriority.UseFont = false;
            this.xrDateValue.StylePriority.UseTextAlignment = false;
            this.xrDateValue.Text = "Date Value";
            this.xrDateValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrDate
            // 
            this.xrDate.Dpi = 254F;
            this.xrDate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrDate.LocationFloat = new DevExpress.Utils.PointFloat(0.7246284F, 279.992F);
            this.xrDate.Multiline = true;
            this.xrDate.Name = "xrDate";
            this.xrDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrDate.SizeF = new System.Drawing.SizeF(95.25177F, 38F);
            this.xrDate.StylePriority.UseFont = false;
            this.xrDate.StylePriority.UseTextAlignment = false;
            this.xrDate.Text = "Date:";
            this.xrDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrOriginalTransId
            // 
            this.xrOriginalTransId.CanShrink = true;
            this.xrOriginalTransId.Dpi = 254F;
            this.xrOriginalTransId.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrOriginalTransId.LocationFloat = new DevExpress.Utils.PointFloat(0.7246337F, 317.9919F);
            this.xrOriginalTransId.Multiline = true;
            this.xrOriginalTransId.Name = "xrOriginalTransId";
            this.xrOriginalTransId.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrOriginalTransId.SizeF = new System.Drawing.SizeF(105.374F, 38F);
            this.xrOriginalTransId.StylePriority.UseFont = false;
            this.xrOriginalTransId.StylePriority.UseTextAlignment = false;
            this.xrOriginalTransId.Text = "Table: ";
            this.xrOriginalTransId.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrOrginalValue
            // 
            this.xrOrginalValue.CanShrink = true;
            this.xrOrginalValue.Dpi = 254F;
            this.xrOrginalValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrOrginalValue.LocationFloat = new DevExpress.Utils.PointFloat(106.0986F, 317.992F);
            this.xrOrginalValue.Name = "xrOrginalValue";
            this.xrOrginalValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrOrginalValue.SizeF = new System.Drawing.SizeF(572.6415F, 38.00003F);
            this.xrOrginalValue.StylePriority.UseFont = false;
            this.xrOrginalValue.StylePriority.UseTextAlignment = false;
            this.xrOrginalValue.Text = "Original Value";
            this.xrOrginalValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrCustomerLabel
            // 
            this.xrCustomerLabel.CanShrink = true;
            this.xrCustomerLabel.Dpi = 254F;
            this.xrCustomerLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrCustomerLabel.LocationFloat = new DevExpress.Utils.PointFloat(0.7246284F, 355.9919F);
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
            this.xrCustomerValue.LocationFloat = new DevExpress.Utils.PointFloat(170.3248F, 355.9919F);
            this.xrCustomerValue.Name = "xrCustomerValue";
            this.xrCustomerValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrCustomerValue.SizeF = new System.Drawing.SizeF(508.4155F, 38.00006F);
            this.xrCustomerValue.StylePriority.UseFont = false;
            this.xrCustomerValue.StylePriority.UseTextAlignment = false;
            this.xrCustomerValue.Text = "Customer Value";
            this.xrCustomerValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrCashierValue
            // 
            this.xrCashierValue.Dpi = 254F;
            this.xrCashierValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrCashierValue.LocationFloat = new DevExpress.Utils.PointFloat(139.3028F, 393.9919F);
            this.xrCashierValue.Name = "xrCashierValue";
            this.xrCashierValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrCashierValue.SizeF = new System.Drawing.SizeF(539.4376F, 38.00003F);
            this.xrCashierValue.StylePriority.UseFont = false;
            this.xrCashierValue.StylePriority.UseTextAlignment = false;
            this.xrCashierValue.Text = "Cahier Value";
            this.xrCashierValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrCashier
            // 
            this.xrCashier.Dpi = 254F;
            this.xrCashier.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrCashier.LocationFloat = new DevExpress.Utils.PointFloat(0.7245601F, 393.9919F);
            this.xrCashier.Multiline = true;
            this.xrCashier.Name = "xrCashier";
            this.xrCashier.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrCashier.SizeF = new System.Drawing.SizeF(138.5782F, 37.99997F);
            this.xrCashier.StylePriority.UseFont = false;
            this.xrCashier.StylePriority.UseTextAlignment = false;
            this.xrCashier.Text = "Cashier:";
            this.xrCashier.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrDetailItem
            // 
            this.xrDetailItem.Dpi = 254F;
            this.xrDetailItem.Font = new System.Drawing.Font("Times New Roman", 9.75F);
            this.xrDetailItem.LocationFloat = new DevExpress.Utils.PointFloat(1.644785F, 0F);
            this.xrDetailItem.Name = "xrDetailItem";
            this.xrDetailItem.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrDetailItem.SerializableRtfString = resources.GetString("xrDetailItem.SerializableRtfString");
            this.xrDetailItem.SizeF = new System.Drawing.SizeF(689.3552F, 58.42F);
            // 
            // xrTotalQty
            // 
            this.xrTotalQty.Dpi = 254F;
            this.xrTotalQty.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrTotalQty.LocationFloat = new DevExpress.Utils.PointFloat(1.644788F, 431.9919F);
            this.xrTotalQty.Multiline = true;
            this.xrTotalQty.Name = "xrTotalQty";
            this.xrTotalQty.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrTotalQty.SizeF = new System.Drawing.SizeF(349.311F, 37.99997F);
            this.xrTotalQty.StylePriority.UseFont = false;
            this.xrTotalQty.StylePriority.UseTextAlignment = false;
            this.xrTotalQty.Text = "Total Quantity :";
            this.xrTotalQty.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrTotalQtyValue
            // 
            this.xrTotalQtyValue.Dpi = 254F;
            this.xrTotalQtyValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTotalQtyValue.LocationFloat = new DevExpress.Utils.PointFloat(368.0768F, 431.9919F);
            this.xrTotalQtyValue.Name = "xrTotalQtyValue";
            this.xrTotalQtyValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrTotalQtyValue.SizeF = new System.Drawing.SizeF(309.9391F, 38F);
            this.xrTotalQtyValue.StylePriority.UseFont = false;
            this.xrTotalQtyValue.StylePriority.UseTextAlignment = false;
            this.xrTotalQtyValue.Text = "Receipt Value";
            this.xrTotalQtyValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // PrintReceiptByGroup
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.Dpi = 254F;
            this.Margins = new System.Drawing.Printing.Margins(0, 5, 471, 79);
            this.PageHeight = 2794;
            this.PageWidth = 696;
            this.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.ReportUnit = DevExpress.XtraReports.UI.ReportUnit.TenthsOfAMillimeter;
            this.SnapGridSize = 25F;
            this.Version = "21.2";
            ((System.ComponentModel.ISupportInitialize)(this.xrDetailItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRLabel xrStoreName;
        private DevExpress.XtraReports.UI.XRLabel xrSaleType;
        private DevExpress.XtraReports.UI.XRBarCode xrBarCode;
        private DevExpress.XtraReports.UI.XRLabel xrDateValue;
        private DevExpress.XtraReports.UI.XRLabel xrDate;
        private DevExpress.XtraReports.UI.XRLabel xrOriginalTransId;
        private DevExpress.XtraReports.UI.XRLabel xrOrginalValue;
        private DevExpress.XtraReports.UI.XRLabel xrCustomerLabel;
        private DevExpress.XtraReports.UI.XRLabel xrCustomerValue;
        private DevExpress.XtraReports.UI.XRLabel xrCashierValue;
        private DevExpress.XtraReports.UI.XRLabel xrCashier;
        private DevExpress.XtraReports.UI.XRRichText xrDetailItem;
        private DevExpress.XtraReports.UI.XRLabel xrTotalQtyValue;
        private DevExpress.XtraReports.UI.XRLabel xrTotalQty;
    }
}
