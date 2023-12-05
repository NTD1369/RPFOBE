
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
    public class KeyCapService : IKeyCapService
    {
        private readonly IGenericRepository<MKeyCap> _keyRepository;

        private readonly IMapper _mapper;
        public KeyCapService(IGenericRepository<MKeyCap> keyRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _keyRepository = keyRepository;
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

         
        public async Task<GenericResult> Create(MKeyCap model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
 
                 
                parameters.Add("Id", model.Id);
                parameters.Add("Name", model.Name); 
                parameters.Add("Description", model.Description);  
                parameters.Add("Status", model.Status);
                var exist = await GetByCode(model.Id);
                if (exist.Data!=null)
                {
                    result.Success = false;
                    result.Message = model.Id + " existed.";
                    return result;
                }
                var affectedRows = _keyRepository.Insert("USP_I_M_KeyCap", parameters, commandType: CommandType.StoredProcedure);
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
         
        public Task<GenericResult> Delete(string Id)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetAll()
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _keyRepository.GetAllAsync($"USP_S_M_KeyCap '', ''", null, commandType: CommandType.Text);
                result.Success = true;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> GetByCode(string Id)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _keyRepository.GetAsync($"USP_S_M_KeyCap '{Id}' , ''", null, commandType: CommandType.Text);
                result.Success = true;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

     

        public async Task<GenericResult> Update(MKeyCap model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Id", model.Id);
                parameters.Add("Name", model.Name);
                parameters.Add("Description", model.Description);
                parameters.Add("Status", model.Status);
                var affectedRows = _keyRepository.Insert("USP_U_M_KeyCap", parameters, commandType: CommandType.StoredProcedure);
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
