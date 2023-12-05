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
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionController : ControllerBase
    {
        IPermissionService _permissionService;
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(ILogger<PermissionController> logger, IPermissionService permissionService)
        {
            _logger = logger;
            _permissionService = permissionService;
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            return await _permissionService.GetAll(CompanyCode);
        }

        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            try
            {
                var data = await _permissionService.GetPagedList(userParams);

                Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
                return Ok(data);
            }
            catch(Exception ex)
            {
                return Ok(ex.Message);
            }
         
        }
        [HttpGet]
        [Route("GetById")]
        public async Task<GenericResult> GetById(string CompanyCode, string id)
        {
            return await _permissionService.GetByCode(CompanyCode, id);
        }
        [HttpGet]
        [Route("CheckApproveFunctionByUser")]
        public async Task<GenericResult> CheckApproveFunctionByUser(string CompanyCode, string User, string Password,string CustomCode, string Function, string ControlId, string Permission)
        {
            return await _permissionService.CheckApproveFunctionByUser(CompanyCode, User,  Password, CustomCode, Function, ControlId, Permission);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MPermission model)
        {
            return await _permissionService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MPermission model)
        {
            return await _permissionService.Update(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<GenericResult> Delete(string Id)
        {
            return await _permissionService.Delete(Id);
        }

        [HttpGet]
        [Route("CopyFromRole")]
        public GenericResult CopyFromRole(string CompanyCode,  string FrRole, string ToRole, string By)
        {
            return  _permissionService.CopyFromRole(CompanyCode, FrRole, ToRole,  By);
        }

        [HttpGet]
        [Route("GetPermissionByFunction")]
        public async Task<GenericResult> GetPermissionByFunction(string FunctionId)
        {
            return await _permissionService.GetPermissionByFunction(FunctionId);
        }
        [HttpGet]
        [Route("GetPermissionByRole")]
        public async Task<GenericResult> GetPermissionByRole(string RoleId)
        {
            return await _permissionService.GetPermissionByRole(RoleId);
        }
        [HttpGet]
        [Route("GetControlPermissionListByFunction")]
        public async Task<GenericResult> GetControlPermissionListByFunction(string FunctionId, string RoleId)
        {
            return await _permissionService.GetControlPermissionListByFunction(FunctionId, RoleId);
        }
        [HttpGet]
        [Route("GetFunctionPermissionByUser")]
        public async Task<GenericResult> GetFunctionPermissionByUser(string UserName)
        {
            return await _permissionService.GetFunctionPermissionByUser(UserName);
        }
        [HttpGet]
        [Route("GetHeaderFunctionPermission")]
        public async Task<GenericResult> GetHeaderFunctionPermission()
        {
           
            return await _permissionService.GetHeaderFunctionPermission();
                 
            
        }
        [HttpGet]
        [Route("GetHeaderPermission")]
        public async Task<GenericResult> GetHeaderPermission()
        {
            
                return await _permissionService.GetHeaderPermission();
                 
            
        }
        [HttpPost]
        [Route("UpdateNode")]
        public async Task<GenericResult> UpdateNode(MPermission model)
        {
            //foreach (KeyValuePair<string, string> formEntry in formdata)
            //{
            //    string key = formEntry.Key;
            //    string value = formEntry.Value;
            //}
            return await _permissionService.UpdateFunctionFermission(model);
            //return await _permissionService.Create(model);
        }
        [HttpPost]
        [Route("UpdateListNode")]
        public async Task<GenericResult> UpdateListNode(List<MPermission> list)
        {
            //foreach (KeyValuePair<string, string> formEntry in formdata)
            //{
            //    string key = formEntry.Key;
            //    string value = formEntry.Value;
            //}
            return await _permissionService.UpdateListFunctionFermission(list);
            //return await _permissionService.Create(model);
        }

        
    }
}
