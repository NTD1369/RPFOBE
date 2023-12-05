
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
    public interface IItemVariantService
    {
        Task<GenericResult> GetAll(DateTime? FromDate, DateTime? ToDate, string Status, string Keyword); 
        Task<GenericResult> GetByCode(string Code);
        Task<GenericResult> Create(MItemVariant model);
        Task<GenericResult> Update(MItemVariant model);
        Task<GenericResult> Delete(MItemVariant model);
        //Task<GenericResult> Import(DataImport model);
    }
}
