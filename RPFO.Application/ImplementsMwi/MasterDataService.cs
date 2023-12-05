using Dapper;
using RPFO.Application.InterfacesMwi;
using RPFO.Data.EntitiesMWI;
using RPFO.Data.Infrastructure;
using RPFO.Data.Repositories;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;

namespace RPFO.Application.ImplementsMwi
{
    public class MasterDataService : IMasterDataService
    {
        private readonly IGenericRepository<MItem> _masterDataRepository;
        private readonly IGenericRepository<MItemUom> _itemUomRepository;
        private readonly IGenericRepository<MPriceList> _priceListRepository;
        private readonly IGenericRepository<MStore> _storeRepository;
        private readonly IGenericRepository<MTax> _taxRepository;
        private readonly IGenericRepository<MUom> _uomRepository;
        private readonly IGenericRepository<MWarehouse> _whsRepository;
        private readonly IGenericRepository<MBomline> _bomLineRepository;

        public MasterDataService(IGenericRepository<MItem> masterData,
            IGenericRepository<MItemUom> itemUomRepository,
            IGenericRepository<MStore> storeRepository,
            IGenericRepository<MPriceList> priceListRepository,
            IGenericRepository<MTax> taxRepository,
            IGenericRepository<MUom> uomRepository,
            IGenericRepository<MWarehouse> whsRepository,
            IGenericRepository<MBomline> bomLineRepository)
        {
            this._masterDataRepository = masterData;
            this._itemUomRepository = itemUomRepository;
            this._priceListRepository = priceListRepository;
            this._storeRepository = storeRepository;
            this._taxRepository = taxRepository;
            this._uomRepository = uomRepository;
            this._whsRepository = whsRepository;
            this._bomLineRepository = bomLineRepository;
        }

        public List<MItem> GetItems(string companyCode, string itemCode)
        {
            List<MItem> items = null;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", companyCode);
                parameters.Add("ItemCode", itemCode);
                items = _masterDataRepository.GetAll("USP_S_M_Items", parameters, CommandType.StoredProcedure, GConnection.MwiConnection);
                var BOMList = items.Where(x => x.IsBom == true);
                foreach (var item in BOMList)
                {
                    parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("BOMId", item.ItemCode);
                    var bomline = _bomLineRepository.GetAll($"USP_S_BOMLine", parameters, CommandType.StoredProcedure, GConnection.MwiConnection);
                    if (bomline != null && bomline.Count > 0)
                    {
                        item.BOMLines = bomline;
                    }
                }
            }
            catch
            {
                throw;
            }

            return items;
        }

        public List<MItemUom> GetItemUom(string itemCode, string uomCode)
        {
            List<MItemUom> result = null;

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ItemCode", itemCode);
                parameters.Add("UomCode", uomCode);
                result = _itemUomRepository.GetAll("USP_S_M_ItemUOM", parameters, CommandType.StoredProcedure, GConnection.MwiConnection);
            }
            catch { }

            return result;
        }

        public List<MPriceList> GetPriceList(string companyCode, string priceListId, string storeId)
        {
            List<MPriceList> result = null;

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", companyCode);
                parameters.Add("PriceListId", priceListId);
                parameters.Add("StoreId", storeId);
                result = _priceListRepository.GetAll("USP_S_M_PriceList", parameters, CommandType.StoredProcedure, GConnection.MwiConnection);
            }
            catch { }

            return result;
        }

        public List<MPriceList> GetPriceCheck(string companyCode, string storeId, string itemCode, string uomCode, string barCode, string date)
        {
            List<MPriceList> result = null;

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", companyCode);
                parameters.Add("StoreId", storeId);
                parameters.Add("ItemCode", itemCode);
                parameters.Add("UomCode", uomCode);
                parameters.Add("BarCode", barCode);
                parameters.Add("Date", date);
                result = _priceListRepository.GetAll("USP_S_M_PriceCheck", parameters, CommandType.StoredProcedure, GConnection.MwiConnection);
            }
            catch { }

            return result;
        }

        public List<MStore> GetStore(string companyCode, string storeId)
        {
            List<MStore> result = null;

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", companyCode);
                parameters.Add("StoreId", storeId);
                result = _storeRepository.GetAll("USP_S_M_Store", parameters, CommandType.StoredProcedure, GConnection.MwiConnection);
            }
            catch { }

            return result;
        }

        public List<MTax> GetTax(string companyCode, string taxCode)
        {
            List<MTax> result = null;

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", companyCode);
                parameters.Add("TaxCode", taxCode);
                result = _taxRepository.GetAll("USP_S_M_Tax", parameters, CommandType.StoredProcedure, GConnection.MwiConnection);
            }
            catch { }

            return result;
        }

        public List<MUom> GetUOM(string companyCode, string uomCode)
        {
            List<MUom> result = null;

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", companyCode);
                parameters.Add("Code", uomCode);
                result = _uomRepository.GetAll("USP_S_M_UOM", parameters, CommandType.StoredProcedure, GConnection.MwiConnection);
            }
            catch { }

            return result;
        }

        public List<MWarehouse> GetWarehouse(string companyCode, string whsCode)
        {
            List<MWarehouse> result = null;

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", companyCode);
                parameters.Add("Code", whsCode);
                result = _whsRepository.GetAll("USP_S_M_Warehouse", parameters, CommandType.StoredProcedure, GConnection.MwiConnection);
            }
            catch { }

            return result;
        }

    }
}
