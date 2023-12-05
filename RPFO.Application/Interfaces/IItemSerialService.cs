
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
    public interface IItemSerialService
    {
        Task<GenericResult> GetAll(string CompanyCode, string StoreId, string SlocId, string ItemCode, string Keyword,int? Selecttop);
        Task<PagedList<MItemSerial>> GetPagedList(UserParams userParams);  
        Task<List<MItemSerial>> GetByItem(string CompanyCode, string StoreId, string ItemCode);  
        Task<GenericResult> Create(MItemSerial model);
        Task<GenericResult> Update(MItemSerial model);
        Task<GenericResult> UpdateWithStock(List<MItemSerial> model);
        Task<GenericResult> Import(DataImport model);
        Task<GenericResult> Delete(List<MItemSerial> models);
        Task<GenericResult> GenerateSerial(string CompanyCode, string StoreId, DateTime? ExpDate, string By, string Prefix, string ItemCode, int NumOfGen, int? RandomNumberLen, int? RuningNumberLen);
    }
}
