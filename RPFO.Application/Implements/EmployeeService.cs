
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IGenericRepository<MEmployee> _employeeRepository;
        private readonly IGenericRepository<MStore> _storeRepository;
        private readonly IGenericRepository<MEmployeeStore> _employeeStoreRepository;
        private readonly string MWIClient = "";
        private readonly IGeneralSettingService _settingService;
        string accessTokenEmployee = "";
        private readonly IMapper _mapper;
        public EmployeeService(IGenericRepository<MEmployee> employeeRepository, IConfiguration config,  IGeneralSettingService settingService, IGenericRepository<MEmployeeStore> employeeStoreRepository, IGenericRepository<MStore> storeRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _employeeRepository = employeeRepository;
            _employeeStoreRepository = employeeStoreRepository;
            _storeRepository = storeRepository;
            _mapper = mapper;
            _settingService = settingService;
            accessTokenEmployee = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("AccessTokenEmployee"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);

        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<EmployeeResultViewModel> resultlist = new List<EmployeeResultViewModel>();
            try
            {
                foreach (var item in model.Employee)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var itemResult = await CreateWithStore(item);

                    EmployeeResultViewModel itemRs = new EmployeeResultViewModel();
                    itemRs = _mapper.Map<EmployeeResultViewModel>(item);
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

        public async Task<bool> checkExist(string CompanyCode, string EmployeeId)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;

            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("Position", "");
            parameters.Add("EmployeeId", EmployeeId);
            parameters.Add("Status", "");
            var affectedRows = await _employeeRepository.GetAsync("USP_S_M_Employee", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> checkStoreExist(string CompanyCode, string StoreId)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("StoreId", StoreId);
            parameters.Add("Status", "");
            var affectedRows = await _storeRepository.GetAsync("USP_S_M_Store", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<GenericResult> CreateWithStore(EmployeeViewModel model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var exist = await checkExist(model.CompanyCode, model.EmployeeId);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.EmployeeId + " existed.";
                    return result;
                }
                var storeExist = await checkStoreExist(model.CompanyCode, model.StoreId);
                if (storeExist == false)
                {
                    result.Success = false;
                    result.Message = model.StoreId + " not existed.";
                    return result;
                }
                using (IDbConnection db = _storeRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {
                                var parameters = new DynamicParameters();
                                parameters.Add("EmployeeId", model.EmployeeId);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("EmployeeName", model.EmployeeName);
                                parameters.Add("Position", model.Position);
                                parameters.Add("Status", model.Status);
                                parameters.Add("CreatedBy", model.CreatedBy);

                                //var affectedRows = _userRepository.Insert("USP_I_M_User", parameters, commandType: CommandType.StoredProcedure);
                                var affectedRows = db.Execute("USP_I_M_Employee", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                parameters = new DynamicParameters();
                                parameters.Add("EmployeeId", model.EmployeeId);
                                parameters.Add("StoreId", model.StoreId);
                                parameters.Add("FromDate", model.FromDate);
                                parameters.Add("ToDate", model.ToDate);

                                //var affected = _userStoreRepository.Insert("USP_I_M_UserStore", parameters, commandType: CommandType.StoredProcedure);
                                var affected = db.Execute("USP_I_M_EmployeeStore", parameters, commandType: CommandType.StoredProcedure, transaction: tran);


                                result.Success = true;
                                //result.Message = model.UserId.ToString();
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
        public async Task<GenericResult> Create(MEmployee model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;

                parameters.Add("EmployeeId", model.EmployeeId);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("EmployeeName", model.EmployeeName);
                parameters.Add("Position", model.Position);
                parameters.Add("Status", model.Status);
                parameters.Add("CreatedBy", model.CreatedBy);

                var exist = await checkExist(model.CompanyCode, model.EmployeeId);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.EmployeeId + " existed.";
                    return result;
                }

                var affectedRows = _employeeRepository.Insert("USP_I_M_Employee", parameters, commandType: CommandType.StoredProcedure);
                if (model.Stores != null && model.Stores.Count > 0)
                {
                    _employeeStoreRepository.Update($"delete M_EmployeeStore where EmployeeId ='{model.EmployeeId}' ", parameters, commandType: CommandType.Text);
                    foreach (var store in model.Stores)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("EmployeeId", store.EmployeeId);
                        parameters.Add("StoreId", store.StoreId);
                        parameters.Add("FromDate", store.FromDate);
                        parameters.Add("ToDate", store.ToDate);
                        //var affected = _userStoreRepository.Insert("USP_I_M_UserStore", parameters, commandType: CommandType.StoredProcedure);
                        var affected = _employeeStoreRepository.Execute("USP_I_M_EmployeeStore", parameters, commandType: CommandType.StoredProcedure);

                    }
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

        public Task<GenericResult> Delete(string Code)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _employeeRepository.GetAllAsync($"select * from M_Employee with (nolock) where CompanyCode='{CompanyCode}'", null, commandType: CommandType.Text);
                foreach (var employee in data)
                {
                    var dataStore = await _employeeStoreRepository.GetAllAsync($"select * from M_EmployeeStore with (nolock) where EmployeeId = N'{employee.EmployeeId}' ", null, commandType: CommandType.Text);
                    employee.Stores = dataStore;
                }
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

        public async Task<GenericResult> GetByUser(string CompanyCode, string User)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _employeeRepository.GetAllAsync($"USP_S_M_EmployeeByUser '{CompanyCode}','{User}'", null, commandType: CommandType.Text);
                foreach (var employee in data)
                {
                    var dataStore = await _employeeStoreRepository.GetAllAsync($"select * from M_EmployeeStore with (nolock) where EmployeeId = N'{employee.EmployeeId}' ", null, commandType: CommandType.Text);
                    employee.Stores = dataStore;
                }
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
        public async Task<GenericResult> GetByStore(string CompanyCode, string StoreCode, bool? CheckAvailable)
        {
            GenericResult result = new GenericResult();
            try
            {
             
                var data = await _employeeRepository.GetAllAsync($"USP_S_M_EmployeeByStore '{CompanyCode}','{StoreCode}'", null, commandType: CommandType.Text);
                if (data!=null && data.Count() > 0 && CheckAvailable != null && CheckAvailable.Value)
                {
                    var settingData = await _settingService.GetGeneralSettingByStore(CompanyCode, StoreCode);
                    List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                    if (settingData.Success)
                    {
                        SettingList = settingData.Data as List<GeneralSettingStore>;
                    }
                    if (SettingList != null && SettingList.Count() > 0)
                    {
                        var setting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "EmployeeSystem").FirstOrDefault();
                        if (setting != null && setting.SettingValue == "MWI")
                        {
                            List<MEmployee> employees = new List<MEmployee>();
                            if (string.IsNullOrEmpty(setting.CustomField1))
                            {
                                result.Success = false;
                                result.Message = "Please check setup. MWI Url can't null";
                                return result;
                            }
                            var employeesGetData =  MWIEmployeeget(setting.CustomField1).Result;
                            if(employeesGetData.Data!=null )
                            {
                                var employeesGet = JsonConvert.DeserializeObject<List<EmployeeXModel>>(employeesGetData.Data.ToString());// employeesGetData.Data as List<EmployeeXModel>;

                                foreach (var employee in data)
                                {
                                    var check = employeesGet.Where(x => x.personnel_code == employee.EmployeeId);

                                    if (check != null && check.Count() > 0)
                                    {
                                        employees.Add(employee);
                                    }
                                }

                                data = employees;
                            }    
                            
                        }
                    }
                     

                }
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
        private HttpClient GetHttpClient(string UrlClient)
        {
            var client = new HttpClient();

            if (!string.IsNullOrEmpty(UrlClient))
            {
                client.BaseAddress = new Uri(UrlClient);
            }
            else
            {
                client.BaseAddress = new Uri(this.MWIClient);
            }
            //


            return client;
        }
        public class EmployeeXModel
        {
            public string personnel_code { get; set; }
            public string date { get; set; }
            public string cal_workday { get; set; }
            public string checkInOut { get; set; }
          
         }

        public async Task<GenericResult> MWIEmployeeget(string Url)
        {
            GenericResult result = new GenericResult();
            try
            {
                if(string.IsNullOrEmpty(accessTokenEmployee))
                {
                    result.Success = false;
                    result.Message = "Access Token is null. Please check Setup in API"; 
                    return result;
                }    
                HttpClient client = GetHttpClient(Url);
                ///api/Auth/Login
                //string requestUriAuth = "/api/Auth/Login";
                //var modelAuth = new { userName = "user_test", password = "1234" };
                //var contentAuth = new StringContent(modelAuth.ToJson(), Encoding.UTF8, "application/json");
                //var responseAuth = await client.PostAsync(requestUriAuth, contentAuth);
                //var responseStringAuth = await responseAuth.Content.ReadAsStringAsync();
                //var rsModelAuth = JsonConvert.DeserializeObject<OMSResponseModel>(responseStringAuth);


                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //string AccessToken = rsModelAuth.token;

                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                var Filter = new EmployeeXModel();
                Filter.date = DateTime.Now.ToString("dd/MM/yyyy");// "28/11/2021";
                string filterStr = "[" + Filter.ToJson() + "]";
                string requestUri = $"/Timekeeping/GetStaffShiftList?access_token={accessTokenEmployee}&filters={filterStr}";
                //var model = new
                //{
                //    terminalID = terminalID,
                //    merchantID = merchantID,
                //    amount = int.Parse(amount.ToString()),
                //    operatorID = operatorID,
                //    product = product,
                //    accountNo = accountNo,
                //    transId = transId
                //};

                //var modelJs = model.ToJson();
                //var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = client.GetAsync(requestUri).Result;

                var responseString = await response.Content.ReadAsStringAsync();
                var rsModel = JsonConvert.DeserializeObject<GenericResult>(responseString);

                return rsModel;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                //result.Status = -1;
                return result;
            }
        }

        public async Task<GenericResult> GetByCode(string CompanyCode, string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
 
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("Position", "");
                parameters.Add("EmployeeId", Code);
                parameters.Add("Status", "");
                var data =  _employeeRepository.Get("USP_S_M_Employee", parameters, commandType: CommandType.StoredProcedure);
                //var data = await _employeeRepository.GetAsync($"select * from M_Employee with (nolock) where CompanyCode= N'{CompanyCode}' and EmployeeId= N'{Code}'", null, commandType: CommandType.Text);
                if(data != null && !string.IsNullOrEmpty(data.EmployeeId))
                {
                    var dataStore = await _employeeStoreRepository.GetAllAsync($"select * from M_EmployeeStore with (nolock) where EmployeeId = N'{Code}' ", null, commandType: CommandType.Text);
                    data.Stores = dataStore;
                    result.Success = true;
                    result.Data = data;
                }     
                else
                {
                    result.Success = false;
                    result.Data = null;
                }    
               
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;

        }

        public async Task<GenericResult> GetByUser(string User)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedList<MEmployee>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _employeeRepository.GetAllAsync($"select * from M_Employee with (nolock) where EmployeeId like N'%{userParams.keyword}%' or EmployeeName like N'%{userParams.keyword}%'", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                if (userParams.OrderBy == "byName")
                {
                    data.OrderByDescending(x => x.EmployeeName);
                }
                if (userParams.OrderBy == "byId")
                {
                    data.OrderByDescending(x => x.EmployeeId);
                }
                return await PagedList<MEmployee>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GenericResult> Update(MEmployee model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("EmployeeId", model.EmployeeId);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("EmployeeName", model.EmployeeName);
                parameters.Add("Position", model.Position);
                parameters.Add("Status", model.Status);
                parameters.Add("ModifiedBy", model.ModifiedBy); 
                var affectedRows = _employeeRepository.Update("USP_U_M_Employee", parameters, commandType: CommandType.StoredProcedure);
                _employeeStoreRepository.Update($"delete M_EmployeeStore where EmployeeId = N'{model.EmployeeId}' ", parameters, commandType: CommandType.Text);
                if (model.Stores!=null && model.Stores.Count > 0)
                {
                    
                    foreach (var store in model.Stores)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("EmployeeId", store.EmployeeId);
                        parameters.Add("StoreId", store.StoreId);
                        parameters.Add("FromDate", store.FromDate);
                        parameters.Add("ToDate", store.ToDate); 
                        //var affected = _userStoreRepository.Insert("USP_I_M_UserStore", parameters, commandType: CommandType.StoredProcedure);
                        var affected = _employeeStoreRepository.Execute("USP_I_M_EmployeeStore", parameters, commandType: CommandType.StoredProcedure );
                         
                    }
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
    } 

}
