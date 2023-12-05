using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RPFO.API.Helpers;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel.RPT;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using RPFO.Utilities.Constants;
using Spire.Pdf;
using System.Drawing;
using Spire.Pdf.HtmlConverter;
using System.Text;
using RPFO.Data.ViewModels;
using Newtonsoft.Json;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using RPFO.Utilities.Extensions;
using Microsoft.AspNetCore.Http;
using System.Xml;

namespace RPFO.API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CommonController : ControllerBase
    {

        ICommonService _commonService;
        ILicenseService _licenseService;
        private readonly ILogger<CommonController> _logger;
        IHostingEnvironment _hostingEnvironment;
        private readonly IGeneralSettingService _settingService;
        IStoreClientService _storeClientService;
        static bool _continue;
        static SerialPort _serialPort;
        IConfiguration _config;
        string MediaFolder = "";
        string MediaLink = "";
        public CommonController(ILogger<CommonController> logger, ICommonService commonService, IConfiguration config, IGeneralSettingService settingService,
            IHostingEnvironment hostingEnvironment, ILicenseService licenseService, IStoreClientService storeClientService)
        {
            _logger = logger;
            _commonService = commonService;
            _storeClientService = storeClientService;
            _settingService = settingService;
            _licenseService = licenseService;
            _config = config;
            _hostingEnvironment = hostingEnvironment;
            MediaFolder = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("MediaFolder"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            if (string.IsNullOrEmpty(MediaFolder))
            {
                MediaFolder = "";
            }
            MediaLink = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("MediaLink"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            if (string.IsNullOrEmpty(MediaFolder))
            {
                MediaLink = "";
            }
        }


        [HttpGet]
        [Route("TimeOnServer")]
        public string TimeOnServer()
        {
            string format = @"yyyy/MM/dd HH:mm:ss zzz";

            try
            {
                //DateTimeOffset clientTime = DateTimeOffset.ParseExact(clientString, format,
                //                            CultureInfo.InvariantCulture);
                DateTimeOffset serverTime = TimeZoneInfo.ConvertTime(DateTime.Now,
                                            TimeZoneInfo.Local);
                return serverTime.ToString(format);
                //SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrefixTest", "ABCD", clientString);


                //return DateTimeOffset.MinValue;
            }
            catch (FormatException)
            {
                return "";
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Route("GenLicense")]
        public IActionResult GenLicense(LicenseInfor infor, string PassPharse)
        {
            //string format = @"M/d/yyyy H:m:s zzz";
            GenericResult rs = new GenericResult();
            try
            {
                if(PassPharse!= RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE)
                {
                    rs.Success = false;
                    rs.Message = "PassPharse wrong";
                }   
                else
                {
                    var jsonStr = infor.ToJson(); 

                    var Token = Utilities.Helpers.Encryptor.EncryptString(jsonStr, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                    rs.Success = true;
                    string returnXML = "<license><key>"+ infor.LicenseNo + "</key><token>" + Token + "</token></license>";

                    var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");


                    string folder = Path.Combine(rootPath, "XLG/");

                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    string path = folder;
                    string fileName = infor.LicenseNo + DateTime.Now.ToString("yyyyMMddsss");
                    using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, fileName + ".xml")))
                    {
                        outputFile.WriteLine(returnXML);

                    }

                    rs.Message = "XLG/" + fileName;

                }    

                
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return Ok(rs);
        }
        //[HttpPost, DisableRequestSizeLimit]
        //[Route("UploadLicenseX")]
        //public FileResult GetXmlFile(LicenseInfor infor, string PassPharse)
        //{
        //    var jsonStr = infor.ToJson();

        //    var Token = Utilities.Helpers.Encryptor.EncryptString(jsonStr, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
        //    string returnXML = "<license><key>" + infor.LicenseNo + "</key><token>" + Token + "</token></license>";

        //    XmlDocument docConfig = new XmlDocument();
        //    XmlNode xmlNode = docConfig.CreateNode(XmlNodeType.XmlDeclaration, "", "");
        //    XmlElement rootElement = docConfig.CreateElement("license");
        //    docConfig.AppendChild(rootElement);

        //    XmlElement environmentElement = docConfig.CreateElement("key");
        //    XmlText environText = docConfig.CreateTextNode(infor.LicenseNo);
        //    environmentElement.AppendChild(environText);

        //    XmlElement tokenElement = docConfig.CreateElement("token");
        //    XmlText tokenText = docConfig.CreateTextNode(Token);
        //    tokenElement.AppendChild(tokenText);


        //    docConfig.PrependChild(environmentElement);
        //    docConfig.PrependChild(tokenElement);


        //    //var stream = new MemoryStream();
        //    //var writer = XmlWriter.Create(stream);
        //    //writer.WriteRaw(docConfig);
        //    //stream.Position = 0;
        //    string fileName = infor.LicenseNo + DateTime.Now.ToString("yyyyMMddsss");
        //    //var fileStreamResult = File(stream, "application/octet-stream", fileName + ".xml");
        //    return fileStreamResult;
        //}

        [HttpPost, DisableRequestSizeLimit]
        [Route("UploadLicense")]
        public async Task<GenericResult>  Upload(IFormFile formFile, string CreatedBy)
        {
            GenericResult result = new GenericResult();
            try
            {
                if (formFile.Length > 0)
                {
                    //var filePath = Path.GetTempFileName();
                    var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");


                    string folder = Path.Combine(rootPath, "license/");

                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    System.IO.DirectoryInfo di = new DirectoryInfo(folder);
                    var fileName = formFile.FileName;
                    foreach (FileInfo file in di.GetFiles())
                    {
                        try
                        {
                            if(file.Name == fileName)
                            {
                                file.Delete();
                            }    
                           
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                    
                    string filePath = folder +fileName;
                   
                    using (var stream = System.IO.File.Create(filePath))
                    {
                         formFile.CopyTo(stream);
                    }


                    XmlDataDocument xmldoc = new XmlDataDocument();
                    XmlNodeList xmlnode;
                    XmlNodeList xmlnodeToken;
                    string str = null;
                    FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    xmldoc.Load(fs);
                    xmlnode = xmldoc.GetElementsByTagName("license");
                    //xmlnodeToken = xmldoc.GetElementsByTagName("token");
                    string key = "";
                    string token = "";
                    foreach (XmlNode node in xmlnode)
                    {
                        key = node.SelectSingleNode("key").InnerText.Replace("\r", "").Replace("\n", "").Replace("\t", "");
                        token = node.SelectSingleNode("token").InnerText.Replace("\r", "").Replace("\n", "").Replace("\t", "");
                        //price = node.SelectSingleNode("Product_price").InnerText;
                        //MessageBox.Show(proID + " " + proName + " " + price);
                    }
                    if (!string.IsNullOrEmpty(token))
                    {
                        LicenseInfor licenseInfor = new LicenseInfor();
                        var Token = Utilities.Helpers.Encryptor.DecryptString(token, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                        licenseInfor = JsonConvert.DeserializeObject<LicenseInfor>(Token);
                        if (licenseInfor != null)
                        {
                            if (licenseInfor.LicenseNo == key)
                            {
                                SLicense license = new SLicense();
                                license.CompanyCode = licenseInfor.CompanyCode;
                                license.LicenseId = key;
                                license.LicenseCode = key;
                                license.LicenseType = licenseInfor.LicenseType;
                                license.LicenseDesc = licenseInfor.Description;
                                license.ValidFrom = licenseInfor.ValidFrom;
                                license.ValidTo = licenseInfor.ValidTo;
                                license.Status = "A";
                                license.Token = token;
                                license.LicenseCusF1 = licenseInfor.CustomF1;
                                license.LicenseCusF2 = licenseInfor.CustomF2;
                                license.LicenseCusF3 = licenseInfor.CustomF3;
                                license.LicenseCusF4 = licenseInfor.CustomF4;
                                license.LicenseCusF5 = licenseInfor.CustomF5;
                                license.NotifyShowOn = licenseInfor.NotifyShowOn.HasValue ? licenseInfor.NotifyShowOn : 7;
                                license.CreatedBy = CreatedBy;

                                result = _licenseService.Create(license).Result;
                            }
                            else
                            {
                                result.Success = false;
                                result.Message = "Key of license invalid";
                            }

                        }
                    }
                   

                }
              
             
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        //[HttpPost]
        //[Route("GenLicense")]
        //public GenericResult ImportLicense(LicenseInfor infor, string PassPharse)
        //{
        //    //string format = @"M/d/yyyy H:m:s zzz";
        //    GenericResult rs = new GenericResult();
        //    try
        //    {
        //        if (PassPharse != RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE)
        //        {
        //            rs.Success = false;
        //            rs.Message = "PassPharse wrong";
        //        }
        //        else
        //        {
        //            var jsonStr = infor.ToJson();

        //            var Token = Utilities.Helpers.Encryptor.EncryptString(jsonStr, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
        //            rs.Success = true;
        //            rs.Message = Token;
        //        } 
        //    }
        //    catch (Exception ex)
        //    {
        //        rs.Success = false;
        //        rs.Message = ex.Message;
        //    }
        //    return rs;
        //}


        [HttpPut]
        [Route("SetAppSetting")]
        public GenericResult SetAppSetting(string clientString, AppSettingModel model)
        {
            GenericResult rs= new GenericResult();
            try
            {
                //DateTimeOffset clientTime = DateTimeOffset.ParseExact(clientString, format,
                //                            CultureInfo.InvariantCulture);
                //DateTimeOffset serverTime = TimeZoneInfo.ConvertTime(clientTime,
                //                            TimeZoneInfo.Local);
                //return serverTime;
                if(!string.IsNullOrEmpty(model.Redis))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:Redis", model.Redis, clientString);
                }
                if (!string.IsNullOrEmpty(model.DefaultConnection))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:DefaultConnection", model.DefaultConnection, clientString);
                }
                if (!string.IsNullOrEmpty(model.MWIService))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:MWIService", model.MWIService, clientString);
                }
                if (!string.IsNullOrEmpty(model.ShiftConnection))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:ShiftConnection", model.ShiftConnection, clientString);
                }
                if (!string.IsNullOrEmpty(model.EndDateConnection))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:EndDateConnection", model.EndDateConnection, clientString);
                }
                if (!string.IsNullOrEmpty(model.ReportConnection))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:ReportConnection", model.ReportConnection, clientString);
                }
                if (!string.IsNullOrEmpty(model.PrefixSO))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrefixSO", model.PrefixSO, clientString);
                }
                if (!string.IsNullOrEmpty(model.PrefixAR))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrefixAR", model.PrefixAR, clientString);
                }
                if (!string.IsNullOrEmpty(model.PrefixPR))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrefixPR", model.PrefixPR, clientString);
                }
                if (!string.IsNullOrEmpty(model.PrefixPO))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrefixPO", model.PrefixPO, clientString);
                }
                if (!string.IsNullOrEmpty(model.PrefixGRPO))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrefixGRPO", model.PrefixGRPO, clientString);
                }
                if (!string.IsNullOrEmpty(model.PrefixTS))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrefixTS", model.PrefixTS, clientString);
                }
                if (!string.IsNullOrEmpty(model.PrefixTR))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrefixTR", model.PrefixTR, clientString);
                }
                if (!string.IsNullOrEmpty(model.PrefixGI))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrefixGI", model.PrefixGI, clientString);
                }
                if (!string.IsNullOrEmpty(model.PrefixGR))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrefixGR", model.PrefixGR, clientString);
                }
                if (!string.IsNullOrEmpty(model.PrefixGRT))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrefixGRT", model.PrefixGRT, clientString);
                }
                if (!string.IsNullOrEmpty(model.PrefixTF))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrefixTF", model.PrefixTF, clientString);
                }
                if (!string.IsNullOrEmpty(model.PrefixSP))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrefixSP", model.PrefixSP, clientString);
                }
                if (!string.IsNullOrEmpty(model.PrefixCounting))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrefixCounting", model.PrefixCounting, clientString);
                }
                if (!string.IsNullOrEmpty(model.PrefixPosting))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrefixPosting", model.PrefixPosting, clientString);
                }
                if (!string.IsNullOrEmpty(model.PrefixShift))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrefixShift", model.PrefixShift, clientString);
                }
                if (!string.IsNullOrEmpty(model.ComBankTerminal))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:ComBankTerminal", model.ComBankTerminal, clientString);
                }
                if (!string.IsNullOrEmpty(model.PrintLoopTime))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrintLoopTime", model.PrintLoopTime, clientString);
                }
                if (!string.IsNullOrEmpty(model.PrintWaitTime))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrintWaitTime", model.PrintWaitTime, clientString);
                }
                if (!string.IsNullOrEmpty(model.TimeCacheAction))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:TimeCacheAction", model.TimeCacheAction, clientString);
                }
                if (!string.IsNullOrEmpty(model.PrintSize57Scale))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrintSize57Scale", model.PrintSize57Scale, clientString);
                }
                if (!string.IsNullOrEmpty(model.PrintSize80Scale))
                {
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrintSize80Scale", model.PrintSize80Scale, clientString);
                }
                if (!string.IsNullOrEmpty(model.PrintFolder))
                { 
                    SettingsHelpers.AddOrUpdateAppSetting<string>("ConnectionStrings:PrintFolder", model.PrintFolder, clientString);
                }



                rs.Success = true;
                //rs.Message = ex.Message
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            } 
            return rs;
        }
        [HttpGet]
        [Route("GetCountries")]
        public async Task<GenericResult> GetCountries(string Area)
        {
            return await _commonService.GetCountries(Area);
        }
        [HttpPost]
        [Route("InitDb")]
        public GenericResult InitDb(ObjectInitNewData model)
        {
            return _commonService.InitDb(model);
        }
       
        
        [HttpPost]
        [Route("GetQuery")]
        public async Task<GenericResult> GetQuery(QueryModel model)
        {
            return await _commonService.GetQuery(model);
        }

        [HttpGet]
        [Route("GetArea")]
        public async Task<GenericResult> GetArea()
        {
            return await _commonService.GetArea();
        }
        [HttpGet]
        [Route("GetMaxValueCurrency")]
        public async Task<GenericResult> GetMaxValueCurrency(string Currency)
        {
            return await _commonService.GetMaxValueCurrency(Currency);
        }
        [HttpGet]
        [Route("ClearCache")]
        public GenericResult ClearCache(string Key, string Prefix)
        {
            return _commonService.ClearRedisCache(Key, Prefix);
        }
        [HttpGet]
        [Route("GetRegion")]
        public async Task<GenericResult> GetRegion()
        {
            return await _commonService.GetRegion();
        }
        [HttpGet]
        [Route("GetProvinceList")]
        public async Task<GenericResult> GetProvinceList()
        {
            return await _commonService.GetProvinceList();
        }
        [HttpGet]
        [Route("GetCurrencyList")]
        public async Task<GenericResult> GetCurrencyList()
        {
            return await _commonService.GetCurrencyList();
        }
        [HttpGet]
        [Route("GetLicenseInfor")]
        public GenericResult GetLicenseInfor(string CompanyCode, string License)
        {
            return _commonService.GetLicenseInfor(CompanyCode, License);
        }

        [HttpGet]
        [Route("GetConfigType")]
        public async Task<GenericResult> GetConfigType()
        {
            return await _commonService.GetConfigType();
        }

        [HttpGet]
        [Route("GetItemCollection")]
        public async Task<GenericResult> GetItemCollection()
        {
            return await _commonService.GetItemCollection();
        }
        [HttpGet]
        [Route("GetDailyId")]
        public async Task<GenericResult> GetDailyId(string CompanyCode, string StoreId, DateTime? date)
        {
            return await _commonService.GetDailyId(CompanyCode, StoreId, date);
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
        [HttpGet]
        [Route("Download")]
        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
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
        [HttpGet]
        [Route("getDirectoryFiles")]
        public async Task<GenericResult>  getDirectoryFiles(string CompanyCode, string StoreId , string Uri, string path, bool? IsFolder)
        {
            GenericResult result = new GenericResult();
            List<string> fileXs = new List<string>();
            try
            {
                
                if (path == "Media")
                {
                    path = MediaFolder;
                }
                var settingData = await _settingService.GetGeneralSettingByStore(CompanyCode, StoreId);
                var settingMedia = new SGeneralSetting();
                List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                if (settingData.Success)
                {

                    SettingList = settingData.Data as List<GeneralSettingStore>;
                     var getSettingMedia = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "MediaLink").FirstOrDefault();
                    if(getSettingMedia!=null )
                    {
                        settingMedia = getSettingMedia;
                    }    
                }
               
               
                if (settingMedia != null && !string.IsNullOrEmpty(settingMedia.SettingValue) && IsFolder != true)
                {
                    //settingMedia.SettingValue = "http://192.168.8.230:8778/GEPOSTERLIST";
                    HttpClient clientGet = new HttpClient();

                    //clientGet.BaseAddress = new Uri(settingMedia.SettingValue);

                    string requestUri = settingMedia.SettingValue;

                    var response = await clientGet.GetAsync(requestUri);
                    //return response;
                    //var response = await _mwiAPIService.HoldTAPTAPVoucherAsync(customerid, voucherid, storeCode, transactionId);
                    var responseString = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseString))
                    {
                        fileXs = new List<string>();
                       
                        var rsModel = JsonConvert.DeserializeObject<List<string>>(responseString);
                        //var result = responseString as List<string>;
                        if (rsModel != null && rsModel.Count > 0)
                        {
                            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                            
                            string folder = Path.Combine(rootPath, "medias/");
                           
                            if (!Directory.Exists(folder))
                            {
                                Directory.CreateDirectory(folder);
                            }
                            System.IO.DirectoryInfo di = new DirectoryInfo(folder);
                            foreach (FileInfo file in di.GetFiles())
                            {
                                try
                                {
                                    file.Delete();
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }

                            } 
                            foreach (string link in rsModel)
                            {
                                using (var client = new HttpClient())
                                {
                                    string NewLink = link.Replace(settingMedia.CustomField1, settingMedia.CustomField2);
                                    using (var s = client.GetStreamAsync(NewLink))
                                    {
                                        var SettingValue = NewLink.Split('/');
                                        var fileName = SettingValue[SettingValue.Length - 1];
                                        string fName = fileName;
                                        string savePath = Path.Combine(folder + fName);
                                        using (var fs = new FileStream(savePath, FileMode.OpenOrCreate))
                                        {
                                            s.Result.CopyTo(fs);
                                            Uri = Uri.Replace("/api" , ""); 
                                            fileXs.Add(Uri + "/medias/" + fileName);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (fileXs == null || fileXs.Count == 0)
                    {
                       result =  await getDirectoryFiles(CompanyCode, StoreId, Uri, path, true);
                       return result;
                    }
                    else
                    {
                        result.Success = true;
                        result.Data = fileXs;
                        return result;
                    }    
                   

                }    
                else
                {
                    DirectoryInfo d = new DirectoryInfo(@path); //Assuming Test is your Folder

                    FileInfo[] Files = d.GetFiles(); //"*.txt" //Getting Text files
                    string str = "";

                    foreach (FileInfo file in Files)
                    {
                        if (file.Extension.ToLower().Contains("mp4") || file.Extension.ToLower().Contains("mwv"))
                        {
                            fileXs = new List<string>();
                            fileXs.Add("./assets/medias/" + file.Name);
                            result.Message = "video";
                            result.Success = true;
                            result.Data = fileXs;
                            return result;
                        }
                        else
                        {
                            fileXs.Add("./assets/medias/" + file.Name);
                        }

                    }
                    result.Success = true;
                    result.Data = fileXs;
                    return result;
                }    
               

                //string[] files = Directory.GetFiles(@"C:\File");
               
                //foreach (var file in files)
                //{
                //    //file.
                //}
             
            }
            catch (IOException e)
            {
                result.Success = false;
                result.Message = e.Message;
            }
            return result;


        }


        //POS Option
        [HttpGet]
        [Route("GetPOSOption")]
        public async Task<GenericResult> GetPOSOption(string Type)
        {
            return await _commonService.GetPOSOption(Type);
        }

        [HttpGet]
        [Route("OpenDrawer")]
        public async Task<GenericResult> OpenDrawer(string Name, string BillNo)
        {
            return await _commonService.OpenDrawerCash(Name, BillNo);
        }

        [HttpGet]
        [Route("PaperCut")]
        public async Task<GenericResult> PaperCut(bool? isFull, string Name)
        {
            return await _commonService.PageCut(isFull, Name);
        }
        [HttpGet]
        [Route("PrintByPDF")]
        public async Task<GenericResult> PrintByPDF(string CompanyCode, string StoreId, string pdfFileName, string PrintName, string PrintSize, string PrintType)
        {
            return await _commonService.PrintByPDF( CompanyCode,  StoreId, pdfFileName, PrintName, PrintSize, PrintType);
        }
   
        //[Cached(600)]
        [HttpGet]
        [Route("GetPOSType")]
        public async Task<ActionResult<GenericResult>> GetPOSType()
        {
            return Ok(await _commonService.GetPOSType());
        }


        private static void IPAddresses(string server)
        {
            try
            {
                System.Text.ASCIIEncoding ASCII = new System.Text.ASCIIEncoding();

                // Get server related information.
                IPHostEntry heserver = Dns.GetHostEntry(server);

                // Loop on the AddressList
                foreach (IPAddress curAdd in heserver.AddressList)
                {


                    // Display the type of address family supported by the server. If the
                    // server is IPv6-enabled this value is: InterNetworkV6. If the server
                    // is also IPv4-enabled there will be an additional value of InterNetwork.
                    Console.WriteLine("AddressFamily: " + curAdd.AddressFamily.ToString());

                    // Display the ScopeId property in case of IPV6 addresses.
                    if (curAdd.AddressFamily.ToString() == ProtocolFamily.InterNetworkV6.ToString())
                        Console.WriteLine("Scope Id: " + curAdd.ScopeId.ToString());


                    // Display the server IP address in the standard format. In
                    // IPv4 the format will be dotted-quad notation, in IPv6 it will be
                    // in in colon-hexadecimal notation.
                    Console.WriteLine("Address: " + curAdd.ToString());

                    // Display the server IP address in byte format.
                    Console.Write("AddressBytes: ");

                    Byte[] bytes = curAdd.GetAddressBytes();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        Console.Write(bytes[i]);
                    }

                    Console.WriteLine("\r\n");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[DoResolve] Exception: " + e.ToString());
            }
        }

        // This IPAddressAdditionalInfo displays additional server address information.
        private static void IPAddressAdditionalInfo()
        {
            try
            {
                // Display the flags that show if the server supports IPv4 or IPv6
                // address schemas.
                Console.WriteLine("\r\nSupportsIPv4: " + Socket.SupportsIPv4);
                Console.WriteLine("SupportsIPv6: " + Socket.SupportsIPv6);

                if (Socket.SupportsIPv6)
                {
                    // Display the server Any address. This IP address indicates that the server
                    // should listen for client activity on all network interfaces.
                    Console.WriteLine("\r\nIPv6Any: " + IPAddress.IPv6Any.ToString());

                    // Display the server loopback address.
                    Console.WriteLine("IPv6Loopback: " + IPAddress.IPv6Loopback.ToString());

                    // Used during autoconfiguration first phase.
                    Console.WriteLine("IPv6None: " + IPAddress.IPv6None.ToString());

                    Console.WriteLine("IsLoopback(IPv6Loopback): " + IPAddress.IsLoopback(IPAddress.IPv6Loopback));
                }
                Console.WriteLine("IsLoopback(Loopback): " + IPAddress.IsLoopback(IPAddress.Loopback));
            }
            catch (Exception e)
            {
                Console.WriteLine("[IPAddresses] Exception: " + e.ToString());
            }
        }

        [HttpGet]
        [Route("GetItemCustomList")]
        public async Task<GenericResult> GetItemCustomList(string Type)
        {
            return await _commonService.GetItemCustomList(Type);
        }

        [HttpGet]
        [Route("GetNameOf")]
        public async Task<GenericResult> GetNameOf(string Model)
        {
            if (Model == "RPT_SalesStoreSummary")
            {
                var properties = typeof(RPT_SalesStoreSummaryModel).GetProperties().Where(p => p.GetCustomAttributes(typeof(RequiredAttribute), false).Length == 1).Select(p => p);

                // This will output the Name of the assigned public properties.
                foreach (var item in properties)
                {
                    Console.WriteLine(item.Name);
                }
            }

            return null;
        }

        [HttpGet]
        [Route("CheckUserStatus")]
        public bool CheckUserStatus(string CompanyCode, string UserName)
        {
            bool result = false;
            if (AppConstants.ActiveUsers != null && AppConstants.ActiveUsers.Count > 0)
            {
                var userList = AppConstants.ActiveUsers.Where(x => x.UserId == UserName).ToList();
                if (userList != null && userList.Count() > 0)
                {
                    return true;
                }
            }
            //Context.

            return result;
        }
        [HttpGet]
        [Route("GetDefaultStore")]
        public GenericResult GetDefaultStore()
        {
            GenericResult resultData = new GenericResult();
            string result = "";
            try
            {
                result = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("DefaultStore"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                resultData.Success = true;
            }
            catch (Exception ex)
            {
                result = "";
                resultData.Success = false;
            }
            resultData.Data = result;
            return resultData;
        }
        [HttpGet]
        [Route("SendEmail")]
        public GenericResult Send(string to, string subject, string html)
        {
            GenericResult resultData = new GenericResult();
            try
            {
                // create message
                string SmtpHost = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("SmtpHost"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                string SmtpPort = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("SmtpPort"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                string username = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("username"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                string pass = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("pass"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(username));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = html };
                int.TryParse(SmtpPort, out int SmtpPort1);
                // send email
                using var smtp = new SmtpClient();
                smtp.Connect(SmtpHost, SmtpPort1, SecureSocketOptions.StartTls);
                smtp.Authenticate(username, pass);
                smtp.Send(email);
                smtp.Disconnect(true);
                resultData.Success = true;
            }
            catch (Exception ex)
            {
                resultData.Success = false;
                resultData.Message = ex.Message;
            }


            return resultData;
        }



        #region Pole Display


        [HttpGet]
        [Route("PoleGetPortName")]
        public GenericResult GetPortName()
        {

            GenericResult result = new GenericResult();
            try
            {
                result.Success = true;
                result.Data = SerialPort.GetPortNames();

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Data = null;
                result.Message = ex.Message;
            }
            return result;
        }

        [HttpGet]
        [Route("PoleGetPortNameParity")]
        public GenericResult GetPortNameParity()
        {

            GenericResult result = new GenericResult();
            try
            {
                result.Success = true;
                result.Data = Enum.GetNames(typeof(Parity));

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Data = null;
                result.Message = ex.Message;
            }
            return result;
        }


        public class PoleMessage
        {
            public string SerialPortName { get; set; }
            public string SerialPortBaudRate { get; set; }
            public string SerialPortParity { get; set; }
            public string SerialPortDataBits { get; set; }
            public string SerialPortStopBits { get; set; }
            public string SerialPortHandshake { get; set; }
            public string CompanyCode { get; set; }
            public string StoreId { get; set; }
            public string CounterId { get; set; }
            public string Message { get; set; }
            public string Message2 { get; set; }
        }


        [HttpPost]
        [Route("PoleShowMess")]
        public async Task<GenericResult> ShowMess(PoleMessage poleMessage)
        {

            GenericResult genericResult = new GenericResult();
            try
            {
                StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
                Thread readThread = new Thread(Read);

                // Create a new SerialPort object with default settings.  
                //_serialPort = new SerialPort();

                if (string.IsNullOrEmpty(poleMessage.SerialPortName))
                {
                    var PoleSettingData = await _storeClientService.GetById(poleMessage.CompanyCode, poleMessage.StoreId, "", "", poleMessage.CounterId);
                    if (PoleSettingData.Success && PoleSettingData.Data != null)
                    {
                        var PoleSetting = PoleSettingData.Data as SStoreClient;
                        poleMessage.SerialPortName = PoleSetting.PoleName;
                        poleMessage.SerialPortParity = PoleSetting.PoleParity;
                        poleMessage.SerialPortBaudRate = PoleSetting.PoleBaudRate;
                        poleMessage.SerialPortDataBits = PoleSetting.PoleDataBits;
                        poleMessage.SerialPortStopBits = PoleSetting.PoleStopBits;
                        poleMessage.SerialPortHandshake = PoleSetting.PoleHandshake;

                    }
                    else
                    {
                        return PoleSettingData;
                    }
                }
                if (!string.IsNullOrEmpty(poleMessage.SerialPortName))
                {
                    //SerialPortName = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("SerialPortName"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                    //SerialPortBaudRate = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("SerialPortBaudRate"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                    //SerialPortParity = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("SerialPortParity"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                    //SerialPortDataBits = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("SerialPortDataBits"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                    //SerialPortStopBits = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("SerialPortStopBits"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                    //SerialPortHandshake = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("SerialPortHandshake"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                    Parity parity = Parity.None;

                    switch (poleMessage.SerialPortParity)
                    {

                        case "Odd":
                            parity = Parity.Odd;
                            break;

                        case "Even":
                            parity = Parity.Odd;
                            break;
                        case "Mark":
                            parity = Parity.Mark;
                            break;
                        case "Space":
                            parity = Parity.Space;
                            break;

                        default:
                            parity = Parity.None;
                            break;
                    }
                    int serialDataBits = 8;
                    if (string.IsNullOrEmpty(poleMessage.SerialPortDataBits))
                    {
                        serialDataBits = 8;
                    }
                    else
                    {
                        serialDataBits = int.Parse(poleMessage.SerialPortDataBits);
                    }
                    int serialPortBaudRate = 9600;
                    if (string.IsNullOrEmpty(poleMessage.SerialPortBaudRate))
                    {
                        serialPortBaudRate = 9600;
                    }
                    else
                    {
                        serialPortBaudRate = int.Parse(poleMessage.SerialPortBaudRate);
                    }
                    _serialPort = new SerialPort(poleMessage.SerialPortName, serialPortBaudRate, parity, serialDataBits, StopBits.One);

                    //// Allow the user to set the appropriate properties.  
                    //_serialPort.PortName = SetPortName(_serialPort.PortName);
                    //_serialPort.BaudRate = SetPortBaudRate(_serialPort.BaudRate);
                    //_serialPort.Parity = SetPortParity(_serialPort.Parity);
                    //_serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
                    //_serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
                    //_serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);

                    // Set the read/write timeouts  
                    _serialPort.ReadTimeout = 500;
                    _serialPort.WriteTimeout = 500;


                    _serialPort.Handshake = Handshake.None;
                    _serialPort.Open();
                    _serialPort.DiscardInBuffer();
                    _serialPort.DiscardOutBuffer();
                    _serialPort.RtsEnable = true;
                    _serialPort.DtrEnable = true;
                    _serialPort.DataReceived += new SerialDataReceivedEventHandler(_serialPort_DataReceived);
                    //_continue = true;
                    //readThread.Start();

                    //Console.Write("Name: ");
                    //name = Console.ReadLine();

                    //Console.WriteLine("Type QUIT to exit");
                    var clear = "                                        ";
                    _serialPort.Write(clear);
                    //poleMessage.Message = clear + poleMessage.Message;
                    if (stringComparer.Equals("clear", poleMessage.Message))
                    {
                        poleMessage.Message = "                                        ";
                    }
                    if (poleMessage.Message.Length > 20)
                    {
                        poleMessage.Message = poleMessage.Message.Substring(0, 20);
                    }
                    string str = "";
                    if (string.IsNullOrEmpty(poleMessage.Message))
                    {
                        poleMessage.Message = "";
                    }
                    if (poleMessage.Message.Length < 20)
                    {
                        int space = 20 - poleMessage.Message.Length;

                        if (space > 0)
                        {
                            for (int i = 0; i < space; i++)
                            {
                                str += " ";
                            }
                        }

                        poleMessage.Message = poleMessage.Message.Substring(0, poleMessage.Message.Length) + str;

                    }
                    if (poleMessage.Message.Length > 20)
                    {
                        poleMessage.Message = poleMessage.Message.Substring(0, 20);
                    }


                    string str2 = "";
                    if (string.IsNullOrEmpty(poleMessage.Message2))
                    {
                        poleMessage.Message2 = "";
                    }
                    if (poleMessage.Message2.Length < 20)
                    {
                        int space = 20 - poleMessage.Message2.Length;

                        if (space > 0)
                        {
                            for (int i = 0; i < space; i++)
                            {
                                str2 += " ";
                            }
                        }
                        poleMessage.Message2 = poleMessage.Message2.Substring(0, poleMessage.Message2.Length) + str2;
                    }
                    if (poleMessage.Message2.Length > 20)
                    {
                        poleMessage.Message2 = poleMessage.Message2.Substring(0, 20);
                    }
                    _serialPort.Write(poleMessage.Message);
                    _serialPort.Write(poleMessage.Message2);


                    //while (_continue)
                    //{
                    //    //message = MessageConsole.ReadLine();

                    //    if (stringComparer.Equals("quit", poleMessage.Message))
                    //    {
                    //        _continue = false;
                    //    }
                    //    else
                    //    {

                    //        _continue = false;

                    //    }
                    //}

                    //readThread.Join();
                    _serialPort.DiscardOutBuffer();
                    _serialPort.DiscardInBuffer();
                    _serialPort.Close();
                    _serialPort = null;
                    genericResult.Success = true;
                }
                else
                {
                    genericResult.Success = false;
                    genericResult.Message = "Please setting pole display data";

                }

            }
            catch (Exception ex)
            {
                genericResult.Success = false;
                genericResult.Message = ex.Message;

            }
            return genericResult;
        }
        private AutoResetEvent _receiveNow;
        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (e.EventType == SerialData.Chars)
                {
                    _receiveNow.Set();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("PoleRead")]
        public void Read()
        {
            while (_continue)
            {
                try
                {
                    string message = _serialPort.ReadLine();
                    Console.WriteLine(message);
                }
                catch (TimeoutException) { }
            }
        }
        [HttpGet]
        [Route("PoleSetPortName")]
        public string SetPortName(string defaultPortName)
        {
            string portName;

            Console.WriteLine("Available Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("COM port({0}): ", defaultPortName);
            portName = Console.ReadLine();

            if (portName == "")
            {
                portName = defaultPortName;
            }
            return portName;
        }
        [HttpGet]
        [Route("PoleSetPortBaudRate")]
        public int SetPortBaudRate(int defaultPortBaudRate)
        {
            string baudRate;

            Console.Write("Baud Rate({0}): ", defaultPortBaudRate);
            baudRate = Console.ReadLine();

            if (baudRate == "")
            {
                baudRate = defaultPortBaudRate.ToString();
            }

            return int.Parse(baudRate);
        }
        [HttpGet]
        [Route("PoleSetPortParity")]
        public Parity SetPortParity(Parity defaultPortParity)
        {
            string parity;

            Console.WriteLine("Available Parity options:");
            foreach (string s in Enum.GetNames(typeof(Parity)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Parity({0}):", defaultPortParity.ToString());
            parity = Console.ReadLine();

            if (parity == "")
            {
                parity = defaultPortParity.ToString();
            }

            return (Parity)Enum.Parse(typeof(Parity), parity);
        }
        [HttpGet]
        [Route("PoleSetPortDataBits")]
        public int SetPortDataBits(int defaultPortDataBits)
        {
            string dataBits;

            Console.Write("Data Bits({0}): ", defaultPortDataBits);
            dataBits = Console.ReadLine();

            if (dataBits == "")
            {
                dataBits = defaultPortDataBits.ToString();
            }

            return int.Parse(dataBits);
        }
        [HttpGet]
        [Route("PoleSetPortStopBits")]
        public StopBits SetPortStopBits(StopBits defaultPortStopBits)
        {
            string stopBits;

            Console.WriteLine("Available Stop Bits options:");
            foreach (string s in Enum.GetNames(typeof(StopBits)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Stop Bits({0}):", defaultPortStopBits.ToString());
            stopBits = Console.ReadLine();

            if (stopBits == "")
            {
                stopBits = defaultPortStopBits.ToString();
            }

            return (StopBits)Enum.Parse(typeof(StopBits), stopBits);
        }
        [HttpGet]
        [Route("PoleSetPortHandshake")]
        public Handshake SetPortHandshake(Handshake defaultPortHandshake)
        {
            string handshake;

            Console.WriteLine("Available Handshake options:");
            foreach (string s in Enum.GetNames(typeof(Handshake)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Handshake({0}):", defaultPortHandshake.ToString());
            handshake = Console.ReadLine();

            if (handshake == "")
            {
                handshake = defaultPortHandshake.ToString();
            }

            return (Handshake)Enum.Parse(typeof(Handshake), handshake);
        }

        #endregion
    }
    //Using:
    //SettingsHelpers.AddOrUpdateAppSetting<int>("PrefixTest", "ABC");
    public static class SettingsHelpers
    {
        public static void AddOrUpdateAppSetting<T>(string sectionPathKey, T value, string fileSrc)
        {
            try
            {
                var filePath = ""; 
                //Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                if(!string.IsNullOrEmpty(fileSrc) )
                {
                    filePath =  Path.Combine(fileSrc);
                }   
                else
                {
                    filePath =   Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                }    
                string json = File.ReadAllText(filePath);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                SetValueRecursively(sectionPathKey, jsonObj, value);

                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filePath, output);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing app settings | {0}", ex.Message);
            }
        }

        private static void SetValueRecursively<T>(string sectionPathKey, dynamic jsonObj, T value)
        {
            // split the string at the first ':' character
            var remainingSections = sectionPathKey.Split(":", 2);

            var currentSection = remainingSections[0];
            if (remainingSections.Length > 1)
            {
                // continue with the procress, moving down the tree
                var nextSection = remainingSections[1];
                SetValueRecursively(nextSection, jsonObj[currentSection], value);
            }
            else
            {
                // we've got to the end of the tree, set the value
                jsonObj[currentSection] = value;
            }
        }
    }
}

public class AppSettingModel
{
    public string DefaultConnection { get; set; }
    public string MWIService { get; set; }
    public string ShiftConnection { get; set; }
    public string EndDateConnection { get; set; }
    public string ReportConnection { get; set; }
    public string Redis { get; set; }
    public string PrefixSO { get; set; }
    public string PrefixAR { get; set; }
    public string PrefixPR { get; set; }
    public string PrefixPO { get; set; }
    public string PrefixGRPO { get; set; }
    public string PrefixTS { get; set; }
    public string PrefixTR { get; set; }
    public string PrefixGI { get; set; }
    public string PrefixGR { get; set; }
    public string PrefixGRT { get; set; }
    public string PrefixTF { get; set; }
    public string PrefixSP { get; set; }
    public string PrefixCounting { get; set; }
    public string PrefixPosting { get; set; }
    public string PrefixShift { get; set; }
    public string ComBankTerminal { get; set; }
    public string PrintLoopTime { get; set; }
    public string PrintWaitTime { get; set; }
    public string TimeCacheAction { get; set; }
    public string PrintSize57Scale { get; set; }
    public string PrintSize80Scale { get; set; }
    public string PrintFolder { get; set; }
    public string TerminalId { get; set; }
}