
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
    public interface IDeliveryInforService
    {
        Task<GenericResult> GetAll(string CompanyCode, string CustomerId, string Phone, string Email, string TaxCode); 
        Task<GenericResult> GetByCode(string CompanyCode, string Id);
        Task<GenericResult> GetDefault(string CompanyCode);
        Task<GenericResult> Create(MDeliveryInfor model);
        Task<GenericResult> Update(MDeliveryInfor model);
        //Task<GenericResult> Delete(string Code);

    }
}
