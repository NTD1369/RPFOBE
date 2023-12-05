using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MTableInfor
    {
        //public Guid Id { get; set; }
        /// <summary>
        /// First Name of Supplier Contact.
        /// </summary>
        /// <example>John</example>
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public int TableId { get; set; }
        public string TableName { get; set; }
        public string Description { get; set; }
        public decimal? Height { get; set; }
        public string Width { get; set; }
        public string Longs { get; set; }
        public decimal? Slot { get; set; }
        public string Remark { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string CustomField4 { get; set; }
        public string CustomField5 { get; set; }
        public string DonViDoDai { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string PlaceOfTable { get; set; }
    }
    public partial class MTablePlace
    {
        //public Guid Id { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; } // Không điền thông tin
        public int PlaceId { get; set; }
        public string PlaceName { get; set; } // Không điền thông tin
        public int TableId { get; set; }
        public string TableName { get; set; } // Không điền thông tin
        public string Description { get; set; }
        public string Remark { get; set; }
        public string UrlImage { get; set; }
        public decimal? Height { get; set; }
        public decimal? Width { get; set; }
        public decimal? Longs { get; set; }
        public decimal? Slot { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string CustomField4 { get; set; }
        public string CustomField5 { get; set; }
        public string OrderCustomF1 { get; set; }
        public string OrderCustomF2 { get; set; }
        public string OrderCustomF3 { get; set; }
        public string OrderCustomF4 { get; set; }
        public string OrderCustomF5 { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string TransId { get; set; }
        public bool? IsOrdered { get; set; }
        public string DonViDoDai { get; set; }
        public string IsDefault { get; set; }


    }
    public partial class MPlaceInfor
    {
        //public Guid Id { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public int PlaceId { get; set; }
        public string PlaceName { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public string UrlImage { get; set; }
        public string AssignMap { get; set; }
        public decimal? Height { get; set; }
        public string Width { get; set; }
        public string Longs { get; set; }
        public decimal? Slot { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string CustomField4 { get; set; }
        public string CustomField5 { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string UrlImageSave { get; set; }
        public string DonViDoDai { get; set; }
        public string IsDefault { get; set; }

    }

    public class Page
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int PageColor { get; set; }
        public int PageWidth { get; set; }
        public int PageHeight { get; set; }
        public bool PageLandscape { get; set; }
    }

    public class CustomData
    {
        public string Id { get; set; }
        public string TableId { get; set; }
        public object TableName { get; set; }
        public object Remark { get; set; }
        public object StoreId { get; set; }
        public object IsOrdered { get; set; }
    }

    public class Shape
    {
        public string Key { get; set; }
        public string DataKey { get; set; }
        public CustomData CustomData { get; set; }
        public bool Locked { get; set; }
        public int ZIndex { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class AssignMapModel
    {
        public Page Page { get; set; }
        public List<object> Connectors { get; set; }
        public List<Shape> Shapes { get; set; }
    }

}
