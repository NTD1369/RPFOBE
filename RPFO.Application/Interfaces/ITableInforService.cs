
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
    public interface ITableInforService
    {
        Task<GenericResult> GetAll(string CompanyCode, string StoreId, string Keyword);
        Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string TableId);
        Task<TablePlaceViewModel> GetTableAndPlaceById(string companyCode, string store, string tableId, string placeId);
        Task<string> GetTableId(string companyCode, string storeId, string tableName);
        Task<MTableInfor> GetMTableInfor(string companyCode, string storeId, string tableName);

        Task<GenericResult> Create(MTableInfor model);
        Task<GenericResult> Update(MTableInfor model);
        Task<GenericResult> Delete(MTableInfor model);
        Task<GenericResult> Import(DataImport models);
    }
}
