using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.OMSModels
{
    public class TapTapVoucherRequest
    {
        public string customer_id { get; set; }
        public string voucher_code { get; set; }
        public string store_code { get; set; }
        public string transaction_id { get; set; }
    }

    public class TapTapVoucherDetails : S4VoucherDetail
    {
        //[JsonProperty("name")]
        public string name { get; set; }
        //[JsonProperty("code")]
        public string code { get; set; }
        //[JsonProperty("image")]
        public string image { get; set; }
        //[JsonProperty("start_date")]
        public string start_date { get; set; }
        //[JsonProperty("end_date")]
        public string end_date { get; set; }
        //[JsonProperty("issued_date")]
        public string issued_date { get; set; }
        //[JsonProperty("intouch_point")]
        public string intouch_point { get; set; }
        //[JsonProperty("intouch_series_id")]
        //public string IntouchSeriesId { get; set; }
        //[JsonProperty("status")]
        public string status { get; set; }
        //[JsonProperty("brand")]
        public string brand { get; set; }
        //[JsonProperty("t&c")]
        public string tc { get; set; }
        //[JsonProperty("discount_code")]
        public string discount_type { get; set; }
        public string discount_value { get; set; }
        public string discount_upto { get; set; }
        public string discount_code { get; set; }
        public string is_ecom { get; set; }

        public int? max_redemption { get; set; }
        public string min_bill_amount { get; set; }


        public int? redemption_count { get; set; }
        public string store_code { get; set; }
        public string voucher_code { get; set; }
        public string source { get; set; }




    }
    public class S4VoucherDetail
    {
        public string plantcode { get; set; }
        public string serialnumber { get; set; }
        public string serialoncardpublic { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public string salespric { get; set; }
        public string cardvalue { get; set; }
        public string remainingamount { get; set; }
        public string statuscode { get; set; }
        public string materialnumber { get; set; }
        public string isgiftvoucher  { get; set; }
        public string vouchercategory { get; set; }// 'PGV' /'SGV'
        public string assigneddate { get; set; }
        public string salesdate { get; set; }
        public string salesoutlet { get; set; }
        public string salestransaction { get; set; }
        public string bonusbuyid { get; set; }
        public string speciaL4REDEMPTION { get; set; }
        public string redemptiontrans { get; set; }
        public string redemptionamount { get; set; }
        public string redemptiondate { get; set; }
        public string redemptionoutlet { get; set; }
        public string customername  { get; set; }
        public string customeraddress  { get; set; }
        public string vatnumber { get; set; }
        public string identificationcard  { get; set; }
        public string statusofcard { get; set; }
        public string phonenumber  { get; set; }
        public string statusmessage { get; set; }
        public string actionsdate { get; set; }
        public string salesvalue { get; set; }
        public string paymentamount { get; set; }
        public string transactionid { get; set; } 
        public string validfrom { get; set; } 
        public string validto { get; set; } 


    }


    public class OrderTapTapModel
    {
        public string OrderNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public List<string> Voucher { get; set; }
        public string CustomerID { get; set; }
        public string SourceID { get; set; }
        public double Gross_Amount { get; set; }
        public double Amount { get; set; }
        public double Discount { get; set; }
        public string Remarks { get; set; }
        public List<OrderTapTapPayment> Payments { get; set; }
        public List<OrderTapTapItem> LineItems { get; set; }
        public string TransactionID { get; set; }
        public string TypeID { get; set; }
    }

    public class OrderTapTapPayment
    {
        public string Mode { get; set; }
        public double Value { get; set; }
    }

    public class OrderTapTapItem
    {
        [JsonProperty("item_id")]
        public string Item_Id { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        [JsonProperty("total_line_discount")]
        public double TotalLineDiscount { get; set; }
        public double Amount { get; set; }
        [JsonProperty("tax_amount")]
        public double TaxAmount { get; set; }
        public string Remark { get; set; }
    }
}
