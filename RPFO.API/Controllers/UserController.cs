using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    public class UserController : ControllerBase
    {
        IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            return await _userService.GetAllUsers(CompanyCode, true);
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string id)
        {
            return await _userService.GetById(CompanyCode, id);
        }
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string CompanyCode, string usercode)
        {
            return await _userService.GetByUsername(CompanyCode, usercode);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GenQRCode")]
        public async Task<GenericResult> GenQRCode(string CompanyCode, string UserName, string Password)
        {

            return await _userService.GenQRCode(CompanyCode, UserName, Password);
        }
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MUser model)
        {
            return await _userService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MUser model)
        {
            return await _userService.Update(model);
        }
        [HttpPut]
        [Route("UpdateLastStore")]
        public async Task<GenericResult> UpdateLastStore(MUser model)
        {
            return await _userService.UpdateLastStore(model);
        }
       
        [HttpPost]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string Code)
        {
            return await _userService.Delete(Code);
        }
        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport model)
        {
            return await _userService.Import(model);
        }
    }
}
