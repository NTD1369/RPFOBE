using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RPFO.API.Helpers;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DivisionController : ControllerBase    
    {

        IDevisionService _deviService;
        private readonly ILogger<DivisionController> _logger; 
        public DivisionController(ILogger<DivisionController> logger, IDevisionService deviService)
        {
            _logger = logger;
            _deviService = deviService; 
        }
        
        [HttpGet]
        [Route("GetAll")] 
        public async Task<GenericResult>  GetAll(string CompanyCode, string FromDate, string ToDate)
        { 
            var response = await _deviService.GetAll(CompanyCode, FromDate, ToDate);
            return response; 
        }
        
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string Id)
        {
            return await _deviService.GetByCode(CompanyCode, Id);
        }
        [HttpGet]
        [Route("GetDetailDivision")]
        public async Task<GenericResult> GetDetailDivision(string CompanyCode, string Id)
        {
            return await _deviService.GetDetailDivision(CompanyCode, Id);
        }
         
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(TDivisionHeader model)
        {
            return await _deviService.Create(model);
        }
        



        [HttpPost]
        [Route("createByList")]
        public async Task<GenericResult> createByList(List<TDivisionHeader> model)
        {
            string xxx = "";
            //ListtoDataTableConverter converter = new ListtoDataTableConverter();
            //DataTable dt = converter.ToDataTable(model);
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            //DataTable pDt = JsonConvert.DeserializeObject<DataTable>(json);
            return await _deviService.CreateByList(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(TDivisionHeader model)
        {
            return await _deviService.Update(model);
        }
        
        //[HttpDelete]
        //[Route("Delete")]
        //public async Task<GenericResult> Delete(TDevisionHeader model)
        //{
        //    return await _deviService.Delete(model);
        //}

    }
}
