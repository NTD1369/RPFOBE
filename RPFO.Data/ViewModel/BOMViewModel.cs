
using RPFO.Data.Entities;
using System;
using System.Collections.Generic;

namespace RPFO.Data.ViewModels
{
    public partial class BOMViewModel : MBomheader
    {
        public List<MBomline> Lines { get; set; } = new List<MBomline>();


    }
    public partial class BOMResultViewModel : BOMViewModel
    {
        public bool? Success { get; set; }
        public string Message { get; set; }
    }

    public partial class BOMDataImport
    {
        public string CreatedBy { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public List<BOMViewModel> Data { get; set; }
    }

}
