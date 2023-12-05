
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Configuration;
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
    public class CustomerService : ICustomerService
    {
        private readonly IGenericRepository<MCustomer> _customerRepository;
        private readonly IGeneralSettingService _settingService;
        private readonly IMapper _mapper;
        private readonly string PrefixSO = "";
        public CustomerService(IGenericRepository<MCustomer> customerRepository, IGeneralSettingService settingService, IMapper mapper, IConfiguration config/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _customerRepository = customerRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
            _settingService = settingService;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;
            PrefixSO = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrefixSO"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);

        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<CutomerResultViewModel> resultlist = new List<CutomerResultViewModel>();
            try
            {
                foreach (var item in model.Customer)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);

                    CutomerResultViewModel itemRs = new CutomerResultViewModel();
                    itemRs = _mapper.Map<CutomerResultViewModel>(item);
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
                //result.Data = failedlist;
            }
            return result;
        }

        public async Task<bool> checkExist(string CompanyCode, string CustomerId, string Phone)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("CustomerGrpId", "");
            parameters.Add("CustomerId", CustomerId);
            parameters.Add("Status", "");
            parameters.Add("Key", "");
            parameters.Add("Type", "");
            parameters.Add("CustomerName", "");
            parameters.Add("Address", "");
            parameters.Add("Phone", Phone);
            parameters.Add("DOB", null);
            var affectedRows = await _customerRepository.GetAsync("USP_S_M_Customer", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MCustomer model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                if (PrefixSO.ToLower() == "so")
                    model.CreatedByStore = "";
                    //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                    //model.ShiftId = key;
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("CustomerId", model.CustomerId, DbType.String);
                parameters.Add("CustomerGrpId", model.CustomerGrpId);
                parameters.Add("CustomerName", model.CustomerName);
                parameters.Add("Address", model.Address);
                parameters.Add("Phone", model.Phone);
                parameters.Add("DOB", model.Dob);
                parameters.Add("JoinedDate", model.JoinedDate);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("CusType", model.CusType);
                parameters.Add("Gender", model.Gender);
                parameters.Add("Email", model.Email);
                parameters.Add("CardNo", model.CardNo);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                parameters.Add("Status", string.IsNullOrEmpty(model.Status) ? "N" : model.Status);
                parameters.Add("DoNotAccumPoints", model.DoNotAccumPoints);
                parameters.Add("CustomerRank", model.CustomerRank);
                parameters.Add("CustomerRankName", model.CustomerRankName);
                parameters.Add("RewardPoints", model.RewardPoints);
                parameters.Add("CreatedByStore", model.CreatedByStore);

                var settingData = await _settingService.GetGeneralSettingByStore(model.CompanyCode, "");
                List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                if (settingData.Success)
                {
                    SettingList = settingData.Data as List<GeneralSettingStore>;
                }
                if (SettingList != null && SettingList.Count > 0)
                {
                    var setting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "AutoCustomerCode").FirstOrDefault();

                    if (setting != null &&  setting.SettingValue == "true")
                    {
                        var exist = await checkExist(model.CompanyCode, "", model.Phone);
                        if (exist == true)
                        {
                            result.Success = false;
                            result.Message = model.Phone + " existed.";
                            return result;
                        }
                        var affectedRows = _customerRepository.Insert("USP_I_M_Customer", parameters, commandType: CommandType.StoredProcedure);
                    }
                    else
                    {
                        var exist = await checkExist(model.CompanyCode, model.CustomerId, model.Phone);
                        if (exist == true)
                        {
                            result.Success = false;
                            result.Message = model.CustomerId + " existed.";
                            return result;
                        }
                        var affectedRows = _customerRepository.Insert("USP_I_M_Customer", parameters, commandType: CommandType.StoredProcedure);
                    }

                }
                else
                {
                    var exist = await checkExist(model.CompanyCode, model.CustomerId, model.Phone);
                    if (exist == true)
                    {
                        result.Success = false;
                        result.Message = model.CustomerId + " existed.";
                        return result;
                    }
                    var affectedRows = _customerRepository.Insert("USP_I_M_Customer", parameters, commandType: CommandType.StoredProcedure);
                }

                result.Success = true;
                //result.Message = model.Phone;
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

        public async Task<GenericResult> GetByCompany(string CompanyCode, string Type, string CustomerGrpId, string CustomerId, string Status
            , string Keyword, string CustomerName, string CustomerRank, string Address, string Phone, DateTime? DOB,decimal? display=null)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("CustomerGrpId", string.IsNullOrEmpty(CustomerGrpId) ? "" : CustomerGrpId);
                parameters.Add("CustomerId", string.IsNullOrEmpty(CustomerId) ? "" : CustomerId);
                parameters.Add("Status", string.IsNullOrEmpty(Status) ? "" : Status);
                parameters.Add("Key", string.IsNullOrEmpty(Keyword) ? "" : Keyword);
                parameters.Add("Type", string.IsNullOrEmpty(Type) ? "" : Type);
                parameters.Add("CustomerName", string.IsNullOrEmpty(CustomerName) ? "" : CustomerName);
                parameters.Add("CustomerRank", string.IsNullOrEmpty(CustomerRank) ? "" : CustomerRank);
                parameters.Add("Address", string.IsNullOrEmpty(Address) ? "" : Address);
                parameters.Add("Phone", string.IsNullOrEmpty(Phone) ? "" : Phone);
                parameters.Add("DOB", DOB.HasValue ? DOB : null);
                if (display != null)
                {
                    parameters.Add("Selectop", display);
                }
                var data = await _customerRepository.GetAllAsync("USP_S_M_Customer", parameters, commandType: CommandType.StoredProcedure);
                //var data = await _customerRepository.GetAllAsync($"select * from M_Customer where CompanyCode='{CompanyCode}'", null, commandType: CommandType.Text);
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
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _customerRepository.GetAllAsync($"select * from M_Customer with (nolock) where CompanyCode=N'{CompanyCode}'", null, commandType: CommandType.Text);
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
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("CustomerGrpId", "");
                parameters.Add("CustomerId", Code);
                parameters.Add("Status", "");
                parameters.Add("Key", Code);
                parameters.Add("Type", "");
                parameters.Add("CustomerName", "");
                parameters.Add("Address", "");
                parameters.Add("Phone", "");
                parameters.Add("DOB", null);

                var data = await _customerRepository.GetAsync("USP_S_M_Customer", parameters, commandType: CommandType.StoredProcedure);
                //var data = await _customerRepository.GetAsync($"select * from M_Customer where  CompanyCode='{CompanyCode}' and CustomerId = '{Code} or '" , null, commandType: CommandType.Text);
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



        public async Task<PagedList<MCustomer>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _customerRepository.GetAllAsync($"select * from M_Customer with (nolock) where CustomerId like N'%{userParams.keyword}%' or CustomerName like N'%{userParams.keyword}%' or Address like N'%{userParams.keyword}%'", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                if (userParams.OrderBy == "byName")
                {
                    data.OrderByDescending(x => x.CustomerName);
                }
                if (userParams.OrderBy == "byId")
                {
                    data.OrderByDescending(x => x.CustomerId);
                }
                return await PagedList<MCustomer>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<GenericResult> Update(MCustomer model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("CustomerId", model.CustomerId, DbType.String);
                parameters.Add("CustomerGrpId", model.CustomerGrpId);
                parameters.Add("CustomerName", model.CustomerName);
                parameters.Add("Address", model.Address);
                parameters.Add("Phone", model.Phone);
                parameters.Add("DOB", model.Dob);
                parameters.Add("JoinedDate", model.JoinedDate);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("CusType", model.CusType);
                parameters.Add("Status", string.IsNullOrEmpty(model.Status) ? "N" : model.Status);
                parameters.Add("Email", model.Email);
                parameters.Add("Gender", model.Gender);
                parameters.Add("CardNo", model.CardNo);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                parameters.Add("DoNotAccumPoints", model.DoNotAccumPoints);
                parameters.Add("CustomerRank", model.CustomerRank);
                parameters.Add("CustomerRankName", model.CustomerRankName);
                parameters.Add("RewardPoints", model.RewardPoints);
                //            public string Email { get; set; }
                //public string Gender { get; set; }
                //public string CardNo { get; set; }
                var affectedRows = _customerRepository.Update("USP_U_M_Customer", parameters, commandType: CommandType.StoredProcedure);
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
