using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel.RPT
{
   public class RPT_SalesByItemModel
    {
		public string CompanyCode { get; set; }
		public string StoreId { get; set; }
		public string StoreName { get; set; }
		public string ItemCode { get; set; }
		public string BarCode { get; set; }
		public string UOMCode { get; set; }
		public decimal? Quantity { get; set; }
		public decimal? Price { get; set; }
		public decimal? LineTotal { get; set; }
		public decimal? LineTotalBefDis { get; set; }
		public decimal? DiscountAmt { get; set; }
		public string CreatedBy { get; set; }
		public DateTime? CreatedOn { get; set; }
		public DateTime? CreatedOnG { get; set; }
		public string Description { get; set; }
		public string ItemGroupId { get; set; }
		public string GroupName { get; set; }


	}
}
