
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
 
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IItemService
    {
        //Task<List<MItem>> GetAll(string companyCode);
        Task<GenericResult> GetAll(string CompanyCode, string StoreId, string ItemCode, string Keyword, string UomCode, string BarCode,  string Merchandise, string Group,
            string ItemCate1, string ItemCate2, string ItemCate3, string CustomF1, string CustomF2, string CustomF3, string CustomF4, string CustomF5
            , string CustomF6, string CustomF7, string CustomF8, string CustomF9, string CustomF10, string ValidFrom, string ValidTo, bool? IsSerial
            , bool? IsBOM, bool? IsVoucher, bool? IsCapacity, string CustomerGroupId, string PriceListId, string PLU, decimal? PriceFrom, decimal? PriceTo, List<string>? ItemList,decimal? display=null);
        Task<GenericResult> CheckMasterData(string companyCode, string storeId, string keyword, string customerGroupId);
        Task<GenericResult> Import(DataImport model);
        Task<GenericResult> GetItemOtherViewList(string CompanyCode, string StoreId, string ItemCode, string UomCode, string BarCode, string Keyword, string Merchandise, string Type, string CustomerGroupId);
        Task<GenericResult> GetItemViewList(string CompanyCode, string StoreId, string ItemCode, string UomCode, string BarCode, string Keyword,
            string Merchandise, string Type, string customerGroupId, string PriceListId,string LicensePlate);
        Task<GenericResult> GetItemInfor(string CompanyCode, string ItemCode, string UomCode, string BarCode, string Keyword, string Merchandise, string Type, string PriceListId);
        Task<GenericResult> GetItemWithoutPriceList(string CompanyCode, string StoreId, string ItemCode, string UomCode, string BarCode, string Keyword, string Merchandise, string Type, Boolean? iscount = false);
        //Task<PagedList<ItemViewModel>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetPagedList(UserParams userParams);
        Task<GenericResult> GetItemPrice(string CompanyCode, string StoreId, string ItemCode, string UomCode);
        Task<GenericResult> GetByCode(string Code);
        Task<GenericResult> GetItemByCode(string companyCode, string itemCode);
        Task<GenericResult> GetByMer(string MerCode);
        Task<GenericResult> Create(MItem model);
        Task<GenericResult> ApplyStoreListing(StoreListingModel model);
        Task<GenericResult> Update(MItem model);
        Task<GenericResult> Delete(string Code);
        //Task<GenericResult> GetSerialByItem(string CompanyCode, string ItemCode);
        Task<GenericResult> GetSerialByItem(string CompanyCode, string Itemcode, string Keyword, string SlocId);
        Task<PagedList<MItemSerial>> GetPagedListSerialByItem(UserParams userParams);
        Task<PagedList<ItemViewModel>> GetItemWPriceList(UserParams userParams);
        Task<GenericResult> AvartaUpdate(string CompanyCode, string ItemCode, string Url);
        Task<GenericResult> GetItemStockAsync(string companyCode, string storeId, string slocId, string itemCode, string uomCode, string barCode, string serialNum);
        Task<GenericResult> GetItemFilter(string CompanyCode, string StoreId, string ItemCode, string UomCode, string BarCode, string Keyword, string Merchandise, string Group,
            string ItemCate1, string ItemCate2, string ItemCate3, string CustomF1, string CustomF2, string CustomF3, string CustomF4, string CustomF5
            , string CustomF6, string CustomF7, string CustomF8, string CustomF9, string CustomF10, string ValidFrom, string ValidTo, bool? IsSerial
            , bool? IsBOM, bool? IsVoucher, bool? IsCapacity, string CustomerGroupId, string PriceListId, string PLU, decimal? PriceFrom, decimal? PriceTo, bool? isScanner, List<ItemCheckModel> FilterItemList);

        Task<GenericResult> GetItemByVoucherCollection(string CompanyCode, string StoreId, string itemCode);
        Task<GenericResult> GetScanBarcode(string CompanyCode, string UserId, string StoreId, string Keyword, string CustomerGroupId);
        Task<GenericResult> GetVariantItem(string CompanyCode, string ItemCode, string UomCode, string BarCode, string Keyword);
    }
}
