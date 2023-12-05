using RPFO.Data.EntitiesMWI;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
   public class MStoreWarehouseModel: M_StoreWarehouse
    {
        public string MWareHouseID { get; set; }
        public string TWareHouseID { get; set; }
        public List<string> OWareHouseID { get; set; }
    }
}
