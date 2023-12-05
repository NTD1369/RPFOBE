
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IBOMService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<PagedList<BOMViewModel>> GetPagedList(UserParams userParams); 
        Task<GenericResult> GetByItemCode(string CompanyCode, string Code); 
        Task<GenericResult> Create(BOMViewModel model);
        Task<GenericResult> BOMImport(BOMDataImport model);
        Task<GenericResult> Update(BOMViewModel model);
        Task<GenericResult> Delete(string CompanyCode, string Code);
        Task<GenericResult> DeleteLine(string Id, string CompanyCode, string BomId);
        Task<GenericResult> UpdateLine(MBomline line);
        Task<GenericResult> CreateLine(MBomline line);
    }
}
