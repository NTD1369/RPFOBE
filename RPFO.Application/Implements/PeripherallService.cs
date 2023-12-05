
using AutoMapper;
using Dapper;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class PeripherallService : IPeripherallService
    {
        private readonly IGenericRepository<MPeripherals> _peripheralsRepository;

        private readonly IMapper _mapper;
        public PeripherallService(IGenericRepository<MPeripherals> peripheralsRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _peripheralsRepository = peripheralsRepository;
            _mapper = mapper; 

        }

        public async Task<GenericResult> Create(MPeripherals model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
               
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Code", model.Code);
                parameters.Add("Name", model.Name);
                parameters.Add("Description", model.Description);
                parameters.Add("Type", model.Type);
                parameters.Add("ConnectType", model.ConnectType);
                parameters.Add("Port", model.Port);
                parameters.Add("BaudRate", model.BaudRate);
                parameters.Add("Parity", model.Parity);
                parameters.Add("DataBits", model.DataBits);
                parameters.Add("StopBits", model.StopBits);
                parameters.Add("Handshake", model.Handshake);
                parameters.Add("Status", model.Status);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                parameters.Add("CreatedBy", model.CreatedBy); 

                var affectedRows = _peripheralsRepository.Insert("USP_I_M_Peripherals", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Delete(MPeripherals model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Code", model.Code);
                var data = await _peripheralsRepository.GetAllAsync("USP_D_M_Peripherals", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;

                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("Code", "");
                parameters.Add("Type", "");
                var data = await _peripheralsRepository.GetAllAsync("USP_S_M_Peripherals", parameters, commandType: CommandType.StoredProcedure);
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
       
        public async Task<GenericResult> GetByCode(string CompanyCode, string PeripheralCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;

                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("Code", PeripheralCode);
                parameters.Add("Type", "");
                var data = await _peripheralsRepository.GetAsync("USP_S_M_Peripherals", parameters, commandType: CommandType.StoredProcedure);
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

       
        public async Task<GenericResult> Update(MPeripherals model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Code", model.Code);
                parameters.Add("Name", model.Name);
                parameters.Add("Description", model.Description);
                parameters.Add("Type", model.Type);
                parameters.Add("ConnectType", model.ConnectType);
                parameters.Add("Port", model.Port);
                parameters.Add("BaudRate", model.BaudRate);
                parameters.Add("Parity", model.Parity);
                parameters.Add("DataBits", model.DataBits);
                parameters.Add("StopBits", model.StopBits);
                parameters.Add("Handshake", model.Handshake);
                parameters.Add("Status", model.Status);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                var affectedRows = _peripheralsRepository.Update("USP_U_M_Peripherals", parameters, commandType: CommandType.StoredProcedure);
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
