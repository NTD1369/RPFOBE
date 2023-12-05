using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RPFO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase    
    {

        ICompanyService _companyService;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(ILogger<CompanyController> logger, ICompanyService companyService)
        {
            _logger = logger;
            _companyService = companyService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<GenericResult> GetAll()
        {
            return await _companyService.GetAll();
        }
        
        [HttpGet]
        [Route("GetByCode")]
        public async Task<GenericResult> GetByCode(string Code)
        {
            return await _companyService.GetByCode(Code);
        }
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MCompany model)
        {
            return await _companyService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MCompany model)
        {
            return await _companyService.Update(model);
        }
        [HttpPost("logoUpdate")]
        [DisableRequestSizeLimit]
        public async Task<GenericResult> UploadFile(string companyCode,   IFormFile image)
        {
            GenericResult result = new GenericResult();
            //IFormFile image = null;
            if (string.IsNullOrEmpty(companyCode))
            {
                result.Success = false;
                result.Message = "CompanyCode is null";
                return result;
            }
           
            if (image == null)
            {
                result.Success = false;
                result.Message = "Image not null";
                return result;
            }
            //else
            //{
            //    image = uploadModel.image;
            //}
            try
            {
                var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                string fName = image.FileName;
                string folder = Path.Combine(rootPath, "images/items/");
                string path = Path.Combine(folder + image.FileName);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                var rsUpdate = await _companyService.LogoUpdate(companyCode,  image.FileName);
                result.Success = true;
                result.Message = "";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
