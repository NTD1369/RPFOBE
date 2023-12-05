using AutoMapper;
using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Constants;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Extensions;
using RPFO.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
//using Dapper.SimpleCRUD;

namespace RPFO.Application.Implements
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<MUser> _userRepository;
        private readonly IGenericRepository<MUserRole> _userRoleRepository;
        private readonly IGenericRepository<MUserLicense> _userLicenseRoleRepository;
        //private readonly IGenericRepository<MUserStore> _userStoreRepository;
        private readonly IGenericRepository<MStore> _storeRepository;
        private readonly IGenericRepository<MRole> _roleRepository;
        private ICommonService _commonService;
        private string CompanyCode;
        string SVKey = "";
        private readonly IMapper _mapper;
        public UserService(IGenericRepository<MUser> userRepository, ICommonService commonService, IGenericRepository<MUserStore> userStoreRepository, IGenericRepository<MUserRole> userRoleRepository,
            IGenericRepository<MStore> storeRepository, IGenericRepository<MUserLicense> userLicenseRoleRepository, IMapper mapper, IGenericRepository<MRole> roleRepository, IRoleService roleService, IConfiguration config/* , IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _userRepository = userRepository;
            //_userStoreRepository = userStoreRepository;
            _storeRepository = storeRepository;
            _userRoleRepository = userRoleRepository;
            _commonService = commonService;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _userLicenseRoleRepository = userLicenseRoleRepository;
            //_roleService = roleService;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;
            SVKey = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("SVKey"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            if (string.IsNullOrEmpty(SVKey))
            {
                SVKey = "";
            }
            this.CompanyCode = Encryptor.DecryptString(config.GetConnectionString("CompanyCode"), AppConstants.TEXT_PHRASE);
            initService();
        }

        public GenericResult initService()
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _userRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    string queryCheckAndCreate = "IF (OBJECT_ID('USP_S_M_UserWRole') IS NULL)  begin declare @string nvarchar(MAX) = '';" +
                        " set @string = 'create PROCEDURE [dbo].[USP_S_M_UserWRole] @CompanyCode nvarchar(50), @UserId	nvarchar(150)  AS " +
                        "begin  select a.* , b.RoleId RoleId, c.RoleName RoleName  from M_User a with (nolock) " +
                        " left join M_UserRole b with (nolock) on a.UserId = b.UserId " +
                        " left join M_Role c with (nolock) on c.RoleId = b.RoleId " +
                        " where (ISNULL(@CompanyCode, '''') = '''' OR a.CompanyCode = @CompanyCode )" +
                        " AND (convert(nvarchar(150), a.UserId) = @UserId  or ISNULL(@UserId, '''')='''')   end '; " +
                        "EXECUTE sp_executesql @string;  end";
                    db.Execute(queryCheckAndCreate);

                    queryCheckAndCreate = "IF (OBJECT_ID('USP_GetLicenseInfor') IS NULL)  begin declare @string nvarchar(MAX) = '';" +
                      " set @string = ' create PROCEDURE[dbo].[USP_GetLicenseInfor] @CompanyCode nvarchar(50), @KeyId nvarchar(50) = null AS begin set @KeyId = '''' end '" +
                      //"begin  select a.* , b.RoleId RoleId, c.RoleName RoleName  from M_User a with (nolock) " +
                      //" left join M_UserRole b with (nolock) on a.UserId = b.UserId " +
                      //" left join M_Role c with (nolock) on c.RoleId = b.RoleId " +
                      //" where (ISNULL(@CompanyCode, '''') = '''' OR a.CompanyCode = @CompanyCode )" +
                      //" AND (convert(nvarchar(150), a.UserId) = @UserId  or ISNULL(@UserId, '''')='''')   end '; " +
                      " EXECUTE sp_executesql @string;  end";
                    db.Execute(queryCheckAndCreate);
                    db.Close();
                    result.Success = true;
                    return result;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                    return result;

                }
            }


        }
        
     

        private List<MRole> roles = new List<MRole>();
        public async Task<List<MRole>> GetAllRole()
        {
            var data = await _roleRepository.GetAllAsync("select * from M_Role with (nolock)", null, commandType: CommandType.Text);
            return data;
        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<UserResultViewModel> resultlist = new List<UserResultViewModel>();
            try
            {
                roles = await GetAllRole();
                
                foreach (var item in model.User)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    MRole role = new MRole();
                    foreach (var roleX in roles)
                    {
                        if (item.Role == roleX.RoleName)
                        {
                            role = roleX;
                            item.RoleId = role.RoleId.ToString();
                        }
                    }
                    var itemResult = await CreateWithStore(item);
                    //if (itemResult.Success == false)
                    //{
                    UserResultViewModel itemRs = new UserResultViewModel();
                    itemRs = _mapper.Map<UserResultViewModel>(item);
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
        public async Task<bool> checkExist(string CompanyCode, string UserName)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("UserName", UserName);
            parameters.Add("Status", "");
            var affectedRows = await _userRepository.GetAsync("USP_S_M_User", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> CreateWithStore(UserViewModel model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var exist = await checkExist(model.CompanyCode, model.Username);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.Username + " existed.";
                    return result;
                }
                var storeExist = await checkStoreExist(model.CompanyCode, model.StoreId);
                if (storeExist == false)
                {
                    result.Success = false;
                    result.Message = model.Username + " not existed.";
                    return result;
                }
                using (IDbConnection db = _userRepository.GetConnection())
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
                                model.UserId = Guid.NewGuid();
                                parameters.Add("UserId", model.UserId);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("Username", model.Username.Replace(" ", ""));
                                parameters.Add("LastName", model.LastName);
                                parameters.Add("FirstName", model.FirstName);
                                parameters.Add("Position", model.Position);
                                string password = Encryptor.EncryptString(model.Password, AppConstants.TEXT_PHRASE);
                                parameters.Add("Password", password);
                                parameters.Add("CreatedBy", model.CreatedBy);
                                parameters.Add("Status", model.Status);
                                parameters.Add("DefEmployee", model.DefEmployee);
                                //string code = model.Username + "_" + model.Password;
                                //string code = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 15);
                                string code = model.UserId.ToString().Replace("-", "").Substring(0, 10) + ConvertUnicodeStringToAscii(model.Password.Substring(0, 2));
                                //model.QRBarcode = Encryptor.EncryptString(model.QRBarcode, AppConstants.TEXT_PHRASE);
                                model.QRBarcode = Encryptor.EncryptString(code, AppConstants.TEXT_PHRASE);
                                parameters.Add("QRBarcode", model.QRBarcode);

                                //var affectedRows = _userRepository.Insert("USP_I_M_User", parameters, commandType: CommandType.StoredProcedure);
                                var affectedRows = db.Execute("USP_I_M_User", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                if (!string.IsNullOrEmpty(model.StoreId))
                                {
                                    parameters = new DynamicParameters();
                                    parameters.Add("UserId", model.UserId);
                                    parameters.Add("StoreId", model.StoreId); 
                                    //var affected = _userStoreRepository.Insert("USP_I_M_UserStore", parameters, commandType: CommandType.StoredProcedure);
                                    var affected = db.Execute("USP_I_M_UserStore", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                }

                                if (!string.IsNullOrEmpty(model.RoleId))
                                {
                                    parameters = new DynamicParameters();
                                    parameters.Add("UserId", model.UserId);
                                    parameters.Add("RoleId", model.RoleId);

                                    //var affected = _userStoreRepository.Insert("USP_I_M_UserStore", parameters, commandType: CommandType.StoredProcedure);
                                    var affectedRole = db.Execute("USP_I_M_UserRole", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                }


                                result.Success = true;
                                result.Message = model.UserId.ToString();
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

        public async Task<GenericResult> Create(MUser model)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                model.UserId = Guid.NewGuid();
                parameters.Add("UserId", model.UserId);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Username", model.Username.Replace(" ", ""));
                parameters.Add("LastName", model.LastName);
                parameters.Add("FirstName", model.FirstName);
                parameters.Add("Position", model.Position);
                string password = Encryptor.EncryptString(model.Password, AppConstants.TEXT_PHRASE);

                parameters.Add("Password", password);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                parameters.Add("DefEmployee", model.DefEmployee);

                //string code = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 15); //model.Username + "_" + model.Password;
                string code = model.UserId.ToString().Replace("-", "").Substring(0, 10) + ConvertUnicodeStringToAscii(model.Password.Substring(0, 2));
                //model.QRBarcode = Encryptor.EncryptString(model.QRBarcode, AppConstants.TEXT_PHRASE);
                model.QRBarcode = Encryptor.EncryptString(code, AppConstants.TEXT_PHRASE);
                parameters.Add("QRBarcode", model.QRBarcode);
                //if (!string.IsNullOrEmpty(model.QRBarcode))
                //{
                //    model.QRBarcode = Encryptor.EncryptString(model.QRBarcode, AppConstants.TEXT_PHRASE);
                //    parameters.Add("QRBarcode", model.QRBarcode);
                //}

                var exist = await checkExist(model.CompanyCode, model.Username);
                if (exist == true)
                {
                    rs.Success = false;
                    rs.Message = model.Username + " existed.";
                    return rs;
                }
                var affectedRows = _userRepository.Insert("USP_I_M_User", parameters, commandType: CommandType.StoredProcedure);
                if (!string.IsNullOrEmpty(model.RoleId))
                {
                    parameters = new DynamicParameters();
                    parameters.Add("UserId", model.UserId);
                    parameters.Add("RoleId", model.RoleId);
                    _userRoleRepository.Insert("USP_I_M_UserRole", parameters, commandType: CommandType.StoredProcedure);
                }
                //result.Success = true;
                //result.Message = key;
                //var Id = await _userRepository.GetConnection().InsertAsync<Guid, MUser>(model);
                //model.UserId = Id;
                rs.Success = true;
                rs.Data = model;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }

        public async Task<GenericResult> Delete(string Id)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetAllUsers(string CompanyCode, bool? getLicense)
        {
            GenericResult rs = new GenericResult();
            try
            {
                //var dataX = _userRepository.GetConnection().GetList


                //select a.* , b.RoleId RoleId, b.RoleId RoleId from M_User a with(nolock) left join M_UserRole b with(nolock) on a.UserId = b.UserId
               
                var data = await _userRepository.GetAllAsync($"USP_S_M_UserWRole '{CompanyCode}',''", null, commandType: CommandType.Text);
                List<MUserLicense> userLicenses = new List<MUserLicense>();
                if(getLicense.HasValue && getLicense.Value)
                {
                    if (data != null && data.Count > 0)
                    {
                        var dataLicense = Get_TokenLicense(CompanyCode, "", "").Result;

                        if (dataLicense != null && dataLicense.Success)
                        {
                            var licenseList = dataLicense.Data as List<SLicense>;
                            if (licenseList != null && licenseList.Count > 0)
                            {
                                foreach (var license in licenseList)
                                {
                                    if (license.Users != null && license.Users.Count > 0)
                                    {
                                        userLicenses.AddRange(license.Users);
                                    }
                                }

                            }
                        }
                    }
                }    
             
                foreach (var user in data)
                {
                    user.Password = Encryptor.DecryptString(user.Password, AppConstants.TEXT_PHRASE);
                    if (!string.IsNullOrEmpty(user.QRBarcode))
                    {
                        user.QRBarcode = Encryptor.DecryptString(user.QRBarcode, AppConstants.TEXT_PHRASE);
                    }
                    if (userLicenses != null && userLicenses.Count > 0)
                    {
                        var lincenData = userLicenses.Where(x => x.UserId == user.UserId.ToString()).FirstOrDefault();
                        if (lincenData != null)
                        {
                            user.License = lincenData.LicenseId;
                        }
                    }
                   
                   
                }
                rs.Success = true;
                rs.Data = data;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }

        public async Task<GenericResult> GetById(string CompanyCode, string Id)
        {
            GenericResult rs = new GenericResult();
            try
            {
                //var xxx = _userRepository.GetConnection().GetList<MUser>();

                //$"select a.* , b.RoleId RoleId from M_User a with (nolock) left join M_UserRole b with (nolock) on a.UserId = b.UserId where a.UserId='{Id}'"
                var data = await _userRepository.GetAsync($"USP_S_M_UserWRole N'{CompanyCode}', N'{Id}'", null,
                  commandType: CommandType.Text);
                rs.Success = true;
                data.Password = Encryptor.DecryptString(data.Password, AppConstants.TEXT_PHRASE);
                if (!string.IsNullOrEmpty(data.QRBarcode))
                {
                  
                    data.QRBarcode = Encryptor.DecryptString(data.QRBarcode, AppConstants.TEXT_PHRASE);
                }
                rs.Data = data;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }
        public async Task<GenericResult> GenQRCode(string CompanyCode, string UserName, string Password)
        {
            GenericResult rs = new GenericResult();
            try
            {
                if (string.IsNullOrEmpty(CompanyCode))
                {
                    rs.Success = false;
                    rs.Message = "Company Code is null";
                    return rs;
                }
                if (string.IsNullOrEmpty(UserName))
                {
                    rs.Success = false;
                    rs.Message = "User Name is null";
                    return rs;
                }
                if (string.IsNullOrEmpty(Password))
                {
                    rs.Success = false;
                    rs.Message = "Password is null";
                    return rs;
                }
                var resultLogin =  Login(UserName,  Password, "").Result; 
                if (resultLogin.Success == false || resultLogin.Data == null)
                {
                    rs.Success = false;
                    rs.Message = "Please login";
                    return rs;
                }    
                else
                {
                    var geneDataa = GetAllUsers(CompanyCode, false).Result;
                    if(geneDataa.Success == true)
                    {
                        var data =  geneDataa.Data as List<MUser>;
                        if(data!=null && data?.Count >0)
                        {
                            string querry = "";
                            foreach(var user in data)
                            {
                                string code = user.UserId.ToString().Replace("-", "").Substring(0, 10) + ConvertUnicodeStringToAscii(user.Password.Substring(0, 2));// model.Username + model.Password;
                                string QRBarcode = Encryptor.EncryptString(code, AppConstants.TEXT_PHRASE);
                              
                                querry += $" update M_User set QRBarcode = N'{QRBarcode}' where UserId = N'{user.UserId}' and CompanyCode = N'{CompanyCode}'; ";

                            }
                            if(!string.IsNullOrEmpty(querry))
                            {
                                _userRepository.Execute(querry, null, commandType: CommandType.Text);
                            }
                            rs.Success = true;
                            rs.Message = "";
                        }    
                        else
                        {
                            rs.Success = false;
                            rs.Message = "Data is null";
                        }    
                    }   
                    else
                    {
                        rs.Success = false;
                        rs.Message = "Please login";

                    }    
                    
                }    
             
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }
        public async Task<GenericResult> GetByUsername(string CompanyCode, string UserName)
        {
            GenericResult rs = new GenericResult();
            try
            {
                //var xxx = _userRepository.GetConnection().GetList<MUser>();

                //$"select a.* , b.RoleId RoleId from M_User a with (nolock) left join M_UserRole b with (nolock) on a.UserId = b.UserId where a.UserId='{Id}'"
                var data = await _userRepository.GetAsync($"USP_S_M_UserWRole N'{CompanyCode}', N'' ,  N'{UserName}'", null, commandType: CommandType.Text);
                rs.Success = true;
                data.Password = Encryptor.DecryptString(data.Password, AppConstants.TEXT_PHRASE);
                if (!string.IsNullOrEmpty(data.QRBarcode))
                {

                    data.QRBarcode = Encryptor.DecryptString(data.QRBarcode, AppConstants.TEXT_PHRASE);
                }
                rs.Data = data;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }
        private HttpClient GetHttpClient(string UrlClient)
        {
            var client = new HttpClient();

            if (!string.IsNullOrEmpty(UrlClient))
            {
                client.BaseAddress = new Uri(UrlClient);
            }


            return client;
        }
        public async Task<GenericResult> Get_TokenLicense(string CompanyCode, string License, string User)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _userLicenseRoleRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("KeyId", string.IsNullOrEmpty(License)?"" : License);

                   
                    var items = await db.QueryAsync<SLicense>("USP_GetLicenseInfor", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);

                    if (items != null && items.ToList().Count > 0)
                    {
                        
                        foreach (var item in items)
                        {
                            item.Users = new List<MUserLicense>();
                            if(!string.IsNullOrEmpty(item.Token))
                            {
                                var Token = Utilities.Helpers.Encryptor.DecryptString(item.Token, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE); 
                                item.LicenseInfor = JsonConvert.DeserializeObject<LicenseInfor>(Token);
                            }    
                          
                            if (!string.IsNullOrEmpty(item.MergeToken))
                            {
                                string descrypt = Encryptor.DecryptString(item.MergeToken, AppConstants.TEXT_PHRASE);
                                if(!string.IsNullOrEmpty(descrypt))
                                {
                                    var list = JsonConvert.DeserializeObject<List<string>>(descrypt);
                                    if(list!=null && list.Count > 0)
                                    {
                                        foreach(string str in list)
                                        {
                                            MUserLicense userLicense = new MUserLicense();
                                            userLicense.CompanyCode = CompanyCode;
                                            userLicense.UserId = str;
                                            userLicense.LicenseId = item.LicenseId;
                                            var checkModel = item.Users.Where(x => x.UserId == str && x.CompanyCode == CompanyCode && x.LicenseId == item.LicenseId).FirstOrDefault();
                                            
                                            if(checkModel==null)
                                            {
                                                item.Users.Add(userLicense); 
                                            }    
                                            //= list;
                                        }    
                                        
                                    }    
                                }


                            }

                        }

                        result.Success = true;
                        result.Data = items;

                        if (!string.IsNullOrEmpty(User))
                        {
                            var userModel = new MUser();
                            var user = await GetByUsername(CompanyCode, User);

                            if (user != null && user.Success)
                            {
                                userModel = user.Data as MUser;
                            }
                            else
                            {
                                result.Success = false;
                                result.Message = "User does not existed.";
                                return result;
                            }
                            bool checkLicense = false;
                            List<SLicense> licenseList = new List<SLicense>();

                            foreach (var item in items)
                            {
                                if(item.Users!=null && item.Users.Count > 0)
                                {
                                    var userCheck = item.Users.Where(x => x.UserId == userModel.UserId.ToString()).FirstOrDefault();
                                    if (userCheck != null)
                                    {
                                        //licenseList.Add(item);
                                        checkLicense = true;
                                        result.Success = true;
                                        result.Data = item;

                                        if (item.LicenseInfor.ValidTo != null && item.LicenseInfor.ValidTo.HasValue)
                                        {
                                            DateTime serverTime = DateTime.Now;
                                            if (!string.IsNullOrEmpty(SVKey))
                                            {
                                                try
                                                {
                                                    HttpClient client = GetHttpClient(SVKey);
                                                    ///api/Auth/Login
                                                    string requestUriAuth = "/api/Common/TimeOnServer";
                                                    //var modelAuth = new { userName = "user_test", password = "1234" };
                                                    //var contentAuth = new StringContent(modelAuth.ToJson(), Encoding.UTF8, "application/json");
                                                    var responseAuth = await client.GetAsync(requestUriAuth);
                                                    var responseStringAuth = await responseAuth.Content.ReadAsStringAsync();
                                                    //var rsModelAuth = JsonConvert.DeserializeObject<string>(responseStringAuth);
                                                    serverTime = DateTime.Parse(responseStringAuth);
                                                }
                                                catch (Exception ex)
                                                {
                                                    //throw ex;
                                                    serverTime = DateTime.Now;
                                                }

                                            }

                                            //TimeSpan difference = Model.ValidTo.Value - serverTime;
                                            if (item.LicenseInfor.ValidTo.Value.Date < serverTime.Date)
                                            {
                                                result.Success = false;
                                                result.Message = "License has expired";
                                            }

                                            TimeSpan difference = item.LicenseInfor.ValidTo.Value - serverTime;
                                            int days = (int)difference.TotalDays + 1;
                                            item.ExpiredNumber = days;

                                            //TimeSpan difference = item.LicenseInfor.ValidTo.Value - serverTime;
                                            //int days = (int)difference.TotalDays;
                                            //item.ExpiredNumber = days;
                                        }
                                        return result;
                                    }
                                   
                                }

                            }
                            //foreach(SLicense license in licenseList)
                            //{

                            //}    
                            if(checkLicense == false)
                            { 
                                result.Success = false;
                                result.Message = "User not set License. Please contact to your admin or support team.";
                                return result; 
                            }    

                            //if(userModel!=null && userModel.UserId)
                            //{
                            //    var userCheck = list.Where(x => x.UserId == userModel.UserId.ToString()).FirstOrDefault();
                            //    if (userCheck != null)
                            //    {
                            //        result.Success = true;
                            //        result.Data = item;
                            //        if (item.ValidTo != null && item.ValidTo.HasValue)
                            //        {
                            //            if (item.ValidTo.Value.Date < DateTime.Now.Date)
                            //            {
                            //                result.Success = false;
                            //                result.Message = "License has expired";
                            //            }
                            //        }
                            //        return result;
                            //    }
                            //    else
                            //    {
                            //        result.Success = false;
                            //        result.Message = "User not set License. Please contact to your admin or support team.";
                            //        return result;

                            //        //result.Data = items;
                            //    }
                            //}    

                        }

                    }
                    else
                    {
                        return null;
                        //result.Success = false;
                        //result.Message = "can't get License infor. Please contact to your admin or support team.";
                        //return result;
                    }

                    //if (items != null && items.ToList().Count > 0)
                    //{

                    //    if (string.IsNullOrEmpty(User))
                    //    {
                    //        result.Success = true;
                    //        result.Data = items;
                    //    }
                    //    else
                    //    {
                    //        var user = await GetByUsername(CompanyCode, User);

                    //        if (user != null && user.Success)
                    //        {
                    //            var userModel = user.Data as MUser;
                               
                    //        }
                    //        else
                    //        {
                    //            result.Success = false;
                    //            result.Message = "User does not existed.";
                    //            return result;
                    //        }


                    //    }

                    //}
                    //else
                    //{
                    //    //result.Success = true;
                    //    //result.Data = null;
                    //    return null;
                    //}


                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                    //result.Data = failedlist;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }
            //result.Data = failedlist;
            return result;
            //throw new NotImplementedException();
        }

        public GenericResult GetUserLicense(string CompanyCode, string License )
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _userRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string queryGet = $"select UserId from M_UserLicense with (nolock) where CompanyCode=N'{CompanyCode}' and LicenseId= '{License}'";
                    var reader =  db.Query<string>(queryGet, null, commandType: CommandType.Text);
                     

                    result.Success = true;
                    result.Data = reader;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                    //result.Data = failedlist;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }
            //result.Data = failedlist;
            return result;
        }
        public GenericResult SetLicenseForUser(string CompanyCode, string License, List<string> UserList)
        {

            GenericResult rs = new GenericResult();
            try
            {
                if(string.IsNullOrEmpty(CompanyCode))
                {
                    rs.Success = false;
                    rs.Message = "Please input companyCode";
                    return rs;

                }
                if (string.IsNullOrEmpty(License))
                {
                    rs.Success = false;
                    rs.Message = "Please input License";
                    return rs;
                }
                if (UserList != null && UserList.Count > 0)
                {

                }
                else
                {
                    var userData = GetAllUsers(CompanyCode, false).Result;
                    if (userData != null)
                    {
                        List<MUser> users = userData.Data as List<MUser>;
                        if (users != null && users.Count > 0)
                        {
                            UserList = new List<string>();
                            foreach (MUser user in users)
                            {
                                UserList.Add(user.UserId.ToString());
                            }
                        }
                    }
                }
                string updateQuer = "";

                if(!string.IsNullOrEmpty(License))
                {
                    var genRS = Get_TokenLicense(CompanyCode, License, "").Result;
                    if (genRS != null && genRS.Success)
                    {
                        var datainList = genRS.Data as List<SLicense>;
                        string queryInitData = "";
                        var SLicense = datainList.FirstOrDefault();
                        if (SLicense != null)
                        {

                            var lincenseDat = _commonService.GetLicenseInfor(CompanyCode, License);
                            if (lincenseDat != null && lincenseDat.Success)
                            {

                                var lincenInfor = lincenseDat.Data as LicenseInfor;
                                if (SLicense.Users.Count < lincenInfor.NumOfDevice)
                                {
                                    foreach (var license in datainList)
                                    {
                                        if (license.Users != null && license.Users.Count > 0)
                                        {
                                            queryInitData += $" delete M_UserLicense where LicenseId = N'{license.LicenseId}' and CompanyCode = N'{CompanyCode}' ;  ";
                                            foreach (var user in license.Users)
                                            {
                                                queryInitData += $" insert into M_UserLicense(UserId, LicenseId, CompanyCode) values(N'{user.UserId}' ,N'{license.LicenseId}', N'{CompanyCode}'); ";
                                            }
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(queryInitData))
                                    {
                                        _userRepository.Execute(queryInitData, null, commandType: CommandType.Text);
                                    }
                                    if (UserList != null && UserList.Count > 0)
                                    {

                                        List<MUserLicense> userLicenses = new List<MUserLicense>();
                                        foreach (string userId in UserList)
                                        {
                                            updateQuer += $" delete M_UserLicense where UserId = N'{userId}' and LicenseId = N'{License}' and CompanyCode = N'{CompanyCode}' ;  ";
                                            updateQuer += $" insert into M_UserLicense(UserId, LicenseId, CompanyCode) values(N'{userId}' ,N'{License}', N'{CompanyCode}'); ";
                                            MUserLicense model = new MUserLicense();
                                            model.UserId = userId;
                                            model.LicenseId = License;
                                            model.CompanyCode = CompanyCode;
                                            userLicenses.Add(model);
                                        }

                                        _userRepository.Execute(updateQuer, null, commandType: CommandType.Text);
                                        //if (data == null)
                                        //{
                                        //    rs.Success = false;
                                        //    rs.Message = "Please check your username/pasword";
                                        //    return rs;
                                        //}
                                        //var connection = _userRepository.GetConnection();
                                        var rsUser = GetUserLicense(CompanyCode, License);
                                        var userLicense = new List<string>();
                                        var jsonString = "";
                                        if (rsUser != null && rsUser.Success)
                                        {
                                            userLicense = rsUser.Data as List<string>;
                                            jsonString = userLicense.ToJson();
                                        }

                                        //string queryGet = $"select UserId from M_UserLicense with (nolock) where CompanyCode=N'{CompanyCode}'";

                                        //var userLicense = _userLicenseRoleRepository.GetAll<string>(queryGet, null, commandType: CommandType.Text);
                                        //var jsonString = userLicense.ToJson();// .ToString();

                                        string jsonStringHash = Encryptor.EncryptString(jsonString, AppConstants.TEXT_PHRASE);
                                        string UpdateJson = "";
                                        int maxVl = 4000;
                                        //string descrypt = Encryptor.DecryptString(jsonStringHash, AppConstants.TEXT_PHRASE);
                                        if (jsonStringHash.Length > maxVl)
                                        {
                                            string jsonClone = jsonStringHash.Substring(0, jsonStringHash.Length);
                                            int stt = 1;
                                            //string updateCusF = "";
                                            int startNum = 0;
                                            int endNum = 4000;
                                            decimal numberX = jsonStringHash.Length / maxVl;
                                            int songuyen = ((int)numberX);
                                            List<string> aList = new List<string>();
                                            for (int i = 0; i <= songuyen; i++)
                                            {
                                                if (endNum > jsonClone.Length)
                                                {
                                                    endNum = jsonClone.Length;
                                                }
                                                var stringSplit = jsonClone.Substring(0, endNum);
                                                aList.Add(stringSplit);
                                                jsonClone = jsonClone.Replace(stringSplit, "");
                                                //jsonClone.Length;
                                                //jsonClone = jsonClone.Substring(endNum + 1, curreLengt); 
                                            }
                                            string testMapping = "";
                                            foreach (string str in aList)
                                            {
                                                string name = "CustomF" + stt;

                                                UpdateJson += $" update S_License  set {name}=N'{str}' where LicenseId = '{License}' and CompanyCode = '{CompanyCode}'; ";
                                                stt++;
                                                testMapping += str;
                                            }
                                            if (stt < 16)
                                            {
                                                for (int i = stt; i <= 15; i++)
                                                {
                                                    string name = "CustomF" + stt;

                                                    UpdateJson += $" update S_License  set {name}=N'' where LicenseId = '{License}' and CompanyCode = '{CompanyCode}'; ";
                                                    //testMapping += str;
                                                }

                                            }
                                            //descrypt = Encryptor.DecryptString(testMapping, AppConstants.TEXT_PHRASE);
                                            //while (jsonClone.Length >0)
                                            //{
                                            //    string name = "CustomF" + stt;
                                            //    if (endNum > jsonString.Length)
                                            //    {
                                            //        endNum = jsonString.Length;
                                            //    }
                                            //    UpdateJson = $" update S_Token   set '{name}' = N'" + jsonClone.Substring(startNum, endNum) + "'; ";
                                            //    jsonClone = jsonClone.Substring(endNum + 1, jsonClone.Length - 1);
                                            //    stt++;
                                            //}


                                            //int startNum = 0;
                                            //int endNum = maxVl;

                                            //for (int i = 1; i <= songuyen; i++)
                                            //{
                                            //    string name = "CustomF" + i;
                                            //    string updateCusF = "";
                                            //    if(endNum > jsonString.Length)
                                            //    {
                                            //        endNum = jsonString.Length;
                                            //    }    
                                            //     updateCusF = $" update S_Token   set '{name}' = N'" + jsonString.Substring(startNum, endNum) + "'; ";
                                            //    startNum = endNum + 1;
                                            //    endNum = endNum + maxVl;

                                            //    UpdateJson += updateCusF;
                                            //}    

                                        }
                                        else
                                        {
                                            string name = "CustomF1";
                                            UpdateJson += $" update S_License  set {name}=N'{jsonStringHash}' where LicenseId = '{License}' and CompanyCode = '{CompanyCode}'; ";
                                            //UpdateJson = jsonString;
                                        }
                                        if (!string.IsNullOrEmpty(UpdateJson))
                                        {
                                            _userRepository.Execute(UpdateJson, null, commandType: CommandType.Text);
                                        }
                                        rs.Success = true;
                                    }
                                    else
                                    {
                                        rs.Success = false;
                                        rs.Message = "Please input user";
                                    }
                                }
                                else
                                {
                                    rs.Success = false;
                                    rs.Message = "Can't set license to user. B/c the number of devices/users has been exceeded";
                                }

                            }
                        }   
                        else
                        {
                            rs.Success = false;
                            rs.Message = "Can't get license information";
                        }    
                      
                    }
                    else
                    {
                        rs.Success = false;
                        rs.Message = "Can't get license information";

                    }

                }
                else
                {
                    var genRS = Get_TokenLicense(CompanyCode, "", "").Result;
                    if (genRS != null && genRS.Success)
                    {
                        var datainList = genRS.Data as List<SLicense>;
                        string queryInitData = "";
                        foreach (var license in datainList)
                        {
                            if (license.Users != null && license.Users.Count > 0)
                            {
                                queryInitData += $" delete M_UserLicense where LicenseId = N'{license.LicenseId}' and CompanyCode = N'{CompanyCode}' ;  ";
                                foreach (var user in license.Users)
                                {
                                    queryInitData += $" insert into M_UserLicense(UserId, LicenseId, CompanyCode) values(N'{user.UserId}' ,N'{license.LicenseId}', N'{CompanyCode}'); ";
                                }

                            }


                        }
                        if (!string.IsNullOrEmpty(queryInitData))
                        {
                            _userRepository.Execute(queryInitData, null, commandType: CommandType.Text);
                        }
                        if (UserList != null && UserList.Count > 0)
                        {

                            List<MUserLicense> userLicenses = new List<MUserLicense>();
                            foreach (string userId in UserList)
                            {
                                updateQuer += $" delete M_UserLicense where UserId = N'{userId}' and LicenseId = N'{License}' and CompanyCode = N'{CompanyCode}' ;  ";
                                updateQuer += $" insert into M_UserLicense(UserId, LicenseId, CompanyCode) values(N'{userId}' ,N'{License}', N'{CompanyCode}'); ";
                                MUserLicense model = new MUserLicense();
                                model.UserId = userId;
                                model.LicenseId = License;
                                model.CompanyCode = CompanyCode;
                                userLicenses.Add(model);
                            }

                            _userRepository.Execute(updateQuer, null, commandType: CommandType.Text);
                            //if (data == null)
                            //{
                            //    rs.Success = false;
                            //    rs.Message = "Please check your username/pasword";
                            //    return rs;
                            //}
                            //var connection = _userRepository.GetConnection();
                            var rsUser = GetUserLicense(CompanyCode, License);
                            var userLicense = new List<string>();
                            var jsonString = "";
                            if (rsUser != null && rsUser.Success)
                            {
                                userLicense = rsUser.Data as List<string>;
                                jsonString = userLicense.ToJson();
                            }

                            //string queryGet = $"select UserId from M_UserLicense with (nolock) where CompanyCode=N'{CompanyCode}'";

                            //var userLicense = _userLicenseRoleRepository.GetAll<string>(queryGet, null, commandType: CommandType.Text);
                            //var jsonString = userLicense.ToJson();// .ToString();

                            string jsonStringHash = Encryptor.EncryptString(jsonString, AppConstants.TEXT_PHRASE);
                            string UpdateJson = "";
                            int maxVl = 4000;
                            string descrypt = Encryptor.DecryptString(jsonStringHash, AppConstants.TEXT_PHRASE);
                            if (jsonStringHash.Length > maxVl)
                            {
                                string jsonClone = jsonStringHash.Substring(0, jsonStringHash.Length);
                                int stt = 1;
                                //string updateCusF = "";
                                int startNum = 0;
                                int endNum = 4000;
                                decimal numberX = jsonStringHash.Length / maxVl;
                                int songuyen = ((int)numberX);
                                List<string> aList = new List<string>();
                                for (int i = 0; i <= songuyen; i++)
                                {
                                    if (endNum > jsonClone.Length)
                                    {
                                        endNum = jsonClone.Length;
                                    }
                                    var stringSplit = jsonClone.Substring(0, endNum);
                                    aList.Add(stringSplit);
                                    jsonClone = jsonClone.Replace(stringSplit, "");
                                    //jsonClone.Length;
                                    //jsonClone = jsonClone.Substring(endNum + 1, curreLengt); 
                                }
                                string testMapping = "";
                                foreach (string str in aList)
                                {
                                    string name = "CustomF" + stt;

                                    UpdateJson += $" update S_License  set {name}=N'{str}' where LicenseId = '{License}' and CompanyCode = '{CompanyCode}'; ";
                                    stt++;
                                    testMapping += str;
                                }
                                if (stt < 16)
                                {
                                    for (int i = stt; i <= 15; i++)
                                    {
                                        string name = "CustomF" + stt;

                                        UpdateJson += $" update S_License  set {name}=N'' where LicenseId = '{License}' and CompanyCode = '{CompanyCode}'; ";
                                        //testMapping += str;
                                    }

                                }
                                //descrypt = Encryptor.DecryptString(testMapping, AppConstants.TEXT_PHRASE);
                                //while (jsonClone.Length >0)
                                //{
                                //    string name = "CustomF" + stt;
                                //    if (endNum > jsonString.Length)
                                //    {
                                //        endNum = jsonString.Length;
                                //    }
                                //    UpdateJson = $" update S_Token   set '{name}' = N'" + jsonClone.Substring(startNum, endNum) + "'; ";
                                //    jsonClone = jsonClone.Substring(endNum + 1, jsonClone.Length - 1);
                                //    stt++;
                                //}


                                //int startNum = 0;
                                //int endNum = maxVl;

                                //for (int i = 1; i <= songuyen; i++)
                                //{
                                //    string name = "CustomF" + i;
                                //    string updateCusF = "";
                                //    if(endNum > jsonString.Length)
                                //    {
                                //        endNum = jsonString.Length;
                                //    }    
                                //     updateCusF = $" update S_Token   set '{name}' = N'" + jsonString.Substring(startNum, endNum) + "'; ";
                                //    startNum = endNum + 1;
                                //    endNum = endNum + maxVl;

                                //    UpdateJson += updateCusF;
                                //}    

                            }
                            else
                            {
                                UpdateJson = jsonString;
                            }
                            if (!string.IsNullOrEmpty(UpdateJson))
                            {
                                _userRepository.Execute(UpdateJson, null, commandType: CommandType.Text);
                            }
                            rs.Success = true;
                        }
                        else
                        {
                            rs.Success = false;
                            rs.Message = "Please input user";
                        }
                    }
                    else
                    {
                        rs.Success = false;
                        rs.Message = "Can't get license information";

                    }

                }



                //rs.Data = data;
                //rs.Message = connection.Database;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }

        public GenericResult RemoveLicenseForUser(string CompanyCode, string License, List<string> UserList)
        {

            GenericResult rs = new GenericResult();
            try
            {
                if (UserList != null && UserList.Count > 0)
                {

                }
                else
                {
                    var userData = GetAllUsers(CompanyCode, false).Result;
                    if (userData != null)
                    {
                        List<MUser> users = userData.Data as List<MUser>;
                        if (users != null && users.Count > 0)
                        {
                            UserList = new List<string>();
                            foreach (MUser user in users)
                            {
                                UserList.Add(user.UserId.ToString());
                            }
                        }
                    }
                }
                string updateQuer = "";



                var genRS = Get_TokenLicense(CompanyCode, "", "").Result;
                if (genRS != null && genRS.Success)
                {
                    var datainList = genRS.Data as List<SLicense>;
                    string queryInitData = "";
                    foreach (var license in datainList)
                    {
                        if (license.Users != null && license.Users.Count > 0)
                        {
                            queryInitData += $" delete M_UserLicense where LicenseId = N'{license.LicenseId}' and CompanyCode = N'{CompanyCode}' ;  ";
                            
                            foreach (var user in license.Users)
                            {
                               
                                queryInitData += $" insert into M_UserLicense(UserId, LicenseId, CompanyCode) values(N'{user.UserId}' ,N'{license.LicenseId}', N'{CompanyCode}'); ";
                            }

                        }


                    }
                    if (!string.IsNullOrEmpty(queryInitData))
                    {
                        _userRepository.Execute(queryInitData, null, commandType: CommandType.Text);
                    }
                    if (UserList != null && UserList.Count > 0)
                    {

                        List<MUserLicense> userLicenses = new List<MUserLicense>();
                        foreach (string userId in UserList)
                        {
                            updateQuer += $" delete M_UserLicense where UserId = N'{userId}' and LicenseId = N'{License}' and CompanyCode = N'{CompanyCode}' ;  ";
                            //updateQuer += $" insert into M_UserLicense(UserId, LicenseId, CompanyCode) values(N'{userId}' ,N'{License}', N'{CompanyCode}'); ";
                            MUserLicense model = new MUserLicense();
                            model.UserId = userId;
                            model.LicenseId = License;
                            model.CompanyCode = CompanyCode;
                            userLicenses.Add(model);
                        }

                        _userRepository.Execute(updateQuer, null, commandType: CommandType.Text); 
                        var rsUser = GetUserLicense(CompanyCode, License);
                        var userLicense = new List<string>();
                        var jsonString = "";
                        if (rsUser != null && rsUser.Success)
                        {
                            userLicense = rsUser.Data as List<string>;
                            jsonString = userLicense.ToJson();
                        } 

                        string jsonStringHash = Encryptor.EncryptString(jsonString, AppConstants.TEXT_PHRASE);
                        string UpdateJson = "";
                        int maxVl = 4000;
                        string descrypt = Encryptor.DecryptString(jsonStringHash, AppConstants.TEXT_PHRASE);
                        if (jsonStringHash.Length > maxVl)
                        {
                            string jsonClone = jsonStringHash.Substring(0, jsonStringHash.Length);
                            int stt = 1;
                            //string updateCusF = "";
                            int startNum = 0;
                            int endNum = 4000;
                            decimal numberX = jsonStringHash.Length / maxVl;
                            int songuyen = ((int)numberX);
                            List<string> aList = new List<string>();
                            for (int i = 0; i <= songuyen; i++)
                            {
                                if (endNum > jsonClone.Length)
                                {
                                    endNum = jsonClone.Length;
                                }
                                var stringSplit = jsonClone.Substring(0, endNum);
                                aList.Add(stringSplit);
                                jsonClone = jsonClone.Replace(stringSplit, "");
                                //jsonClone.Length;
                                //jsonClone = jsonClone.Substring(endNum + 1, curreLengt); 
                            }
                            string testMapping = "";
                            foreach (string str in aList)
                            {
                                string name = "CustomF" + stt;

                                UpdateJson += $" update S_License  set {name}=N'{str}' where LicenseId = '{License}' and CompanyCode = '{CompanyCode}'; ";
                                stt++;
                                testMapping += str;
                            }
                            if (stt < 16)
                            {
                                for (int i = stt; i <= 15; i++)
                                {
                                    string name = "CustomF" + stt;

                                    UpdateJson += $" update S_License  set {name}=N'' where LicenseId = '{License}' and CompanyCode = '{CompanyCode}'; ";
                                    //testMapping += str;
                                }

                            }
                       

                        }
                        else
                        {
                            //UpdateJson = jsonString;
                            string name = "CustomF1";

                            UpdateJson += $" update S_License  set {name}=N'{jsonStringHash}' where LicenseId = '{License}' and CompanyCode = '{CompanyCode}'; ";
                        }
                        if (!string.IsNullOrEmpty(UpdateJson))
                        {
                            _userRepository.Execute(UpdateJson, null, commandType: CommandType.Text);
                        }
                        rs.Success = true;
                    }
                    else
                    {
                        rs.Success = false;
                        rs.Message = "Please input user";
                    }
                }
                else
                {
                    rs.Success = false;
                    rs.Message = "Can't get license information";

                }


                //rs.Data = data;
                //rs.Message = connection.Database;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }

        public async Task<GenericResult> Login(string userName, string pass, string customCode)
        {
            GenericResult rs = new GenericResult();
            try
            { 
                string passEn = "";
                string customCodeEn = "";
                //if (!string.IsNullOrEmpty(customCode))
                //{
                //    customCode = customCode.Replace(" ", "");
                //    string customCodeEnc = Encryptor.DecryptString(customCode, AppConstants.TEXT_PHRASE);
                //    string[] strSplit = customCodeEnc.Split(" ");
                //    if (strSplit != null && strSplit.Length >= 2)
                //    {
                //        userName = strSplit[0];
                //        passEn = strSplit[1];
                //        passEn = Encryptor.EncryptString(passEn, AppConstants.TEXT_PHRASE);
                //    }
                //    else
                //    {
                //        rs.Success = false;
                //        rs.Message = "Barcode/QRCode not valid";
                //        return rs;
                //    }

                //}
                //else
                //{
                //    
                //}    
                if(string.IsNullOrEmpty(pass))
                {
                    pass = "";
                }
                if (string.IsNullOrEmpty(userName))
                {
                    userName = "";
                }

                passEn = Encryptor.EncryptString(pass, AppConstants.TEXT_PHRASE);
                if(!string.IsNullOrEmpty(customCode))
                {
                    customCodeEn = Encryptor.EncryptString(customCode, AppConstants.TEXT_PHRASE);
                    passEn = "";
                    userName = "";
                }    
                var parameters = new DynamicParameters();

                parameters.Add("@UserName", userName);
                parameters.Add("@Password", passEn);

                if (!string.IsNullOrEmpty(customCodeEn))
                {
                    parameters.Add("@QRBarcode", customCodeEn);
                } 
                var data = await _userRepository.GetAsync($"USP_S_UserByUserName", parameters, commandType: CommandType.StoredProcedure);
                if(data==null)
                {
                    rs.Success = false;
                    rs.Message = "Please check your username/pasword";
                    return rs;
                }
                var connection = _userRepository.GetConnection();
                rs.Success = true;
                rs.Data = data;
                rs.Message = connection.Database;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }
        public async Task<GenericResult> UpdateLastStore(MUser model)
        {
            GenericResult rs = new GenericResult();
            try
            {
                string LastLoginStoreIdUpdat = $"update M_User set LastLoginStoreId =N'{model.LastLoginStoreId}'  where UserId =N'{model.UserId}' and CompanyCode =N'{model.CompanyCode}' and Username =N'{model.Username}'";
                _userRepository.Execute(LastLoginStoreIdUpdat, null, commandType: CommandType.Text);
                rs.Success = true;
                rs.Data = model;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;

        }
        public string ConvertUnicodeStringToAscii(string text)
        {
            var sb = new StringBuilder();
            foreach (char c in text)
            {
                int unicode = c;
                if (unicode < 128)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        public async Task<GenericResult> Update(MUser model)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("UserId", model.UserId);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Username", model.Username);
                parameters.Add("LastName", model.LastName);
                parameters.Add("FirstName", model.FirstName);
                parameters.Add("Position", model.Position);
                string password = Encryptor.EncryptString(model.Password, AppConstants.TEXT_PHRASE);
                parameters.Add("Password", password);
                parameters.Add("LastLoginStoreId", model.LastLoginStoreId);
                parameters.Add("LastLoginStoreLang", model.LastLoginStoreLang);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("DefEmployee", model.DefEmployee);
                parameters.Add("Status", model.Status);
                //if (!string.IsNullOrEmpty(model.QRBarcode))
                //{

                //}
                string code = model.UserId.ToString().Replace("-","").Substring(0, 10) + ConvertUnicodeStringToAscii( model.Password.Substring(0, 2));// model.Username + model.Password;
                //model.QRBarcode = Encryptor.EncryptString(model.QRBarcode, AppConstants.TEXT_PHRASE);
                model.QRBarcode = Encryptor.EncryptString(code, AppConstants.TEXT_PHRASE);
                parameters.Add("QRBarcode", model.QRBarcode);
                var affectedRows = _userRepository.Insert("USP_U_M_User", parameters, commandType: CommandType.StoredProcedure);

                if (!string.IsNullOrEmpty(model.RoleId))
                {
                    parameters = new DynamicParameters();
                    parameters.Add("UserId", model.UserId);
                    parameters.Add("RoleId", model.RoleId);
                    _userRoleRepository.Insert("USP_U_M_UserRole", parameters, commandType: CommandType.StoredProcedure);
                }
                //result.Success = true;
                //result.Message = key;
                //var Id = await _userRepository.GetConnection().InsertAsync<Guid, MUser>(model);
                //model.UserId = Id;
                rs.Success = true;
                rs.Data = model;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;

        }
        public async Task<GenericResult> LoginMwi(string userName, string pass)
        {
            GenericResult rs = new GenericResult();
            try
            {
                string passHash = Utilities.Helpers.Encryptor.EncryptString(pass, Utilities.Constants.AppConstants.TEXT_PHRASE);
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("CompanyCode", this.CompanyCode);
                parameters.Add("Username", userName);
                parameters.Add("Password", passHash);
                parameters.Add("UserType", "API");
                var data = await _userRepository.GetAsync("USP_S_M_UserLogin", parameters, CommandType.StoredProcedure, Data.Infrastructure.GConnection.MwiConnection);
                rs.Success = true;
                rs.Data = data;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }
    }
}
