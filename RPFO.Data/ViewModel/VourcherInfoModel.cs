using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class VourcherInfoModel
    {
       public List<CustomerinfoModel> Customerinfo { get; set; }
       public List<VourcherDetailModel> VourcherDetail { get; set; }
        public List<VourcherDetailBomModel> VourcherDetailBom { get; set; }
    }
    public class CustomerinfoModel
    {
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CustomF1 { get; set; }
        public string CustomerId { get; set; }
        public string CustomerGrpId { get; set; }
    }
    public class VourcherDetailModel
    {
        public string TransId { get; set; }
        public string SerialNum { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string UOMCode { get; set; }
        public decimal? Quantity { get; set; }
        public DateTime? EndDate { get; set; }
        public string BarCode { get; set; }
        public decimal? DefaultPrice { get; set; }
        public string CustomField10 { get; set; }
    }

    public class VourcherDetailBomModel
    {
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string UOMCode { get; set; }
        public decimal? Quantity { get; set; }
        public string BarCode { get; set; }
        public string BOMid { get; set; }
    }

}
