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
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GoodsReturnController : ControllerBase
    {

        IGoodsReturnService _GoodsReturnService;
        private readonly ILogger<GoodsReturnController> _logger;

        public GoodsReturnController(ILogger<GoodsReturnController> logger, IGoodsReturnService InvoiceService)
        {
            _logger = logger;
            _GoodsReturnService = InvoiceService;
        }

        //[HttpGet]
        //[Route("testDateTime")]
        //public async Task<DateTime> testDateTime(DateTime date)
        //{
        //    return date;
        //}
        [HttpGet]
        [Route("getNewNum")]
        public async Task<string> getNewNum(string companyCode, string storeId)
        {
            return await _GoodsReturnService.GetNewOrderCode(companyCode, storeId);
        }
        [HttpGet]
        [Route("GetOrderById")]
        public async Task<GenericResult> GetOrderById(string id, string companycode, string storeid)
        {

            var data = await _GoodsReturnService.GetOrderById(id, companycode, storeid);
            return data;



        }
        [HttpGet]
        [Route("GetByType")]

        public async Task<GenericResult> GetByType(string companycode, string storeId, string fromdate, string todate, string key, string status)
        {

            return await _GoodsReturnService.GetByType(companycode, storeId, fromdate, todate, key, status);
        }
        [HttpPost]
        [Route("CreateInvoice")]
        public async Task<GenericResult> CreateInvoice(GReturnPOViewModel model)
        {
            return await _GoodsReturnService.Create(model);
        }

        [HttpPost]
        [Route("UpdateStatus")]
        public async Task<GenericResult> UpdateStatus(GReturnPOViewModel model)
        {
            return await _GoodsReturnService.UpdateStatus(model);
        }
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _GoodsReturnService.GetPagedList(userParams);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
    }
}
