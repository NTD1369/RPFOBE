
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
    public interface IItemRejectPaymentService
    {
        Task<GenericResult> GetAll(string CompanyCode, string ItemCode, string Status); 
        Task<GenericResult> Create(MItemRejectPayment model);
        Task<GenericResult> Update(MItemRejectPayment model);
        Task<GenericResult> Delete(MItemRejectPayment model);
        //Task<GenericResult> Import(DataImport model);
    }
}
