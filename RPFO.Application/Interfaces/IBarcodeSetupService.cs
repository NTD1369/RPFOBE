
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
    public interface IBarcodeSetupService
    {
        Task<GenericResult> GetAll(string CompanyCode, string Keyword); 
        Task<GenericResult> GetById(string CompanyCode, string Id); 
        Task<GenericResult> Create(SBarcodeSetup model);
        Task<GenericResult> Update(SBarcodeSetup model);
        Task<GenericResult> Delete(SBarcodeSetup model); 
      
    }
}
