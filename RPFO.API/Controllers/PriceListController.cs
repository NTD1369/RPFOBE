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
    public class PriceListController : ControllerBase    
    {

        IPriceListService _priceListService;
        private readonly ILogger<PriceListController> _logger;

        public PriceListController(ILogger<PriceListController> logger, IPriceListService priceListService)
        {
            _logger = logger;
            _priceListService = priceListService;
        }
        [Cached(600)]
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, string ItemCode)
        {
            return await _priceListService.GetAll(CompanyCode, StoreId, ItemCode);
        }
        [HttpGet]
        [Route("GetAllId")]
        public async Task<GenericResult> GetAllId(string CompanyCode)
        {
            return await _priceListService.GetAllId(CompanyCode);
        }

        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _priceListService.GetPagedList(userParams);
            
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string id)
        {
            return await _priceListService.GetByCode(CompanyCode, id);
        }
        [HttpGet]
        [Route("GetByMer")]
        public async Task<IActionResult> GetByMer([FromQuery] UserParams userParams)
        {
            var data = await _priceListService.GetPagedList(userParams);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport model)
        {
            return await _priceListService.Import(model);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MPriceList model)
        {
            return await _priceListService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MPriceList model)
        {
            return await _priceListService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string Id)
        {
            return await _priceListService.Delete(Id);
        }




    }
}
