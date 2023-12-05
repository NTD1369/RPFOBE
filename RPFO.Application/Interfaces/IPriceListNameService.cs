
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
    public interface IPriceListNameService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<GenericResult> GetById(string CompanyCode, string Id);
        Task<PagedList<MPriceListName>> GetPagedList(UserParams userParams); 
        Task<GenericResult> Create(MPriceListName model);
        Task<GenericResult> Update(MPriceListName model);
        Task<GenericResult> Delete(MPriceListName model); 
    }
}
