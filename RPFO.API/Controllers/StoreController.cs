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
    public class StoreController : ControllerBase    
    {

        IStoreService _storeService;
        private readonly ILogger<StoreController> _logger;

        public StoreController(ILogger<StoreController> logger, IStoreService storeService)
        {
            _logger = logger;
            _storeService = storeService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            return await _storeService.GetAll(CompanyCode);
        }
        
        [HttpGet]
        [Route("GetStoreByUserWithStatus")]
        public async Task<GenericResult> GetStoreListByUser(string UserCode)
        {
            return await _storeService.GetStoreListByUser(UserCode);
        }
        [HttpGet]
        [Route("GetByUser")]
        public async Task<GenericResult> GetByUser(string UserCode)
        {
            return await _storeService.GetByUser(UserCode);
        }
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            try
            {
                var data = await _storeService.GetPagedList(userParams);
                Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
                return Ok(data);
            }
            catch(Exception ex)
            {
                return null;
            }
          
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string id)
        {
            return await _storeService.GetByCode(CompanyCode, id);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MStore store)
        {
            return await _storeService.Create(store);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MStore store)
        {
            return await _storeService.Update(store);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string code)
        {
            return await _storeService.Delete(code);
        }

        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport model)
        {
            return await _storeService.Import(model);
        }

        [HttpGet]
        [Route("GetStoreByUserWhsType")]
        public async Task<GenericResult> GetStoreByUserWhsType(string UserCode)
        {
            return await _storeService.GetStoreByUserWhsType(UserCode);
        }

        [HttpGet]
        [Route("GetAllByWhstype")]
        public async Task<GenericResult> GetAllByWhstype(string CompanyCode)
        {
            return await _storeService.GetAllByWhstype(CompanyCode);
        }
    }


}
