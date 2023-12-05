
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface ICommonService
    {
        GenericResult InitService(string ServiceName, List<string> TableNameList);
        DataTable CreaDataTable(string TableName);
        Task<GenericResult> GetCountries(string Area);
        Task<GenericResult> GetQuery(QueryModel model);
        GenericResult ClearRedisCache(string Key, string Prefix);
        Task<GenericResult> GetRegion();
        Task<GenericResult> GetArea();
        Task<GenericResult> GetPOSVersion(string CompanyCode, string Version);
        Task<GenericResult> GetMaxValueCurrency(string Currency);
        Task<GenericResult> GetProvinceList();
        Task<GenericResult> GetCurrencyList();
        Task<GenericResult> GetConfigType();
        Task<GenericResult> GetItemCollection();
        Task<GenericResult> GetDailyId(string CompanyCode, string StoreId, DateTime? Date);
        Task<GenericResult> GetPOSOption(string Type);
        Task<GenericResult> OpenDrawerCash(string Name, string BillNo);
        Task<GenericResult> PageCut(bool? IsFull, string Name);
       
        Task<GenericResult> GetItemCustomList(string Field);
        Task<GenericResult> GetPOSType();
        GenericResult GetLicenseInfor(string CompanyCode, string License);
        Task<GenericResult> PrintByPDF(string CompanyCode, string StoreId, string pdfFileName, string PrintName, string PrintSize, string PrintType);
        GenericResult InitDb(ObjectInitNewData model);
        GenericResult FirstInitDB(ObjectInitNewData model, string customStr); 
    }
}
