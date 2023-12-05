
using AutoMapper;
using Dapper;
using DevExpress.Printing.Utils.DocumentStoring;
using DevExpress.XtraRichEdit.Fields;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class PlacePrintService : IPlacePrintService
    {
        private readonly IGenericRepository<TPlacePrint> _placePrintRepository;
        private readonly IGenericRepository<ItemGroupModel> _itemGroupRepository;
        private readonly IMapper _mapper;

        public PlacePrintService(
            IGenericRepository<TPlacePrint> placePrintRepository,
            IGenericRepository<ItemGroupModel> itemGroupRepository,
            IMapper mapper)
        {
            _placePrintRepository = placePrintRepository;
            _itemGroupRepository = itemGroupRepository;
            _mapper = mapper;
        }

        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                var data = await _placePrintRepository.GetAllAsync($"USP_S_SettingPrint", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> ViewItemByItemGroup(string CompanyCode, string StoreId, string itemGroup, string Status)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                parameters.Add("itemGroup", itemGroup);
                parameters.Add("Status", Status);
                var data = await _placePrintRepository.GetAllAsync($"USP_S_ViewItemByItemGroup", parameters, commandType: CommandType.StoredProcedure);
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



        public async Task<GenericResult> GetListItemGroup(string CompanyCode, string StoreId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                var data = await _itemGroupRepository.GetAllAsync($"USP_S_ItemGroup", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Create(TPlacePrint model)
        {
            GenericResult result = new GenericResult();

            if (string.IsNullOrEmpty(model.GroupItem))
            {
                result.Success = false;
                result.Message = $"Group item is required";
                return await Task.FromResult(result);
            }

            if (string.IsNullOrEmpty(model.PrintName))
            {
                result.Success = false;
                result.Message = $"Print name is required";
                return await Task.FromResult(result);
            }

            //if (!string.IsNullOrEmpty(model.PrintName) || !string.IsNullOrEmpty(model.GroupItem))
            //{

            //    var tableIdQuery = $"select * from M_PlacePrint where CompanyCode = N'{model.CompanyCode}' " +
            //        $"and StoreId = N'{model.StoreId}' and (PrintName = N'{model.PrintName}' or  GroupItem = N'{model.GroupItem}' ) and Status ='A'";

            //    var settingPrint = _placePrintRepository.Get(tableIdQuery, null, commandType: CommandType.Text);

            //    if (settingPrint != null)
            //    {
            //        result.Success = false;
            //        result.Message = $"Print name is exist system";
            //        return await Task.FromResult(result);
            //    }
            //}

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("PlaceId", model.PlaceId);
                parameters.Add("PrintName", model.PrintName);
                parameters.Add("GroupItem", model.GroupItem);
                parameters.Add("Status", model.Status);
                parameters.Add("CreatedOn", DateTime.Now);
                parameters.Add("CreatedBy", model.CreatedBy);
                var affectedRows = _placePrintRepository.Insert("USP_I_SettingPrint", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return await Task.FromResult(result);
        }

        public async Task<GenericResult> Update(TPlacePrint model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("PrintId", model.PrintId);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("PlaceId", model.PlaceId);
                parameters.Add("PrintName", model.PrintName);
                parameters.Add("GroupItem", model.GroupItem);
                parameters.Add("Status", model.Status);
                parameters.Add("CreatedOn", DateTime.Now);
                parameters.Add("ModifiedBy", model.CreatedBy);
                var affectedRows = _placePrintRepository.Insert("USP_U_SettingPrint", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                //result.Message = key;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return await Task.FromResult(result);
        }

        public async Task<GenericResult> Delete(int PrintId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("PrintId", PrintId);
                var affectedRows = _placePrintRepository.Execute("USP_D_SettingPrint", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                //result.Message = key;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return await Task.FromResult(result);
        }
    }
}
