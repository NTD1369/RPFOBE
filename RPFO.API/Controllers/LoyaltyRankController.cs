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
    public class LoyaltyRankController : ControllerBase    
    {

        ILoyaltyRankService _rankService;
        private readonly ILogger<LoyaltyRankController> _logger;

        public LoyaltyRankController(ILogger<LoyaltyRankController> logger, ILoyaltyRankService rankService)
        {
            _logger = logger;
            _rankService = rankService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string companyCode)
        {
            return await _rankService.GetAll(companyCode);
        }

        
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string companyCode, string Code)
        {
            return await _rankService.GetByCode(companyCode, Code);
        }
        
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(SLoyaltyRank model)
        {
            return await _rankService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(SLoyaltyRank model)
        {
            return await _rankService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string CompanyCode, string RankId)
        {
            return await _rankService.Delete(CompanyCode, RankId);
        }
    }
}
