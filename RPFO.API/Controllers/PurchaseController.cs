using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RPFO.API.Helpers;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.ViewModels;
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
    public class PurchaseController : ControllerBase    
    {

        IPurchaseService _PurchaseService;
        private readonly ILogger<PurchaseController> _logger;

        public PurchaseController(ILogger<PurchaseController> logger, IPurchaseService InvoiceService)
        {
            _logger = logger;
            _PurchaseService = InvoiceService;
        }

       [HttpGet]
        [Route("getNewNum")]
        public async Task<string> getNewNum(string companyCode, string storeId)
        {
            return await _PurchaseService.GetNewOrderCode(companyCode, storeId);
        }
        [HttpGet]
        [Route("GetLastPricePO")]
        public async Task<string> GetLastPricePO(string companyCode, string storeId, string ItemCode, string UomCode, string Barcode)
        {
            return await _PurchaseService.GetLastPricePO(companyCode, storeId, ItemCode, UomCode, Barcode);
    }
        [HttpGet]
        [Route("GetOrderById")]
        public async Task<PurchaseOrderViewModel> GetOrderById(string id, string companycode, string storeid)
        {
            return await _PurchaseService.GetOrderById(id, companycode, storeid);
        }
        [HttpGet]
        [Route("GetByType")]
        
        public async Task<GenericResult> GetByType(string companycode, string storeId, string fromdate, string todate, string key, string status)
        {
            return await _PurchaseService.GetByType( companycode, storeId, fromdate, todate, key, status);
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("SavePO")]
        public async Task<GenericResult> SavePO(PurchaseOrderViewModel model)
        {
            return await _PurchaseService.SavePO(model);
        }
        [HttpPost]
        [Route("UpdateStatus")]
        public async Task<GenericResult> UpdateStatus(PurchaseOrderViewModel model)
        {
            return await _PurchaseService.UpdateStatus(model);
        }
        //[HttpPost]
        //[Route("Update")]
        //public async Task<GenericResult> Update(PurchaseOrderViewModel model)
        //{
        //    return await _PurchaseService.Update(model);
        //}
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _PurchaseService.GetPagedList(userParams);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
    }
}
