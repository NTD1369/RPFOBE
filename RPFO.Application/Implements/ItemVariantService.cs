
using AutoMapper;
using Dapper;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Infrastructure;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPFO.Application.Implements.ReportService;

namespace RPFO.Application.Implements
{
    public class ItemVariantService : IItemVariantService
    {
        private readonly IGenericRepository<MItemVariant> _variantRepository;
        string ServiceName = "M_ItemVariant";
        List<string> TableNameList = new List<string>();
        private readonly ICommonService _commonService;
        private readonly IMapper _mapper;
        public ItemVariantService(IGenericRepository<MItemVariant> variantRepository, ICommonService commonService, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _variantRepository = variantRepository;
            _commonService = commonService;
            _mapper = mapper;
            TableNameList.Add(ServiceName + "Buy");
            TableNameList.Add(ServiceName + "Map");
            _commonService.InitService(ServiceName, TableNameList);
        }
     
        public async Task<GenericResult> Create(MItemVariant model)
        {
            GenericResult result = new GenericResult();
            try
            {
                using (IDbConnection db = _variantRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {

                                string BuyTbl = "M_ItemVariantBuy";
                                var VariantBuy = _commonService.CreaDataTable(BuyTbl);
                                string MapTbl = "M_ItemVariantMap";
                                string Key = "";
                                var VariantMap = _commonService.CreaDataTable(MapTbl);

                                if (VariantBuy == null || VariantMap == null)
                                {
                                    result.Success = false;
                                    result.Message = "Table Type Object can't init";
                                    return result;
                                }
                                var parameters = new DynamicParameters();

                                VariantBuy = ExtensionsNew.ConvertListToDataTable(model.BuyList, VariantBuy);
                                VariantMap = ExtensionsNew.ConvertListToDataTable(model.MapList, VariantMap);
                              
                                parameters.Add("VariantId", model.VariantId);
                                parameters.Add("Description", model.Description);
                                parameters.Add("Status", model.Status);
                                parameters.Add("CreatedBy", model.CreatedBy);
                                parameters.Add("CreatedOn", model.CreatedOn);
                                parameters.Add("ModifiedBy", model.ModifiedBy);
                                parameters.Add("ModifiedOn", model.ModifiedOn);
                                parameters.Add("ValidFrom", model.ValidFrom);
                                parameters.Add("ValidTo", model.ValidTo);
                                parameters.Add("CustomF1", model.CustomF1);
                                parameters.Add("CustomF2", model.CustomF2);
                                parameters.Add("CustomF3", model.CustomF3);
                                parameters.Add("CustomF4", model.CustomF4);
                                parameters.Add("CustomF5", model.CustomF5); 

                                parameters.Add("@LineItemVariantTableBuy", VariantBuy.AsTableValuedParameter(BuyTbl + "TableType"));
                                parameters.Add("@LineItemVariantTableMap", VariantMap.AsTableValuedParameter(MapTbl + "TableType"));

                                Key =  db.ExecuteScalar("USP_I_M_ItemVariant", parameters, commandType: CommandType.StoredProcedure, transaction: tran).ToString();
 
                                result.Success = true;
                                result.Message = Key;
                                tran.Commit();
                                 
                            }
                            catch (Exception ex) {
                                tran.Rollback();
                                throw ex;
                            }
                        }
                    }
                    catch (Exception ex) { 
                        throw ex;  
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

        public async Task<GenericResult> Delete(MItemVariant model)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetAll(DateTime? FromDate, DateTime? ToDate, string Status, string Keyword)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("VariantId", "");
                parameters.Add("Description", Keyword);
                parameters.Add("Status", Status);
                parameters.Add("From", FromDate.HasValue ? FromDate : null);
                parameters.Add("To", ToDate.HasValue ? FromDate : null);

                var data = await _variantRepository.GetAllAsync($"USP_Get_M_ItemVariant", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetByCode(string Code)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _variantRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("VariantId", Code);
                    parameters.Add("Description", "");
                    parameters.Add("Status", "");
                    parameters.Add("From", null);
                    parameters.Add("To", null);
                    var reader = await db.QueryMultipleAsync("USP_Get_M_ItemVariant", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    var HeaderData = reader.Read<MItemVariant>().ToList();
                    var data = new MItemVariant();
                    if (HeaderData != null && HeaderData?.Count > 0)
                    {
                        data = HeaderData.FirstOrDefault();
                        var ListBuy = reader.Read<MItemVariantBuy>().ToList();
                        var ListMap = reader.Read<MItemVariantMap>().ToList();
                        data.BuyList = ListBuy;
                        data.MapList = ListMap;
                    }
                   

                    //var data = await _variantRepository.GetAsync($"USP_Get_M_ItemVariant", parameters, commandType: CommandType.StoredProcedure);
                    //if (data != null)
                    //{

                    //}
                    result.Success = true;
                    result.Data = data;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                }
            } 
            return result;
        }

        
        public async Task<GenericResult> Update(MItemVariant model)
        {
            GenericResult result = new GenericResult();
            //try
            //{
            //    var parameters = new DynamicParameters();
                
            //    parameters.Add("CompanyCode", model.CompanyCode);
            //    parameters.Add("CompanyName", model.CompanyName);
            //    parameters.Add("ForeignName", model.ForeignName);
            //    parameters.Add("ShortName", model.ShortName);
            //    parameters.Add("Logo", model.Logo);
            //    parameters.Add("Address", model.Address);
            //    parameters.Add("Email", model.Email);
            //    parameters.Add("Phone", model.Phone);
            //    parameters.Add("TaxCode", model.TaxCode);
            //    parameters.Add("ModifiedBy", model.ModifiedBy);
            //    parameters.Add("Status", model.Status);
            //    var affectedRows = _variantRepository.Update("USP_U_M_Company", parameters, commandType: CommandType.StoredProcedure);
            //    result.Success = true;
            //    //result.Message = key;
            //}
            //catch (Exception ex)
            //{
            //    result.Success = false;
            //    result.Message = ex.Message;
            //}
            return result;
        }

       
    }

}
