using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RPFO.API.Helpers;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
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
    public class InventoryController : ControllerBase    
    {

        IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(ILogger<InventoryController> logger, IInventoryService inventoryService)
        {
            _logger = logger;
            _inventoryService = inventoryService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string companycode)
        {
            return await _inventoryService.GetAll(companycode);
        }
        [HttpGet]
        [Route("GetInventoryList")]
        public async Task<GenericResult> GetInventoryList(string CompanyCode, string FromStore, string ToStore, string DocType, string Status, DateTime? FrDate,
            DateTime? ToDate, string Keyword, string ViewBy)
        {
            return await _inventoryService.GetInventoryList(CompanyCode, FromStore, ToStore, DocType, Status, FrDate, ToDate, Keyword, ViewBy);
        }
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _inventoryService.GetPagedList(userParams);
            
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string companycode,string storeid,string id)
        {
            return await _inventoryService.GetById(companycode, storeid, id);
        }
         
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(InventoryViewModel model)
        {
            return await _inventoryService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(InventoryViewModel model)
        {
            return await _inventoryService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string companycode, string storeid, string id)
        {
            return await _inventoryService.Delete(companycode, storeid, id); 
        }
        //[AllowAnonymous]
        [HttpGet]
        [Route("GetTranferNotify")]
        public async Task<GenericResult> GetTranferNotify(string CompanyCode, string StoreId)
        {
            return await _inventoryService.GetTranferNotify(CompanyCode, StoreId);
        }
        //[AllowAnonymous]
        [HttpPost]
        [Route("CheckitemImport")]
        public async Task<GenericResult> CheckitemImport(List<ItemModel> models)
        {
           return await _inventoryService.CheckitemImport(models);
        }
        [HttpPost]
        [Route("Cancel")]
        public async Task<GenericResult> Cancel(InventoryViewModel model)
        {
            return await _inventoryService.Cancel(model);
        }
    }
}
