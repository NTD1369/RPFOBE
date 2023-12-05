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
    public class PriceListNameController : ControllerBase    
    {

        IPriceListNameService _priceListService;
        private readonly ILogger<PriceListNameController> _logger;

        public PriceListNameController(ILogger<PriceListNameController> logger, IPriceListNameService priceListService)
        {
            _logger = logger;
            _priceListService = priceListService;
        }
        
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            return await _priceListService.GetAll(CompanyCode);
        }
      
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _priceListService.GetPagedList(userParams);
            
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        //[HttpGet]
        //[Route("GetById")]
        //public async Task<GenericResult> GetById(string CompanyCode, string id)
        //{
        //    return await _priceListService.GetById(CompanyCode, id);
        //}
        
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MPriceListName model)
        {
            return await _priceListService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MPriceListName model)
        {
            return await _priceListService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(MPriceListName model)
        {
            return await _priceListService.Delete(model);
        }




    }
}
