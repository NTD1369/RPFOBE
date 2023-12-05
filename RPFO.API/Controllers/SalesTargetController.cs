using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SalesTargetController : ControllerBase    
    {

        //ISalesTargetService _targetService;
        //private readonly ILogger<SalesTargetController> _logger;

        //public SalesTargetController(ILogger<SalesTargetController> logger, ISalesTargetService targetService)
        //{
        //    _logger = logger;
        //    _targetService = targetService;
        //}

        //[HttpGet]
        //[Route("GetAll")]
        //public async Task<GenericResult> GetAll(string CompanyCode, string Employee, string Id, DateTime? FromDate, DateTime? ToDate)
        //{
        //    return await _targetService.GetAll(CompanyCode, Employee, Id, FromDate, ToDate);
        //}
         
        //[HttpPost]
        //[Route("Create")]
        //public async Task<GenericResult> Create(MSalesTarget model)
        //{
        //    return await _targetService.Create(model);
        //}
        //[HttpPut]
        //[Route("Update")]
        //public async Task<GenericResult> Update(MSalesTarget model)
        //{
        //    return await _targetService.Update(model);
        //}
        //[HttpPost]
        //[Route("Delete")]
        //public async Task<GenericResult> Delete(MSalesTarget model)
        //{
        //    return await _targetService.Delete(model);
        //}

    }
}
