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
    public class StoreGroupController : ControllerBase    
    {

        IStoreGroupService _storegroupService;
        private readonly ILogger<StoreGroupController> _logger;

        public StoreGroupController(ILogger<StoreGroupController> logger, IStoreGroupService storegroupService)
        {
            _logger = logger;
            _storegroupService = storegroupService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            return await _storegroupService.GetAll(CompanyCode);
        }

        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _storegroupService.GetPagedList(userParams);
            
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string id)
        {
            return await _storegroupService.GetByCode(CompanyCode, id);
        }
        [HttpGet]
        [Route("GetByMer")]
        public async Task<IActionResult> GetByMer([FromQuery] UserParams userParams)
        {
            var data = await _storegroupService.GetPagedList(userParams);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport model)
        {
            return await _storegroupService.Import(model);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MStoreGroup model)
        {
            return await _storegroupService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MStoreGroup model)
        {
            return await _storegroupService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string CpmanyCode, string Id)
        {
            return await _storegroupService.Delete(CpmanyCode, Id);
        }

    }
}
