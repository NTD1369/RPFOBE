
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RPFO.Application.Interfaces;
using RPFO.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {

        IBasketService _basketService;
        private readonly ILogger<BasketController> _logger;

        public BasketController(ILogger<BasketController> logger, IBasketService basketService)
        {
            _logger = logger;
            _basketService = basketService;
        }

        [HttpGet]
        [Route("GetBasketId")]
        public async Task<ActionResult<BasketViewModel>> GetBasketId(string id)
        {
            var basket = await _basketService.GetBasketAsync(id);
            return Ok(basket ?? new BasketViewModel(id));
        }
        [HttpPost]
        [Route("UpdateBasket")]
        public async Task<ActionResult<BasketViewModel>> UpdateBasket(BasketViewModel basket)
        {
            var updatebasket = await _basketService.UpdateBasketAsync(basket);
            return Ok(updatebasket);
        }
        [HttpDelete]
        [Route("DeleteBasket")]
        public async Task DeleteBasket(string id)
        {
            await _basketService.DeleteBasketAsync(id);

        }
    }
}
