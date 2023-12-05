
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
    public interface IPlaceInforService
    {
        Task<MPlaceInfor> GetMPlaceInfor(string companyCode, string storeId, string placeName);
        Task<GenericResult> GetAll(string CompanyCode, string StoreId, string Keyword);
        Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string PlaceId);
        Task<GenericResult> Create(MPlaceInfor model);
        Task<GenericResult> Update(MPlaceInfor model);
        Task<GenericResult> Delete(MPlaceInfor model);
    }
}
