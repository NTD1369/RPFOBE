using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class SLog
    {
        public Guid? Id { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string Type { get; set; }
        public string TransId { get; set; }
        public string LineNum { get; set; }
        public string Action { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Time { get; set; }
        public string Value { get; set; }
        public string Result { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
        //public DateTime? ModifiedOn { get; set; }

        //public List<MEmployeeStore> Stores { get; set; } = new List<MEmployeeStore>();
    }
}
//[Id]
//      ,[CompanyCode]
//      ,[StoreId]
//      ,[Type]
//      ,[TransId]
//      ,[LineNum]
//      ,[Action]
//      ,[Time]
//      ,[Value]
//      ,[Result]
//      ,[CustomF1]
//      ,[CustomF2]
//      ,[CustomF3]
//      ,[CustomF4]
//      ,[CustomF5] 