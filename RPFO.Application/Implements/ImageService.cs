
using AutoMapper;
using Dapper;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class ImageService : IImageService
    {
        private readonly IGenericRepository<MImage> _imageRepository;

        private readonly IMapper _mapper;
        public ImageService(IGenericRepository<MImage> imageRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _imageRepository = imageRepository;
            _mapper = mapper; 

        }
        //public async Task<GenericResult> Import(DataImport model)
        //{
        //    GenericResult result = new GenericResult();
        //    List<UOMResultViewModel> resultlist = new List<UOMResultViewModel>();
        //    try
        //    {
        //        foreach (var item in model.Uom)
        //        {
        //            item.CreatedBy = model.CreatedBy;
        //            item.CompanyCode = model.CompanyCode;
        //            var itemResult = await Create(item);
        //            //if (itemResult.Success == false)
        //            //{
        //                UOMResultViewModel itemRs = new UOMResultViewModel();
        //                itemRs = _mapper.Map<UOMResultViewModel>(item);
        //                itemRs.Success = itemResult.Success;
        //                itemRs.Message = itemResult.Message;
        //            resultlist.Add(itemRs);
        //            //}
        //        }
        //        result.Success = true;
        //        result.Data = resultlist;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = ex.Message;
        //        //result.Data = failedlist;
        //    } 
        //    return result;
        //}

         
        public async Task<GenericResult> Create(MImage model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parametersImg = new DynamicParameters();



                parametersImg.Add("Id", model.Id, DbType.String);
                parametersImg.Add("CompanyCode", model.CompanyCode);
                parametersImg.Add("Description", model.Description);
                parametersImg.Add("Num", model.Num);
                parametersImg.Add("Type", model.Type);
                parametersImg.Add("Image", model.Image);
                parametersImg.Add("CreateOn", model.CreateOn);
                parametersImg.Add("CustomerPhone", model.CustomerPhone);
                parametersImg.Add("CustomerName", model.CustomerName);

                //db.Execute("USP_I_M_Image", parametersImg, commandType: CommandType.StoredProcedure, transaction: tran);

                var affectedRows = _imageRepository.Update("USP_I_M_Image", parametersImg, commandType: CommandType.StoredProcedure);
                result.Success = true;
                //result.Message = key;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> Delete(string Code)
        {
            throw new NotImplementedException();
        }
        public   string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public async Task<List<MImage>> GetImage(string CompanyCode, string Type, string Code, string Phone)
        {
            try
            {
                var data = await _imageRepository.GetAllAsync($"USP_S_M_Image '{CompanyCode}','{Code}','{Phone}','{Type}'", null, commandType: CommandType.Text);
                //foreach(var item in data)
                //{
                //    item.Image = Base64Decode(item.Image);
                //}    
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

     

        public Task<List<MCompany>> GetByItem(string Item)
        {
            throw new NotImplementedException();
        }

      

        public async Task<GenericResult> Update(MImage model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parametersImg = new DynamicParameters();




                parametersImg.Add("Id", model.Id, DbType.String);
                parametersImg.Add("CompanyCode", model.CompanyCode);
                parametersImg.Add("Description", model.Description);
                parametersImg.Add("Num", model.Num);
                parametersImg.Add("Type", model.Type);
                parametersImg.Add("Image", model.Image);
                parametersImg.Add("CreateOn", model.CreateOn);
                parametersImg.Add("CustomerPhone", model.CustomerPhone);
                parametersImg.Add("CustomerName", model.CustomerName);


                //db.Execute("USP_I_M_Image", parametersImg, commandType: CommandType.StoredProcedure, transaction: tran);

                var affectedRows = _imageRepository.Update("USP_I_M_Image", parametersImg, commandType: CommandType.StoredProcedure);
                result.Success = true;
                //result.Message = key;
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
