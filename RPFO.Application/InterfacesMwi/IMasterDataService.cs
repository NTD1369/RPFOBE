using RPFO.Data.EntitiesMWI;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Application.InterfacesMwi
{
    public interface IMasterDataService
    {
        List<MItem> GetItems(string companyCode, string itemCode );
        //List<MItem> GetItems(string companyCode, string itemCode);
        List<MItemUom> GetItemUom(string itemCode, string uomCode);
        List<MPriceList> GetPriceList(string companyCode, string priceListId, string storeId);
        List<MPriceList> GetPriceCheck(string companyCode, string storeId, string itemCode, string uomCode, string barCode, string date);
        List<MStore> GetStore(string companyCode, string storeId);
        List<MTax> GetTax(string companyCode, string taxCode);
        List<MUom> GetUOM(string companyCode, string uomCode);
        List<MWarehouse> GetWarehouse(string companyCode, string whsCode); 
        
    }
}
