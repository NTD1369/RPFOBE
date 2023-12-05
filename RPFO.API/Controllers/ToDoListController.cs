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
    public class ToDoListController : ControllerBase    
    {

        IToDoListService _todoService;
        private readonly ILogger<ToDoListController> _logger;

        public ToDoListController(ILogger<ToDoListController> logger, IToDoListService todoService)
        {
            _logger = logger;
            _todoService = todoService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string Id, string Code, string Name, string Description, string Content, string Remark, string Status,
            DateTime? FromDate, DateTime? ToDate, string CreatedBy, DateTime? CreatedOn)
        {
            return await _todoService.GetAll( Id,  Code,  Name,  Description,  Content,  Remark,  Status,
             FromDate,  ToDate,  CreatedBy, CreatedOn);
        }

         
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string id)
        {
            return await _todoService.GetById(id);
        }
        
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(SToDoList model)
        {
            return await _todoService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(SToDoList model)
        {
            return await _todoService.Update(model);
        }
        //[HttpDelete]
        //[Route("Delete")]
        //public async Task<GenericResult> Delete(string Id)
        //{
        //    return await _todoService.Delete(Id);
        //}

    }
}
