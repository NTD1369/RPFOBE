
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
    public class ItemSerialStockService : IItemSerialStockService
    {
        private readonly IGenericRepository<MItemSerialStock> _serialStockRepository;
        private readonly IGenericRepository<MItemSerial> _serialRepository;
        private readonly IMapper _mapper;
        public ItemSerialStockService(IGenericRepository<MItemSerialStock> serialStockRepository, IGenericRepository<MItemSerial> serialRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _serialStockRepository = serialStockRepository;
            _serialRepository = serialRepository;
            _mapper = mapper; 

        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<ItemSerialStockResultViewModel> resultlist = new List<ItemSerialStockResultViewModel>();
            try
            {
                foreach (var item in model.ItemSerialStock)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);

                    ItemSerialStockResultViewModel itemRs = new ItemSerialStockResultViewModel();
                    itemRs = _mapper.Map<ItemSerialStockResultViewModel>(item);
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

        public async Task<bool> checkExist(string CompanyCode, string SlocId, string ItemCode, string SerialNum)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;

            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("ItemCode", ItemCode);
            parameters.Add("SlocId", SlocId);
            parameters.Add("SerialNum", SerialNum);
            var affectedRows = await _serialStockRepository.GetAsync("USP_S_M_ItemSerialStock", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> checkSerialExist(string CompanyCode, string ItemCode, string SerialNum)
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
        public async Task<GenericResult> Create(MItemSerialStock model)
        {
            GenericResult result = new GenericResult();
            try
            {
                if(string.IsNullOrEmpty(model.CompanyCode)    )
                {
                    result.Success = false;
                    result.Message = "CompanyCode can't null";
                    return result;
                }
                if (string.IsNullOrEmpty(model.ItemCode))
                {
                    result.Success = false;
                    result.Message = "ItemCode can't null";
                    return result;
                }
                if (string.IsNullOrEmpty(model.SerialNum))
                {
                    result.Success = false;
                    result.Message = "Serial Num can't null";
                    return result;
                }
                if (string.IsNullOrEmpty(model.StockQty.ToString()))
                {
                    result.Success = false;
                    result.Message = "Stock Qty can't null";
                    return result;
                }
                if (string.IsNullOrEmpty(model.SlocId.ToString()))
                {
                    result.Success = false;
                    result.Message = "Sloc Id can't null";
                    return result;
                }
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("ItemCode", model.ItemCode);
                parameters.Add("SerialNum", model.SerialNum);
                parameters.Add("StockQty", model.StockQty);
                parameters.Add("SlocId", model.SlocId);
                 
                var exist = await checkExist(model.CompanyCode, model.SlocId , model.ItemCode, model.SerialNum);
                if (exist == true)
                {
                    parameters.Add("ModifiedBy", model.ModifiedBy);
                    parameters.Add("Status", model.Status);
                    var affectedRows = _serialStockRepository.Insert("USP_U_M_ItemSerialStock", parameters, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    parameters.Add("CreatedBy", model.CreatedBy);
                    parameters.Add("Status", model.Status);
                    var affectedRows = _serialStockRepository.Insert("USP_I_M_ItemSerialStock", parameters, commandType: CommandType.StoredProcedure);
                }     
                var serialExist = await checkSerialExist(model.CompanyCode, model.ItemCode, model.SerialNum);
                if (serialExist == false)
                {
                    parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", model.CompanyCode);
                    parameters.Add("ItemCode", model.ItemCode);
                    parameters.Add("SerialNum", model.SerialNum);
                    parameters.Add("Quantity", model.StockQty);
                    parameters.Add("ExpDate", model.ExpDate??DateTime.Now.AddYears(1));
                    parameters.Add("StoredDate", DateTime.Now);
                    parameters.Add("CreatedBy", model.CreatedBy);
                    parameters.Add("Status", string.IsNullOrEmpty(model.Status) ? "A" : model.Status); 
                    _serialRepository.Insert("USP_I_M_ItemSerial", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Delete(MItemSerialStock model)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _serialStockRepository.GetAllAsync($"select * from M_ItemSerialStock with (nolock) where CompanyCode= N'{CompanyCode}'", null, commandType: CommandType.Text);
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
        public async Task<GenericResult> GetBySlocItem(string CompanyCode, string Sloc, string ItemCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _serialStockRepository.GetAllAsync($"select * from M_ItemSerialStock with (nolock) where CompanyCode= N'{CompanyCode}' and ItemCode='{ItemCode}' and SlocId='{Sloc}'", null, commandType: CommandType.Text);
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

        public async Task<GenericResult> GetByItem(string CompanyCode, string ItemCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _serialStockRepository.GetAllAsync($"select * from M_ItemSerialStock with (nolock) where  CompanyCode= N'{CompanyCode}' and ItemCode='{ItemCode}'", null, commandType: CommandType.Text);
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

      
        public async Task<PagedList<MItemSerialStock>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _serialStockRepository.GetAllAsync($"select * from M_ItemSerialStock with (nolock)", null, commandType: CommandType.Text);
             
                return await PagedList<MItemSerialStock>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        public async Task<GenericResult> Update(MItemSerialStock model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("ItemCode", model.ItemCode);
                parameters.Add("SerialNum", model.SerialNum);
                parameters.Add("StockQty", model.StockQty);
                parameters.Add("SlocId", model.SlocId);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                var affectedRows = _serialStockRepository.Update("USP_U_M_ItemSerialStock", parameters, commandType: CommandType.StoredProcedure);
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
