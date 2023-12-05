using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
  public  interface ILicensePlateService
    {
        Task<GenericResult> Import(DataImport model);
        Task<GenericResult> CheckLicensePlate(string CompanyCode, string LicensePlate,decimal quantity);
        Task<GenericResult> GetVoucherInfo ( string CompanyCode,string StoreId, string key, string Type );
        Task<GenericResult> Redeem(TSalesRedeemMenber model);
        Task<GenericResult> GetAll(string CompanyCode, string key);
        Task<GenericResult> GetSerialInfo(string CompanyCode, string StoreId, string key);
        Task<GenericResult> GetById(string companyCode, string Id);
        Task<GenericResult> Search(string companyCode, string key);
    }
}
