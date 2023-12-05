using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RPFO.API.Helpers;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReasonController : ControllerBase    
    {

        IReasonService _reasonService;
        private readonly ILogger<ReasonController> _logger;
        SemaphoreSlim _slim = new SemaphoreSlim(1);
        SemaphoreWaiter _semaphoreWaiter = new SemaphoreWaiter();
        public ReasonController(ILogger<ReasonController> logger, IReasonService reasonService)
        {
            _logger = logger;
            _reasonService = reasonService;
            //_semaphoreWaiter = semaphoreWaiter;
        }
        
        [HttpGet]
        [Route("GetAll")]
        //Task<IEnumerable<MControl>>
        public async Task<GenericResult>  GetAll(string CompanyCode)
        {
          
                return await _reasonService.GetAll(CompanyCode);
                
          
        }
         
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string id  )
        {
            return await _reasonService.GetByCode(CompanyCode, id );
        }
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MReason model)
        {
            return await _reasonService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MReason model)
        {
            return await _reasonService.Update(model);
        }
       
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string CompanyCode, string Id)
        {
            return await _reasonService.Delete(CompanyCode, Id);
        }

    }
}
