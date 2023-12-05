
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
    public interface IUomService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<PagedList<MUom>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string CompanyCode, string Code);
        Task<GenericResult> GetByItem(string Item);  
        Task<GenericResult> Create(MUom model);
        Task<GenericResult> Update(MUom model);
        Task<GenericResult> Import(DataImport model);
    }
}
