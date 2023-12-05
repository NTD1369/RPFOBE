
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IStoreClientService
    {
        Task<GenericResult> GetAll(string CompanyCode, string StoreId, DateTime? From, DateTime? To); 
        Task<GenericResult> GetById(string CompanyCode, string StoreId, string Id, string LocalIP, string PublicIP);

        Task<GenericResult>  GetCounterSalesInDay(string CompanyCode, string StoreId, DateTime? Date);
        //Task<GenericResult> LogoUpdate(string CompanyCode, string Url);
        Task<GenericResult> Create(SStoreClient model);
        Task<GenericResult> Update(SStoreClient model);
        Task<GenericResult> Delete(SStoreClient model);


        Task<GenericResult> UpdateByPublicId(SStoreClient model);
        //Task<GenericResult> Import(DataImport model);
    }
}
