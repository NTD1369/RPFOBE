using Newtonsoft.Json;
using System;
namespace RPFO.Data.Models
{
    public partial class SerialNumber
    {
        [JsonProperty("ManufacturerSerialNumber")]
        public String ManufacturerSerialNumber { get; set; }
        //[JsonProperty("InternalSerialNumber")]
        //public String InternalSerialNumber { get; set; }
        //[JsonProperty("ExpiryDate")]
        //public DateTime? ExpiryDate { get; set; }
        //[JsonProperty("ManufactureDate")]
        //public DateTime? ManufactureDate { get; set; }
        //[JsonProperty("ReceptionDate")]
        //public DateTime? ReceptionDate { get; set; }
        //[JsonProperty("WarrantyStart")]
        //public DateTime? WarrantyStart { get; set; }
        //[JsonProperty("WarrantyEnd")]
        //public DateTime? WarrantyEnd { get; set; }
        //[JsonProperty("Location")]
        //public String Location { get; set; }
        //[JsonProperty("Notes")]
        //public String Notes { get; set; }
        //[JsonProperty("BatchID")]
        //public String BatchID { get; set; }
        //[JsonProperty("SystemSerialNumber")]
        //public Int32? SystemSerialNumber { get; set; }
        [JsonProperty("BaseLineNumber")]
        public Int32? BaseLineNumber { get; set; }
        [JsonProperty("Quantity")]
        public Double? Quantity { get; set; }
        //[JsonProperty("TrackingNote")]
        //public Int32? TrackingNote { get; set; }
        [JsonProperty("TrackingNoteLine")]
        public Int32? TrackingNoteLine { get; set; }
        [JsonProperty("ItemCode")]
        public String ItemCode { get; set; }
    }
}


