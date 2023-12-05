
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
    public partial interface ISaleService
    {
        GenericResult GetOrderByContractNo(string CompanyCode, string StoreId, string ContractNo, string ShiftId, string PlateId);
        Task<GenericResult> CreateSaleOrderByTableType(SaleViewModel model);
        Task<string> CheckOrderInGroupTable(SaleViewModel model);
        Task UpdatePaymentForTable(SaleViewModel model);
        Task<GenericResult> MoveTable(string CompanyCode, string StoreId, string PlaceId, string FromTable, string ToTable, string TransIdList);
        Task<GenericResult> MergeTable(string CompanyCode, string StoreId, string ShiftId, string PlaceId, string CreatedBy, List<string> TableList, string ToTable, bool? ClearTable);
        Task<GenericResult> SplitTable(string CompanyCode, string StoreId, string ShiftId, string PlaceId, string TableId, string groupKey);
        Task<GenericResult> CancelTableExclude(string CompanyCode, string StoreId, string TableId, string PlaceId, string TransId);
        Task<GenericResult> PrintByTypeAsync(SaleViewModel model, string companyCode, string storeId, TablePlaceViewModel tablePlace, string size, string printName);
        Task<GenericResult> GroupItemByPrint(List<string> lines, string companyCode, string storeId);
        Task AutoPrintSetting(SaleViewModel model);
    }
}
