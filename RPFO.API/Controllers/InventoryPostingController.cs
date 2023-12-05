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
    public class InventoryPostingController : ControllerBase    
    {

        IInventoryPostingService _inventoryService;
        private readonly ILogger<InventoryPostingController> _logger;

        public InventoryPostingController(ILogger<InventoryPostingController> logger, IInventoryPostingService inventoryService)
        {
            _logger = logger;
            _inventoryService = inventoryService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string companyCode)
        {
            return await _inventoryService.GetAll(companyCode);
        }
        [HttpGet]
        [Route("GetByStore")]
        public async Task<GenericResult> GetByStore(string companyCode, string storeId)
        {
            return await _inventoryService.GetByStore(companyCode, storeId);
        }
        [HttpGet]
        [Route("GetInventoryList")]
        public async Task<GenericResult> GetInventoryList(string CompanyCode, string StoreId, string Status, DateTime? FrDate, DateTime? ToDate, string Keyword, string ViewBy)
        {
            return await _inventoryService.GetInventoryList(CompanyCode, StoreId, Status, FrDate, ToDate, Keyword, ViewBy);
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
        public async Task<GenericResult> Create(InventoryPostingViewModel model)
        {
            return await _inventoryService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(InventoryPostingViewModel model)
        {
            return await _inventoryService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string companycode, string id)
        {
            return await _inventoryService.Delete(companycode,  id); 
        }

    }
}
