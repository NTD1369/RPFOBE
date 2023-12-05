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
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase    
    {

        IImageService _imageService;
        private readonly ILogger<ImageController> _logger;

        public ImageController(ILogger<ImageController> logger, IImageService imageService)
        {
            _logger = logger;
            _imageService = imageService;
        }

         
        [HttpGet]
        [Route("GetImage")]
        public async Task<List<MImage>> GetImage(string CompanyCode, string Type, string Code, string Phone)
        {
            return await _imageService.GetImage(CompanyCode, Type, Code, Phone);
        }
       
        [HttpPost]
        [Route("Create")]
        public async Task<GenericResult> Create(MImage model)
        {
            return await _imageService.Create(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<GenericResult> Update(MImage model)
        {
            return await _imageService.Update(model);
        }
        //[HttpPost("logoUpdate")]
        //[DisableRequestSizeLimit]
        //public async Task<GenericResult> UploadFile(string companyCode,   IFormFile image)
        //{
        //    GenericResult result = new GenericResult();
        //    //IFormFile image = null;
        //    if (string.IsNullOrEmpty(companyCode))
        //    {
        //        result.Success = false;
        //        result.Message = "CompanyCode is null";
        //        return result;
        //    }
           
        //    if (image == null)
        //    {
        //        result.Success = false;
        //        result.Message = "Image not null";
        //        return result;
        //    }
           
        //    try
        //    {
        //        var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        //        string fName = image.FileName;
        //        string folder = Path.Combine(rootPath, "images/items/");
        //        string path = Path.Combine(folder + image.FileName);
        //        if (!Directory.Exists(folder))
        //        {
        //            Directory.CreateDirectory(folder);
        //        }
        //        using (var stream = new FileStream(path, FileMode.Create))
        //        {
        //            await image.CopyToAsync(stream);
        //        }
        //        var rsUpdate = await _companyService.LogoUpdate(companyCode,  image.FileName);
        //        result.Success = true;
        //        result.Message = "";
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = ex.Message;
        //    }

        //    return result;
        //}
    }
}
