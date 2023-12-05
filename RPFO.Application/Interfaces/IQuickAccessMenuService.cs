
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IQuickAccessMenuService
    {
        Task<List<SQuickAccessMenu>> GetAll();
        Task<PagedList<SQuickAccessMenu>> GetPagedList(UserParams userParams);
        Task<GenericResult> ReOrder(string SourceId, string DesId);
        Task<SQuickAccessMenu> GetByCode(string Code);
        Task<List<SQuickAccessMenu>> GetByUser(string User); 
        Task<GenericResult> Create(SQuickAccessMenu model);
        Task<GenericResult> Update(SQuickAccessMenu model);
        Task<GenericResult> Delete(string Code);
    }
}
