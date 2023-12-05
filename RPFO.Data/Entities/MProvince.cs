using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MProvince
    {
        public string ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string DistrictId { get; set; }
        public string DistrictName { get; set; }
        public string WardId { get; set; }
        public string WardName { get; set; }
        public string WardLevel { get; set; }
        public string ForeignName { get; set; }
        public string AreaCode { get; set; }
        public string CountryCode { get; set; }
        public string RegionCode { get; set; }
        public string Status { get; set; }
    }
}
