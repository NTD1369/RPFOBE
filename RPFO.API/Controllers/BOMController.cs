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
    public class BOMController : ControllerBase    
    {

        IBOMService _BOMService;
        private readonly ILogger<BOMController> _logger;

        public BOMController(ILogger<BOMController> logger, IBOMService BOMService)
        {
            _logger = logger;
            _BOMService = BOMService;
        }

        [HttpGet]
        [Route("GetByItemCode")]
        public async Task<GenericResult> GetByItemCode(string CompanyCode, string ItemCode)
        {
            return await _BOMService.GetByItemCode(CompanyCode, ItemCode);
        }         

        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _BOMService.GetPagedList(userParams);

            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(BOMViewModel model)
        {
            return await _BOMService.Create(model);
        }

        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(BOMDataImport models)
        {
            return await _BOMService.BOMImport(models);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(BOMViewModel model)
        {
            return await _BOMService.Update(model);
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string CompanyCode, string ItemCode)
        {
            return await _BOMService.Delete(CompanyCode,ItemCode);
        }

        [HttpPost]
        [Route("CreateLine")]
        public async Task<GenericResult> CreateLine(MBomline model)
        {
            return await _BOMService.CreateLine(model);
        }

        [HttpPut]
        [Route("UpdateLine")]
        public async Task<GenericResult> UpdateLine(MBomline model)
        {
            return await _BOMService.UpdateLine(model);
        }

        [HttpDelete]
        [Route("DeleteLine")]
        public async Task<GenericResult> DeleteLine(string CompanyCode, string BomId, string Id)
        {
            return await _BOMService.DeleteLine(Id, CompanyCode, BomId);
        }
    }
}
