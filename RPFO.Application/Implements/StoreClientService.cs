
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
    public class StoreClientService : IStoreClientService
    {
        private readonly IGenericRepository<SStoreClient> _clientRepository;
    
        private readonly IGeneralSettingService _settingService;
        IClientDisallowanceService _clientDisallowanceService;
        private readonly IMapper _mapper;
        public StoreClientService(IGenericRepository<SStoreClient> clientRepository, IClientDisallowanceService clientDisallowanceService, IGeneralSettingService settingService, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _clientRepository = clientRepository;
            _settingService = settingService;
            _clientDisallowanceService = clientDisallowanceService;
            _mapper = mapper; 

        }
        //public async Task<GenericResult> Import(DataImport model)
        //{
        //    GenericResult result = new GenericResult();
        //    List<UOMResultViewModel> resultlist = new List<UOMResultViewModel>();
        //    try
        //    {
        //        foreach (var item in model.Uom)
        //        {
        //            item.CreatedBy = model.CreatedBy;
        //            item.CompanyCode = model.CompanyCode;
        //            var itemResult = await Create(item);
        //            //if (itemResult.Success == false)
        //            //{
        //                UOMResultViewModel itemRs = new UOMResultViewModel();
        //                itemRs = _mapper.Map<UOMResultViewModel>(item);
        //                itemRs.Success = itemResult.Success;
        //                itemRs.Message = itemResult.Message;
        //            resultlist.Add(itemRs);
        //            //}
        //        }
        //        result.Success = true;
        //        result.Data = resultlist;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = ex.Message;
        //        //result.Data = failedlist;
        //    } 
        //    return result;
        //}

         
        public async Task<GenericResult> Create(SStoreClient model)
        {
            GenericResult result = new GenericResult();
            try
            {

                var settingData = await _settingService.GetGeneralSettingByStore(model.CompanyCode, model.StoreId);
                List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                if (settingData.Success)
                {
                    SettingList = settingData.Data as List<GeneralSettingStore>;
                }
                var setting1 = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "DefaultCounter").FirstOrDefault();
                if (setting1 != null && (setting1.SettingValue == "none" || setting1.SettingValue.ToLower() == "none"))
                {
                   
                    if (string.IsNullOrEmpty(model.PublicIP))
                    {
                        result.Success = false;
                        result.Message = model.PublicIP + " can't null.";
                        return result;
                    }
                }
                else
                {
                    model.PublicIP = model.LocalIP;

                }

                var parameters = new DynamicParameters();
                string Id = Guid.NewGuid().ToString();
                parameters.Add("Id", Id);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("Name", model.Name);
                parameters.Add("LocalIP", model.LocalIP);
                parameters.Add("PublicIP", model.PublicIP);
                parameters.Add("Custom1", model.Custom1);
                parameters.Add("Custom2", model.Custom2);
                parameters.Add("Custom3", model.Custom3);
                parameters.Add("Custom4", model.Custom4);
                parameters.Add("Custom5", model.Custom5);
                parameters.Add("FromDate", model.FromDate);
                parameters.Add("ToDate", model.ToDate);
                parameters.Add("Status", model.Status);

                parameters.Add("PoleName", model.PoleName);
                parameters.Add("PoleBaudRate", model.PoleBaudRate);
                parameters.Add("PoleParity", model.PoleParity);
                parameters.Add("PoleDataBits", model.PoleDataBits);
                parameters.Add("PoleStopBits", model.PoleStopBits);
                parameters.Add("PoleHandshake", model.PoleHandshake);
                parameters.Add("PrintSize", model.PrintSize);
                parameters.Add("PrintName", model.PrintName);
                var exist = await GetById(model.CompanyCode, model.StoreId, "", model.LocalIP , "");
                if (exist.Success == true && exist.Data !=null)
                {
                    result.Success = false;
                    result.Message = model.LocalIP + " has existed.";
                    return result;
                }
                var affectedRows = _clientRepository.Insert("USP_I_S_StoreClient", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                result.Message = Id;
                model.Id = Guid.Parse(Id) ;
                var dataAf = await GetById(model.CompanyCode, model.StoreId, "", model.LocalIP, "");
                result.Data = dataAf.Data;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
         
        public async Task<GenericResult> Delete(SStoreClient model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("Id", model.Id);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId); 
               
                var affectedRows = _clientRepository.Insert("USP_D_S_StoreClient", parameters, commandType: CommandType.StoredProcedure);
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
       
        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, DateTime? From, DateTime? To)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _clientRepository.GetAllAsync($"USP_S_S_StoreClient '{CompanyCode}' , '{StoreId}' , '', '','', '{From}', '{To}'", null, commandType: CommandType.Text);
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

        public async Task<GenericResult> GetById(string CompanyCode, string StoreId, string Id, string LocalIP, string PublicIP)
        {
            GenericResult result = new GenericResult();
            try
            {
                string querry = $"USP_S_S_StoreClient '{CompanyCode}', '{StoreId}' , '{Id}', '{LocalIP}','{PublicIP}',  '', ''";
                var data = await _clientRepository.GetAsync(querry, null, commandType: CommandType.Text);
                var settingData = await _settingService.GetGeneralSettingByStore(CompanyCode, StoreId);

                List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                if (settingData.Success)
                {
                    SettingList = settingData.Data as List<GeneralSettingStore>;
                }
                if (data != null && SettingList != null && SettingList.Count > 0)
                {
                    var canCheckDevice = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "ClientDisallowance").FirstOrDefault();
                    //var  = true;
                    if (canCheckDevice != null && (canCheckDevice.SettingValue == "true" || canCheckDevice.SettingValue == "1"))
                    {
                        var disableResult = await _clientDisallowanceService.GetAll(CompanyCode, StoreId, "", LocalIP, "");
                        if (disableResult.Success)
                        {
                            data.Disallowances = disableResult.Data as List<SClientDisallowance>;

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

        public async Task<GenericResult> GetCounterSalesInDay(string CompanyCode, string StoreId,  DateTime? Date)
        {
            GenericResult result = new GenericResult();
            try
            {
                string querry = $"USP_GetCounterSalesInDay '{CompanyCode}', '{StoreId}' , '{Date.Value.ToString("yyyy-MM-dd")}'";
                var data = await _clientRepository.GetAllAsync(querry, null, commandType: CommandType.Text);
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

        public async Task<GenericResult> Update(SStoreClient model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("Id", model.Id, DbType.Guid);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("Name", model.Name);
                parameters.Add("LocalIP", model.LocalIP);
                parameters.Add("PublicIP", model.PublicIP); 
                parameters.Add("Custom1", model.Custom1);
                parameters.Add("Custom2", model.Custom2);
                parameters.Add("Custom3", model.Custom3);
                parameters.Add("Custom4", model.Custom4);
                parameters.Add("Custom5", model.Custom5);
                parameters.Add("FromDate", model.FromDate);
                parameters.Add("ToDate", model.ToDate);
                parameters.Add("Status", model.Status);

                parameters.Add("PoleName", model.PoleName);
                parameters.Add("PoleBaudRate", model.PoleBaudRate);
                parameters.Add("PoleParity", model.PoleParity);
                parameters.Add("PoleDataBits", model.PoleDataBits);
                parameters.Add("PoleStopBits", model.PoleStopBits);
                parameters.Add("PoleHandshake", model.PoleHandshake);
                parameters.Add("PrintSize", model.PrintSize);
                parameters.Add("PrintName", model.PrintName);

                //var exist = await GetById(model.CompanyCode, model.StoreId, "", model.LocalIP, "");
                //if (exist.Success == true && exist.Data != null)
                //{
                //    result.Success = false;
                //    result.Message = model.LocalIP + " has existed.";
                //    return result;
                //}

                var affectedRows = _clientRepository.Update("USP_U_S_StoreClient", parameters, commandType: CommandType.StoredProcedure);
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



        public async Task<GenericResult> UpdateByPublicId(SStoreClient model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();  
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("Name", model.Name);
                parameters.Add("LocalIP", model.LocalIP);
                parameters.Add("PublicIP", model.PublicIP);
                parameters.Add("Custom1", model.Custom1);
                parameters.Add("Custom2", model.Custom2);
                parameters.Add("Custom3", model.Custom3);
                parameters.Add("Custom4", model.Custom4);
                parameters.Add("Custom5", model.Custom5);
                parameters.Add("FromDate", model.FromDate);
                parameters.Add("ToDate", model.ToDate);
                parameters.Add("Status", model.Status);
                //var exist = await checkExist(model.CompanyCode, model.UomCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.UomCode + " existed.";
                //    return result;
                //}
                var affectedRows = _clientRepository.Update("USP_U_S_StoreClientByPublicId", parameters, commandType: CommandType.StoredProcedure);
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
