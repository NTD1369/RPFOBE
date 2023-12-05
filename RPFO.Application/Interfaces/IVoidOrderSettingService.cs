
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
    public interface IVoidOrderSettingService
    {
        Task<GenericResult> GetAll(); 
        Task<GenericResult> GetByCode(string Type, string Code); 
        Task<GenericResult> Create(SVoidOrderSetting model);
        Task<GenericResult> Update(SVoidOrderSetting model);
        Task<GenericResult> Delete(string Code);
       
    }
}
