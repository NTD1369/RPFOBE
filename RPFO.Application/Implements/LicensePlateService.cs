
using AutoMapper;
using Dapper;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Infrastructure;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPFO.Data.Entities;
using RPFO.Utilities.Extensions;

namespace RPFO.Application.Implements
{
    public class LicensePlateService : ILicensePlateService
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<LicensePlateHearder> _LicensePlateRepository;
        private readonly IGenericRepository<LicensePlateLine> _LicensePlateLineRepository;
        private readonly IGenericRepository<TSalesRedeemMenber> _RedeemRepository;
        private readonly ICommonService _commonService;
        string ServiceName = "LicensePlate";
        private string PrefixLP = "LP";
        List<string> TableNameList = new List<string>();
        public LicensePlateService(IMapper mapper, IGenericRepository<LicensePlateHearder> licensePlateRepository,
            IGenericRepository<TSalesRedeemMenber> redeemRepository, IGenericRepository<LicensePlateLine> LicensePlateLineRepository, ICommonService commonService)
        {
            _mapper = mapper;
            _LicensePlateRepository = licensePlateRepository;
            _LicensePlateLineRepository = LicensePlateLineRepository;
            _RedeemRepository = redeemRepository;
            _commonService = commonService;

            TableNameList.Add(ServiceName + "Line");
            _commonService.InitService(ServiceName, TableNameList);
        }

        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<LicensePlateResultViewModel> resultlist = new List<LicensePlateResultViewModel>();
            try
            {
                foreach (var item in model.LicensePlateImport)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);

                    LicensePlateResultViewModel itemRs = new LicensePlateResultViewModel();
                    itemRs = _mapper.Map<LicensePlateResultViewModel>(item);
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
        public async Task<GenericResult> Create(LicensePlateViewModel model)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _LicensePlateRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    //string LicensePlateLine1 = "LicensePlateLine";
                    //var IVLines1 = _commonService.CreaDataTable(LicensePlateLine1);
                    //IVLines1 = ExtensionsNew.ConvertListToDataTable(model.Lines, IVLines1);
                  
                    using (var tran = db.BeginTransaction())
                    {
                        if(model.Lines.Count<=0)
                        {
                            result.Message = "Doc line not null.";
                            return result;
                        }
                        result.Message = "";
                        model.Lines.ForEach(async line =>
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("CompanyCode", model.CompanyCode);
                            parameters.Add("LicensePlate", line.LicensePlate);
                            parameters.Add("StartDate", model.StartDate);
                            parameters.Add("ItemCode", model.ItemCode);
                            var affectedRows = _LicensePlateRepository.GetAsync("CheckLicensePlate", parameters, commandType: CommandType.StoredProcedure).Result;
                            if (affectedRows != null)
                            {
                                result.Message += line.LicensePlate + ',';
                            }
                        });
                        if (result.Message.Length > 0)
                        {
                            var test = "Biển số xe đang được sử dụng: " + result.Message;
                            result.Success = false;
                            result.Message = test;
                            return result;
                        }
                        //var parameters = new DynamicParameters();
                        //parameters.Add("CompanyCode", model.CompanyCode);
                        //parameters.Add("Contract", model.Contract);
                        //parameters.Add("StartDate", model.StartDate);
                        //parameters.Add("EndDate", model.EndDate);
                        //parameters.Add("Times", model.TimesInDay);
                        //parameters.Add("CreatedBy", model.CreatedBy);
                        ////parameters.Add("CreatedOn", model.CreatedOn);
                        //parameters.Add("CustomF1", model.CustomF1);
                        //parameters.Add("CustomF2", model.CustomF2);
                        //parameters.Add("CustomF3", model.CustomF3);
                        //parameters.Add("CustomF4", model.CustomF4);
                        //parameters.Add("CustomF5", model.CustomF5);
                        //parameters.Add("ItemCode", model.ItemCode);

                        //var exist = await checkExist(model.CompanyCode, model.LicensePlate, model.StartDate,model.EndDate);
                        //if (exist == true)
                        //{
                        //    result.Success = false;
                        //    result.Message = model.LicensePlate +'-'+model.StartDate.ToString() + '-' + model.EndDate.ToString() + " existed.";
                        //    return result;
                        //}
                        //var affectedRows = _LicensePlateRepository.Insert("USP_I_M_LicensePlate", parameters, commandType: CommandType.StoredProcedure);

                        //var LicensePlateLine = new DataTable("LicensePlateLine");
                      

