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
    public class ItemSerialStockController : ControllerBase    
    {

        IItemSerialStockService _itemSerialStockService;
        private readonly ILogger<ItemSerialStockController> _logger;

        public ItemSerialStockController(ILogger<ItemSerialStockController> logger, IItemSerialStockService itemSerialStockService)
        {
            _logger = logger;
            _itemSerialStockService = itemSerialStockService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            return await _itemSerialStockService.GetAll(CompanyCode);
        }

        [HttpGet]
        [Route("GetBySlocItem")]
        public async Task<GenericResult> GetBySlocItem(string CompanyCode, string Sloc, string ItemCode)
        {
            return await _itemSerialStockService.GetBySlocItem(CompanyCode, Sloc, ItemCode);
        }
        [HttpGet]
        [Route("GetByItem")]
        public async Task<GenericResult> GetByItem(string CompanyCode, string ItemCode)
        {
            return await _itemSerialStockService.GetByItem(CompanyCode, ItemCode);
        }

        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _itemSerialStockService.GetPagedList(userParams);
            
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        } 
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MItemSerialStock model)
        {
            return await _itemSerialStockService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MItemSerialStock model)
        {
            return await _itemSerialStockService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(MItemSerialStock model)
        {
            return await _itemSerialStockService.Update(model);
        }
        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport model)
        {
            return await _itemSerialStockService.Import(model);
        }
    }
}
