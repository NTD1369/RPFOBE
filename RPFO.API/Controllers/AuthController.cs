using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RPFO.API.Errors;
using RPFO.Application.Implements;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Constants;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Extensions;
using RPFO.Utilities.Helpers;

namespace RPFO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IUserService _userService;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;
        private readonly IFormatConfigService _fomartconfig;
        private readonly ICompanyService _companyService;
        private readonly IStoreService _storeService;
        private readonly IGeneralSettingService _generalSettingService;
        private readonly ICustomerService _customerService;
        private readonly IPermissionService _permissionService;
        private string LogPath;
        string SVKey = "";
        public AuthController(ILogger<AuthController> logger, IConfiguration config, IUserService userService, IFormatConfigService fomartconfig, IStoreService storeService, ICompanyService companyService, IPermissionService permissionService,
            IGeneralSettingService generalSettingService, ICustomerService customerService)
        {
            _logger = logger;
            _config = config;
            _userService = userService;
            SVKey = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("SVKey"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            this.LogPath = Encryptor.DecryptString(config.GetConnectionString("ApiLogPath"), AppConstants.TEXT_PHRASE);
            if (string.IsNullOrEmpty(SVKey))
            {
                SVKey = "";
            }
        }
        //private HttpClient GetHttpClient(string UrlClient)
        //{
        //    var client = new HttpClient();

        //    if (!string.IsNullOrEmpty(UrlClient))
        //    {
        //        client.BaseAddress = new Uri(UrlClient);
        //    }


        //    return client;
        //}
         
        public class ResultAtomModel
        {
            public string resultTxnID { get; set; }
            public string Status { get; set; }
            public string requestID { get; set; }
            public string requestType { get; set; }
            public string ipnUrl { get; set; }
            public string amount { get; set; }
            public string tip { get; set; }
            public string resultResponseCode { get; set; }
          
        }

        //Post thông tin đi với số tiền, mã merchan (mã partner), đường dẫn trả kết quả 
        //Sau post xong thì kết quả được trả về theo link phía trên gửi đi.
        // POSTPaymentAtom => wait infor | Máy pos gửi thông tin lại vào GetNotifi
        #region POST ASTEM
       
        [AllowAnonymous]
        [HttpGet]
        [Route("POSTPaymentAtom")]
        public async Task<GenericResult> POSTPaymentAtom(string VerifiedCode = "219936")
        {
            //192.168.68.158:8080 /? verify_code = 531941
            //string ip = "192.168.68.158";
            //IPResponse ipResponse = await client.IPApi.GetDetailsAsync(ip);
            GenericResult result = new GenericResult();
            try
            {
                var client = new HttpClient();
                string url = "http://192.168.4.57:8080/";
                if (!string.IsNullOrEmpty(url))
                {
                    client.BaseAddress = new Uri(url);
                }
                string urlPost = "?verify_code=" + VerifiedCode;

                var model = new
                {
                    direct = 1,
                    requestID = Guid.NewGuid().ToString(), // unique...SQL,// "00017-058-0001",
                    orderId = "1234:5263763487", //mã bill bán hàng 
                    requestType = "SALE",
                    ipnUrl = "http://192.168.4.55:9090/api/auth/GetNotifi", //URL để trả về kết quả // "https://kafka.thebeanfamily.org/posatom/notification",
                    amount = 10000,
                    tip = 0,
                    //"extraData": [
                    //{ "key1": "value1"},
                    //{ "key2": "value2"}
                    //]
                };
                var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(urlPost, content);

                var responseString = await response.Content.ReadAsStringAsync();
                //result = JsonConvert.DeserializeObject<string>(responseString);


                //var parameters = new DynamicParameters();
                //var data = await _paymentRepository.GetAsync($"", parameters);
                result.Success = true;
                //result.Data = data;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("GetNotifi")]
        // Nhận kết quá
        public async Task<GenericResult> GetNotifi(ResultAtomModel resultAtomModel)
        {
          
            GenericResult result = new GenericResult();
            try
            {
                //var body = new StreamReader(Request.Body);
                ////The modelbinder has already read the stream and need to reset the stream index
                //body.BaseStream.Seek(0, SeekOrigin.Begin);
                //var requestBody = body.ReadToEnd();
               
                //RPFO.Utilities.Helpers.LogUtils.WriteLogData("C:\\RPFO.API.MWIEpayModel\\", "SalesOrders", "MWIEpayModel", resultAtomModel.ToJson());
                RPFO.Utilities.Helpers.LogUtils.WriteLogData("C:\\RPFO.API.MWIEpayModel\\", "SalesOrders", "MWIEpayModel", resultAtomModel.ToJson());

                //RPFO.Utilities.Helpers.LogUtils.WriteLogData("C:\\RPFO.API.MWIEpayModel\\", "GetNotifi", "GetNotifi  Y", "Test");

                //model..
                //insert db
                result.Success = true;
                //result.Data = data;
            }
            catch (Exception ex)
            {
                
                result.Success = false;
                result.Message = ex.Message;
            }
            RPFO.Utilities.Helpers.LogUtils.WriteLogData("C:\\RPFO.API.MWIEpayModel\\", "GetNotifi", "GetNotifi X", result.ToJson());
            return result;
        }
        #endregion

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> GetByUsername(LoginViewModel model)
        {
            string DB = "";

            if (!string.IsNullOrEmpty(model.CustomCode))
            {
                model.CustomCode = model.CustomCode.Replace(" ", "");
            }

            var userFromRepo = await _userService.Login(model.UserName.Replace(" ", ""), model.Password.Replace(" ", ""), model.CustomCode);
            if (userFromRepo.Success == false || userFromRepo.Data == null)
                return Unauthorized();
            var Model = userFromRepo.Data as MUser;
            DB = userFromRepo.Message;
            var licenseRs = Get_TokenLicense(Model.CompanyCode, "", Model.Username).Result;
            //tokenRs.Success = false;
            //tokenRs.Message = "sdsds";
            if (licenseRs != null && licenseRs.Success == false)
            {

                return Ok(licenseRs);
            }
            else
            {
                if (licenseRs != null && licenseRs.Success)
                {
                    var licenseData = licenseRs.Data as SLicense;
                    if (licenseData != null)
                    {
                        Model.ValidFrom = licenseData.LicenseInfor.ValidFrom;
                        Model.ValidTo = licenseData.LicenseInfor.ValidTo;
                        Model.License = licenseData.LicenseInfor.LicenseNo;
                        Model.NotifyShowOn =  licenseData.NotifyShowOn.HasValue  ? licenseData.NotifyShowOn : 7;
                        if(licenseData.ExpiredNumber!=null && licenseData.ExpiredNumber.HasValue)
                        {
                            Model.ExpiredNumber = licenseData.ExpiredNumber;
                        }    
                        //if (Model.ValidTo != null && Model.ValidTo.HasValue)
                        //{
                        //    DateTime serverTime = DateTime.Now;
                        //    if (!string.IsNullOrEmpty(SVKey))
                        //    { 
                        //        try
                        //        {
                        //            HttpClient client = GetHttpClient(SVKey);
                        //            ///api/Auth/Login
                        //            string requestUriAuth = "/api/Common/TimeOnServer";
                                   
                        //            var responseAuth = await client.GetAsync(requestUriAuth);
                        //            var responseStringAuth = await responseAuth.Content.ReadAsStringAsync();
                                   
                        //            serverTime = DateTime.Parse(responseStringAuth);
                        //        }
                        //        catch (Exception ex)
                        //        {
                                    
                        //            serverTime = DateTime.Now;
                        //        }
                               
                        //    }
                        //    TimeSpan difference = Model.ValidTo.Value - serverTime;
                        //    int days = (int)difference.TotalDays;
                        //    Model.ExpiredNumber = days;
                        //}

                    }

                }
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, Model.UserId.ToString()),
                    new Claim(ClaimTypes.Name, Model.Username)
                };
                string KeyConf = _config.GetSection("AppSettings:Token").Value;
                var key = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(KeyConf));

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = creds
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.CreateToken(tokenDescriptor);

                //var user = _mapper.Map<UserForListDto>(userFromRepo);

                return Ok(new
                {
                    token = tokenHandler.WriteToken(token),
                    user = userFromRepo,
                    message = DB
                });
            }


        }

        [HttpPost]
        [Route("LoginWConfig")]
        public async Task<IActionResult> LoginWConfig(LoginViewModel model)
        {
            string DB = "";

            if (!string.IsNullOrEmpty(model.CustomCode))
            {
                model.CustomCode = model.CustomCode.Replace(" ", "");
            }

            var userFromRepo = await _userService.Login(model.UserName.Replace(" ", ""), model.Password.Replace(" ", ""), model.CustomCode);
            if (userFromRepo.Success == false || userFromRepo.Data == null)
                return Unauthorized();
            var Model = userFromRepo.Data as MUser;
            DB = userFromRepo.Message;
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Model.UserId.ToString()),
                new Claim(ClaimTypes.Name, Model.Username)
            };
            string KeyConf = _config.GetSection("AppSettings:Token").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(KeyConf));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var company = new MCompany();
            if (!string.IsNullOrEmpty(Model.CompanyCode))
            {
                var companyRS = _companyService.GetByCode(Model.CompanyCode).Result;
                if (companyRS != null && companyRS.Success == true)
                {
                    company = companyRS.Data as MCompany;

                }

            }

            var permissions = new object();
            if (!string.IsNullOrEmpty(Model.Username))
            {
                var permissionRS = _permissionService.GetFunctionPermissionByUser(Model.Username).Result;
                if (permissionRS != null && permissionRS.Success == true)
                {
                    permissions = permissionRS.Data;

                }

            }

            var store = new MStore();
            var format = new SFormatConfig();
            var generalSetting = new List<SGeneralSetting>();
            var customerDefault = new MCustomer();

            if (!string.IsNullOrEmpty(Model.DefStore))
            {
                var storeRS = _storeService.GetByCode(Model.CompanyCode, Model.DefStore).Result;
                if (storeRS != null && storeRS.Success == true)
                {
                    store = storeRS.Data as MStore;

                }
                //var user = _mapper.Map<UserForListDto>(userFromRepo);
                //Load format
              
                if (!string.IsNullOrEmpty(Model.DefStore))
                {
                    var formatRS = _fomartconfig.GetByStore(Model.CompanyCode, Model.DefStore).Result;
                    if (formatRS != null && formatRS.Success == true)
                    {
                        format = formatRS.Data as SFormatConfig;

                    }

                }
              
                if (!string.IsNullOrEmpty(Model.DefStore))
                {

                    var generalSettingRS = _generalSettingService.GetByStore(Model.CompanyCode, Model.DefStore ).Result;
                    if (generalSettingRS != null && generalSettingRS.Success == true)
                    {
                        generalSetting = generalSettingRS.Data as List<SGeneralSetting>;
                    }
                }

                var DefCustomer = "";
                if (store != null && !string.IsNullOrEmpty(store.StoreId) && !string.IsNullOrEmpty(store.DefaultCusId))
                {
                    DefCustomer = store.DefaultCusId;
                }
                if (!string.IsNullOrEmpty(Model.DefCustomer))
                {
                    DefCustomer = Model.DefCustomer;
                }
                if (!string.IsNullOrEmpty(DefCustomer))
                {

                    var customerRS = _customerService.GetByCode(Model.CompanyCode, DefCustomer).Result;
                    if (customerRS != null && customerRS.Success == true)
                    {
                        customerDefault = customerRS.Data as MCustomer;

                    }

                }

            }



            //var generalSettingService = _generalSettingService.GetByStore(Model.CompanyCode, Model.DefStore).Result;
           
            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user = userFromRepo,
                company = company,
                store = store,
                permissions = permissions,
                storeFormat = format,
                generalSetting = generalSetting,
                customerDF = customerDefault,
                message = DB
            });

        }

        public class LicenseModel 
        {
            public string CompanyCode { get; set; }
            public string License { get; set; }
            public List<string> Users { get; set; }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Route("SetLicenseForUser")]
        public  GenericResult SetLicenseForUser(LicenseModel license)
        { 

            return _userService.SetLicenseForUser(license.CompanyCode, license.License, license.Users);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Route("RemoveLicenseForUser")]
        public GenericResult RemoveLicenseForUser(LicenseModel license)
        {

            return _userService.RemoveLicenseForUser(license.CompanyCode, license.License, license.Users);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        [Route("Get_TokenLicense")]
        public async Task<GenericResult> Get_TokenLicense(string CompanyCode, string License, string User)
        {
            return await _userService.Get_TokenLicense(CompanyCode, License, User);
        }
      
        [HttpGet]
        [Route("notfound")]
        public IActionResult GetNotFoundRequest()
        {
            var thing = "";
            if(string.IsNullOrEmpty(thing))
            {
                return NotFound(new ApiResponse(404));
            }
            return Ok();
        }
        [HttpGet]
        [Route("badrequest")]
        public async Task<IActionResult> GetBadRequest()
        {
            
            var thing = DateTime.Now.Ticks.ToString() + "";

            if (string.IsNullOrEmpty(thing))
            {
                //var a = thing.Length.ToString();
                return NotFound(new ApiResponse(400));
            }
            return Ok();
        }
    }
}
