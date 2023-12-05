namespace RPFO.Data.ViewModel
{
    public class SplitTableViewModel
    {
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string ShiftId { get; set; }
        public string PlaceId { get; set; }
        public string TableId { get; set; }
        public string GroupKey { get; set; }
    }
    public class TablePlaceViewModel
    {
        public string PlaceName { get; set; }
        public string TableName { get; set; }
    }
    public class SyncPrintNameViewModel
    {
        public string PrintName { get; set; }
        public string PrinterStatus { get; set; }
        public string PortName { get; set; }
        public string ErrorMessage { get; set; }
    }
}
