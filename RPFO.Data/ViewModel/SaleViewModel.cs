
using Newtonsoft.Json.Linq;
using RPFO.Data.Entities;
using RPFO.Data.OMSModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace RPFO.Data.ViewModels
{
    public partial class SaleViewModel : TSalesHeader
    {
        public SaleViewModel()
        {
            Lines = new List<TSalesLineViewModel>();
            Payments = new List<TSalesPayment>();
            VoucherApply = new List<TapTapVoucherDetails>();
            Deliveries = new List<TSalesDelivery>();
            OrderStaffs = new List<TSalesStaff>();
            BreakOrders = new List<SaleViewModel>();
            OrderItems = new List<ItemViewModel>();
            //PrepaidLines = new List<TPrepaidCardTrans>();
        }

        public MCustomer Customer { get; set; }
        public double? RewardPoints { get; set; }
        public bool? HasVoucher { get; set; }
        public List<SaleViewModel> BreakOrders { get; set; }
        public List<TSalesLineViewModel> Lines { get; set; }
        public List<TSalesStaff> OrderStaffs { get; set; }
        public List<ItemViewModel> OrderItems { get; set; }

        public List<TSalesLineSerialViewModel> SerialLines { get; set; }
        public List<TSalesPromoViewModel> PromoLines { get; set; }
        public List<TSalesPayment> Payments { get; set; }
        public List<TSalesPayment> ContractPayments { get; set; }
        public TSalesInvoice Invoice { get; set; }
        public TSalesDelivery Delivery { get; set; }
        public List<TapTapVoucherDetails> VoucherApply { get; set; }

        public List<TSalesDelivery> Deliveries { get; set; }
        public List<TSalesRedeemVoucher> Vouchers { get; set; }
        public List<OrderLogModel> Logs { get; set; }

        public string PrintName { get; set; }
        public string TempTransId { get; set; }
        public string BillList { get; set; }
        public string CheckOutOn { get; set; }
        public string isDrawer { get; set; }
        public string isPrint { get; set; }
        public List<TSalesStaff> Staffs { get; set; } = new List<TSalesStaff>();

    }
    public class OrderLogModel
    {
        public Guid Id { get; set; }
        public int? LineNum { get; set; }
        public string Action { get; set; }
        public string Type { get; set; }
        public DateTime? Time { get; set; }
        public string Result { get; set; }
        public string Value { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
        public string CustomF6 { get; set; }
        public string CustomF7 { get; set; }
        public string CustomF8 { get; set; }
        public string CustomF9 { get; set; }
        public string CustomF10 { get; set; }
        public string TerminalId { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string TransId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
    public class TSalesLineViewModel : TSalesLine
    {
        public string ItemName { get; set; }
        public string UomName { get; set; }
        public string SapBonusBuyId { get; set; }
        public string SapPromoId { get; set; }
        public decimal? CheckedQty { get; set; }
        public List<TSalesLineSerialViewModel> SerialLines { get; set; }
        public List<TSalesLineViewModel> Lines { get; set; }
        public List<TSalesStaff> Staffs { get; set; } = new List<TSalesStaff>();
        //Staffs 

    }
    public class TSalesPromoViewModel : TSalesPromo
    {
        public string ItemName { get; set; }
        public string UomName { get; set; }
    }
    public class TSalesLineSerialViewModel : TSalesLineSerial
    {
        public string ItemName { get; set; }
        public string UomName { get; set; }
        public decimal? Price { get; set; }
        public string SapBonusBuyId { get; set; }
        public string SapPromoId { get; set; }
        public DateTime? ExpDate { get; set; }
        public string SerialNum { get; set; }
    }  
}
