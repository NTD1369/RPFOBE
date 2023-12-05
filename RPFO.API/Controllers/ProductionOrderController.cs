using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RPFO.Application.Interfaces;
using RPFO.Utilities.Dtos;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductionOrderController : ControllerBase
    {
        private readonly IProductionOrderService _ProductionOrderService;

        public ProductionOrderController(IProductionOrderService productionOrderService)
        {
            _ProductionOrderService = productionOrderService;
        }
        [HttpGet]
        [Route("GetId")]
        public async Task<GenericResult> GetById(string id)
        {
            return await _ProductionOrderService.GetById(id);
        }
        [HttpGet]
        [Route("GetOrderById")]
        public async Task<GenericResult> GetOrderById(string id, string companycode, string storeid)

        {
            GenericResult result = new GenericResult();
            return await _ProductionOrderService.GetOrderById(id, companycode, storeid);
        }
        [AllowAnonymous]

        [HttpGet]
        [Route("GetByType")]

        public async Task<GenericResult> GetByType(string companycode, string storeId, string fromdate, string todate, string key, string status)
        {
            return await _ProductionOrderService.GetByType(companycode, storeId, fromdate, todate, key, status);
        }
    }
}
