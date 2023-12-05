using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using RPFO.Data.ViewModel;

namespace RPFO.API.Controllers
{
    public partial class SaleController : ControllerBase
    {
        [HttpGet]
        [Route("GetOrderByContractNo")]
        public GenericResult GetOrderByContractNo(string Companycode, string StoreId, string ContractNo, string ShiftId, string PlateId)
        {
            return _saleService.GetOrderByContractNo(Companycode, StoreId, ContractNo, ShiftId, PlateId);
        }
        
        [HttpGet]
        [Route("MoveTable")]
        public async Task<GenericResult> MoveTable(string CompanyCode, string StoreId, string PlaceId, string FromTable, string ToTable, string TransIdList, string Itemcode, decimal? Quantity)
        {
            return await _saleService.MoveTable(CompanyCode, StoreId, PlaceId, FromTable, ToTable, TransIdList);
        }

        [HttpPost]
        [Route("MergeTable")]
        public async Task<GenericResult> MergeTable(string CompanyCode, string StoreId, string ShiftId, string PlaceId, string CreatedBy, List<string> TableList, string ToTable, bool? ClearTable)
        {
            return await _saleService.MergeTable(CompanyCode, StoreId, ShiftId, PlaceId, CreatedBy, TableList, ToTable, ClearTable);
        }

        [HttpPost]
        [Route("SplitTable")]
        public async Task<GenericResult> SplitTable(SplitTableViewModel model)
        {
            return await _saleService.SplitTable(model.CompanyCode, model.StoreId, model.ShiftId, model.PlaceId, model.TableId, model.GroupKey);
        }

        [HttpGet]
        [Route("GetContractItem")]
        public GenericResult GetContractItem(string Companycode, string StoreId, string PlaceId, string ContractNo, string TransId, string ShiftId)
        {
            return _saleService.GetContractItem(Companycode, StoreId, PlaceId, ContractNo, TransId, ShiftId).Result;
        }
    }
}