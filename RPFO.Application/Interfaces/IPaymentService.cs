
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
    public interface IPaymentService
    {
        Task<GenericResult> GetAll(string CompanyCode, string CusId, string FromDate, string ToDate, string Status, string top); 
        Task<GenericResult> GetByCode(string CompanyCode, string Id);
        //Task<GenericResult> LogoUpdate(string CompanyCode, string Url);
        Task<GenericResult> Create(TPaymentHeader model);
        Task<GenericResult> Update(TPaymentHeader model);
        Task<GenericResult> Delete(TPaymentHeader model);
        //Task<GenericResult> Import(DataImport model);
    }
}
