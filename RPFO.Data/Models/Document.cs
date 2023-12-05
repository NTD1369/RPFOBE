using Newtonsoft.Json;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.Models
{
    public partial class Document
    {
        public Document()
        {
            PromotionApply = new List<PromotionViewModel>();
        }
        [JsonProperty("DocEntry")]
        public Int32 DocEntry { get; set; }
        [JsonProperty("DocNum")]
        public Int32? DocNum { get; set; } 
        //[JsonProperty("DocType")]
        //public string DocType  { get; set; }
        //[JsonProperty("HandWritten")]
        //public String HandWritten  { get; set; }
        //[JsonProperty("Printed")]
        //public String Printed  { get; set; }
        [JsonProperty("DocDate")]
        public DateTime? DocDate { get; set; }  //Y
        [JsonProperty("DocDueDate")]
        public DateTime? DocDueDate { get; set; }
        [JsonProperty("CardCode")]
        public String CardCode { get; set; } //Y
        [JsonProperty("CardName")]
        public String CardName { get; set; }
        [JsonProperty("CardGroup")]
        public String CardGroup { get; set; } //Y
        [JsonProperty("Address")]
        public String Address { get; set; }
        [JsonProperty("NumAtCard")]
        public String NumAtCard { get; set; }
        //[JsonProperty("DocTotal")]
        //public Double? DocTotal  { get; set; }
        //[JsonProperty("AttachmentEntry")]
        //public Int32? AttachmentEntry  { get; set; }
        [JsonProperty("DocCurrency")]
        public String DocCurrency { get; set; }  //Y
        [JsonProperty("DocRate")]
        public Double? DocRate { get; set; } 
        //[JsonProperty("Reference1")]
        //public String Reference1  { get; set; }
        //[JsonProperty("Reference2")]
        //public String Reference2  { get; set; }
        [JsonProperty("Comments")]
        public String Comments { get; set; }
        //[JsonProperty("JournalMemo")]
        //public String JournalMemo  { get; set; }
        //[JsonProperty("PaymentGroupCode")]
        //public Int32? PaymentGroupCode  { get; set; }
        //[JsonProperty("DocTime")]
        //public DateTime? DocTime  { get; set; }
        //[JsonProperty("SalesPersonCode")]
        //public Int32? SalesPersonCode  { get; set; }
        //[JsonProperty("TransportationCode")]
        //public Int32? TransportationCode  { get; set; }
        //[JsonProperty("Confirmed")]
        //public String Confirmed  { get; set; }
        //[JsonProperty("ImportFileNum")]
        //public Int32? ImportFileNum  { get; set; }
        //[JsonProperty("SummeryType")]
        //public String SummeryType  { get; set; }
        //[JsonProperty("ContactPersonCode")]
        //public Int32? ContactPersonCode  { get; set; }
        //[JsonProperty("ShowSCN")]
        //public String ShowSCN  { get; set; }
        //[JsonProperty("Series")]
        //public Int32? Series  { get; set; }
        //[JsonProperty("TaxDate")]
        //public DateTime? TaxDate  { get; set; }
        //[JsonProperty("PartialSupply")]
        //public String PartialSupply  { get; set; }
        [JsonProperty("DocObjectCode")]
        public string DocObjectCode { get; set; }
        //[JsonProperty("ShipToCode")]
        //public String ShipToCode  { get; set; }
        //[JsonProperty("Indicator")]
        //public String Indicator  { get; set; }
        //[JsonProperty("FederalTaxID")]
        //public String FederalTaxID  { get; set; }
        [JsonProperty("DiscountPercent")]
        public Double? DiscountPercent { get; set; }
        //[JsonProperty("PaymentReference")]
        //public String PaymentReference  { get; set; }
        //[JsonProperty("CreationDate")]
        //public DateTime? CreationDate  { get; set; }
        //[JsonProperty("UpdateDate")]
        //public DateTime? UpdateDate  { get; set; }
        //[JsonProperty("FinancialPeriod")]
        //public Int32? FinancialPeriod  { get; set; }
        //[JsonProperty("TransNum")]
        //public Int32? TransNum  { get; set; }
        //[JsonProperty("VatSum")]
        //public Double? VatSum  { get; set; }
        //[JsonProperty("VatSumSys")]
        //public Double? VatSumSys  { get; set; }
        //[JsonProperty("VatSumFc")]
        //public Double? VatSumFc  { get; set; }
        //[JsonProperty("NetProcedure")]
        //public String NetProcedure  { get; set; }
        //[JsonProperty("DocTotalFc")]
        //public Double? DocTotalFc  { get; set; }
        //[JsonProperty("DocTotalSys")]
        //public Double? DocTotalSys  { get; set; }
        //[JsonProperty("Form1099")]
        //public Int32? Form1099  { get; set; }
        //[JsonProperty("Box1099")]
        //public String Box1099  { get; set; }
        //[JsonProperty("RevisionPo")]
        //public String RevisionPo  { get; set; }
        //[JsonProperty("RequriedDate")]
        //public DateTime? RequriedDate  { get; set; }
        //[JsonProperty("CancelDate")]
        //public DateTime? CancelDate  { get; set; }
        //[JsonProperty("BlockDunning")]
        //public String BlockDunning  { get; set; }
        //[JsonProperty("Submitted")]
        //public String Submitted  { get; set; }
        //[JsonProperty("Segment")]
        //public Int32? Segment  { get; set; }
        //[JsonProperty("PickStatus")]
        //public String PickStatus  { get; set; }
        //[JsonProperty("Pick")]
        //public String Pick  { get; set; }
        //[JsonProperty("PaymentMethod")]
        //public String PaymentMethod  { get; set; }
        //[JsonProperty("PaymentBlock")]
        //public String PaymentBlock  { get; set; }
        //[JsonProperty("PaymentBlockEntry")]
        //public Int32? PaymentBlockEntry  { get; set; }
        //[JsonProperty("CentralBankIndicator")]
        //public String CentralBankIndicator  { get; set; }
        //[JsonProperty("MaximumCashDiscount")]
        //public String MaximumCashDiscount  { get; set; }
        //[JsonProperty("Reserve")]
        //public String Reserve  { get; set; }
        //[JsonProperty("Project")]
        //public String Project  { get; set; }
        //[JsonProperty("ExemptionValidityDateFrom")]
        //public DateTime? ExemptionValidityDateFrom  { get; set; }
        //[JsonProperty("ExemptionValidityDateTo")]
        //public DateTime? ExemptionValidityDateTo  { get; set; }
        //[JsonProperty("WareHouseUpdateType")]
        //public String WareHouseUpdateType  { get; set; }
        //[JsonProperty("Rounding")]
        //public String Rounding  { get; set; }
        //[JsonProperty("ExternalCorrectedDocNum")]
        //public String ExternalCorrectedDocNum  { get; set; }
        //[JsonProperty("InternalCorrectedDocNum")]
        //public Int32? InternalCorrectedDocNum  { get; set; }
        //[JsonProperty("NextCorrectingDocument")]
        //public Int32? NextCorrectingDocument  { get; set; }
        //[JsonProperty("DeferredTax")]
        //public String DeferredTax  { get; set; }
        //[JsonProperty("TaxExemptionLetterNum")]
        //public String TaxExemptionLetterNum  { get; set; }
        //[JsonProperty("WTApplied")]
        //public Double? WTApplied  { get; set; }
        //[JsonProperty("WTAppliedFC")]
        //public Double? WTAppliedFC  { get; set; }
        //[JsonProperty("BillOfExchangeReserved")]
        //public String BillOfExchangeReserved  { get; set; }
        //[JsonProperty("AgentCode")]
        //public String AgentCode  { get; set; }
        //[JsonProperty("WTAppliedSC")]
        //public Double? WTAppliedSC  { get; set; }
        //[JsonProperty("TotalEqualizationTax")]
        //public Double? TotalEqualizationTax  { get; set; }
        //[JsonProperty("TotalEqualizationTaxFC")]
        //public Double? TotalEqualizationTaxFC  { get; set; }
        //[JsonProperty("TotalEqualizationTaxSC")]
        //public Double? TotalEqualizationTaxSC  { get; set; }
        //[JsonProperty("NumberOfInstallments")]
        //public Int32? NumberOfInstallments  { get; set; }
        //[JsonProperty("ApplyTaxOnFirstInstallment")]
        //public String ApplyTaxOnFirstInstallment  { get; set; }
        //[JsonProperty("WTNonSubjectAmount")]
        //public Double? WTNonSubjectAmount  { get; set; }
        //[JsonProperty("WTNonSubjectAmountSC")]
        //public Double? WTNonSubjectAmountSC  { get; set; }
        //[JsonProperty("WTNonSubjectAmountFC")]
        //public Double? WTNonSubjectAmountFC  { get; set; }
        //[JsonProperty("WTExemptedAmount")]
        //public Double? WTExemptedAmount  { get; set; }
        //[JsonProperty("WTExemptedAmountSC")]
        //public Double? WTExemptedAmountSC  { get; set; }
        //[JsonProperty("WTExemptedAmountFC")]
        //public Double? WTExemptedAmountFC  { get; set; }
        //[JsonProperty("BaseAmount")]
        //public Double? BaseAmount  { get; set; }
        //[JsonProperty("BaseAmountSC")]
        //public Double? BaseAmountSC  { get; set; }
        //[JsonProperty("BaseAmountFC")]
        //public Double? BaseAmountFC  { get; set; }
        //[JsonProperty("WTAmount")]
        //public Double? WTAmount  { get; set; }
        //[JsonProperty("WTAmountSC")]
        //public Double? WTAmountSC  { get; set; }
        //[JsonProperty("WTAmountFC")]
        //public Double? WTAmountFC  { get; set; }
        //[JsonProperty("VatDate")]
        //public DateTime? VatDate  { get; set; }
        //[JsonProperty("DocumentsOwner")]
        //public Int32? DocumentsOwner  { get; set; }
        //[JsonProperty("FolioPrefixString")]
        //public String FolioPrefixString  { get; set; }
        //[JsonProperty("FolioNumber")]
        //public Int32? FolioNumber  { get; set; }
        //[JsonProperty("DocumentSubType")]
        //public String DocumentSubType  { get; set; }
        //[JsonProperty("BPChannelCode")]
        //public String BPChannelCode  { get; set; }
        //[JsonProperty("BPChannelContact")]
        //public Int32? BPChannelContact  { get; set; }
        //[JsonProperty("Address2")]
        //public String Address2  { get; set; }
        [JsonProperty("DocumentStatus")]
        public String DocumentStatus { get; set; }
        //[JsonProperty("PeriodIndicator")]
        //public String PeriodIndicator  { get; set; }
        //[JsonProperty("PayToCode")]
        //public String PayToCode  { get; set; }
        //[JsonProperty("ManualNumber")]
        //public String ManualNumber  { get; set; }
        //[JsonProperty("UseShpdGoodsAct")]
        //public String UseShpdGoodsAct  { get; set; }
        //[JsonProperty("IsPayToBank")]
        //public String IsPayToBank  { get; set; }
        //[JsonProperty("PayToBankCountry")]
        //public String PayToBankCountry  { get; set; }
        //[JsonProperty("PayToBankCode")]
        //public String PayToBankCode  { get; set; }
        //[JsonProperty("PayToBankAccountNo")]
        //public String PayToBankAccountNo  { get; set; }
        //[JsonProperty("PayToBankBranch")]
        //public String PayToBankBranch  { get; set; }
        //[JsonProperty("BPL_IDAssignedToInvoice")]
        //public Int32? BPLIDAssignedToInvoice  { get; set; }
        //[JsonProperty("DownPayment")]
        //public Double? DownPayment  { get; set; }
        //[JsonProperty("ReserveInvoice")]
        //public String ReserveInvoice  { get; set; }
        //[JsonProperty("LanguageCode")]
        //public Int32? LanguageCode  { get; set; }
        //[JsonProperty("TrackingNumber")]
        //public String TrackingNumber  { get; set; }
        //[JsonProperty("PickRemark")]
        //public String PickRemark  { get; set; }
        //[JsonProperty("ClosingDate")]
        //public DateTime? ClosingDate  { get; set; }
        //[JsonProperty("SequenceCode")]
        //public Int32? SequenceCode  { get; set; }
        //[JsonProperty("SequenceSerial")]
        //public Int32? SequenceSerial  { get; set; }
        //[JsonProperty("SeriesString")]
        //public String SeriesString  { get; set; }
        //[JsonProperty("SubSeriesString")]
        //public String SubSeriesString  { get; set; }
        //[JsonProperty("SequenceModel")]
        //public String SequenceModel  { get; set; }
        //[JsonProperty("UseCorrectionVATGroup")]
        //public String UseCorrectionVATGroup  { get; set; }
        //[JsonProperty("TotalDiscount")]
        //public Double? TotalDiscount  { get; set; }
        //[JsonProperty("DownPaymentAmount")]
        //public Double? DownPaymentAmount  { get; set; }
        //[JsonProperty("DownPaymentPercentage")]
        //public Double? DownPaymentPercentage  { get; set; }
        //[JsonProperty("DownPaymentType")]
        //public String DownPaymentType  { get; set; }
        //[JsonProperty("DownPaymentAmountSC")]
        //public Double? DownPaymentAmountSC  { get; set; }
        //[JsonProperty("DownPaymentAmountFC")]
        //public Double? DownPaymentAmountFC  { get; set; }
        //[JsonProperty("VatPercent")]
        //public Double? VatPercent  { get; set; }
        //[JsonProperty("ServiceGrossProfitPercent")]
        //public Double? ServiceGrossProfitPercent  { get; set; }
        //[JsonProperty("OpeningRemarks")]
        //public String OpeningRemarks  { get; set; }
        //[JsonProperty("ClosingRemarks")]
        //public String ClosingRemarks  { get; set; }
        //[JsonProperty("RoundingDiffAmount")]
        //public Double? RoundingDiffAmount  { get; set; }
        //[JsonProperty("RoundingDiffAmountFC")]
        //public Double? RoundingDiffAmountFC  { get; set; }
        //[JsonProperty("RoundingDiffAmountSC")]
        //public Double? RoundingDiffAmountSC  { get; set; }
        //[JsonProperty("Cancelled")]
        //public String Cancelled  { get; set; }
        //[JsonProperty("SignatureInputMessage")]
        //public String SignatureInputMessage  { get; set; }
        //[JsonProperty("SignatureDigest")]
        //public String SignatureDigest  { get; set; }
        //[JsonProperty("CertificationNumber")]
        //public String CertificationNumber  { get; set; }
        //[JsonProperty("PrivateKeyVersion")]
        //public Int32? PrivateKeyVersion  { get; set; }
        //[JsonProperty("ControlAccount")]
        //public String ControlAccount  { get; set; }
        //[JsonProperty("InsuranceOperation347")]
        //public String InsuranceOperation347  { get; set; }
        //[JsonProperty("ArchiveNonremovableSalesQuotation")]
        //public String ArchiveNonremovableSalesQuotation  { get; set; }
        //[JsonProperty("GTSChecker")]
        //public Int32? GTSChecker  { get; set; }
        //[JsonProperty("GTSPayee")]
        //public Int32? GTSPayee  { get; set; }
        //[JsonProperty("ExtraMonth")]
        //public Int32? ExtraMonth  { get; set; }
        //[JsonProperty("ExtraDays")]
        //public Int32? ExtraDays  { get; set; }
        //[JsonProperty("CashDiscountDateOffset")]
        //public Int32? CashDiscountDateOffset  { get; set; }
        //[JsonProperty("StartFrom")]
        //public String StartFrom  { get; set; }
        //[JsonProperty("NTSApproved")]
        //public String NTSApproved  { get; set; }
        //[JsonProperty("ETaxWebSite")]
        //public Int32? ETaxWebSite  { get; set; }
        //[JsonProperty("ETaxNumber")]
        //public String ETaxNumber  { get; set; }
        //[JsonProperty("NTSApprovedNumber")]
        //public String NTSApprovedNumber  { get; set; }
        //[JsonProperty("EDocGenerationType")]
        //public String EDocGenerationType  { get; set; }
        //[JsonProperty("EDocSeries")]
        //public Int32? EDocSeries  { get; set; }
        //[JsonProperty("EDocNum")]
        //public String EDocNum  { get; set; }
        //[JsonProperty("EDocExportFormat")]
        //public Int32? EDocExportFormat  { get; set; }
        //[JsonProperty("EDocStatus")]
        //public String EDocStatus  { get; set; }
        //[JsonProperty("EDocErrorCode")]
        //public String EDocErrorCode  { get; set; }
        //[JsonProperty("EDocErrorMessage")]
        //public String EDocErrorMessage  { get; set; }
        //[JsonProperty("DownPaymentStatus")]
        //public String DownPaymentStatus  { get; set; }
        //[JsonProperty("GroupSeries")]
        //public Int32? GroupSeries  { get; set; }
        //[JsonProperty("GroupNumber")]
        //public Int32? GroupNumber  { get; set; }
        //[JsonProperty("GroupHandWritten")]
        //public String GroupHandWritten  { get; set; }
        //[JsonProperty("ReopenOriginalDocument")]
        //public String ReopenOriginalDocument  { get; set; }
        //[JsonProperty("ReopenManuallyClosedOrCanceledDocument")]
        //public String ReopenManuallyClosedOrCanceledDocument  { get; set; }
        //[JsonProperty("CreateOnlineQuotation")]
        //public String CreateOnlineQuotation  { get; set; }
        //[JsonProperty("POSEquipmentNumber")]
        //public String POSEquipmentNumber  { get; set; }
        //[JsonProperty("POSManufacturerSerialNumber")]
        //public String POSManufacturerSerialNumber  { get; set; }
        //[JsonProperty("POSCashierNumber")]
        //public Int32? POSCashierNumber  { get; set; }
        //[JsonProperty("ApplyCurrentVATRatesForDownPaymentsToDraw")]
        //public String ApplyCurrentVATRatesForDownPaymentsToDraw  { get; set; }
        //[JsonProperty("ClosingOption")]
        //public String ClosingOption  { get; set; }
        //[JsonProperty("SpecifiedClosingDate")]
        //public DateTime? SpecifiedClosingDate  { get; set; }
        //[JsonProperty("OpenForLandedCosts")]
        //public String OpenForLandedCosts  { get; set; }
        //[JsonProperty("AuthorizationStatus")]
        //public String AuthorizationStatus  { get; set; }
        //[JsonProperty("TotalDiscountFC")]
        //public Double? TotalDiscountFC  { get; set; }
        //[JsonProperty("TotalDiscountSC")]
        //public Double? TotalDiscountSC  { get; set; }
        //[JsonProperty("RelevantToGTS")]
        //public String RelevantToGTS  { get; set; }
        //[JsonProperty("BPLName")]
        //public String BPLName  { get; set; }
        //[JsonProperty("VATRegNum")]
        //public String VATRegNum  { get; set; }
        //[JsonProperty("AnnualInvoiceDeclarationReference")]
        //public Int32? AnnualInvoiceDeclarationReference  { get; set; }
        //[JsonProperty("Supplier")]
        //public String Supplier  { get; set; }
        //[JsonProperty("Releaser")]
        //public Int32? Releaser  { get; set; }
        //[JsonProperty("Receiver")]
        //public Int32? Receiver  { get; set; }
        //[JsonProperty("BlanketAgreementNumber")]
        //public Int32? BlanketAgreementNumber  { get; set; }
        //[JsonProperty("IsAlteration")]
        //public String IsAlteration  { get; set; }
        //[JsonProperty("CancelStatus")]
        //public String CancelStatus  { get; set; }
        //[JsonProperty("AssetValueDate")]
        //public DateTime? AssetValueDate  { get; set; }
        //[JsonProperty("Requester")]
        //public String Requester  { get; set; }
        //[JsonProperty("RequesterName")]
        //public String RequesterName  { get; set; }
        //[JsonProperty("RequesterBranch")]
        //public Int32? RequesterBranch  { get; set; }
        //[JsonProperty("RequesterDepartment")]
        //public Int32? RequesterDepartment  { get; set; }
        //[JsonProperty("RequesterEmail")]
        //public String RequesterEmail  { get; set; }
        //[JsonProperty("SendNotification")]
        //public String SendNotification  { get; set; }
        //[JsonProperty("ReqType")]
        //public Int32? ReqType  { get; set; }
        //[JsonProperty("DocumentDelivery")]
        //public String DocumentDelivery  { get; set; }
        //[JsonProperty("AuthorizationCode")]
        //public String AuthorizationCode  { get; set; }
        //[JsonProperty("StartDeliveryDate")]
        //public DateTime? StartDeliveryDate  { get; set; }
        //[JsonProperty("StartDeliveryTime")]
        //public DateTime? StartDeliveryTime  { get; set; }
        //[JsonProperty("EndDeliveryDate")]
        //public DateTime? EndDeliveryDate  { get; set; }
        //[JsonProperty("EndDeliveryTime")]
        //public DateTime? EndDeliveryTime  { get; set; }
        //[JsonProperty("VehiclePlate")]
        //public String VehiclePlate  { get; set; }
        //[JsonProperty("ATDocumentType")]
        //public String ATDocumentType  { get; set; }
        //[JsonProperty("ElecCommStatus")]
        //public String ElecCommStatus  { get; set; }
        //[JsonProperty("ElecCommMessage")]
        //public String ElecCommMessage  { get; set; }
        //[JsonProperty("ReuseDocumentNum")]
        //public String ReuseDocumentNum  { get; set; }
        //[JsonProperty("ReuseNotaFiscalNum")]
        //public String ReuseNotaFiscalNum  { get; set; }
        //[JsonProperty("PrintSEPADirect")]
        //public String PrintSEPADirect  { get; set; }
        //[JsonProperty("FiscalDocNum")]
        //public String FiscalDocNum  { get; set; }
        //[JsonProperty("POSDailySummaryNo")]
        //public Int32? POSDailySummaryNo  { get; set; }
        //[JsonProperty("POSReceiptNo")]
        //public Int32? POSReceiptNo  { get; set; }
        //[JsonProperty("PointOfIssueCode")]
        //public String PointOfIssueCode  { get; set; }
        //[JsonProperty("Letter")]
        //public String Letter  { get; set; }
        //[JsonProperty("FolioNumberFrom")]
        //public Int32? FolioNumberFrom  { get; set; }
        //[JsonProperty("FolioNumberTo")]
        //public Int32? FolioNumberTo  { get; set; }
        //[JsonProperty("InterimType")]
        //public String InterimType  { get; set; }
        //[JsonProperty("RelatedType")]
        //public Int32? RelatedType  { get; set; }
        //[JsonProperty("RelatedEntry")]
        //public Int32? RelatedEntry  { get; set; }
        //[JsonProperty("SAPPassport")]
        //public String SAPPassport  { get; set; }
        //[JsonProperty("DocumentTaxID")]
        //public String DocumentTaxID  { get; set; }
        //[JsonProperty("DateOfReportingControlStatementVAT")]
        //public DateTime? DateOfReportingControlStatementVAT  { get; set; }
        //[JsonProperty("ReportingSectionControlStatementVAT")]
        //public String ReportingSectionControlStatementVAT  { get; set; }
        //[JsonProperty("ExcludeFromTaxReportControlStatementVAT")]
        //public String ExcludeFromTaxReportControlStatementVAT  { get; set; }
        //[JsonProperty("POS_CashRegister")]
        //public Int32? POSCashRegister  { get; set; }
        //[JsonProperty("UpdateTime")]
        //public DateTime? UpdateTime  { get; set; }
        //[JsonProperty("PriceMode")]
        //public String PriceMode  { get; set; }
        //[JsonProperty("DownPaymentTrasactionID")]
        //public String DownPaymentTrasactionID  { get; set; }
        //[JsonProperty("Revision")]
        //public String Revision  { get; set; }
        [JsonProperty("OriginalRefNo")]
        public String OriginalRefNo { get; set; }
        [JsonProperty("OriginalRefDate")]
        public DateTime? OriginalRefDate { get; set; }
        //[JsonProperty("GSTTransactionType")]
        //public String GSTTransactionType  { get; set; }
        //[JsonProperty("OriginalCreditOrDebitNo")]
        //public String OriginalCreditOrDebitNo  { get; set; }
        //[JsonProperty("OriginalCreditOrDebitDate")]
        //public DateTime? OriginalCreditOrDebitDate  { get; set; }
        //[JsonProperty("ECommerceOperator")]
        //public String ECommerceOperator  { get; set; }
        //[JsonProperty("ECommerceGSTIN")]
        //public String ECommerceGSTIN  { get; set; }
        //[JsonProperty("TaxInvoiceNo")]
        //public String TaxInvoiceNo  { get; set; }
        //[JsonProperty("TaxInvoiceDate")]
        //public DateTime? TaxInvoiceDate  { get; set; }
        //[JsonProperty("ShipFrom")]
        //public String ShipFrom  { get; set; }
        //[JsonProperty("CommissionTrade")]
        //public String CommissionTrade  { get; set; }
        //[JsonProperty("CommissionTradeReturn")]
        //public String CommissionTradeReturn  { get; set; }
        //[JsonProperty("UseBillToAddrToDetermineTax")]
        //public String UseBillToAddrToDetermineTax  { get; set; }
        //[JsonProperty("IssuingReason")]
        //public Int32? IssuingReason  { get; set; }
        //[JsonProperty("Cig")]
        //public Int32? Cig  { get; set; }
        //[JsonProperty("Cup")]

        //public string UTemplateCode { get; set; }

        //[JsonProperty("Document_ApprovalRequests")]
        //public List<Document_ApprovalRequest> DocumentApprovalRequests  { get; set; }
        [JsonProperty("DocumentLines")]
        public List<DocumentLine> DocumentLines { get; set; }

        //[JsonProperty("EWayBillDetails")]
        //public EWayBillDetails EWayBillDetails  { get; set; }
        //[JsonProperty("ElectronicProtocols")]
        //public List<ElectronicProtocol> ElectronicProtocols  { get; set; }
        //[JsonProperty("DocumentAdditionalExpenses")]
        //public List<DocumentAdditionalExpense> DocumentAdditionalExpenses  { get; set; }
        //[JsonProperty("WithholdingTaxDataWTXCollection")]
        //public List<WithholdingTaxDataWTX> WithholdingTaxDataWTXCollection  { get; set; }
        //[JsonProperty("WithholdingTaxDataCollection")]
        //public List<WithholdingTaxData> WithholdingTaxDataCollection  { get; set; }
        //[JsonProperty("DocumentPackages")]
        //public List<DocumentPackage> DocumentPackages  { get; set; }
        //[JsonProperty("DocumentSpecialLines")]
        //public List<DocumentSpecialLine> DocumentSpecialLines  { get; set; }
        //[JsonProperty("DocumentInstallments")]
        //public List<DocumentInstallment> DocumentInstallments  { get; set; }
        //[JsonProperty("DownPaymentsToDraw")]
        //public List<DownPaymentToDraw> DownPaymentsToDraw  { get; set; }
        //[JsonProperty("TaxExtension")]
        //public TaxExtension TaxExtension  { get; set; }
        //[JsonProperty("AddressExtension")]
        //public AddressExtension AddressExtension  { get; set; }

        [JsonProperty("U_CreatedBy")]
        public String UCreatedBy { get; set; } = "ABEOADDINS";

        [JsonIgnore]
        public String Message { get; set; }
        public String PromotionId { get; set; }
        public String StoreId { get; set; }
        /// <summary>
        /// Dùng cho promotion là voucher
        /// </summary>
        public String PromotionCode { get; set; }
        public Nullable<bool> VoucherIsApply { get; set; }
        //[JsonProperty("U_DeliveryDate")]
        //public DateTime? UDeliveryDate { get; set; }
        //[JsonProperty("U_PONo")]
        //public String UPONo { get; set; }
        //[JsonProperty("U_Route")]
        //public String URoute { get; set; }
        //[JsonProperty("U_LicPlates")]
        //public String ULicPlates { get; set; }
        //[JsonProperty("U_vehicleNo")]
        //public String UvehicleNo { get; set; }
        //[JsonProperty("U_Shipping")]
        //public String UShipping { get; set; }
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

        [JsonProperty("U_CompanyCode")]
        public String UCompanyCode { get; set; }  //Y

        public List<PromotionViewModel> PromotionApply { get; set; }
        [JsonIgnore]
        public String TransId { get; set; }  //Y
        //[JsonIgnore]
        public Double? UDiscountAmount { get; set; }
        public int RoundingDigit { get; set; } = 6;
        [JsonProperty("CustomerRank")]
        public String CustomerRank { get; set; }

        public List<TSalesPayment> SalesPayments { get; set; }
    }
}
