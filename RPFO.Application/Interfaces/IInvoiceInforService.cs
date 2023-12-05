
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
    public interface IInvoiceInforService
    {
        Task<GenericResult> GetAll(string CompanyCode, string CustomerId, string Phone, string Email, string TaxCode); 
        Task<GenericResult> GetByCode(string CompanyCode, string Id); 
        Task<GenericResult> Create(MInvoiceInfor model);
        Task<GenericResult> Update(MInvoiceInfor model);
        //Task<GenericResult> Delete(string Code);

    }
}
