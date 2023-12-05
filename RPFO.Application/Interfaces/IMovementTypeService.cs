
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
    public interface IMovementTypeService
    {
        Task<GenericResult> GetAll();
        Task<GenericResult> GetByCode(string Code);
        Task<GenericResult> Create(MMovementType model);
        Task<GenericResult> Update(MMovementType model);
        Task<GenericResult> Delete(string Code);
      
    }
}
