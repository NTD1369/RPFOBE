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
    public class ItemSerialController : ControllerBase    
    {

        IItemSerialService _itemSerialService;
        private readonly ILogger<ItemUomController> _logger;

        public ItemSerialController(ILogger<ItemUomController> logger, IItemSerialService itemSerialService)
        {
            _logger = logger;
            _itemSerialService = itemSerialService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, string SlocId, string ItemCode, string Keyword,int? Selecttop)
        {
            return await _itemSerialService.GetAll(CompanyCode, StoreId, SlocId, ItemCode, Keyword,Selecttop);
        }
        //Task<GenericResult> GenerateSerial(string CompanyCode, string StoreId, DateTime? ExpDate, 
        //    string By, string Prefix, string ItemCode, int NumOfGen, int? RandomNumberLen, int? RuningNumberLen)
        [HttpGet]
        [Route("GetByItem")]
        public async Task<IEnumerable<MItemSerial>> GetByItem(string CompanyCode, string StoreId,  string ItemCode)
        {
            return await _itemSerialService.GetByItem(CompanyCode, StoreId, ItemCode);
        }
       

        [HttpGet]
        [Route("GenerateSerial")]
        public async Task<GenericResult> GenerateSerial(string CompanyCode, string StoreId, DateTime? ExpDate,
          string By, string Prefix, string ItemCode, int NumOfGen, int? RandomNumberLen, int? RuningNumberLen)
        {
            return await _itemSerialService.GenerateSerial(CompanyCode, StoreId, ExpDate, By,  Prefix,  ItemCode,  NumOfGen,  RandomNumberLen, RuningNumberLen);
        }


        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _itemSerialService.GetPagedList(userParams);
            
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        } 
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MItemSerial model)
        {
            return await _itemSerialService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MItemSerial model)
        {
            return await _itemSerialService.Update(model);
        }

        [HttpPut]
        [Route("UpdateWithStock")]
        public async Task<GenericResult> UpdateWithStock(List<MItemSerial> model)
        {
            return await _itemSerialService.UpdateWithStock(model);
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(List<MItemSerial> models)
        {
            return await _itemSerialService.Delete(models);
        }

        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport model)
        {
            return await _itemSerialService.Import(model);
        }
    }
}
