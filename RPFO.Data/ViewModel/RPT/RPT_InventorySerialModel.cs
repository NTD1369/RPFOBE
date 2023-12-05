using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel.RPT
{
    public class RPT_InventorySerialModel
	{
		public string CompanyCode{ get; set; }
		public string StoreId	{ get; set; }
		public string StoreName	{ get; set; }
		public string TransType { get; set; } 
		public string TransId{ get; set; }
		public DateTime? TransDate { get; set; } 
		public string ItemCode { get; set; }		 
		public string SerialNum { get; set; }		 
		public string ItemName { get; set; }		 
		public string UOMCode { get; set; }
		public decimal? Quantity { get; set; }
		public decimal? Qty { get; set; }
	 
	}
}
