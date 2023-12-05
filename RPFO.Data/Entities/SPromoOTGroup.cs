using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.Entities
{
    public class SPromoOTGroup
    {
        public string PromoId { get; set; }
        public string CompanyCode { get; set; }
        public string GroupID { get; set; }
        public int LineNum { get; set; }
        public string LineType { get; set; }
        public string LineCode { get; set; }
        public string LineName { get; set; }
        public string LineUom { get; set; }

        /// <summary>
        /// Giá trị để đánh dấu dòng OT đã được sự dụng chưa
        /// </summary>
        [JsonIgnore]
        public bool IsCount { get; set;  }
    }
}
