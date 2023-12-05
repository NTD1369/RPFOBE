
using AutoMapper;
using Dapper;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities; 
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class ItemService : IItemService
    {
        private readonly IGenericRepository<MItem> _itemRepository;
        private readonly IGenericRepository<MItemSerial> _itemSerialRepository;

        private TimeSpan timeQuickAction = TimeSpan.FromMilliseconds(750);
        private IResponseCacheService cacheService;
        private string PrefixCacheGetItem = "QAITM-{0}-{1}";

        private readonly IMapper _mapper;
        private IBOMService _bomService;
        public ItemService(IGenericRepository<MItem> itemRepository, IGenericRepository<MItemSerial> itemSerialRepository, IBOMService bomService, IMapper mapper, IResponseCacheService responseCacheService/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _itemRepository = itemRepository;
            _bomService = bomService;
            _itemSerialRepository = itemSerialRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;
            this.cacheService = responseCacheService;
        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<ItemResultViewModel> resultlist = new List<ItemResultViewModel>();
            try
            {
                foreach (var item in model.Item)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);

                    ItemResultViewModel itemRs = new ItemResultViewModel();
                    itemRs = _mapper.Map<ItemResultViewModel>(item);
                    itemRs.Success = itemResult.Success;
                    itemRs.Message = itemResult.Message;
                    resultlist.Add(itemRs);

                }
                result.Success = true;
                result.Data = resultlist;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;

            }
            return result;
        }

        public async Task<bool> checkExist(string CompanyCode, string ItemCode, string UomCode)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;

            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("ItemCode", ItemCode);
            parameters.Add("UomCode", UomCode);
            parameters.Add("BarCode", "");
            var affectedRows = await _itemRepository.GetAsync("USP_GetItem", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MItem model)
        {

            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;

                parameters.Add("ItemCode", model.ItemCode);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("ProductId", model.ProductId);
                parameters.Add("VariantId", model.VariantId);
                parameters.Add("CapacityValue", model.CapacityValue);
                parameters.Add("ItemGroupId", model.ItemGroupId);
                parameters.Add("SalesTaxCode", model.SalesTaxCode);
                parameters.Add("PurchaseTaxCode", model.PurchaseTaxCode);
                parameters.Add("ItemName", model.ItemName);
                parameters.Add("ItemDescription", model.ItemDescription);
                parameters.Add("ItemCategory_1", model.ItemCategory_1);
                parameters.Add("ItemCategory_2", model.ItemCategory_2);
                parameters.Add("ItemCategory_3", model.ItemCategory_3);
                parameters.Add("ForeignName", model.ForeignName);
                parameters.Add("InventoryUOM", model.InventoryUom);
                parameters.Add("ImageURL", model.ImageUrl);
                parameters.Add("ImageLink", model.ImageLink);
                parameters.Add("MCId", model.Mcid);
                parameters.Add("CustomField1", model.CustomField1);
                parameters.Add("CustomField2", model.CustomField2);
                parameters.Add("CustomField3", model.CustomField3);
                parameters.Add("CustomField4", model.CustomField4);
                parameters.Add("CustomField5", model.CustomField5);
                parameters.Add("CustomField6", model.CustomField6);
                parameters.Add("CustomField7", model.CustomField7);
                parameters.Add("CustomField8", model.CustomField8);
                parameters.Add("CustomField9", model.CustomField9);
                parameters.Add("CustomField10", model.CustomField10);
                parameters.Add("DefaultPrice", model.DefaultPrice);
                parameters.Add("IsSerial", model.IsSerial);
                parameters.Add("IsBOM", model.IsBom);
                parameters.Add("IsVoucher", model.IsVoucher);
                parameters.Add("ValidFrom", model.ValidFrom);
                parameters.Add("ValidTo", model.ValidTo);
                parameters.Add("Status", model.Status);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("RejectPayType", model.RejectPayType);
                parameters.Add("Returnable", model.Returnable);
                parameters.Add("VoucherCollection", model.VoucherCollection);
                parameters.Add("IsPriceTime", model.IsPriceTime);
                //var exist = await checkExist(model.CompanyCode, model.ItemCode, model.InventoryUom);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.ItemCode + " uom " + model.InventoryUom + " existed.";
                //    return result;
                //}
                var affectedRows = _itemRepository.Insert("USP_I_M_Item", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                //result.Message = key;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        
        public async Task<GenericResult> ApplyStoreListing(StoreListingModel model)
        {

            GenericResult result = new GenericResult();
            try
            {
                string SqlStr = "";

                foreach (var item in model.ItemList)
                {
                    SqlStr += $"delete M_ItemStoreListing where ItemCode = N'{item.ItemCode}' and CompanyCode = N'{model.CompanyCode}'; ";
                    foreach (var store in model.StoreList)
                    {
                        SqlStr += $" insert into  M_ItemStoreListing (CompanyCode, StoreId, ItemCode, Status, CreatedBy, CreatedOn)  " +
                            $"values (N'{model.CompanyCode}', N'{store.StoreId}', N'{item.ItemCode}', N'A', N'{model.CreatedBy}', GETDATE()); ";
                    }

                } 
                if(!string.IsNullOrEmpty(SqlStr))
                {
                    var affectedRows = _itemRepository.Insert(SqlStr, null, commandType: CommandType.Text);
                    result.Success = true;
                }    
                else
                {
                    result.Message = "Can't generate query. Please contact to support team";
                    result.Success = false;
                }    

                //result.Message = key;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }


        public async Task<GenericResult> Delete(string Code)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetItemFilter(string CompanyCode, string StoreId, string ItemCode, string UomCode, string BarCode, string Keyword, string Merchandise, string Group,
            string ItemCate1, string ItemCate2, string ItemCate3, string CustomF1, string CustomF2, string CustomF3, string CustomF4, string CustomF5
            , string CustomF6, string CustomF7, string CustomF8, string CustomF9, string CustomF10, string ValidFrom, string ValidTo, bool? IsSerial
            , bool? IsBOM, bool? IsVoucher, bool? IsCapacity, string CustomerGroupId, string PriceListId, string PLU, decimal? PriceFrom, decimal? PriceTo, bool? isScanner, List<ItemCheckModel> FilterItemList)
        {
            GenericResult result = new GenericResult();
            if (isScanner == true)
            {
                string keyCache = string.Format(PrefixCacheGetItem, $"{PLU}+{StoreId}+{ItemCode}+{UomCode}", BarCode);
                string storeCache = cacheService.GetCachedData<string>(keyCache);
                if (string.IsNullOrEmpty(storeCache))
                {
                    cacheService.CacheData<string>(keyCache, keyCache, timeQuickAction);
                }
                else
                {
                    result.Success = false;
                    result.Message = "Your actions are too fast and too dangerous.";
                    return result;
                }
            }
            using (IDbConnection db = _itemRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode);
                    parameters.Add("StoreId", string.IsNullOrEmpty(StoreId) ? "" : StoreId);
                    //parameters.Add("ItemCode", string.IsNullOrEmpty(ItemCode) ? "" : ItemCode);
                    //parameters.Add("UomCode", string.IsNullOrEmpty(UomCode) ? "" : UomCode);
                    //parameters.Add("BarCode", string.IsNullOrEmpty(BarCode) ? "" : BarCode);
                    parameters.Add("Keyword", string.IsNullOrEmpty(Keyword) ? "" : Keyword);
                    parameters.Add("Merchandise", string.IsNullOrEmpty(Merchandise) ? "" : Merchandise);
                    parameters.Add("Group", string.IsNullOrEmpty(Group) ? "" : Group);
                    parameters.Add("ItemCate1", string.IsNullOrEmpty(ItemCate1) ? "" : ItemCate1);
                    parameters.Add("ItemCate2", string.IsNullOrEmpty(ItemCate2) ? "" : ItemCate2);
                    parameters.Add("ItemCate3", string.IsNullOrEmpty(ItemCate3) ? "" : ItemCate3);
                    parameters.Add("CustomF1", string.IsNullOrEmpty(CustomF1) ? "" : CustomF1);
                    parameters.Add("CustomF2", string.IsNullOrEmpty(CustomF2) ? "" : CustomF2);
                    parameters.Add("CustomF3", string.IsNullOrEmpty(CustomF3) ? "" : CustomF3);
                    parameters.Add("CustomF4", string.IsNullOrEmpty(CustomF4) ? "" : CustomF4);
                    parameters.Add("CustomF5", string.IsNullOrEmpty(CustomF5) ? "" : CustomF5);
                    parameters.Add("CustomF6", string.IsNullOrEmpty(CustomF6) ? "" : CustomF6);
                    parameters.Add("CustomF7", string.IsNullOrEmpty(CustomF7) ? "" : CustomF7);
                    parameters.Add("CustomF8", string.IsNullOrEmpty(CustomF8) ? "" : CustomF8);
                    parameters.Add("CustomF9", string.IsNullOrEmpty(CustomF9) ? "" : CustomF9);
                    parameters.Add("CustomF10", string.IsNullOrEmpty(CustomF10) ? "" : CustomF10);
                    parameters.Add("ValidFrom", string.IsNullOrEmpty(ValidFrom) ? null : ValidFrom);
                    parameters.Add("ValidTo", string.IsNullOrEmpty(ValidTo) ? null : ValidTo);
                    parameters.Add("IsSerial", IsSerial);
                    parameters.Add("IsBOM", IsBOM);
                    parameters.Add("IsVoucher", IsVoucher);
                    parameters.Add("IsCapacity", IsCapacity);
                    if (!string.IsNullOrEmpty(CustomerGroupId))
                    {
                        parameters.Add("CustomerGroupId", string.IsNullOrEmpty(CustomerGroupId) ? "" : CustomerGroupId);
                    }
                    if (!string.IsNullOrEmpty(PriceListId))
                    {
                        parameters.Add("PriceListId", string.IsNullOrEmpty(PriceListId) ? "" : PriceListId);
                    }
                    //if (!string.IsNullOrEmpty(PLU))
                    //{
                    //    parameters.Add("PLU", PLU);
                    //}
                    if (PriceFrom.HasValue)
                    {
                        parameters.Add("PriceFrom", PriceFrom);
                    }
                    if (PriceTo.HasValue)
                    {
                        parameters.Add("PriceTo", PriceTo);
                    }
                    List<ItemViewModel> ItemList = new List<ItemViewModel>();

                    if(FilterItemList!=null && FilterItemList.Count() > 0)
                    {
                        //string ItemFilter = "";
                        //foreach(var item in FilterItemList)
                        //{
                        //    ItemFilter += item.ItemCode + "~" + item.UomCode + "~" + item.Quantity + "~" + item.SlocId + "~" + item.Barcode + "~" + item.PLU + ";";
                        //}    
                        //parameters.Add("FilterList", ItemFilter);
                        List<ItemViewModel> list = new List<ItemViewModel>();
                        foreach (var item in FilterItemList)
                        {
                            parameters.Add("ItemCode", string.IsNullOrEmpty(item.ItemCode) ? "" : item.ItemCode);
                            parameters.Add("UomCode", string.IsNullOrEmpty(item.UomCode) ? "" : item.UomCode);
                            parameters.Add("BarCode", string.IsNullOrEmpty(item.Barcode) ? "" : item.Barcode);
                            if (!string.IsNullOrEmpty(item.PLU))
                            {
                                parameters.Add("PLU", PLU);
                            }
                            var itemData = db.Query<ItemViewModel>($"USP_GetItem_Filter", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 600);
                            if(itemData!=null)
                            {
                                var itemAdd = itemData.FirstOrDefault();
                                if (itemAdd!=null)
                                {
                                    if(item.Quantity!=null)
                                    {
                                        itemAdd.Quantity = (decimal) item.Quantity;
                                    }    
                                    list.Add(itemAdd);
                                }    
                               
                            }    
                        }
                        result.Success = true;
                        result.Data = list.ToList();
                    }
                    else
                    {
                        parameters.Add("ItemCode", string.IsNullOrEmpty(ItemCode) ? "" : ItemCode);
                        parameters.Add("UomCode", string.IsNullOrEmpty(UomCode) ? "" : UomCode);
                        parameters.Add("BarCode", string.IsNullOrEmpty(BarCode) ? "" : BarCode);
                        if (!string.IsNullOrEmpty(PLU))
                        {
                            parameters.Add("PLU", PLU);
                        }
                        var dblist = await db.QueryAsync<ItemViewModel>($"USP_GetItem_Filter", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 600);
                        result.Success = true;
                        result.Data = dblist.ToList();
                    }    
                    

                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }
            return result;

        }
      
        public async Task<GenericResult> GetItemWithoutPriceList(string CompanyCode, string StoreId, string ItemCode, string UomCode, string BarCode, string Keyword, string Merchandise, string Type, Boolean? Iscount = false)
        {
            GenericResult result = new GenericResult();
            string keyCache = string.Format(PrefixCacheGetItem, $"{StoreId}+{ItemCode}+{UomCode}", BarCode);
            string storeCache = cacheService.GetCachedData<string>(keyCache);
            if (string.IsNullOrEmpty(storeCache))
            {
                cacheService.CacheData<string>(keyCache, keyCache, timeQuickAction);
            }
            else
            {
                result.Success = false;
                result.Message = "Your actions are too fast and too dangerous.";
                return result;
            }
            using (IDbConnection db = _itemRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode);
                    parameters.Add("StoreId", string.IsNullOrEmpty(StoreId) ? "" : StoreId);
                    parameters.Add("ItemCode", string.IsNullOrEmpty(ItemCode) ? "" : ItemCode);
                    parameters.Add("UomCode", string.IsNullOrEmpty(UomCode) ? "" : UomCode);
                    parameters.Add("BarCode", string.IsNullOrEmpty(BarCode) ? "" : BarCode);
                    parameters.Add("Keyword", string.IsNullOrEmpty(Keyword) ? "" : Keyword);
                    parameters.Add("Merchandise", string.IsNullOrEmpty(Merchandise) ? "" : Merchandise);
                    parameters.Add("Type", string.IsNullOrEmpty(Type) ? "" : Type);
                    if(Iscount == true) { parameters.Add("Iscount", Iscount); }
                        
                    var dblist = await db.QueryAsync<ItemViewModel>($"USP_GetItemWithoutPrice", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 600);
                    dblist = dblist.Where(x => x.CustomField8 != "999999");
                    db.Close();
                    result.Success = true;
                    result.Data = dblist.ToList();
                    //return dblist.ToList(); 
                }
                catch (Exception ex)
                {
                    db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }
            return result;
        }
        public async Task<GenericResult> GetItemViewList(string CompanyCode, string StoreId, string ItemCode, string UomCode, string BarCode,
            string Keyword, string Merchandise, string Type, string CustomerGroupId, string PriceListId,string licensePlate)
        {
            GenericResult result = new GenericResult();
            string keyCache = string.Format(PrefixCacheGetItem, $"{StoreId}+{ItemCode}+{UomCode}", BarCode);
            string storeCache = cacheService.GetCachedData<string>(keyCache);
            if (string.IsNullOrEmpty(storeCache))
            {
                cacheService.CacheData<string>(keyCache, keyCache, timeQuickAction);
            }
            else
            {
                result.Success = false;
                result.Message = "Your actions are too fast and too dangerous.";
                return result;
            }
            using (IDbConnection db = _itemRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode);
                    parameters.Add("StoreId", string.IsNullOrEmpty(StoreId) ? "" : StoreId);
                    parameters.Add("ItemCode", string.IsNullOrEmpty(ItemCode) ? "" : ItemCode);
                    parameters.Add("UomCode", string.IsNullOrEmpty(UomCode) ? "" : UomCode);
                    parameters.Add("BarCode", string.IsNullOrEmpty(BarCode) ? "" : BarCode);
                    parameters.Add("Keyword", string.IsNullOrEmpty(Keyword) ? "" : Keyword);
                    parameters.Add("Merchandise", string.IsNullOrEmpty(Merchandise) ? "" : Merchandise);
                    parameters.Add("Type", string.IsNullOrEmpty(Type) ? "" : Type);
                    parameters.Add("LicensePlate", string.IsNullOrEmpty(licensePlate) ? "" : licensePlate);
                    if (!string.IsNullOrEmpty(CustomerGroupId))
                    {
                        parameters.Add("CustomerGroupId", string.IsNullOrEmpty(CustomerGroupId) ? "" : CustomerGroupId);
                    }
                    if (!string.IsNullOrEmpty(PriceListId))
                    {
                        parameters.Add("PriceListId", string.IsNullOrEmpty(PriceListId) ? "" : PriceListId);
                    }
                    //parameters.Add("CustomerId", string.IsNullOrEmpty(CustomerId) ? "" : CustomerId);
                    var dblist = await db.QueryAsync<ItemViewModel>($"USP_GetItem", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 600);
                    dblist = dblist.Where(x => x.CustomField8 != "999999");
                    //foreach(var item in dblist)
                    //{
                    //    if(item.IsBom== true)
                    //    {
                    //       var bOMViewModel =  _bomService.GetByItemCode(item.ItemCode);
                    //       if(bOMViewModel.Result!=null && bOMViewModel.Result.Lines.Count > 0)
                    //        {
                    //            item.Lines = bOMViewModel.Result.Lines;
                    //        }    
                    //    }    
                    //}    
                    db.Close();
                    result.Success = true;
                    result.Data = dblist.ToList();

                }
                catch (Exception ex)
                {
                    db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }
            return result;
        }

        public async Task<GenericResult> GetItemOtherViewList(string CompanyCode, string StoreId, string ItemCode, string UomCode, string BarCode, string Keyword, string Merchandise, string Type, string CustomerGroupId)
        {
            GenericResult result = new GenericResult();
            string keyCache = string.Format(PrefixCacheGetItem, $"{StoreId}+{ItemCode}+{UomCode}", BarCode);
            string storeCache = cacheService.GetCachedData<string>(keyCache);
            if (string.IsNullOrEmpty(storeCache))
            {
                cacheService.CacheData<string>(keyCache, keyCache, timeQuickAction);
            }
            else
            {
                result.Success = false;
                result.Message = "Your actions are too fast and too dangerous.";
                return result;
            }
            using (IDbConnection db = _itemRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode);
                    parameters.Add("StoreId", string.IsNullOrEmpty(StoreId) ? "" : StoreId);
                    parameters.Add("ItemCode", string.IsNullOrEmpty(ItemCode) ? "" : ItemCode);
                    parameters.Add("UomCode", string.IsNullOrEmpty(UomCode) ? "" : UomCode);
                    parameters.Add("BarCode", string.IsNullOrEmpty(BarCode) ? "" : BarCode);
                    parameters.Add("Keyword", string.IsNullOrEmpty(Keyword) ? "" : Keyword);
                    parameters.Add("Merchandise", string.IsNullOrEmpty(Merchandise) ? "" : Merchandise);
                    parameters.Add("Type", string.IsNullOrEmpty(Type) ? "" : Type);
                    if (!string.IsNullOrEmpty(CustomerGroupId))
                    {
                        parameters.Add("CustomerGroupId", string.IsNullOrEmpty(CustomerGroupId) ? "" : CustomerGroupId);
                    }
                    //parameters.Add("CustomerId", string.IsNullOrEmpty(CustomerId) ? "" : CustomerId);
                    var dblist = await db.QueryAsync<ItemViewModel>($"USP_GetItemCustom", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 600);
                    dblist = dblist.Where(x => x.CustomField8 != "999999");

                    db.Close();
                    result.Success = true;
                    result.Data = dblist.ToList();

                }
                catch (Exception ex)
                {
                    db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }
            return result;
        }
        public async Task<GenericResult> GetVariantItem(string CompanyCode, string ItemCode, string UomCode, string BarCode, string Keyword)
        {
            GenericResult result = new GenericResult();
            
            using (IDbConnection db = _itemRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode);
                    parameters.Add("ItemCode", string.IsNullOrEmpty(ItemCode) ? "" : ItemCode);
                    parameters.Add("UomCode", string.IsNullOrEmpty(UomCode) ? "" : UomCode);
                    parameters.Add("BarCode", string.IsNullOrEmpty(BarCode) ? "" : BarCode);
                    parameters.Add("Keyword", string.IsNullOrEmpty(Keyword) ? "" : Keyword);
                     
                    var dblist = await db.QueryAsync<ItemViewModel>($"USP_GetVariantItem", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 600);

 
                    db.Close();
                    //return dblist.ToList();
                    result.Success = true;
                    result.Data = dblist.ToList();
                }
                catch (Exception ex)
                {
                    db.Close();
                    //return null;
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }
            return result;
        }

        public async Task<GenericResult> GetItemInfor(string CompanyCode, string ItemCode, string UomCode, string BarCode, string Keyword, string Merchandise, string Type, string PriceListId  )
        {
            GenericResult result = new GenericResult();
            string keyCache = string.Format(PrefixCacheGetItem, $"{ItemCode}+{UomCode}", BarCode);
            string storeCache = cacheService.GetCachedData<string>(keyCache);
            if (string.IsNullOrEmpty(storeCache))
            {
                cacheService.CacheData<string>(keyCache, keyCache, timeQuickAction);
            }
            else
            {
                result.Success = false;
                result.Message = "Your actions are too fast and too dangerous.";
                return result;
            }
            using (IDbConnection db = _itemRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode);
                    parameters.Add("ItemCode", string.IsNullOrEmpty(ItemCode) ? "" : ItemCode);
                    parameters.Add("UomCode", string.IsNullOrEmpty(UomCode) ? "" : UomCode);
                    parameters.Add("BarCode", string.IsNullOrEmpty(BarCode) ? "" : BarCode);
                    parameters.Add("Keyword", string.IsNullOrEmpty(Keyword) ? "" : Keyword);
                    parameters.Add("Merchandise", string.IsNullOrEmpty(Merchandise) ? "" : Merchandise);
                    parameters.Add("Type", string.IsNullOrEmpty(Type) ? "" : Type);
                    //parameters.Add("IsMaster", IsMaster.HasValue ? IsMaster.Value : false);
                    if (!string.IsNullOrEmpty(PriceListId))
                    {
                        parameters.Add("PriceListId", string.IsNullOrEmpty(PriceListId) ? "" : PriceListId);
                    }
                    var dblist = await db.QueryAsync<ItemViewModel>($"USP_GetItemInfor", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 600);
                   
                    
                    //foreach(var item in dblist)
                    //{
                    //    if(item.IsBom== true)
                    //    {
                    //       var bOMViewModel =  _bomService.GetByItemCode(item.ItemCode);
                    //       if(bOMViewModel.Result!=null && bOMViewModel.Result.Lines.Count > 0)
                    //        {
                    //            item.Lines = bOMViewModel.Result.Lines;
                    //        }    
                    //    }    
                    //}    
                    db.Close();
                    //return dblist.ToList();
                    result.Success = true;
                    result.Data = dblist.ToList();
                }
                catch (Exception ex)
                {
                    db.Close();
                    //return null;
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }
            return result;
        }

        public async Task<GenericResult> GetItemPrice(string CompanyCode, string StoreId, string ItemCode, string UomCode)
        {
            GenericResult result = new GenericResult();

            string keyCache = string.Format(PrefixCacheGetItem, StoreId, $"{ItemCode}+{UomCode}");
            string storeCache = cacheService.GetCachedData<string>(keyCache);
            if (string.IsNullOrEmpty(storeCache))
            {
                cacheService.CacheData<string>(keyCache, keyCache, timeQuickAction);
            }
            else
            {
                result.Success = false;
                result.Message = "Your actions are too fast and too dangerous.";
                return result;
            }

            using (IDbConnection db = _itemRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode);
                    parameters.Add("StoreId", string.IsNullOrEmpty(StoreId) ? "" : StoreId);
                    parameters.Add("ItemCode", string.IsNullOrEmpty(ItemCode) ? "" : ItemCode);
                    parameters.Add("UomCode", string.IsNullOrEmpty(UomCode) ? "" : UomCode);

                    var dblist = await db.QueryAsync<ItemViewModel>($"USP_GetPriceItem", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 600);

                    db.Close();
                    //return dblist.FirstOrDefault();
                    result.Success = true;
                    result.Data = dblist.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                }

            }
            return result;
        }

        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, string ItemCode, string Keyword, string UomCode, string BarCode, string Merchandise, string Group,
            string ItemCate1, string ItemCate2, string ItemCate3, string CustomF1, string CustomF2, string CustomF3, string CustomF4, string CustomF5
            , string CustomF6, string CustomF7, string CustomF8, string CustomF9, string CustomF10, string ValidFrom, string ValidTo, bool? IsSerial
            , bool? IsBOM, bool? IsVoucher, bool? IsCapacity, string CustomerGroupId, string PriceListId, string PLU, decimal? PriceFrom, decimal? PriceTo, List<string>? ItemList,decimal? display =null)
        {
            GenericResult rs = new GenericResult();
            using (IDbConnection db = _itemRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    var resultList = new List<ItemViewModel>();

                    if (ItemList!=null && ItemList.Count > 0)
                    {
                        foreach(var item in ItemList)
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode);
                            parameters.Add("StoreId", "");
                            parameters.Add("ItemCode", string.IsNullOrEmpty(item) ? "" : item);
                            parameters.Add("Keyword", "");

                            ////Thêm điều kiện filter mới
                            //if (!string.IsNullOrEmpty(UomCode))
                            //{
                            //    parameters.Add("UomCode", string.IsNullOrEmpty(UomCode) ? "" : UomCode);
                            //}
                            //if (!string.IsNullOrEmpty(BarCode))
                            //{
                            //    parameters.Add("BarCode", string.IsNullOrEmpty(BarCode) ? "" : BarCode);
                            //}
                            //if (!string.IsNullOrEmpty(Merchandise))
                            //{
                            //    parameters.Add("Merchandise", string.IsNullOrEmpty(Merchandise) ? "" : Merchandise);
                            //}
                            //if (!string.IsNullOrEmpty(CustomF2))
                            //{
                            //    parameters.Add("CustomF2", string.IsNullOrEmpty(CustomF2) ? "" : CustomF2);
                            //}
                            //if (!string.IsNullOrEmpty(CustomF1))
                            //{
                            //    parameters.Add("CustomF1", string.IsNullOrEmpty(CustomF1) ? "" : CustomF1);
                            //}
                            //if (!string.IsNullOrEmpty(Merchandise))
                            //{
                            //    parameters.Add("Merchandise", string.IsNullOrEmpty(Merchandise) ? "" : Merchandise);
                            //}
                            //if (!string.IsNullOrEmpty(ItemCate3))
                            //{
                            //    parameters.Add("ItemCate3", string.IsNullOrEmpty(ItemCate3) ? "" : ItemCate3);
                            //}
                            //if (!string.IsNullOrEmpty(ItemCate2))
                            //{
                            //    parameters.Add("ItemCate2", string.IsNullOrEmpty(ItemCate2) ? "" : ItemCate2);
                            //}
                            //if (!string.IsNullOrEmpty(ItemCate1))
                            //{
                            //    parameters.Add("ItemCate1", string.IsNullOrEmpty(ItemCate1) ? "" : ItemCate1);
                            //}
                            //if (!string.IsNullOrEmpty(Group))
                            //{
                            //    parameters.Add("Group", string.IsNullOrEmpty(Group) ? "" : Group);
                            //}

                            //if (!string.IsNullOrEmpty(CustomF3))
                            //{
                            //    parameters.Add("CustomF3", string.IsNullOrEmpty(CustomF3) ? "" : CustomF3);
                            //}
                            //if (!string.IsNullOrEmpty(CustomF4))
                            //{
                            //    parameters.Add("CustomF4", string.IsNullOrEmpty(CustomF4) ? "" : CustomF4);
                            //}
                            //if (!string.IsNullOrEmpty(CustomF5))
                            //{
                            //    parameters.Add("CustomF5", string.IsNullOrEmpty(CustomF5) ? "" : CustomF5);
                            //}
                            //if (!string.IsNullOrEmpty(CustomF6))
                            //{
                            //    parameters.Add("CustomF6", string.IsNullOrEmpty(CustomF6) ? "" : CustomF6);
                            //}
                            //if (!string.IsNullOrEmpty(CustomF7))
                            //{
                            //    parameters.Add("CustomF7", string.IsNullOrEmpty(CustomF7) ? "" : CustomF7);
                            //}
                            //if (!string.IsNullOrEmpty(CustomF8))
                            //{
                            //    parameters.Add("CustomF8", string.IsNullOrEmpty(CustomF8) ? "" : CustomF8);
                            //}

                            //if (!string.IsNullOrEmpty(CustomF9))
                            //{
                            //    parameters.Add("CustomF9", string.IsNullOrEmpty(CustomF9) ? "" : CustomF9);
                            //}
                            //if (!string.IsNullOrEmpty(CustomF10))
                            //{
                            //    parameters.Add("CustomF10", string.IsNullOrEmpty(CustomF10) ? "" : CustomF10);
                            //}

                            //if (!string.IsNullOrEmpty(ValidFrom))
                            //{
                            //    parameters.Add("ValidFrom", string.IsNullOrEmpty(ValidFrom) ? null : ValidFrom);
                            //}
                            //if (!string.IsNullOrEmpty(ValidTo))
                            //{
                            //    parameters.Add("ValidTo", string.IsNullOrEmpty(ValidTo) ? null : ValidTo);
                            //}

                            //if (IsSerial.HasValue)
                            //{
                            //    parameters.Add("IsSerial", IsSerial);
                            //}
                            //if (IsBOM.HasValue)
                            //{
                            //    parameters.Add("IsBOM", IsBOM);
                            //}
                            //if (IsVoucher.HasValue)
                            //{
                            //    parameters.Add("IsVoucher", IsVoucher);
                            //}
                            //if (IsCapacity.HasValue)
                            //{
                            //    parameters.Add("IsCapacity", IsCapacity);
                            //}
                            //if (!string.IsNullOrEmpty(CustomerGroupId))
                            //{
                            //    parameters.Add("CustomerGroupId", string.IsNullOrEmpty(CustomerGroupId) ? "" : CustomerGroupId);
                            //}
                            //if (!string.IsNullOrEmpty(PriceListId))
                            //{
                            //    parameters.Add("PriceListId", string.IsNullOrEmpty(PriceListId) ? "" : PriceListId);
                            //}
                            //if (!string.IsNullOrEmpty(PLU))
                            //{
                            //    parameters.Add("PLU", PLU);
                            //}
                            //if (PriceFrom.HasValue)
                            //{
                            //    parameters.Add("PriceFrom", PriceFrom);
                            //}
                            //if (PriceTo.HasValue)
                            //{
                            //    parameters.Add("PriceTo", PriceTo);
                            //}


                            var resultItemList = await db.QueryAsync<ItemViewModel>($"USP_GetAllItemInfor", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 600);

                            resultList.AddRange(resultItemList);
                            //foreach(var item in dblist)
                            //{
                            //    if(item.IsBom== true)
                            //    {
                            //       var bOMViewModel =  _bomService.GetByItemCode(item.ItemCode);
                            //       if(bOMViewModel.Result!=null && bOMViewModel.Result.Lines.Count > 0)
                            //        {
                            //            item.Lines = bOMViewModel.Result.Lines;
                            //        }    
                            //    }    
                            //}    
                        }
                    }   
                    else
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode);
                        parameters.Add("StoreId", string.IsNullOrEmpty(StoreId) ? "" : StoreId);
                        parameters.Add("ItemCode", string.IsNullOrEmpty(ItemCode) ? "" : ItemCode);
                        parameters.Add("Keyword", string.IsNullOrEmpty(Keyword) ? "" : Keyword);
                        if(display !=null)
                        {
                            parameters.Add("Selectop",  display);
                        }    
                        //Thêm điều kiện filter mới
                        if (!string.IsNullOrEmpty(UomCode))
                        {
                            parameters.Add("UomCode", string.IsNullOrEmpty(UomCode) ? "" : UomCode);
                        }
                        if (!string.IsNullOrEmpty(BarCode))
                        {
                            parameters.Add("BarCode", string.IsNullOrEmpty(BarCode) ? "" : BarCode);
                        }
                        if (!string.IsNullOrEmpty(Merchandise))
                        {
                            parameters.Add("Merchandise", string.IsNullOrEmpty(Merchandise) ? "" : Merchandise);
                        }
                        if (!string.IsNullOrEmpty(CustomF2))
                        {
                            parameters.Add("CustomF2", string.IsNullOrEmpty(CustomF2) ? "" : CustomF2);
                        }
                        if (!string.IsNullOrEmpty(CustomF1))
                        {
                            parameters.Add("CustomF1", string.IsNullOrEmpty(CustomF1) ? "" : CustomF1);
                        }
                        if (!string.IsNullOrEmpty(Merchandise))
                        {
                            parameters.Add("Merchandise", string.IsNullOrEmpty(Merchandise) ? "" : Merchandise);
                        }
                        if (!string.IsNullOrEmpty(ItemCate3))
                        {
                            parameters.Add("ItemCate3", string.IsNullOrEmpty(ItemCate3) ? "" : ItemCate3);
                        }
                        if (!string.IsNullOrEmpty(ItemCate2))
                        {
                            parameters.Add("ItemCate2", string.IsNullOrEmpty(ItemCate2) ? "" : ItemCate2);
                        }
                        if (!string.IsNullOrEmpty(ItemCate1))
                        {
                            parameters.Add("ItemCate1", string.IsNullOrEmpty(ItemCate1) ? "" : ItemCate1);
                        }
                        if (!string.IsNullOrEmpty(Group))
                        {
                            parameters.Add("Group", string.IsNullOrEmpty(Group) ? "" : Group);
                        }

                        if (!string.IsNullOrEmpty(CustomF3))
                        {
                            parameters.Add("CustomF3", string.IsNullOrEmpty(CustomF3) ? "" : CustomF3);
                        }
                        if (!string.IsNullOrEmpty(CustomF4))
                        {
                            parameters.Add("CustomF4", string.IsNullOrEmpty(CustomF4) ? "" : CustomF4);
                        }
                        if (!string.IsNullOrEmpty(CustomF5))
                        {
                            parameters.Add("CustomF5", string.IsNullOrEmpty(CustomF5) ? "" : CustomF5);
                        }
                        if (!string.IsNullOrEmpty(CustomF6))
                        {
                            parameters.Add("CustomF6", string.IsNullOrEmpty(CustomF6) ? "" : CustomF6);
                        }
                        if (!string.IsNullOrEmpty(CustomF7))
                        {
                            parameters.Add("CustomF7", string.IsNullOrEmpty(CustomF7) ? "" : CustomF7);
                        }
                        if (!string.IsNullOrEmpty(CustomF8))
                        {
                            parameters.Add("CustomF8", string.IsNullOrEmpty(CustomF8) ? "" : CustomF8);
                        }

                        if (!string.IsNullOrEmpty(CustomF9))
                        {
                            parameters.Add("CustomF9", string.IsNullOrEmpty(CustomF9) ? "" : CustomF9);
                        }
                        if (!string.IsNullOrEmpty(CustomF10))
                        {
                            parameters.Add("CustomF10", string.IsNullOrEmpty(CustomF10) ? "" : CustomF10);
                        }

                        if (!string.IsNullOrEmpty(ValidFrom))
                        {
                            parameters.Add("ValidFrom", string.IsNullOrEmpty(ValidFrom) ? null : ValidFrom);
                        }
                        if (!string.IsNullOrEmpty(ValidTo))
                        {
                            parameters.Add("ValidTo", string.IsNullOrEmpty(ValidTo) ? null : ValidTo);
                        }

                        if (IsSerial.HasValue)
                        {
                            parameters.Add("IsSerial", IsSerial);
                        }
                        if (IsBOM.HasValue)
                        {
                            parameters.Add("IsBOM", IsBOM);
                        }
                        if (IsVoucher.HasValue)
                        {
                            parameters.Add("IsVoucher", IsVoucher);
                        }
                        if (IsCapacity.HasValue)
                        {
                            parameters.Add("IsCapacity", IsCapacity);
                        }
                        if (!string.IsNullOrEmpty(CustomerGroupId))
                        {
                            parameters.Add("CustomerGroupId", string.IsNullOrEmpty(CustomerGroupId) ? "" : CustomerGroupId);
                        }
                        if (!string.IsNullOrEmpty(PriceListId))
                        {
                            parameters.Add("PriceListId", string.IsNullOrEmpty(PriceListId) ? "" : PriceListId);
                        }
                        if (!string.IsNullOrEmpty(PLU))
                        {
                            parameters.Add("PLU", PLU);
                        }
                        if (PriceFrom.HasValue)
                        {
                            parameters.Add("PriceFrom", PriceFrom);
                        }
                        if (PriceTo.HasValue)
                        {
                            parameters.Add("PriceTo", PriceTo);
                        }


                        resultList =  db.QueryAsync<ItemViewModel>($"USP_GetAllItemInfor", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 600).Result.ToList();
                        //foreach(var item in dblist)
                        //{
                        //    if(item.IsBom== true)
                        //    {
                        //       var bOMViewModel =  _bomService.GetByItemCode(item.ItemCode);
                        //       if(bOMViewModel.Result!=null && bOMViewModel.Result.Lines.Count > 0)
                        //        {
                        //            item.Lines = bOMViewModel.Result.Lines;
                        //        }    
                        //    }    
                        //}    
                    }

                    db.Close();
                    rs.Success = true;
                    rs.Data = resultList;


                }
                catch (Exception ex)
                {
                    db.Close();
                    rs.Success = false;
                    rs.Message = ex.Message;
                }
                return rs;
            }
            //try
            //{
            //    var data = await _itemRepository.GetAllAsync($"select * from M_Item with (nolock) where CompanyCode='{companyCode}'", null, commandType: CommandType.Text);

            //    return data;
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}

        }


        public async Task<GenericResult> CheckMasterData(string companyCode, string storeId, string keyword, string CustomerGroupId)
        {
            GenericResult result = new GenericResult();
            try
            {
                using (IDbConnection db = _itemRepository.GetConnection())
                {
                    try
                    {
                        var item = new ItemViewModel();
                        if (db.State == ConnectionState.Closed)
                            db.Open(); 
                        string query = $"[USP_Check_ItemMasterData] '{companyCode}','{storeId}', N'{keyword}', N'{CustomerGroupId}'";
                        var data = await db.QueryAsync(query, null, commandType: CommandType.Text, commandTimeout: 600);
                        //parameters.Add("CustomerId", "");
                        //var data = await db.QueryAsync<ItemViewModel>($"USP_GetItem", parameters, commandType: CommandType.StoredProcedure);
                        //var data = await _itemRepository.GetAsync($"select * from M_Item with (nolock) where  CompanyCode= '{companyCode}' and ItemCode = '{itemCode}'", null, commandType: CommandType.Text);
                        //return data;
                        result.Success = true;
                        result.Data = data;
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.Message = ex.Message;
                    }
                    db.Close();
                }
                //var data = _itemRepository.G(", null, commandType: CommandType.Text);
                ////data = data.Take(50).ToList();
                //result.Success = true;
                //result.Data = data;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<PagedList<MItemSerial>> GetPagedListSerialByItem(UserParams userParams)
        {
            GenericResult result = new GenericResult();
            try
            {
                //string query = $"select * from M_ItemSerial with (nolock) where 1 =1 and Status = 'A' ";
                //if (!string.IsNullOrEmpty(userParams.Item))
                //    query += $" and ItemCode = '{userParams.Item}' ";
                //if (!string.IsNullOrEmpty(userParams.keyword))
                //    query += $" and (ItemCode like '%{userParams.keyword}%' or SerialNum like '%{userParams.keyword}%' )";

                //N'{userParams.Company}', N'{userParams.Item}' , N'{userParams.keyword}' , N'{userParams.SlocId}';

                string query = $"USP_S_GetItemSerialAvailable";

                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", string.IsNullOrEmpty(userParams.Company) ? "" : userParams.Company);
                parameters.Add("ItemCode", string.IsNullOrEmpty(userParams.Item) ? "" : userParams.Item);
                parameters.Add("Keyword", string.IsNullOrEmpty(userParams.keyword) ? "" : userParams.keyword);
                parameters.Add("SlocId", string.IsNullOrEmpty(userParams.SlocId) ? "" : userParams.SlocId);
                if(!string.IsNullOrEmpty(userParams.Customer))
                {
                    parameters.Add("Customer", string.IsNullOrEmpty(userParams.Customer) ? "" : userParams.Customer);
                }
                

                var data = await _itemSerialRepository.GetAllAsync(query, parameters, commandType: CommandType.StoredProcedure );
                var pagedata = await PagedList<MItemSerial>.Create(data, userParams.PageNumber, userParams.PageSize);
                return pagedata;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<GenericResult> GetSerialByItem(string CompanyCode, string Itemcode, string keyword, string SlocId)
        {
            GenericResult result = new GenericResult();
            try
            {
                string query = $"USP_S_GetItemSerialAvailable N'{CompanyCode}', N'{Itemcode}' , N'{keyword}' ,N'{SlocId}' ";

                var data = await _itemSerialRepository.GetAllAsync(query, null, commandType: CommandType.Text );
                result.Success = true;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> GetItemByCode(string companyCode, string itemCode)
        {
            GenericResult result = new GenericResult();
            //try
            //   {
            //       var item = new ItemViewModel();
            //       if (db.State == ConnectionState.Closed)
            //           db.Open();

            //       var dblist = await _itemRepository.GetAllAsync($"select * from M_Item with (nolock) where CompanyCode='" + CompanyCode, parameters, commandType: CommandType.StoredProcedure);
            //       if (dblist.Count() > 0)
            //       {
            //           db.Close();
            //           item = dblist.ToList().FirstOrDefault();
            //       }

            //       return item;

            //   }
            //   catch (Exception ex)
            //   {
            //       db.Close();
            //       return null;
            //   }

            string keyCache = string.Format(PrefixCacheGetItem, companyCode, itemCode);
            string storeCache = cacheService.GetCachedData<string>(keyCache);
            if (string.IsNullOrEmpty(storeCache))
            {
                cacheService.CacheData<string>(keyCache, keyCache, timeQuickAction);
            }
            else
            {
                result.Success = false;
                result.Message = "Your actions are too fast and too dangerous.";
                return result;
            }

            using (IDbConnection db = _itemRepository.GetConnection())
            {
                try
                {
                    var item = new ItemViewModel();
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", string.IsNullOrEmpty(companyCode) ? "" : companyCode);
                    parameters.Add("ItemCode", string.IsNullOrEmpty(itemCode) ? "" : itemCode);
                    parameters.Add("UomCode", "");
                    parameters.Add("BarCode", "");
                    parameters.Add("Keyword", "");
                    parameters.Add("Merchandise", "");
                    parameters.Add("Type", "");
                    //parameters.Add("IsMaster", null);
                    //if (!string.IsNullOrEmpty(PriceListId))
                    //{
                    //    parameters.Add("PriceListId", string.IsNullOrEmpty(PriceListId) ? "" : PriceListId);
                    //}
                    var data = await db.QueryFirstAsync<ItemViewModel>($"USP_GetItemInfor", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 600);
                    //parameters.Add("CustomerId", "");
                    //var data = await db.QueryAsync<ItemViewModel>($"USP_GetItem", parameters, commandType: CommandType.StoredProcedure);
                    //var data = await _itemRepository.GetAsync($"select * from M_Item with (nolock) where  CompanyCode= '{companyCode}' and ItemCode = '{itemCode}'", null, commandType: CommandType.Text);
                    //return data;
                    result.Success = true;
                    result.Data = data;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }
            return result;
        }

        public async Task<GenericResult> GetByCode(string Code)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _itemRepository.GetConnection())
            {
                try
                {
                    var item = new ItemViewModel();
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", "");
                    parameters.Add("ItemCode", Code);
                    parameters.Add("UomCode", "");
                    parameters.Add("BarCode", "");
                    parameters.Add("Keyword", "");
                    parameters.Add("Merchandise", "");
                    parameters.Add("Type", "");
                    //parameters.Add("CustomerId", "");
                    var dblist = await db.QueryAsync<ItemViewModel>($"USP_GetItem", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 600);
                    if (dblist.Count() > 0)
                    {
                        db.Close();
                        item = dblist.ToList().FirstOrDefault();
                    }
                    result.Success = true;
                    result.Data = item;
                    //return item;

                }
                catch (Exception ex)
                {
                    db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }
            return result;
            //try
            //{

            //    var data = await _itemRepository.GetAsync($"select * from M_Item with (nolock) where ItemCode = '{Code}'", null, commandType: CommandType.Text);
            //    return data;
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
        }

        public async Task<GenericResult> GetByMer(string MerCode)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedList<ItemViewModel>> GetItemWPriceList(UserParams userParams)
        {
            using (IDbConnection db = _itemRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", string.IsNullOrEmpty(userParams.Company) ? "" : userParams.Company);
                    parameters.Add("StoreId", string.IsNullOrEmpty(userParams.Store) ? "" : userParams.Store);
                    parameters.Add("Keyword", string.IsNullOrEmpty(userParams.keyword) ? "" : userParams.keyword);
                    parameters.Add("Merchandise", string.IsNullOrEmpty(userParams.Merchandise) ? "" : userParams.Merchandise);
                    parameters.Add("ItemCode", string.IsNullOrEmpty(userParams.ItemCode) ? "" : userParams.ItemCode);
                    parameters.Add("UomCode", string.IsNullOrEmpty(userParams.UomCode) ? "" : userParams.UomCode);
                    parameters.Add("BarCode", string.IsNullOrEmpty(userParams.BarCode) ? "" : userParams.BarCode);
                    parameters.Add("Type", "");
                    //parameters.Add("CustomerId", "");
                    var dblist = await db.QueryAsync<ItemViewModel>($"USP_GetItem", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 600);

                    db.Close();
                    //return dblist.ToList();
                    if (userParams.OrderBy == "byName")
                    {
                        dblist.OrderByDescending(x => x.ItemName);
                    }
                    if (userParams.OrderBy == "byId")
                    {
                        dblist.OrderByDescending(x => x.ItemCode);
                    }
                    return await PagedList<ItemViewModel>.Create(dblist.ToList(), userParams.PageNumber, userParams.PageSize );

                }
                catch (Exception ex)
                {
                    db.Close();
                    return null;
                }
            }
            //try
            //{
            //    var parameters = new DynamicParameters();

            //    //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //    //model.ShiftId = key;

            //    parameters.Add("CompanyCode", string.IsNullOrEmpty(userParams.Company)?"" :userParams.Company);
            //    //parameters.Add("StoreId", string.IsNullOrEmpty(userParams.Store) ? "" : userParams.Store );
            //    parameters.Add("Keyword", string.IsNullOrEmpty(userParams.keyword) ? "" : userParams.keyword );
            //    parameters.Add("Merchandise", string.IsNullOrEmpty(userParams.Merchandise) ? "" : userParams.Merchandise );
            //    parameters.Add("ItemCode", string.IsNullOrEmpty(userParams.Merchandise) ? "" : userParams.Merchandise );
            //    parameters.Add("UomCode", string.IsNullOrEmpty(userParams.Merchandise) ? "" : userParams.Merchandise );
            //    parameters.Add("BarCode", string.IsNullOrEmpty(userParams.Merchandise) ? "" : userParams.Merchandise );
            //    //string query = "[USP_GetItem]";
            //    string query = "[USP_GetItem]";

            //    //if (!string.IsNullOrEmpty(userParams.keyword))
            //    //{
            //    //    query += $" and ItemCode like N'%{userParams.keyword}%' or ProductId like N'%{userParams.keyword}%' or ItemName like N'%{userParams.keyword}%' or ItemDescription like N'%{userParams.keyword}%'";
            //    //}
            //    //if (!string.IsNullOrEmpty(userParams.Merchandise))
            //    //{
            //    //    query += $" and MCId = '{userParams.Merchandise}'";
            //    //}

            //    var data = await db.GetAllAsync<ItemViewModel>(query, parameters, commandType: CommandType.StoredProcedure);
            //    //var mock = data.AsQueryable().BuildMock();
            //    if (userParams.OrderBy == "byName")
            //    {
            //        data.OrderByDescending(x => x.ItemName);
            //    }
            //    if (userParams.OrderBy == "byId")
            //    {
            //        data.OrderByDescending(x => x.ItemCode);
            //    }
            //    return await PagedList<ItemViewModel>.Create(data, userParams.PageNumber, userParams.PageSize);
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
        }
        public async Task<GenericResult> GetPagedList(UserParams userParams)
        {
            GenericResult result = new GenericResult();
            try
            {
                //string query = "select * from M_Item with (nolock) where 1=1 ";

                //if (!string.IsNullOrEmpty(userParams.keyword))
                //{
                //    query += $" and ItemCode like N'%{userParams.keyword}%' or ProductId like N'%{userParams.keyword}%' or ItemName like N'%{userParams.keyword}%' or ItemDescription like N'%{userParams.keyword}%'";
                //}
                //if (!string.IsNullOrEmpty(userParams.Merchandise))
                //{
                //    query += $" and MCId = '{userParams.Merchandise}'";
                //}
                using (IDbConnection db = _itemRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        var parameters = new DynamicParameters();
                        parameters.Add("CompanyCode", string.IsNullOrEmpty(userParams.Company) ? "" : userParams.Company);
                        parameters.Add("StoreId", string.IsNullOrEmpty(userParams.Store) ? "" : userParams.Store);
                        parameters.Add("ItemCode", string.IsNullOrEmpty(userParams.ItemCode) ? "" : userParams.ItemCode);
                        parameters.Add("Keyword", string.IsNullOrEmpty(userParams.keyword) ? "" : userParams.keyword);

                        var dblist = await db.QueryAsync<ItemViewModel>($"USP_GetAllItemInfor", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 600);

                        var data = await PagedList<ItemViewModel>.Create(dblist.ToList(), userParams.PageNumber, userParams.PageSize );
                        //foreach(var item in dblist)
                        //{
                        //    if(item.IsBom== true)
                        //    {
                        //       var bOMViewModel =  _bomService.GetByItemCode(item.ItemCode);
                        //       if(bOMViewModel.Result!=null && bOMViewModel.Result.Lines.Count > 0)
                        //        {
                        //            item.Lines = bOMViewModel.Result.Lines;
                        //        }    
                        //    }    
                        //}    
                        db.Close();
                        result.Success = true;
                        result.Data = data;


                    }
                    catch (Exception ex)
                    {
                        db.Close();
                        result.Success = false;
                        result.Message = ex.Message;
                    }

                }
                //var parameters = new DynamicParameters();
                //parameters.Add("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode);
                //parameters.Add("StoreId", string.IsNullOrEmpty(StoreId) ? "" : StoreId);
                //parameters.Add("ItemCode", string.IsNullOrEmpty(ItemCode) ? "" : ItemCode);
                //parameters.Add("Keyword", string.IsNullOrEmpty(Keyword) ? "" : Keyword);

                //var data = await db.QueryAsync<ItemViewModel>($"USP_GetAllItemInfor", parameters, commandType: CommandType.StoredProcedure);
                ////var data = await _itemRepository.GetAllAsync(query, null, commandType: CommandType.Text);
                ////var mock = data.AsQueryable().BuildMock();
                //if (userParams.OrderBy == "byName")
                //{
                //    data.OrderByDescending(x => x.ItemName);
                //}
                //if (userParams.OrderBy == "byId")
                //{
                //    data.OrderByDescending(x => x.ItemCode);
                //}
                //return await PagedList<ItemViewModel>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> AvartaUpdate(string CompanyCode, string ItemCode, string Url)
        {

            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;

                var affectedRows = _itemRepository.Update($"update M_Item set ImageURL = N'{Url}' where CompanyCode = '{CompanyCode}' and ItemCode = '{ItemCode}' ", parameters, commandType: CommandType.Text);
                result.Success = true;
                //result.Message = key;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> Update(MItem model)
        {

            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                using (IDbConnection db = _itemRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {
                                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                                //model.ShiftId = key;

                                parameters.Add("ItemCode", model.ItemCode);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("ProductId", model.ProductId);
                                parameters.Add("VariantId", model.VariantId);
                                parameters.Add("CapacityValue", model.CapacityValue);
                                parameters.Add("ItemGroupId", model.ItemGroupId);
                                parameters.Add("SalesTaxCode", model.SalesTaxCode);
                                parameters.Add("PurchaseTaxCode", model.PurchaseTaxCode);
                                parameters.Add("ItemName", model.ItemName);
                                parameters.Add("ItemDescription", model.ItemDescription);
                                parameters.Add("ItemCategory_1", model.ItemCategory_1);
                                parameters.Add("ItemCategory_2", model.ItemCategory_2);
                                parameters.Add("ItemCategory_3", model.ItemCategory_3);
                                parameters.Add("ForeignName", model.ForeignName);
                                parameters.Add("InventoryUOM", model.InventoryUom);
                                parameters.Add("ImageURL", model.ImageUrl);
                                parameters.Add("ImageLink", model.ImageLink);
                                parameters.Add("MCId", model.Mcid);
                                parameters.Add("CustomField1", model.CustomField1);
                                parameters.Add("CustomField2", model.CustomField2);
                                parameters.Add("CustomField3", model.CustomField3);
                                parameters.Add("CustomField4", model.CustomField4);
                                parameters.Add("CustomField5", model.CustomField5);
                                parameters.Add("CustomField6", model.CustomField6);
                                parameters.Add("CustomField7", model.CustomField7);
                                parameters.Add("CustomField8", model.CustomField8);
                                parameters.Add("CustomField9", model.CustomField9);
                                parameters.Add("CustomField10", model.CustomField10);
                                parameters.Add("DefaultPrice", model.DefaultPrice);
                                parameters.Add("IsSerial", model.IsSerial);
                                parameters.Add("IsBOM", model.IsBom);
                                parameters.Add("IsVoucher", model.IsVoucher);
                                parameters.Add("ValidFrom", model.ValidFrom);
                                parameters.Add("ValidTo", model.ValidTo);

                                parameters.Add("Status", model.Status);
                                parameters.Add("ModifiedBy", model.ModifiedBy);
                                parameters.Add("RejectPayType", model.RejectPayType);
                                parameters.Add("Returnable", model.Returnable);
                                parameters.Add("VoucherCollection", model.VoucherCollection);
                                parameters.Add("IsPriceTime", model.IsPriceTime);
                                var affectedRows = _itemRepository.Update("USP_U_M_Item", parameters, commandType: CommandType.StoredProcedure);
                                result.Success = true;
                                //result.Message = key;
                                result.Success = true;
                                tran.Commit();

                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                throw ex;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> GetItemStockAsync(string companyCode, string storeId, string slocId, string itemCode, string uomCode, string barCode, string serialNum)
        {
            GenericResult result = new GenericResult();
            List<ItemStockViewModel> itemStocks = new List<ItemStockViewModel>();
            using (IDbConnection db = _itemRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("StoreId", storeId);
                    parameters.Add("SlocId", slocId);
                    parameters.Add("ItemCode", itemCode);
                    parameters.Add("UomCode", uomCode);
                    parameters.Add("BarCode", barCode);
                    parameters.Add("SerialNum", serialNum);

                    var dblist = await db.QueryAsync<ItemStockViewModel>($"USP_GetItemStock", param: parameters, commandType: CommandType.StoredProcedure, commandTimeout: 600);

                    db.Close();

                    if (dblist.Any())
                    {
                        itemStocks = dblist.ToList();
                    }
                    result.Success = true;
                    result.Data = itemStocks;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                }
                //return result;
            }
            return result;
            //return itemStocks;
        }

        public async Task<GenericResult> GetItemByVoucherCollection(string CompanyCode, string StoreId, string Code)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _itemRepository.GetConnection())
            {
                try
                {
                    var item = new ItemViewModel();
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("ItemCode", Code);
                    var dblist = await db.QueryAsync<ItemViewModel>($"USP_GetItemByVoucherCollection", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 600);
                    if (dblist.Count() > 0)
                    {
                        db.Close();
                    }
                    result.Success = true;
                    result.Data = dblist.ToList();
                    ;
                    //return item;

                }
                catch (Exception ex)
                {
                    db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }
            return result;
        }

        public async Task<GenericResult> GetScanBarcode(string CompanyCode, string UserId,string StoreId, string Keyword, string CustomerGroupId)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _itemRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode);
                    parameters.Add("UserId", string.IsNullOrEmpty(UserId) ? "" : UserId);
                    parameters.Add("StoreId", string.IsNullOrEmpty(StoreId) ? "" : StoreId);
                    parameters.Add("Keyword", string.IsNullOrEmpty(Keyword) ? "" : Keyword);
                    parameters.Add("CustomerGroupId", string.IsNullOrEmpty(CustomerGroupId) ? "" : CustomerGroupId);

                    var dblist = await db.QueryAsync<ItemBarcodeViewModel>($"USP_ScanBarcode", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 600 );

                    result.Success = true;
                    result.Data = dblist.ToList();
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }
            return result;

        }

    }
}
