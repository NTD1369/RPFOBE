
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IItemUomService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<PagedList<MItemUom>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string CompanyCode, string itemCode, string uomCode);
        Task<GenericResult> GetByBarcode(string CompanyCode, string barCode);
        Task<GenericResult> GetByItem(string CompanyCode, string Item);  
        Task<GenericResult> Create(MItemUom model);
        Task<GenericResult> Update(MItemUom model);
        Task<GenericResult> Import(DataImport model);
    }
}
