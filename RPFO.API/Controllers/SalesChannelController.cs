using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesChannelController : ControllerBase
    {
        ISalesChannelService _saleschannelService;

        private readonly ILogger<SalesChannelController> _logger;
        public SalesChannelController(ISalesChannelService saleschannelService, ILogger<SalesChannelController> logger)
        {
            _saleschannelService = saleschannelService;
            _logger = logger;
        }
        [HttpGet]
        [Route("GetAll")]
        //<param name = "CompanyCode" > Mã Công ty(Bắt buộc)</param>
        //<param name = "StoreId" > Mã Cửa hàng(Bắt buộc)</param>
        /// <summary>
        /// This is method summary I want displayed
        /// </summary>
        public async Task<GenericResult> GetAll(string CompanyCode,  string Keyword)
        {
            return await _saleschannelService.GetAll(CompanyCode, Keyword);
        }
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string CompanyCode, string Key)
        {
            return await _saleschannelService.GetByCode(CompanyCode,Key);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MSalesChannel model)
        {
            return await _saleschannelService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MSalesChannel model)
        {
            return await _saleschannelService.Update(model);
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(MSalesChannel model)
        {
            return await _saleschannelService.Delete(model);
        }

    }
}
