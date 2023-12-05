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
    public class DeliveryOrderController : ControllerBase    
    {

        IDeliveryOrderService _DeliveryService;
        private readonly ILogger<DeliveryOrderController> _logger;

        public DeliveryOrderController(ILogger<DeliveryOrderController> logger, IDeliveryOrderService DeliveryService)
        {
            _logger = logger;
            _DeliveryService = DeliveryService;
        }

      
        [HttpGet]
        [Route("getNewNum")]
        public async Task<GenericResult> getNewNum(string companyCode, string storeId)
        {
            return await _DeliveryService.GetNewOrderCode(companyCode, storeId);
        }  
        [AllowAnonymous]
        [HttpGet]
        [Route("CreateByDate")]
        public async Task<GenericResult> CreateByDate(string CompanyCode, string Date, string CreatedBy)
        {
            //Task<GenericResult> CreateByDate(string CompanyCode, string Date, string CreatedBy)
            return await _DeliveryService.CreateByDate(CompanyCode,  Date,  CreatedBy);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string companycode, string storeid, string id)
        { 
            var data = await _DeliveryService.GetById(companycode, storeid, id);
            return data;
            
        }
        [HttpGet]
        [Route("GetByType")]

        public async Task<GenericResult> GetByType(string companyCode, string storeId, string fromdate, string todate, string TransId,
            string DeliveryBy, string key, string status, string ViewBy)
        {
            return await _DeliveryService.GetByType(companyCode, storeId, fromdate, todate, TransId,
             DeliveryBy, key, status,  ViewBy);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(TDeliveryHeader model)
        {
            return await _DeliveryService.Create(model);
        } 

        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(TDeliveryHeader model)
        {
            return await _DeliveryService.Update(model);
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string CompanyCode, string StoreId, string Id)
        {
            return await _DeliveryService.Delete(CompanyCode, StoreId, Id);
        }

        //[HttpGet]
        //[Route("GetPagedList")]
        //public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        //{
        //    var data = await _DeliveryService.GetPagedList(userParams);
        //    Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
        //    return Ok(data);
        //}
    }
}
