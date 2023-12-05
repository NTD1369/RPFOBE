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
    public class GRPOController : ControllerBase    
    {

        IGRPOService _GRPOService;
        private readonly ILogger<GRPOController> _logger;

        public GRPOController(ILogger<GRPOController> logger, IGRPOService InvoiceService)
        {
            _logger = logger;
            _GRPOService = InvoiceService;
        }

        [HttpGet]
        [Route("testDateTime")]
        public async Task<DateTime> testDateTime(DateTime date)
        {
            return date;
        }
        [HttpGet]
        [Route("getNewNum")]
        public async Task<string> getNewNum(string companyCode, string storeId)
        {
            return await _GRPOService.GetNewOrderCode(companyCode, storeId);
        }
        [HttpGet]
        [Route("GetOrderById")]
        public async Task<GenericResult> GetOrderById(string id, string companycode, string storeid)
        {
             
                var data = await _GRPOService.GetOrderById(id, companycode, storeid);
                return data;
           

           
        }
        [HttpGet]
        [Route("GetByType")]
        
        public async Task<GenericResult> GetByType(string companycode, string storeId, string fromdate, string todate, string key, string status)
        {
            return await _GRPOService.GetByType( companycode, storeId, fromdate, todate, key, status);
        }
        [HttpPost]
        [Route("CreateInvoice")]
        public async Task<GenericResult> CreateInvoice(GRPOViewModel model)
        {
            return await _GRPOService.Create(model);
        }

        [HttpPost]
        [Route("UpdateStatus")]
        public async Task<GenericResult> UpdateStatus(GRPOViewModel model)
        {
            return await _GRPOService.UpdateStatus(model);
        }
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _GRPOService.GetPagedList(userParams);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
    }
}
