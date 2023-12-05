
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
    public interface IPriorityPriceListService
    {
        Task<GenericResult> GetAll(string CompanyCode, string CusGrpId, string PriceListId); 
        Task<GenericResult> GetByCode(string CompanyCode, string Code); 
        Task<GenericResult> Create(MPriorityPriceList model);
        Task<GenericResult> Update(MPriorityPriceList model);
        Task<GenericResult> Delete(MPriorityPriceList model);
        //Task<GenericResult> Import(DataImport model);
    }
}
