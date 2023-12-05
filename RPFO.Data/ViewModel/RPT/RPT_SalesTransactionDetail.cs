using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel.RPT
{
    public class RPT_SalesTransactionDetailModel
    {
		public string CompanyCode{ get; set; }
		public string StoreId	{ get; set; }
		public string StoreName	{ get; set; }
		public string SLocId{ get; set; }
		public string ContractNo	{ get; set; }
		public string CusIdentifier	{ get; set; }
		public string ShiftId	{ get; set; }
		public string CusId	{ get; set; }
		public string HeaderStatus{ get; set; }
		public string IsCanceled	{ get; set; }
		public string Remarks	{ get; set; }
		public string SalesPerson	{ get; set; }
		public string HeaderSalesMode{ get; set; }
		public string RefTransId	{ get; set; }
		public string ManualDiscount { get; set; }	
		public string DataSource { get; set; }	
		public string POSType { get; set; }	
		public string Phone { get; set; }	
		public string CusName { get; set; }	
		public string CusAddress { get; set; }	 
		public string TransId{ get; set; }
		public string LineId{ get; set; }
		public string ItemCode{ get; set; }		
		public string BarCode{ get; set; }
		public string UOMCode{ get; set; }
		public decimal? Quantity { get; set; }
		public decimal? Price { get; set; }
		public decimal? LineTotalBefDiscount { get; set; }
		public decimal? LineTotal { get; set; }
		public decimal? FinalLineTotal { get; set; }
		public string DiscountType{ get; set; }
		public decimal? LineDiscountAmt { get; set; }
		public decimal? DiscountAmt { get; set; }
		public decimal? DiscountRate { get; set; }
		public decimal? LineDiscountRate { get; set; }
		public string CreatedBy{ get; set; }
		public string CreatedOn{ get; set; }
		public string ModifiedBy{ get; set; }
		public string ModifiedOn{ get; set; }
		public string Status{ get; set; }
		public string Remark{ get; set; }
		public string PromoId{ get; set; }
		public string PromoName{ get; set; }
		public string PromoType{ get; set; }
		public decimal? PromoPercent { get; set; }
		public string PromoBaseItem{ get; set; }
		public string SalesMode{ get; set; }
		public decimal? TaxRate { get; set; }
		public decimal? TaxAmt { get; set; }
		public string TaxCode{ get; set; }
		public decimal? MinDepositAmt { get; set; }
		public decimal? MinDepositPercent { get; set; }
		public string DeliveryType{ get; set; }
		public string POSService{ get; set; }
		public string StoreAreaId{ get; set; }
		public string TimeFrameId{ get; set; }
		public Nullable<DateTime> AppointmentDate{ get; set; }
		public string BomID{ get; set; }
		public decimal? PromoPrice { get; set; }
		public decimal? PromoLineTotal { get; set; }

		public string ApprovalId { get; set; }
		public string Description { get; set; }
		public string PrepaidCardNo { get; set; }
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
		public decimal? DefaultPrice { get; set; }


		public string CusGrpId { get; set; }
		public string PriceListId { get; set; }
		public string ItemGroupName { get; set; }
		public string ItemCategory1 { get; set; }
		public string ItemCategory2 { get; set; }
		public string ItemCategory3 { get; set; }
		public string Reason { get; set; } 
		public string CounterId { get; set; }
		public string CusGrpDesc { get; set; } 
		public string ItemGroupId { get; set; }
		public string SyncMWIStatus { get; set; }
		public string OrgQuantity { get; set; }
		public string OMSId { get; set; }
		public DateTime? SyncMWIDate { get; set; }
		public decimal? HeaderDiscount { get; set; }
		public decimal? HeaderDiscountAmt { get; set; }
		public string SaleCatergoryID { get; set; }
		public string SaleCategoryID { get; set; }
		public string SaleCategoryID1 { get; set; }
		public string SaleCategoryID2 { get; set; }
		public string SaleCategoryID3 { get; set; }
		public string SaleCategoryID4 { get; set; }
		public string SaleCategoryID5 { get; set; }
		public string SaleCaterogyName { get; set; }
		public string SaleCategoryName { get; set; } 
		public string SaleCategoryName1 { get; set; } 
		public string SaleCategoryName2 { get; set; } 
		public string SaleCategoryName3 { get; set; } 
		public string SaleCategoryName4 { get; set; } 
	}
}
