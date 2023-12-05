
using AutoMapper;
using Dapper;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class ItemSerialService : IItemSerialService
    {
        private readonly IGenericRepository<MItemSerial> _serialRepository;
        private readonly IGenericRepository<MItemSerialStock> _serialStockRepository;
        private IWarehouseService _warehouseService;
        private readonly IMapper _mapper;
        public ItemSerialService(IGenericRepository<MItemSerial> serialRepository, IWarehouseService warehouseService, IGenericRepository<MItemSerialStock> serialStockRepository,  IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _serialRepository = serialRepository;
            _serialStockRepository = serialStockRepository;
            _warehouseService = warehouseService;
            _mapper = mapper; 

        }
        private static string ListCheck<T>(IEnumerable<T> l1, IEnumerable<T> l2)
        {
            //// TODO: Null parm checks
            //if (l1.Intersect(l2).Any())
            //{
            //    //Console.WriteLine("matched");
            //    return true;
            //}
            //else
            //{
            //    //Console.WriteLine("not matched");
            //    return false;
            //}
            string resultStr = "";
            foreach (var itemSloc in l2)
            { 
                if(!l1.Contains(itemSloc))
                {
                    resultStr += itemSloc + "," ;
                     
                }    
            }
            if(resultStr.Length > 0)
            {
                resultStr = resultStr.Substring(0, resultStr.Length - 1);
            }    
            return resultStr;
        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<ItemSerialResultViewModel> resultlist = new List<ItemSerialResultViewModel>();
            try
            {
                var storeId = model.StoreId;
                //var listSlocData = await _warehouseService.GetByStore(model.CompanyCode, storeId);
                //if(listSlocData.Success && listSlocData.Data!=null)
                //{
                //    var lst = listSlocData.Data as List<MWarehouse>;
                //    if(lst.Count > 0)
                //    {

                //        var SlocInStore = lst.Select(x => x.WhsCode).Distinct(); 
                //        var SlocInData = model.ItemSerial.Select(x => x.SlocId).Distinct();
                //        string checkStr = ListCheck(SlocInStore, SlocInData);
                //        if (string.IsNullOrEmpty(checkStr))
                //        {
                            foreach (var item in model.ItemSerial)
                            {
                                item.CreatedBy = model.CreatedBy;
                                item.CompanyCode = model.CompanyCode;
                                item.Status = "N/A";
                                //MItem
                                //item.serialStock 
                                var itemResult = await Create(item); 
                                ItemSerialResultViewModel itemRs = new ItemSerialResultViewModel();
                                itemRs = _mapper.Map<ItemSerialResultViewModel>(item);
                                itemRs.Success = itemResult.Success;
                                itemRs.Message = itemResult.Message;
                                resultlist.Add(itemRs);
                            }
                            result.Success = true;
                            result.Data = resultlist;
                //        }   
                //        else
                //        {
                //            result.Success = false;
                //            result.Message = "Sloc:" + checkStr  + " not in existed in Store: " + model.StoreId;
                //        }    
                       
                //    }    
                //    else
                //    {
                //        result.Success = false;
                //        result.Message = "Please check Setup Sloc for Store: " + storeId;
                //    }    
                    
                //}   
                //else
                //{
                //    result.Success = false;
                //    result.Message = "Please check Setup Sloc for Store: " + storeId;
                //}    
               
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;

            }
            return result;
        }

        public async Task<bool> checkExist(string CompanyCode, string ItemCode, string SerialNum)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;

            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("ItemCode", ItemCode);
            parameters.Add("SerialNum", SerialNum); 
            var affectedRows = await _serialRepository.GetAsync("USP_S_M_ItemSerial", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MItemSerial model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var exist = await checkExist(model.CompanyCode, model.ItemCode, model.SerialNum);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.ItemCode + " - " + model.SerialNum + " existed.";
                    return result;
                }
                 

                var parameters = new DynamicParameters();
  
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("ItemCode", model.ItemCode); 
                parameters.Add("SerialNum", model.SerialNum); 
                parameters.Add("Quantity", model.Quantity == null ? 1 : model.Quantity); 
                parameters.Add("ExpDate", model.ExpDate); 
                parameters.Add("StoredDate", model.StoredDate);  
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", string.IsNullOrEmpty(model.Status) ? "N/A" : model.Status);

                var affectedRows = _serialRepository.Insert("USP_I_M_ItemSerial", parameters, commandType: CommandType.StoredProcedure);
                if (model.serialStock!=null)
                {
                    parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", model.CompanyCode);
                    parameters.Add("ItemCode", model.ItemCode);
                    parameters.Add("SerialNum", model.SerialNum);
                    parameters.Add("StockQty", model.serialStock.StockQty);
                    parameters.Add("SlocId", model.serialStock.SlocId); 
                    parameters.Add("CreatedBy", model.CreatedBy);
                    parameters.Add("Status", model.Status);
                    _serialStockRepository.Insert("USP_I_M_ItemSerialStock", parameters, commandType: CommandType.StoredProcedure);
                }     
                //else
                //{
                //    string defaultWhs = _serialRepository.GetScalar($"select WhsCode from M_Store with (nolock) where companyCode =N'{model.CompanyCode}' and StoreId = N'{model.StoreId}'", null, commandType: CommandType.Text);

                //    parameters = new DynamicParameters();
                //    parameters.Add("CompanyCode", model.CompanyCode);
                //    parameters.Add("ItemCode", model.ItemCode);
                //    parameters.Add("SerialNum", model.SerialNum);
                //    parameters.Add("StockQty", model.Quantity == null ? 1 : model.Quantity);
                //    parameters.Add("SlocId", model.serialStock.SlocId);
                //    parameters.Add("CreatedBy", model.CreatedBy);
                //    parameters.Add("Status", model.Status);
                //    _serialStockRepository.Insert("USP_I_M_ItemSerialStock", parameters, commandType: CommandType.StoredProcedure);
                //}    
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

        public async Task<GenericResult> GenerateSerial(string CompanyCode, string StoreId, DateTime? ExpDate, string By, string Prefix, string ItemCode , int NumOfGen, int? RandomNumberLen, int? RuningNumberLen)
        {
            GenericResult result = new GenericResult();
            try
            {
                 
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("ItemCode", ItemCode);
                parameters.Add("Prefix", Prefix);
                parameters.Add("RandomNumberLen", RandomNumberLen);
                parameters.Add("RuningNumberLen", RuningNumberLen);
                string query = $"select   [dbo].[fnc_AutoGenSerial](N'{CompanyCode}', N'{ItemCode}',N'{Prefix}', {RandomNumberLen}, {RuningNumberLen})";
               
                List<MItemSerial> list = new List<MItemSerial>();
                for(int i=1; i< NumOfGen; i++ )
                {
                    var affectedRows = _serialRepository.GetScalar(query, null, commandType: CommandType.Text);
                    MItemSerial item = new MItemSerial();
                    item.ItemCode = ItemCode; 
                    item.CompanyCode = CompanyCode;
                    item.CreatedBy = By;
                    item.SerialNum = affectedRows;
                    item.ExpDate = ExpDate ?? DateTime.Now;
                    item.StoredDate = DateTime.Now;
                    var rs = await Create(item);
                    if(rs.Success)
                    {
                        list.Add(item);
                    }    
                }
                result.Success = true;
                result.Data = list;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> Delete(List<MItemSerial> models)
        {

            GenericResult result = new GenericResult();
            try
            {
                //var exist = await checkExist(model.CompanyCode, model.ItemCode, model.SerialNum);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.ItemCode + " - " + model.SerialNum + " existed.";
                //    return result;
                //}
                foreach(var model in models)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", model.CompanyCode);
                    parameters.Add("ItemCode", model.ItemCode);
                    parameters.Add("SerialNum", model.SerialNum);
                    var affectedRows = _serialRepository.Execute("USP_D_M_ItemSerial", parameters, commandType: CommandType.StoredProcedure);

                }
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

        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, string SlocId, string ItemCode, string Keyword,int? Selecttop)
        {
            GenericResult result = new GenericResult();
            //try
            //{
            //    var data = await _controlRepository.GetAllAsync($"select * from M_Control with (nolock) where CompanyCode = '{CompanyCode}' order By OrderNum", null, commandType: CommandType.Text);
            //    result.Success = true;
            //    result.Data = data;
            //}
          
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode);
                parameters.Add("ItemCode", string.IsNullOrEmpty(ItemCode) ? "" : ItemCode); 
                parameters.Add("StoreId", string.IsNullOrEmpty(StoreId) ? "" : StoreId);
                parameters.Add("Keyword", string.IsNullOrEmpty(Keyword) ? "" : Keyword); 
                parameters.Add("SlocId", string.IsNullOrEmpty(SlocId) ? "" : SlocId);
              if(Selecttop != null)  parameters.Add("Selecttop", Selecttop);
                var data = await _serialRepository.GetAllAsync($"USP_S_GetItemSerial", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<List<MItemSerial>> GetByItem(string CompanyCode, string StoreId, string ItemCode)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode);
                parameters.Add("ItemCode", string.IsNullOrEmpty(ItemCode) ? "" : ItemCode);
                parameters.Add("SlocId", string.IsNullOrEmpty(StoreId) ? "" : StoreId);
                parameters.Add("Keyword", "");
                var data = await _serialRepository.GetAllAsync($"USP_S_GetItemSerial", parameters, commandType: CommandType.StoredProcedure);
                //var data = await _serialRepository.GetAllAsync($"select * from M_ItemSerial with (nolock) where ItemCode='{ItemCode}'", null, commandType: CommandType.Text);
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

      

        public async Task<PagedList<MItemSerial>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _serialRepository.GetAllAsync($"select * from M_ItemSerial with (nolock)", null, commandType: CommandType.Text);
             
                return await PagedList<MItemSerial>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<GenericResult> UpdateWithStock(List<MItemSerial> models)
        {
            GenericResult result = new GenericResult();
            try
            {
                foreach(var model in models)
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("CompanyCode", model.CompanyCode);
                    parameters.Add("ItemCode", model.ItemCode);
                    parameters.Add("SerialNum", model.SerialNum);
                    parameters.Add("Quantity", model.Quantity);
                    parameters.Add("ExpDate", model.ExpDate);
                    parameters.Add("ModifiedBy", model.ModifiedBy);
                    parameters.Add("Status", string.IsNullOrEmpty(model.Status) ? "N/A" : model.Status);
                    var affectedRows = _serialRepository.Update("USP_U_M_ItemSerialWithStock", parameters, commandType: CommandType.StoredProcedure);

                }
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

        public async Task<GenericResult> Update(MItemSerial model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("ItemCode", model.ItemCode);
                parameters.Add("SerialNum", model.SerialNum);
                parameters.Add("Quantity", model.Quantity == null ? 1 : model.Quantity);
                parameters.Add("ExpDate", model.ExpDate);
                parameters.Add("StoredDate", model.StoredDate);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", string.IsNullOrEmpty(model.Status) ? "N/A" : model.Status);
                var affectedRows = _serialRepository.Update("USP_U_M_ItemSerial", parameters, commandType: CommandType.StoredProcedure);
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

       
    }

}
