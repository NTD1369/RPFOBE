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
    public class ItemUomController : ControllerBase    
    {

        IItemUomService _itemUomService;
        private readonly ILogger<ItemUomController> _logger;

        public ItemUomController(ILogger<ItemUomController> logger, IItemUomService itemUomService)
        {
            _logger = logger;
            _itemUomService = itemUomService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            return await _itemUomService.GetAll(CompanyCode);
        }
         
        [HttpGet]
        [Route("GetByItem")]
        public async Task<GenericResult> GetByItem(string CompanyCode, string ItemCode)
        {
            return await _itemUomService.GetByItem(CompanyCode, ItemCode);
        }
        [HttpGet]
        [Route("GetByBarcode")]
        public async Task<GenericResult> GetByBarcode(string CompanyCode, string BarCode)
        {
            return await _itemUomService.GetByBarcode(CompanyCode, BarCode);
        }
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _itemUomService.GetPagedList(userParams);
            
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string CompanyCode, string itemCode, string uomCode)
        {
            return await _itemUomService.GetByCode(CompanyCode, itemCode, uomCode);
        }
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MItemUom model)
        {
            return await _itemUomService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MItemUom model)
        {
            return await _itemUomService.Update(model);
        }
        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport model)
        {
            return await _itemUomService.Import(model);
        }
    }
}
