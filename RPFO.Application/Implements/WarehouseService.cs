
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
    public class WarehouseService : IWarehouseService
    {
        private readonly IGenericRepository<MWarehouse> _warehouseRepository;

        private readonly IMapper _mapper;
        public WarehouseService(IGenericRepository<MWarehouse> warehouseGroupRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _warehouseRepository = warehouseGroupRepository;
            _mapper = mapper; 

        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<WarehouseResultViewModel> resultlist = new List<WarehouseResultViewModel>();
            try
            {
                foreach (var item in model.Warehouse)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);
                    //if (itemResult.Success == false)
                    //{
                        WarehouseResultViewModel itemRs = new WarehouseResultViewModel();
                        itemRs = _mapper.Map<WarehouseResultViewModel>(item);
                        itemRs.Success = itemResult.Success;
                        itemRs.Message = itemResult.Message;
                        resultlist.Add(itemRs);
                    //}
                }
                result.Success = true;
                result.Data = resultlist;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                //result.Data = failedlist;
            } 
            return result;
        }

        public async Task<bool> checkExist(string CompanyCode, string WhsCode)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("WhsCode", WhsCode);
            parameters.Add("WhsType", "");
            parameters.Add("Status", "");
            var affectedRows = await _warehouseRepository.GetAsync("USP_S_M_Warehouse", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MWarehouse model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("WhsCode", model.WhsCode, DbType.String);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("WhsName", model.WhsName);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                parameters.Add("WhsType", model.WhsType);
                parameters.Add("DefaultSlocId", model.DefaultSlocId);


                var exist = await checkExist(model.CompanyCode, model.WhsCode);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.WhsCode + " existed.";
                    return result;
                }

                var affectedRows = _warehouseRepository.Insert("USP_I_M_Warehouse", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Delete(string Code)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _warehouseRepository.GetAllAsync($"select * from M_Warehouse with (nolock) where CompanyCode='{CompanyCode}'", null, commandType: CommandType.Text);
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
        public async Task<GenericResult> GetByStore(string CompanyCode, string StoreId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _warehouseRepository.GetAllAsync($"select * from M_Warehouse with (nolock) where CompanyCode=N'{CompanyCode}' and StoreId=N'{StoreId}'", null, commandType: CommandType.Text);
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
        public async Task<GenericResult> GetByCode(string CompanyCode, string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _warehouseRepository.GetAsync($"select * from M_Warehouse with (nolock) where WhsCode='{Code}'", null, commandType: CommandType.Text);
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

        //public Task<List<MWarehouse>> GetByUser(string User)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<PagedList<MWarehouse>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _warehouseRepository.GetAllAsync($"select * from M_Warehouse with (nolock) where WhsCode like N'%{userParams.keyword}%' or WhsName like N'%{userParams.keyword}%'", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                if (userParams.OrderBy == "byName")
                {
                    data.OrderByDescending(x => x.WhsName);
                }
                if (userParams.OrderBy == "byId")
                {
                    data.OrderByDescending(x => x.WhsCode);
                }
                return await PagedList<MWarehouse>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GenericResult> GetWhsType()
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _warehouseRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    using (var tran = db.BeginTransaction())
                    {
                        try
                        {
                            string query = "select * from [fn_GetWhsType]()";
                            var dataX = await db.QueryAsync(query, null, tran);

                            db.Close();
                            result.Success = true;
                            result.Data = dataX;
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
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        }

        public async Task<GenericResult> Update(MWarehouse model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("WhsCode", model.WhsCode, DbType.String);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("WhsName", model.WhsName);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                parameters.Add("WhsType", model.WhsType);
                parameters.Add("DefaultSlocId", model.DefaultSlocId);
                var affectedRows = _warehouseRepository.Update("USP_U_M_Warehouse", parameters, commandType: CommandType.StoredProcedure);
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

        Task<GenericResult> IWarehouseService.GetByUser(string User)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetWarehouseByWhsType(string CompanyCode, string StoreId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);

                var data = await _warehouseRepository.GetAllAsync("USP_GetWarehouseByWhsType", parameters, commandType: CommandType.StoredProcedure);
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

    }

}
