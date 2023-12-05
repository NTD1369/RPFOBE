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
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MovementTypeController : ControllerBase    
    {

        IMovementTypeService _movementService;
        private readonly ILogger<MovementTypeController> _logger;

        public MovementTypeController(ILogger<MovementTypeController> logger, IMovementTypeService movementService)
        {
            _logger = logger;
            _movementService = movementService;
        }
        [Cached(600)]
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll()
        {
            return await _movementService.GetAll();
        }
        
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string Code)
        {
            return await _movementService.GetByCode(Code);
        }
         
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MMovementType model)
        {
            return await _movementService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MMovementType model)
        {
            return await _movementService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string Id)
        {
            return await _movementService.Delete(Id);
        }

    }
}
