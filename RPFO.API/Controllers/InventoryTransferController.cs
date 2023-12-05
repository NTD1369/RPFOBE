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
    public class InventoryTransferController : ControllerBase    
    {

        IInventoryTransferService _inventoryTransferService;
        private readonly ILogger<InventoryTransferController> _logger;

        public InventoryTransferController(ILogger<InventoryTransferController> logger, IInventoryTransferService inventoryTransferService)
        {
            _logger = logger;
            _inventoryTransferService = inventoryTransferService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string companycode)
        {
            return await _inventoryTransferService.GetAll(companycode);
        }
        [HttpGet]
        [Route("GetInventoryList")]
        public async Task<GenericResult> GetInventoryList(string CompanyCode,string StoreId, string FromSloc, string ToSloc, string DocType, string Status, DateTime? FrDate,
            DateTime? ToDate, string Keyword, string ViewBy)
        {
            return await _inventoryTransferService.GetInventoryList(CompanyCode, StoreId, FromSloc, ToSloc, DocType, Status, FrDate, ToDate, Keyword, ViewBy);
        }
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _inventoryTransferService.GetPagedList(userParams);

            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string companycode, string storeid, string id)
        {
            return await _inventoryTransferService.GetById(companycode, storeid, id);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(InventoryTransferViewModel model)
        {
            return await _inventoryTransferService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(InventoryTransferViewModel model)
        {
            return await _inventoryTransferService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string companycode, string storeid, string id)
        {
            return await _inventoryTransferService.Delete(companycode, storeid, id);
        }

    }
}
