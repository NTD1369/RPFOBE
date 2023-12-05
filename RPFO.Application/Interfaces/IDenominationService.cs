
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
    public interface IDenominationService
    {
        Task<GenericResult> GetAll(string CurrencyCode);
        //Task<PagedList<MDenomination>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string Code);  
        Task<GenericResult> Create(MDenomination model);
        Task<GenericResult> Update(MDenomination model);
        Task<GenericResult> Delete(string Code);
         
    }
}
