
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
    public interface ISaleTypeService
    {
        Task<List<SSalesType>> GetAll(); 
        Task<SSalesType> GetByCode(string Code); 
        Task<GenericResult> Create(SSalesType model);
        Task<GenericResult> Update(SSalesType model); 
    }
}
