using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RPFO.API.Helpers;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseRequestController : ControllerBase    
    {

        IPurchaseRequestService _PurchaseRequestService;
        private readonly ILogger<PurchaseRequestController> _logger;

        public PurchaseRequestController(IPurchaseRequestService purchaseRequestService, ILogger<PurchaseRequestController> logger)
        {
            _PurchaseRequestService = purchaseRequestService;
            _logger = logger;
        }

        [HttpGet]
        [Route("getNewNum")]
        public async Task<string> getNewNum(string companyCode, string storeId)
        {
            return await _PurchaseRequestService.GetNewOrderCode(companyCode, storeId);
        }
        [HttpGet]
        [Route("GetLastPricePO")]
        public async Task<string> GetLastPricePO(string companyCode, string storeId, string ItemCode, string UomCode, string Barcode)
        {
            return await _PurchaseRequestService.GetLastPricePO(companyCode, storeId, ItemCode, UomCode, Barcode);
        }
        [HttpGet]
        [Route("GetOrderById")]
        public async Task<PurchaseRequestViewModel> GetOrderById(string id, string companycode, string storeid)
        {
            return await _PurchaseRequestService.GetOrderById(id, companycode, storeid);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetByType")]
        
        public async Task<GenericResult> GetByType(string companycode, string storeId, string fromdate, string todate, string key, string status)
        {
            return await _PurchaseRequestService.GetByType( companycode, storeId, fromdate, todate, key, status);
        }
        [HttpPost]
        [Route("SavePO")]
        public async Task<GenericResult> SavePO(PurchaseRequestViewModel model)
        {
            return await _PurchaseRequestService.SavePO(model);
        }
        [HttpPost]
        [Route("UpdateStatus")]
        public async Task<GenericResult> UpdateStatus(PurchaseRequestViewModel model)
        {
            return await _PurchaseRequestService.UpdateStatus(model);
        }
        [HttpPost]
        [Route("UpdateCancel")]
        public async Task<GenericResult> UpdateCancel(PurchaseRequestViewModel model)
        {
            return await _PurchaseRequestService.UpdateCancel(model);
        }
        //[HttpPost]
        //[Route("Update")]
        //public async Task<GenericResult> Update(PurchaseRequestOrderViewModel model)
        //{
        //    return await _PurchaseRequestService.Update(model);
        //}
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _PurchaseRequestService.GetPagedList(userParams);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [AllowAnonymous]
         [HttpPost]
        [Route("GetSalesPeriod")]
        public async Task<GenericResult> GetSalesPeriod(AverageNumberSaleModel model)
        {
            return await _PurchaseRequestService.GetSalesPeriod(model);
        }
    }
}
