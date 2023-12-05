using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RPFO.Application.Interfaces;
using RPFO.Utilities.Dtos;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    public class DeliveryReturnController: ControllerBase
    {
        private readonly IDeliveryReturnService _DeliveryReturnService;


        public DeliveryReturnController(IDeliveryReturnService deliveryReturnService)
        {
            _DeliveryReturnService = deliveryReturnService;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetOrderById")]
        public async Task<GenericResult> GetOrderById(string id, string companycode, string storeid)

        {
            GenericResult result = new GenericResult();
            return await _DeliveryReturnService.GetOrderById(id, companycode, storeid);
        }
        [AllowAnonymous]

        [HttpGet]
        [Route("GetByType")]

        public async Task<GenericResult> GetByType(string companycode, string storeId, string fromdate, string todate, string key, string status)
        {
            return await _DeliveryReturnService.GetByType(companycode, storeId, fromdate, todate, key, status);
        }
    }
}
