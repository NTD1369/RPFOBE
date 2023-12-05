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
    public class MerchandiseController : ControllerBase    
    {

        IMerchandiseCategoryService  _merchandisService;
        private readonly ILogger<MerchandiseController> _logger;

        public MerchandiseController(ILogger<MerchandiseController> logger, IMerchandiseCategoryService merchandisService)
        {
            _logger = logger;
            _merchandisService = merchandisService;
        }
        [Cached(600)]
        [HttpGet]
        [Route("GetByCompany")]
        public async Task<GenericResult> GetByCompany(string companyCode, string mcid, string status, string keyword)
        {
            
              return await _merchandisService.GetByCompany(companyCode, mcid, status, keyword);
            
        }
        [Cached(600)]
        [HttpGet]
        [Route("GetMerchandiseCategoryShow")]
        public async Task<GenericResult> GetMerchandiseCategoryShow(string companyCode, string mcid, string status, string keyword)
        {
            return await _merchandisService.GetMerchandiseCategoryShow(companyCode, mcid, status, keyword);
               
        }
        [Cached(600)]
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string companyCode)
        {
            return await _merchandisService.GetAll(companyCode);
        }

        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _merchandisService.GetPagedList(userParams);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }

        
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string companyCode, string id)
        {
            return await _merchandisService.GetByCode(companyCode, id);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MMerchandiseCategory model)
        {
            return await _merchandisService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MMerchandiseCategory model)
        {
            return await _merchandisService.Update(model);
        }

        [HttpPut]
        [Route("UpdateSetting")]
        public async Task<GenericResult> UpdateSetting(List<MMerchandiseCategory> models)
        {
            return await _merchandisService.UpdateSetting(models);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string Id)
        {
            return await _merchandisService.Delete(Id);
        }
        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport model)
        {
            return await _merchandisService.Import(model);
        }

    }
}
