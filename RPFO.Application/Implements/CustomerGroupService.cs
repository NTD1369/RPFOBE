
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
    public class CustomerGroupService : ICustomerGroupService
    {
        private readonly IGenericRepository<MCustomerGroup> _customerGroupRepository;

        private readonly IMapper _mapper;
        public CustomerGroupService(IGenericRepository<MCustomerGroup> customerGroupRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _customerGroupRepository = customerGroupRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }

        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<CutomerGroupResultViewModel> resultlist = new List<CutomerGroupResultViewModel>();
            try
            {
                foreach (var item in model.CustomerGroup)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);
                    
                        CutomerGroupResultViewModel itemRs = new CutomerGroupResultViewModel();
                        itemRs = _mapper.Map<CutomerGroupResultViewModel>(item);
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
                result.Data = resultlist;
            } 
            return result;
        }

        public async Task<bool> checkExist(string CompanyCode, string GroupId)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CusGrpId", GroupId);
            parameters.Add("CompanyCode", CompanyCode); 
            parameters.Add("Status","");
            var affectedRows = await _customerGroupRepository.GetAsync("USP_S_M_CustomerGroup", parameters, commandType: CommandType.StoredProcedure);
            if(affectedRows!=null)
            {
                return true;
            }    
            else
            {
                return false;
            }    
        }
        public async Task<GenericResult> Create(MCustomerGroup model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CusGrpId", model.CusGrpId, DbType.String);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("CusGrpDesc", model.CusGrpDesc);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", string.IsNullOrEmpty(model.Status) ? "N" : model.Status);

                var exist= await  checkExist(model.CompanyCode, model.CusGrpId);
                if(exist== true)
                {
                    result.Success = false;
                    result.Message = model.CusGrpId + " existed.";
                    return result;
                }
                 
                var affectedRows = _customerGroupRepository.Insert("USP_I_M_CustomerGroup", parameters, commandType: CommandType.StoredProcedure);
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

        public Task<GenericResult> Delete(string Code)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _customerGroupRepository.GetAllAsync($"select * from M_CustomerGroup with (nolock)  where CompanyCode= N'{CompanyCode}'", null, commandType: CommandType.Text);
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

        public async Task<GenericResult> GetAllViewModel(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _customerGroupRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    //string query = $"select CusGrpId CustomerGrpId, CusGrpDesc CustomerGrpDesc from M_CustomerGroup with (nolock) where CompanyCode= N'{CompanyCode}'";
                    var parameters = new DynamicParameters();

                    //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                    //model.ShiftId = key;
                    parameters.Add("CusGrpId", "");
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("Status", "");
                    var affectedRows = await _customerGroupRepository.GetAllAsync("USP_S_M_CustomerGroup", parameters, commandType: CommandType.StoredProcedure);
                    //var dataX = await db.QueryAsync<CustomerGroupViewModel>(query, null);

                    db.Close();
                    result.Success = true;
                    result.Data = affectedRows;
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;

                }
               
               
            }
             
            return result;
        }
        public async Task<GenericResult> GetByCode(string CompanyCode , string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _customerGroupRepository.GetAsync($"select * from M_CustomerGroup with (nolock)  where CompanyCode= N'{CompanyCode}' and CusGrpId ='{Code}'", null, commandType: CommandType.Text);
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

        public async Task<PagedList<MCustomerGroup>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _customerGroupRepository.GetAllAsync($"select * from M_CustomerGroup with (nolock) where CusGrpId like N'%{userParams.keyword}%' or CompanyCode like N'%{userParams.keyword}%'  or CusGrpDesc like N'%{userParams.keyword}%'", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                //if (userParams.OrderBy == "byName")
                //{
                //    data.OrderByDescending(x => x.StoreGroupName);
                //}
                //if (userParams.OrderBy == "byId")
                //{
                //    data.OrderByDescending(x => x.StoreGroupId);
                //}
                return await PagedList<MCustomerGroup>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GenericResult> Update(MCustomerGroup model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CusGrpId", model.CusGrpId, DbType.String);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("CusGrpDesc", model.CusGrpDesc); 
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                var affectedRows = _customerGroupRepository.Insert("USP_U_M_CustomerGroup", parameters, commandType: CommandType.StoredProcedure);
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
