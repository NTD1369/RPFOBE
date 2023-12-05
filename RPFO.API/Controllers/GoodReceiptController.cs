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
    public class GoodReceiptController : ControllerBase    
    {

        IGoodReceiptService _goodreceiptService;
        private readonly ILogger<GoodReceiptController> _logger;

        public GoodReceiptController(ILogger<GoodReceiptController> logger, IGoodReceiptService goodreceiptService)
        {
            _logger = logger;
            _goodreceiptService = goodreceiptService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string companycode)
        {
            return await _goodreceiptService.GetAll(companycode);
        }
        [HttpGet]
        [Route("GetByStore")]
        public async Task<GenericResult> GetByStore(string companycode, string storeid)
        {
            return await _goodreceiptService.GetByStore(companycode, storeid);
        }

        [HttpGet]
        [Route("GetGoodsReceiptList")]
        public async Task<GenericResult> GetGoodsReceiptList(string CompanyCode, string StoreId, string Status, DateTime? FrDate, DateTime? ToDate, string Keyword, string ViewBy)
        {
            return await _goodreceiptService.GetGoodsReceiptList(CompanyCode, StoreId, Status, FrDate, ToDate, Keyword, ViewBy);
        }

        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _goodreceiptService.GetPagedList(userParams);
            
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string companycode,string storeid,string id)
        {
            return await _goodreceiptService.GetById(companycode, storeid, id);
        }
         
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(GoodReceiptViewModel model)
        {
            return await _goodreceiptService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(GoodReceiptViewModel model)
        {
            return await _goodreceiptService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string companycode, string storeid, string id)
        {
            return await _goodreceiptService.Delete(companycode, storeid, id); 
        }

        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport models)
        {
            return await _goodreceiptService.Import(models);
        }
    }
}
