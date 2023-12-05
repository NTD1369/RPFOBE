using System;
using System.Collections.Generic;

namespace RPFO.Data.ViewModel
{
    public class PlacePrintViewModel
    {
        public string PrintName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal? Quantity { get; set; }
    }

    public class GroupItemByPrint
    {
        public DateTime? CreatedOn { get; set; }
        public string StoreName { get; set; }

        public string CompanyCode { get; set; }
        public string Store { get; set; }
        public string TableId { get; set; }
        public string PlaceId { get; set; }
        public string TerminalId { get; set; }

        public string TableName { get; set; }
        public string PlaceName { get; set; }
        public string SalesPersonName { get; set; }
        public string PrintName { get; set; }
        public List<GroupedItem> Items { get; set; } = new List<GroupedItem>();
    }

    public class GroupedItem
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal? Quantity { get; set; }
    }
}