                        string key = _LicensePlateRepository.GetScalar($" select   dbo.fnc_AutoGenDocumentCode('{PrefixLP}','{model.CompanyCode}','')", null, commandType: CommandType.Text);
                        var parameters = new DynamicParameters();
                        parameters.Add("TransId", key);
                        parameters.Add("CompanyCode", model.CompanyCode);
                        parameters.Add("Contract", model.Contract);
                        parameters.Add("StartDate", model.StartDate);
                        parameters.Add("EndDate", model.EndDate);
                        parameters.Add("Times", model.TimesInDay);
                        parameters.Add("CreatedBy", model.CreatedBy);
                        //parameters.Add("CreatedOn", model.CreatedOn);
                        parameters.Add("CustomF1", model.CustomF1);
                        parameters.Add("CustomF2", model.CustomF2);
                        parameters.Add("CustomF3", model.CustomF3);
                        parameters.Add("CustomF4", model.CustomF4);
                        parameters.Add("CustomF5", model.CustomF5);
                        parameters.Add("ItemCode", model.ItemCode);
                        parameters.Add("Remark", model.Remark);

                        //var exist = await checkExist(model.CompanyCode, model.LicensePlate, model.StartDate,model.EndDate);
                        //if (exist == true)
                        //{
                        //    result.Success = false;
                        //    result.Message = model.LicensePlate +'-'+model.StartDate.ToString() + '-' + model.EndDate.ToString() + " existed.";
                        //    return result;
                        //}
                        string LicensePlateLine = "LicensePlateLine";
                        model.Lines.ForEach(line => {
                            line.CompanyCode = model.CompanyCode;
                            line.TransId = key;
                            line.CreatedOn = DateTime.Now;
                            line.CreatedBy = model.CreatedBy;
                        });

