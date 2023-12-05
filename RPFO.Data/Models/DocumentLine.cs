using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.Models
{
    public partial class DocumentLine
    {
        [JsonProperty("LineNum")] 
        public Int32? LineNum { get; set; }
        [JsonProperty("ItemCode")]
        public String ItemCode { get; set; } //Y
        [JsonProperty("ItemDescription")]
        public String ItemDescription { get; set; }
        [JsonProperty("ItemGroup")]
        public String ItemGroup { get; set; } //Y
        [JsonProperty("Quantity")]
        public Double? Quantity { get; set; } //Y
        //[JsonProperty("ShipDate")]
        //public DateTime? ShipDate { get; set; }
        //[JsonProperty("Price")]
        //public Double? Price { get; set; }
        //[JsonProperty("PriceAfterVAT")]
        //public Double? PriceAfterVAT { get; set; }
        [JsonProperty("Currency")]
        public String Currency { get; set; } = "MYR"; //Y
        [JsonProperty("Rate")]
        public Double? Rate { get; set; } = 1;
        [JsonProperty("DiscountPercent")]
        public Double? DiscountPercent { get; set; }
        //[JsonProperty("VendorNum")]
        //public String VendorNum { get; set; }
        //[JsonProperty("SerialNum")]
        //public String SerialNum { get; set; }
        //[JsonProperty("WarehouseCode")]
        //public String WarehouseCode { get; set; }
        //[JsonProperty("SalesPersonCode")]
        //public Int32? SalesPersonCode { get; set; }
        //[JsonProperty("CommisionPercent")]
        //public Double? CommisionPercent { get; set; }
        //[JsonProperty("TreeType")]
        //public String TreeType { get; set; }
        //[JsonProperty("AccountCode")]
        //public String AccountCode { get; set; }
        //[JsonProperty("UseBaseUnits")]
        //public String UseBaseUnits { get; set; }
        //[JsonProperty("SupplierCatNum")]
        //public String SupplierCatNum { get; set; }
        //[JsonProperty("CostingCode")]
        //public String CostingCode { get; set; }
        //[JsonProperty("ProjectCode")]
        //public String ProjectCode { get; set; }
        [JsonProperty("BarCode")]
        public String BarCode { get; set; }
        [JsonProperty("VatGroup")]
        public String VatGroup { get; set; } //Y
        //[JsonProperty("Height1")]
        //public Double? Height1 { get; set; }
        //[JsonProperty("Hight1Unit")]
        //public Int32? Hight1Unit { get; set; }
        //[JsonProperty("Height2")]
        //public Double? Height2 { get; set; }
        //[JsonProperty("Height2Unit")]
        //public Int32? Height2Unit { get; set; }
        //[JsonProperty("Lengh1")]
        //public Double? Lengh1 { get; set; }
        //[JsonProperty("Lengh1Unit")]
        //public Int32? Lengh1Unit { get; set; }
        //[JsonProperty("Lengh2")]
        //public Double? Lengh2 { get; set; }
        //[JsonProperty("Lengh2Unit")]
        //public Int32? Lengh2Unit { get; set; }
        //[JsonProperty("Weight1")]
        //public Double? Weight1 { get; set; }
        //[JsonProperty("Weight1Unit")]
        //public Int32? Weight1Unit { get; set; }
        //[JsonProperty("Weight2")]
        //public Double? Weight2 { get; set; }
        //[JsonProperty("Weight2Unit")]
        //public Int32? Weight2Unit { get; set; }
        //[JsonProperty("Factor1")]
        //public Double? Factor1 { get; set; }
        //[JsonProperty("Factor2")]
        //public Double? Factor2 { get; set; }
        //[JsonProperty("Factor3")]
        //public Double? Factor3 { get; set; }
        //[JsonProperty("Factor4")]
        //public Double? Factor4 { get; set; }
        //[JsonProperty("BaseType")]
        //public Int32? BaseType { get; set; }
        //[JsonProperty("BaseEntry")]
        //public Int32? BaseEntry { get; set; }
        //[JsonProperty("BaseLine")]
        //public Int32? BaseLine { get; set; }
        //[JsonProperty("Volume")]
        //public Double? Volume { get; set; }
        //[JsonProperty("VolumeUnit")]
        //public Int32? VolumeUnit { get; set; }
        //[JsonProperty("Width1")]
        //public Double? Width1 { get; set; }
        //[JsonProperty("Width1Unit")]
        //public Int32? Width1Unit { get; set; }
        //[JsonProperty("Width2")]
        //public Double? Width2 { get; set; }
        //[JsonProperty("Width2Unit")]
        //public Int32? Width2Unit { get; set; }
        //[JsonProperty("Address")]
        //public String Address { get; set; }
        //[JsonProperty("TaxCode")]
        //public String TaxCode { get; set; }
        //[JsonProperty("TaxType")]
        //public String TaxType { get; set; }
        //[JsonProperty("TaxLiable")]
        //public String TaxLiable { get; set; }
        //[JsonProperty("PickStatus")]
        //public String PickStatus { get; set; }
        //[JsonProperty("PickQuantity")]
        //public Double? PickQuantity { get; set; }
        //[JsonProperty("PickListIdNumber")]
        //public Int32? PickListIdNumber { get; set; }
        //[JsonProperty("OriginalItem")]
        //public String OriginalItem { get; set; }
        //[JsonProperty("BackOrder")]
        //public String BackOrder { get; set; }
        //[JsonProperty("FreeText")]
        //public String FreeText { get; set; }
        //[JsonProperty("ShippingMethod")]
        //public Int32? ShippingMethod { get; set; }
        //[JsonProperty("POTargetNum")]
        //public Int32? POTargetNum { get; set; }
        //[JsonProperty("POTargetEntry")]
        //public String POTargetEntry { get; set; }
        //[JsonProperty("POTargetRowNum")]
        //public Int32? POTargetRowNum { get; set; }
        //[JsonProperty("CorrectionInvoiceItem")]
        //public String CorrectionInvoiceItem { get; set; }
        //[JsonProperty("CorrInvAmountToStock")]
        //public Double? CorrInvAmountToStock { get; set; }
        //[JsonProperty("CorrInvAmountToDiffAcct")]
        //public Double? CorrInvAmountToDiffAcct { get; set; }
        //[JsonProperty("AppliedTax")]
        //public Double? AppliedTax { get; set; }
        //[JsonProperty("AppliedTaxFC")]
        //public Double? AppliedTaxFC { get; set; }
        //[JsonProperty("AppliedTaxSC")]
        //public Double? AppliedTaxSC { get; set; }
        //[JsonProperty("WTLiable")]
        //public String WTLiable { get; set; }
        //[JsonProperty("DeferredTax")]
        //public String DeferredTax { get; set; }
        //[JsonProperty("EqualizationTaxPercent")]
        //public Double? EqualizationTaxPercent { get; set; }
        //[JsonProperty("TotalEqualizationTax")]
        //public Double? TotalEqualizationTax { get; set; }
        //[JsonProperty("TotalEqualizationTaxFC")]
        //public Double? TotalEqualizationTaxFC { get; set; }
        //[JsonProperty("TotalEqualizationTaxSC")]
        //public Double? TotalEqualizationTaxSC { get; set; }
        //[JsonProperty("NetTaxAmount")]
        //public Double? NetTaxAmount { get; set; }
        //[JsonProperty("NetTaxAmountFC")]
        //public Double? NetTaxAmountFC { get; set; }
        //[JsonProperty("NetTaxAmountSC")]
        //public Double? NetTaxAmountSC { get; set; }
        //[JsonProperty("MeasureUnit")]
        //public String MeasureUnit { get; set; }
        //[JsonProperty("UnitsOfMeasurment")]
        //public Double? UnitsOfMeasurment { get; set; }
        [JsonProperty("LineTotal")]
        public Double? LineTotal { get; set; } //Y
        [JsonProperty("TaxPercentagePerRow")]
        public Double? TaxPercentagePerRow { get; set; }
        //[JsonProperty("TaxTotal")]
        //public Double? TaxTotal { get; set; }
        //[JsonProperty("ConsumerSalesForecast")]
        //public String ConsumerSalesForecast { get; set; }
        //[JsonProperty("ExciseAmount")]
        //public Double? ExciseAmount { get; set; }
        //[JsonProperty("TaxPerUnit")]
        //public Double? TaxPerUnit { get; set; }
        //[JsonProperty("TotalInclTax")]
        //public Double? TotalInclTax { get; set; }
        //[JsonProperty("CountryOrg")]
        //public String CountryOrg { get; set; }
        //[JsonProperty("SWW")]
        //public String SWW { get; set; }
        //[JsonProperty("TransactionType")]
        //public String TransactionType { get; set; }
        //[JsonProperty("DistributeExpense")]
        //public String DistributeExpense { get; set; }
        //[JsonProperty("RowTotalFC")]
        //public Double? RowTotalFC { get; set; }
        //[JsonProperty("RowTotalSC")]
        //public Double? RowTotalSC { get; set; }
        //[JsonProperty("LastBuyInmPrice")]
        //public Double? LastBuyInmPrice { get; set; }
        //[JsonProperty("LastBuyDistributeSumFc")]
        //public Double? LastBuyDistributeSumFc { get; set; }
        //[JsonProperty("LastBuyDistributeSumSc")]
        //public Double? LastBuyDistributeSumSc { get; set; }
        //[JsonProperty("LastBuyDistributeSum")]
        //public Double? LastBuyDistributeSum { get; set; }
        //[JsonProperty("StockDistributesumForeign")]
        //public Double? StockDistributesumForeign { get; set; }
        //[JsonProperty("StockDistributesumSystem")]
        //public Double? StockDistributesumSystem { get; set; }
        //[JsonProperty("StockDistributesum")]
        //public Double? StockDistributesum { get; set; }
        //[JsonProperty("StockInmPrice")]
        //public Double? StockInmPrice { get; set; }
        //[JsonProperty("PickStatusEx")]
        //public String PickStatusEx { get; set; }
        //[JsonProperty("TaxBeforeDPM")]
        //public Double? TaxBeforeDPM { get; set; }
        //[JsonProperty("TaxBeforeDPMFC")]
        //public Double? TaxBeforeDPMFC { get; set; }
        //[JsonProperty("TaxBeforeDPMSC")]
        //public Double? TaxBeforeDPMSC { get; set; }
        //[JsonProperty("CFOPCode")]
        //public String CFOPCode { get; set; }
        //[JsonProperty("CSTCode")]
        //public String CSTCode { get; set; }
        //[JsonProperty("Usage")]
        //public Int32? Usage { get; set; }
        //[JsonProperty("TaxOnly")]
        //public String TaxOnly { get; set; }
        //[JsonProperty("VisualOrder")]
        //public Int32? VisualOrder { get; set; }
        //[JsonProperty("BaseOpenQuantity")]
        //public Double? BaseOpenQuantity { get; set; }
        [JsonProperty("UnitPrice")]
        public Double? UnitPrice { get; set; }  //Y
        //[JsonProperty("LineStatus")]
        //public BoStatus LineStatus { get; set; }
        //[JsonProperty("PackageQuantity")]
        //public Double? PackageQuantity { get; set; }
        //[JsonProperty("Text")]
        //public String Text { get; set; }
        //[JsonProperty("LineType")]
        //public String LineType { get; set; }
        //[JsonProperty("COGSCostingCode")]
        //public String COGSCostingCode { get; set; }
        //[JsonProperty("COGSAccountCode")]
        //public String COGSAccountCode { get; set; }
        //[JsonProperty("ChangeAssemlyBoMWarehouse")]
        //public String ChangeAssemlyBoMWarehouse { get; set; }
        //[JsonProperty("GrossBuyPrice")]
        //public Double? GrossBuyPrice { get; set; }
        //[JsonProperty("GrossBase")]
        //public Int32? GrossBase { get; set; }
        //[JsonProperty("GrossProfitTotalBasePrice")]
        //public Double? GrossProfitTotalBasePrice { get; set; }
        //[JsonProperty("CostingCode2")]
        //public String CostingCode2 { get; set; }
        //[JsonProperty("CostingCode3")]
        //public String CostingCode3 { get; set; }
        //[JsonProperty("CostingCode4")]
        //public String CostingCode4 { get; set; }
        //[JsonProperty("CostingCode5")]
        //public String CostingCode5 { get; set; }
        //[JsonProperty("ItemDetails")]
        //public String ItemDetails { get; set; }
        //[JsonProperty("LocationCode")]
        //public Int32? LocationCode { get; set; }
        //[JsonProperty("ActualDeliveryDate")]
        //public DateTime? ActualDeliveryDate { get; set; }
        //[JsonProperty("RemainingOpenQuantity")]
        //public Double? RemainingOpenQuantity { get; set; }
        //[JsonProperty("OpenAmount")]
        //public Double? OpenAmount { get; set; }
        //[JsonProperty("OpenAmountFC")]
        //public Double? OpenAmountFC { get; set; }
        //[JsonProperty("OpenAmountSC")]
        //public Double? OpenAmountSC { get; set; }
        //[JsonProperty("ExLineNo")]
        //public String ExLineNo { get; set; }
        //[JsonProperty("RequiredDate")]
        //public DateTime? RequiredDate { get; set; }
        //[JsonProperty("RequiredQuantity")]
        //public Double? RequiredQuantity { get; set; }
        //[JsonProperty("COGSCostingCode2")]
        //public String COGSCostingCode2 { get; set; }
        //[JsonProperty("COGSCostingCode3")]
        //public String COGSCostingCode3 { get; set; }
        //[JsonProperty("COGSCostingCode4")]
        //public String COGSCostingCode4 { get; set; }
        //[JsonProperty("COGSCostingCode5")]
        //public String COGSCostingCode5 { get; set; }
        //[JsonProperty("CSTforIPI")]
        //public String CSTforIPI { get; set; }
        //[JsonProperty("CSTforPIS")]
        //public String CSTforPIS { get; set; }
        //[JsonProperty("CSTforCOFINS")]
        //public String CSTforCOFINS { get; set; }
        //[JsonProperty("CreditOriginCode")]
        //public String CreditOriginCode { get; set; }
        //[JsonProperty("WithoutInventoryMovement")]
        //public String WithoutInventoryMovement { get; set; }
        //[JsonProperty("AgreementNo")]
        //public Int32? AgreementNo { get; set; }
        //[JsonProperty("AgreementRowNumber")]
        //public Int32? AgreementRowNumber { get; set; }
        //[JsonProperty("ActualBaseEntry")]
        //public Int32? ActualBaseEntry { get; set; }
        //[JsonProperty("ActualBaseLine")]
        //public Int32? ActualBaseLine { get; set; }
        //[JsonProperty("DocEntry")]
        //public Int32 DocEntry { get; set; }
        //[JsonProperty("Surpluses")]
        //public Double? Surpluses { get; set; }
        //[JsonProperty("DefectAndBreakup")]
        //public Double? DefectAndBreakup { get; set; }
        //[JsonProperty("Shortages")]
        //public Double? Shortages { get; set; }
        //[JsonProperty("ConsiderQuantity")]
        //public String ConsiderQuantity { get; set; }
        //[JsonProperty("PartialRetirement")]
        //public String PartialRetirement { get; set; }
        //[JsonProperty("RetirementQuantity")]
        //public Double? RetirementQuantity { get; set; }
        //[JsonProperty("RetirementAPC")]
        //public Double? RetirementAPC { get; set; }
        //[JsonProperty("ThirdParty")]
        //public String ThirdParty { get; set; }
        //[JsonProperty("ExpenseType")]
        //public String ExpenseType { get; set; }
        //[JsonProperty("ReceiptNumber")]
        //public String ReceiptNumber { get; set; }
        //[JsonProperty("ExpenseOperationType")]
        //public String ExpenseOperationType { get; set; }
        //[JsonProperty("FederalTaxID")]
        //public String FederalTaxID { get; set; }
        //[JsonProperty("EnableReturnCost")]
        //public String EnableReturnCost { get; set; }
        //[JsonProperty("ReturnCost")]
        //public Double? ReturnCost { get; set; }
        //[JsonProperty("LineVendor")]
        //public String LineVendor { get; set; }
        //[JsonProperty("StgSeqNum")]
        //public Int32? StgSeqNum { get; set; }
        //[JsonProperty("StgEntry")]
        //public Int32? StgEntry { get; set; }
        //[JsonProperty("StgDesc")]
        //public String StgDesc { get; set; }
        [JsonProperty("UoMEntry")]
        public Int32? UoMEntry { get; set; } 
        [JsonProperty("UoMCode")]
        public String UoMCode { get; set; }  //Y
        //[JsonProperty("InventoryQuantity")]
        //public Double? InventoryQuantity { get; set; }
        //[JsonProperty("RemainingOpenInventoryQuantity")]
        //public Double? RemainingOpenInventoryQuantity { get; set; }
        //[JsonProperty("ParentLineNum")]
        //public Int32? ParentLineNum { get; set; }
        //[JsonProperty("Incoterms")]
        //public Int32? Incoterms { get; set; }
        //[JsonProperty("TransportMode")]
        //public Int32? TransportMode { get; set; }
        [JsonProperty("ItemType")]
        public String ItemType { get; set; }
        //[JsonProperty("ChangeInventoryQuantityIndependently")]
        //public String ChangeInventoryQuantityIndependently { get; set; }
        //[JsonProperty("FreeOfChargeBP")]
        //public String FreeOfChargeBP { get; set; }
        //[JsonProperty("SACEntry")]
        //public Int32? SACEntry { get; set; }
        //[JsonProperty("HSNEntry")]
        //public Int32? HSNEntry { get; set; }
        //[JsonProperty("GrossPrice")]
        //public Double? GrossPrice { get; set; }
        //[JsonProperty("GrossTotal")]
        //public Double? GrossTotal { get; set; }
        //[JsonProperty("GrossTotalFC")]
        //public Double? GrossTotalFC { get; set; }
        //[JsonProperty("GrossTotalSC")]
        //public Double? GrossTotalSC { get; set; }
        //[JsonProperty("NCMCode")]
        //public Int32? NCMCode { get; set; }
        //[JsonProperty("ShipToCode")]
        //public String ShipToCode { get; set; }
        //[JsonProperty("ShipToDescription")]
        //public String ShipToDescription { get; set; }
        //[JsonProperty("ShipFromCode")]
        //public String ShipFromCode { get; set; }
        //[JsonProperty("ShipFromDescription")]
        //public String ShipFromDescription { get; set; }
        //[JsonProperty("LineTaxJurisdictions")]
        //public List<LineTaxJurisdiction> LineTaxJurisdictions { get; set; }
        //[JsonProperty("GeneratedAssets")]
        //public List<GeneratedAsset> GeneratedAssets { get; set; }
        //[JsonProperty("DocumentLineAdditionalExpenses")]
        //public List<DocumentLineAdditionalExpense> DocumentLineAdditionalExpenses { get; set; }
        //[JsonProperty("WithholdingTaxLines")]
        //public List<WithholdingTaxLine> WithholdingTaxLines { get; set; }
        [JsonProperty("SerialNumbers")]
        public List<SerialNumber> SerialNumbers { get; set; }
        //[JsonProperty("BatchNumbers")]
        //public List<BatchNumber> BatchNumbers { get; set; }
        //[JsonProperty("DocumentLinesBinAllocations")]
        //public List<DocumentLinesBinAllocation> DocumentLinesBinAllocations { get; set; }

        //[JsonProperty("DeliverySchedules")]
        //public List<DeliveryScheduleModel> DeliverySchedules { get; set; }
        [JsonProperty("U_PromoCode")]
        public String UPromoCode { get; set; }
        [JsonProperty("U_DisPrcnt")]
        public Double? UDisPrcnt { get; set; }
        [JsonProperty("U_DisAmt")]
        public Double? UDisAmt { get; set; }
        [JsonProperty("U_TotalAfDis")]
        public Double? UTotalAfDis { get; set; }
        [JsonProperty("U_PromoName")]
        public String UPromoName { get; set; }
        [JsonProperty("U_SchemaCode")]
        public String USchemaCode { get; set; }
        [JsonProperty("U_SchemaName")]
        public String USchemaName { get; set; }
        [JsonProperty("U_PriceAfDis")]
        public Double? UPriceAfDis { get; set; }
        /// <summary>
        /// Để xác định đây là line promotion(tặng), không phải line gốc
        /// </summary>
        [JsonProperty("U_IsPromo")]
        public String UIsPromo { get; set; }
        //[JsonProperty("U_DeliveryDate")]
        //public DateTime? UDeliveryDate { get; set; }
        //[JsonProperty("U_PONo")]
        //public String UPONo { get; set; }
        //[JsonProperty("U_Route")]
        //public String URoute { get; set; }
        //[JsonProperty("U_LicPlates")]
        //public String ULicPlates { get; set; }
        //[JsonProperty("U_UDF1")]
        //public String UUDF1 { get; set; }
        //[JsonProperty("U_UDF2")]
        //public String UUDF2 { get; set; }
        //[JsonProperty("U_UDF3")]
        //public String UUDF3 { get; set; }
        //[JsonProperty("U_UDF4")]
        //public String UUDF4 { get; set; }
        //[JsonProperty("U_UDF5")]
        //public String UUDF5 { get; set; }
        [JsonProperty("U_Collection")]
        public String UCollection { get; set; }

        [JsonIgnore]
        public string UCheckPromo { get; set; }

        [JsonIgnore]
        public String PromoType { get; set; }
        //[JsonIgnore]
        //public Int32? ItmsGrpCod { get; set; }
        [JsonIgnore]
        public Double? BaseQuantity { get; set; }
        [JsonIgnore]
        public String BaseUomCode { get; set; }
        [JsonIgnore]
        public Double? VatPerPriceAfDis { get; set; }
        [JsonIgnore]
        public Double? PriceAfDisAndVat { get; set; }

        public string ItemCategory1 { get; set; }
        public string ItemCategory2 { get; set; }
        public string ItemCategory3 { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string CustomField4 { get; set; }
        public string CustomField5 { get; set; }
        public string CustomField6 { get; set; }
        public string CustomField7 { get; set; }
        public string CustomField8 { get; set; }
        public string CustomField9 { get; set; }
        public string CustomField10 { get; set; }

        public string ApplyType { get; set; }
        public string RefTransId { get; set; }
        public string SalesTaxCode { get; set; }
        public decimal? SalesTaxRate { get; set; } 
        public string PurchaseTaxCode { get; set; }
        public decimal? PurchaseTaxRate { get; set; }
        public bool? IsSerial { get; set; }
        public bool? IsVoucher { get; set; } 
        public bool? IsNegative { get; set; }
        public string PriceListId { get; set; }
        public string ProductId { get; set; }
        public string WeighScaleBarcode { get; set; }
        public string BookletNo { get; set; }

        public string PrepaidCardNo { get; set; }
        public int? MemberValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SerialNum { get; set; }
    }
}
