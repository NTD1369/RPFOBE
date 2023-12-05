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
    public class UserStoreController : ControllerBase    
    {

        IUserStoreService _userStoreService;
        private readonly ILogger<UserStoreController> _logger;

        public UserStoreController(ILogger<UserStoreController> logger, IUserStoreService userStoreService)
        {
            _logger = logger;
            _userStoreService = userStoreService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll()
        {
            return await _userStoreService.GetAll();
        }

        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _userStoreService.GetPagedList(userParams);
            
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string id)
        {
            return await _userStoreService.GetByCode(id);
        }
        [HttpGet]
        [Route("GetByMer")]
        public async Task<IActionResult> GetByMer([FromQuery] UserParams userParams)
        {
            var data = await _userStoreService.GetPagedList(userParams);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport model)
        {
            return await _userStoreService.Import(model);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MUserStore model)
        {
            return await _userStoreService.Create(model);
        }
        
       
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MUserStore model)
        {
            return await _userStoreService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string Id)
        {
            return await _userStoreService.Delete(Id);
        }

    }
}