                        var IVLines = _commonService.CreaDataTable(LicensePlateLine);
                        IVLines = ExtensionsNew.ConvertListToDataTable(model.Lines, IVLines);
                        string tblLineType = IVLines + "TableType";
                        parameters.Add("@Lines", IVLines.AsTableValuedParameter(LicensePlateLine + "TableType"));
                        var affectedRows = _LicensePlateRepository.Insert("USP_I_LicensePlate", parameters, commandType: CommandType.StoredProcedure);
                        //model.Lines.ForEach(lines =>
                        //{
                        //    var parameters = new DynamicParameters();
                        //    parameters.Add("TransId", key);
                        //    parameters.Add("CompanyCode", model.CompanyCode);
                        //    parameters.Add("LineId", lines.LineId);
                        //    parameters.Add("LicensePlate", lines.LicensePlate);
                        //    parameters.Add("Remark", lines.Remark);
                        //    parameters.Add("CreatedBy", model.CreatedBy);
                        //    //parameters.Add("CreatedOn", model.CreatedOn);
                        //    parameters.Add("CustomF1", model.CustomF1);
                        //    parameters.Add("CustomF2", model.CustomF2);
                        //    parameters.Add("CustomF3", model.CustomF3);
                        //    parameters.Add("CustomF4", model.CustomF4);
                        //    parameters.Add("CustomF5", model.CustomF5);
                        //    //parameters.Add("PaymentCode", model.LicensePlateImport.PaymentCode);
                        //    var affectedRows = _LicensePlateRepository.Insert("USP_I_LicensePlateLine", parameters, commandType: CommandType.StoredProcedure);
                        //});
                        result.Success = true;
                        tran.Commit();
                        //result.Success = true;
                    }
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            } 
                return result;
            }
            public async Task<string> checkExist(List<LicensePlateLineViewModel> lines,string CompanyCode, string ItemCode,DateTime StartDate)
            {

                var parameters = new DynamicParameters();
                var Message = "";
            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            lines.ForEach(line =>
            {
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("LicensePlate", line.LicensePlate);
                parameters.Add("StartDate", StartDate);
                parameters.Add("ItemCode", ItemCode);
                var affectedRows = _LicensePlateRepository.GetAsync("CheckLicensePlate", parameters, commandType: CommandType.StoredProcedure).Result;
                if (affectedRows != null)
                {
                    Message += line.LicensePlate + ',';
                }
            });
               
            return Message;
        }
            public async Task<GenericResult> CheckLicensePlate(string CompanyCode, string LicensePlate, decimal quantity)
            {
                GenericResult result = new GenericResult();
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("LicensePlate", LicensePlate);
                parameters.Add("quantity", quantity);
                try
                {
                    var affectedRows = await _LicensePlateRepository.GetAsync("USP_S_CheckLicensePlate", parameters, commandType: CommandType.StoredProcedure);
                    if (affectedRows != null)
                    {
                        result.Success = true;
                    }
                    else
                    {
                        result.Success = false;
                    }

                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                }
                return result;
            }
            public async Task<GenericResult> GetVoucherInfo(string CompanyCode, string StoreId, string key, string Type)
            {
                GenericResult result = new GenericResult();

                using (IDbConnection db = _LicensePlateRepository.GetConnection(GConnection.Default))
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        var parameters = new DynamicParameters();
                        parameters.Add("CompanyCode", CompanyCode);
                        parameters.Add("StoreId", StoreId);
                        parameters.Add("Key", key);
                        parameters.Add("Type", Type);
                        var reader = await db.QueryMultipleAsync("USP_S_GetCardInformation", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                        var Customerinfo = reader.Read<CustomerinfoModel>().ToList();
                        var VourcherDetail = reader.Read<VourcherDetailModel>().ToList();
                        var VourcherDetailBom = reader.Read<VourcherDetailBomModel>().ToList();

                    VourcherInfoModel rst = new VourcherInfoModel();
                        rst.Customerinfo = Customerinfo;
                        rst.VourcherDetail = VourcherDetail;
                        rst.VourcherDetailBom = VourcherDetailBom;
                        result.Success = true;
                        result.Data = rst;
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
            public async Task<GenericResult> Redeem(TSalesRedeemMenber model)
            {
                GenericResult result = new GenericResult();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", model.CompanyCode);
                    parameters.Add("TransId", model.TransId);
                    parameters.Add("InvoiceId", model.InvoiceId);
                    parameters.Add("ItemCode", model.ItemCode);
                    parameters.Add("SerialNum", model.SerialNum);
                    parameters.Add("Quantity", model.Quantity);
                    parameters.Add("TimesInDay", model.TimesInDay);
                    parameters.Add("CreatedBy", model.CreatedBy);
                    //parameters.Add("CreatedOn", model.CreatedOn);
                    parameters.Add("CustomF1", model.CustomF1);
                    parameters.Add("CustomF2", model.CustomF2);
                    parameters.Add("CustomF3", model.CustomF3);
                    parameters.Add("CustomF4", model.CustomF4);
                    parameters.Add("CustomF5", model.CustomF5);
                    parameters.Add("UomCode", model.UomCode);

                    var affectedRows = _LicensePlateRepository.Insert("USP_I_T_SalesRedeemMenber", parameters, commandType: CommandType.StoredProcedure);

                    result.Success = true;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                }
                return result;
            }

            public async Task<GenericResult> GetAll(string CompanyCode, string key)
            {
                GenericResult result = new GenericResult();
                try
                {

                    var data = await _LicensePlateRepository.GetAllAsync($"select * from LicensePlateHearder with (nolock) where CompanyCode like N'%{CompanyCode}%'", null, commandType: CommandType.Text);

                    result.Data = data;
                    result.Success = true;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                }
                return result;
            }
            public async Task<GenericResult> GetById(string companyCode, string Id)
            {
                GenericResult result = new GenericResult();
                try
                {
                    var data = await _LicensePlateRepository.GetAsync($"USP_S_LicensePlateHeader '{companyCode}', '{Id}'", null, commandType: CommandType.Text);

                    if (data == null)
                        return null;
                    LicensePlateViewModel licensePlate = new LicensePlateViewModel();
                    licensePlate = _mapper.Map<LicensePlateViewModel>(data);
                    var lines = await _LicensePlateLineRepository.GetAllAsync($"USP_S_LicensePlateLine '{companyCode}',  '{Id}',''", null, commandType: CommandType.Text);

                    var lineData = _mapper.Map<List<LicensePlateLineViewModel>>(lines);
                    licensePlate.Lines = new List<LicensePlateLineViewModel>();
                    licensePlate.Lines = lineData;
                    result.Success = true;
                    result.Data = licensePlate;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                }
            return result;

        }
        public async Task<GenericResult> GetSerialInfo(string CompanyCode, string StoreId, string key)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _LicensePlateRepository.GetConnection(GConnection.Default))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Key", key);
                    var reader = await db.QueryMultipleAsync("USP_S_GetSerialInfo", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    var Customerinfo = reader.Read<CustomerinfoModel>().ToList();
                    var VourcherDetail = reader.Read<VourcherDetailModel>().ToList();

                    VourcherInfoModel rst = new VourcherInfoModel();
                    rst.Customerinfo = Customerinfo;
                    rst.VourcherDetail = VourcherDetail;
                    result.Success = true;
                    result.Data = rst;
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
        public async Task<GenericResult> Search(string CompanyCode, string key)
        {
            GenericResult result = new GenericResult();
            try
            {

                var data = await _LicensePlateLineRepository.GetAllAsync($"USP_S_LicensePlateline '{CompanyCode}','', '{key}'", null, commandType: CommandType.Text);

                result.Data = data;
                result.Success = true;
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
