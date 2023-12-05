
using RPFO.Data.EntitiesMWI;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.EntitiesVietWashMWI
{
    public class SubmitOrders : TSalesHeader
    {
        //public string DocType { get; set; }
        public string SourceChanel { get; set; }
        public string InvoiceFullName { get; set; }
        public string InvoiceTaxCode { get; set; }
        public string InvoiceEmail { get; set; }
        public string InvoicePhone { get; set; }
        public string InvoiceAddress { get; set; }
        public List<SalesLines> SalesLines { get; set; }
        public List<Entities.TSalesPayment> Payments { get; set; }
        public string PhoneNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string EcomTransId { get; set; }
        public string OMSId { get; set; }
        public decimal? TotalReceipt { get; set; }
        public decimal? PaymentDiscount { get; set; }
        public string DataSource { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
    }

    public class SalesLines : TSalesLine
    {
        public string IsPromo { get; set; }
        public bool? IsSerial { get; set; }
        public bool? IsVoucher { get; set; }
        public string Description { get; set; }
        public string PrepaidCardNo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string EcomTransId { get; set; }
        public int EcomLineId { get; set; }
        //public List<TimeFrameViewModel> TimeFrames { get; set; }

        public List<TSalesLineSerial> LineSerials { get; set; }
    }
    public class TSalesLineSerial
    {
        public string Serial { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }

}
