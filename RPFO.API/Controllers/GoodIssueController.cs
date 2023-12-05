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
    public class GoodIssueController : ControllerBase    
    {

        IGoodIssueService _goodIssueService;
        private readonly ILogger<GoodIssueController> _logger;

        public GoodIssueController(ILogger<GoodIssueController> logger, IGoodIssueService goodIssueService)
        {
            _logger = logger;
            _goodIssueService = goodIssueService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            return await _goodIssueService.GetAll(CompanyCode);
        }
        [HttpGet]
        [Route("GetByStore")]
        public async Task<GenericResult> GetByStore(string companycode, string storeid)
        {
            return await _goodIssueService.GetByStore(companycode, storeid);
        }
        [HttpGet]
        [Route("GetGoodsIssueList")]
        public async Task<GenericResult> GetGoodsIssueList(string CompanyCode, string StoreId, string Status, DateTime? FrDate, DateTime? ToDate, string Keyword, string ViewBy)
        {
            return await _goodIssueService.GetGoodsIssueList(CompanyCode, StoreId, Status, FrDate, ToDate, Keyword, ViewBy);
        }
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _goodIssueService.GetPagedList(userParams);
            
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string companycode,string storeid,string id)
        {
            return await _goodIssueService.GetById(companycode, storeid, id);
        }
         
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(GoodsIssueViewModel model)
        {
            return await _goodIssueService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(GoodsIssueViewModel model)
        {
            return await _goodIssueService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string companycode, string storeid, string id)
        {
            return await _goodIssueService.Delete(companycode, storeid, id); 
        }

    }
}
