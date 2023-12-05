using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RPFO.API.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotifyController : ControllerBase
    {

        private IHubContext<SignaRHub> _hub;
        public NotifyController(IHubContext<SignaRHub> hub)
        {
            _hub = hub;
        }
        /// <summary>
        /// Send message to all
        /// </summary>
        /// <param name="message"></param>
        [HttpPost("{message}")]
        public void Post(string message)
        {
            
            _hub.Clients.All.SendAsync("publicMessageMethodName", message);
        }

        /// <summary>
        /// Send message to specific client
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="message"></param>
        [HttpPost("{connectionId}/{message}")]
        public void Post(string connectionId, string message)
        {
            _hub.Clients.Client(connectionId).SendAsync("privateMessageMethodName", message);
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
