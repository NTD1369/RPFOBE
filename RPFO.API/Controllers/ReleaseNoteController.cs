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
    public class ReleaseNoteController : ControllerBase    
    {

        IReleaseNoteService _releaseService;
        private readonly ILogger<ReleaseNoteController> _logger;

        public ReleaseNoteController(ILogger<ReleaseNoteController> logger, IReleaseNoteService releaseService)
        {
            _logger = logger;
            _releaseService = releaseService;
        }
        
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            return await _releaseService.GetAll(CompanyCode);
        }
        
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _releaseService.GetPagedList(userParams);
                if(data!=null)
                { 
                    Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
                }    
                result.Success = true;
                result.Data = data;
                //return Ok(data);
            }
            catch(Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return Ok(result);


        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string Id, string Version)
        {
            return await _releaseService.GetByCode(CompanyCode, Id, Version);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(SReleaseNote model)
        {
            return await _releaseService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(SReleaseNote model)
        {
            return await _releaseService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(SReleaseNote model)
        {
            return await _releaseService.Delete(model);
        }
    }
}
