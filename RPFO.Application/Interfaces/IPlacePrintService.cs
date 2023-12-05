
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
    public interface ITablePlaceService
    {
        Task<GenericResult> GetAll(string CompanyCode, string StoreId, string ShiftId, string PlaceId, string Keyword,string IsSetup,string IsDesign); 
        Task<GenericResult> GetByCode(string CompanyCode, string StoreId , string ShiftId, string PlaceId, string TableId);
        Task<GenericResult> GetAllTableNoActiveInPlace(string CompanyCode, string StoreId, string ShiftId, string PlaceId, string Keyword);
        Task<GenericResult> Apply(MTablePlace model);
        Task<GenericResult> Create(MTablePlace model);
        Task<GenericResult> Update(MTablePlace model);
        Task<GenericResult> Delete(MTablePlace model);  
    }
}
