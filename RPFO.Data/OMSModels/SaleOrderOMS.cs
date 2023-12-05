using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.OMSModels
{
    public class SaleOrderOMS
    {
        public string ChanelID { get; set; }
        public string OrderTypeID { get; set; }
        public string SourceID { get; set; }
        public string OrderID { get; set; }
        public string Phonenumber { get; set; }
        public string CustomerName { get; set; }
        public string Note { get; set; }
        public double SubTotal { get; set; }
        public double Discount { get; set; }
        public double VatTotal { get; set; }
        public double Total { get; set; }
        public string Center { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Deliverytype { get; set; }    //PIS - Pick in Store, SHI{} - Ship to Home, NONE - Don't Ship//
        public string Deliverymethod { get; set; }
        public string Deliveryfee { get; set; }
        public List<PaymentOMS> Payment { get; set; }
        public List<OrderDetailOMS> OrderDetail { get; set; }
        public string Customfield1 { get; set; }
        public string Customfield2 { get; set; }
        public DeliveryInforOMS DeliveryInfor { get; set; }
        public InvoiceInforOMS InvoiceInfor { get; set; }
        public string OtherreferenceID { get; set; }
        public string Customertaptapid { get; set; }
    }

    public class DeliveryInforOMS
    {
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string CityName { get; set; }
        public string CityID { get; set; }
        public string DistrictName { get; set; }
        public string DistrictID { get; set; }
        public string WardName { get; set; }
        public string WardID { get; set; }
    }

    public class InvoiceInforOMS
    {
        public string Taxnumber { get; set; }
        public string Companyname { get; set; }
        public string Email { get; set; }
        public string CompanyAddress { get; set; }
    }

    public class PaymentOMS
    {
        public string PaymentType { get; set; }
        public string Amount { get; set; }
        public string TransactionID { get; set; }
        public string PaymentStatus { get; set; }
    }

    public class OrderDetailOMS
    {
        public string Itemcode { get; set; }
        public string ProductName { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double ItemDiscount { get; set; }
        public double VatPrice { get; set; }
        public double TotalPrice { get; set; }
        public string Note { get; set; }
        public List<CustomFieldOMS> CustomField { get; set; }
    }

    public class OrderUpdateOMS
    {
        public string SourceID { get; set; }
        public string OrderID { get; set; }
        public string OrderStatusID { get; set; }
        //public string PaymentStatusID { get; set; }
        //public string DeliveryStatusID { get; set; }
    }

    public class CustomFieldOMS
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Date { get; set; }
    }
}
