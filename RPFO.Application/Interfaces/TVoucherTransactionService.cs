
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
    public interface IVoucherTransactionService
    {
        Task<GenericResult> GetAll(string CompanyCode); 
        Task<GenericResult> GetByCode(string CompanyCode, string ItemCode, string VoucherNo, string VoucherType); 
        Task<GenericResult> Create(TVoucherTransaction model);
        Task<GenericResult> Update(TVoucherTransaction model);
        //Task<GenericResult> Import(DataImport model);
    }
}
