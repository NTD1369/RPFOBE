
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
    public interface IItemSerialStockService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<PagedList<MItemSerialStock>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetBySlocItem(string CompanyCode, string Sloc, string ItemCode);
        Task<GenericResult> GetByItem(string CompanyCode, string ItemCode);
        Task<GenericResult> Create(MItemSerialStock model);
        Task<GenericResult> Update(MItemSerialStock model);  
        Task<GenericResult> Delete(MItemSerialStock model);
        Task<GenericResult> Import(DataImport model);
    }
}
