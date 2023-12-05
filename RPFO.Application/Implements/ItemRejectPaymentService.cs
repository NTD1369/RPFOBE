
using AutoMapper;
using Dapper;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class ItemRejectPaymentService : IItemRejectPaymentService
    {
        private readonly IGenericRepository<MItemRejectPayment> _itemRejectPaymentRepository;
        //private readonly IGenericRepository<MItem> _itemRepository;
        //private readonly IGenericRepository<MUom> _uomRepository;
        private readonly IMapper _mapper;
        public ItemRejectPaymentService(IGenericRepository<MItemRejectPayment> itemRejectPaymentRepository,   IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _itemRejectPaymentRepository = itemRejectPaymentRepository;
            _mapper = mapper; 
          
        }
        //public async Task<GenericResult> Import(DataImport model)
        //{
        //    GenericResult result = new GenericResult();
        //    List<IItemUomResultViewModel> resultlist = new List<IItemUomResultViewModel>();
        //    try
        //    {
        //        foreach (var item in model.ItemUom)
        //        {
        //            //item.CreatedBy = model.CreatedBy;
        //            //item.CompanyCode = model.CompanyCode;
        //            var itemResult = await Create(item);
                  
        //                IItemUomResultViewModel itemRs = new IItemUomResultViewModel();
        //                itemRs = _mapper.Map<IItemUomResultViewModel>(item);
        //                itemRs.Success = itemResult.Success;
        //                itemRs.Message = itemResult.Message;
        //                resultlist.Add(itemRs);
                     
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
        //public async Task<List<ItemViewModel>> GetItemInfor(string CompanyCode, string ItemCode, string UomCode, string BarCode, string Keyword, string Merchandise, string Type)
        //{
        //    using (IDbConnection db = _itemRepository.GetConnection())
        //    {
        //        try
        //        {
        //            if (db.State == ConnectionState.Closed)
        //                db.Open();
        //            var parameters = new DynamicParameters();
        //            parameters.Add("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode);
        //            parameters.Add("ItemCode", string.IsNullOrEmpty(ItemCode) ? "" : ItemCode);
        //            parameters.Add("UomCode", string.IsNullOrEmpty(UomCode) ? "" : UomCode);
        //            parameters.Add("BarCode", string.IsNullOrEmpty(BarCode) ? "" : BarCode);
        //            parameters.Add("Keyword", string.IsNullOrEmpty(Keyword) ? "" : Keyword);
        //            parameters.Add("Merchandise", string.IsNullOrEmpty(Merchandise) ? "" : Merchandise);
        //            parameters.Add("Type", string.IsNullOrEmpty(Type) ? "" : Type);
        //            var dblist = await db.QueryAsync<ItemViewModel>($"USP_GetItemInfor", parameters, commandType: CommandType.StoredProcedure);
                   
        //            db.Close();
        //            return dblist.ToList();

        //        }
        //        catch (Exception ex)
        //        {
        //            db.Close();
        //            return null;
        //        }
        //    }

        //}
        //public async Task<bool> checkUomExist(string CompanyCode, string UomCode)
        //{
        //    var parameters = new DynamicParameters();

        //    //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
        //    //model.ShiftId = key;
        //    parameters.Add("CompanyCode", CompanyCode);
        //    parameters.Add("UomCode", UomCode);
        //    parameters.Add("Status", "");
        //    var affectedRows = await _itemUomRepository.GetAsync("USP_S_M_UOM", parameters, commandType: CommandType.StoredProcedure);
        //    if (affectedRows != null)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        //public async Task<bool> checkExist(string ItemCode, string UomCode, string Barcode)
        //{
        //    var parameters = new DynamicParameters();

        //    //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
        //    //model.ShiftId = key;
        //    parameters.Add("ItemCode", ItemCode);
        //    parameters.Add("UomCode", UomCode);
        //    parameters.Add("Barcode", Barcode);
        //    parameters.Add("Status", "");
        //    var affectedRows = await _itemUomRepository.GetAsync("USP_S_M_ItemUOM", parameters, commandType: CommandType.StoredProcedure);
        //    if (affectedRows != null)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        public async Task<GenericResult> Create(MItemRejectPayment model)
        {
            GenericResult result = new GenericResult();

            try
            {
                //var itemexist = await GetItemInfor("", model.ItemCode,"","","","","");
                //if (itemexist == null || itemexist.Count <= 0)
                //{
                //    result.Success = false;
                //    result.Message = model.ItemCode + " not existed.";
                //    return result;
                //}
                //var uomexist = await checkUomExist("", model.UomCode);
                //if (uomexist == false)
                //{
                //    result.Success = false;
                //    result.Message =   model.UomCode + " not existed.";
                //    return result;
                //}

                //var exist = await checkExist(model.ItemCode, model.UomCode, model.BarCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.ItemCode + " - " + model.UomCode + " existed.";
                //    return result;
                //}
                 
                var parameters = new DynamicParameters(); 
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("ItemCode", model.ItemCode);
                parameters.Add("PaymentType", model.PaymentType);  
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status); 
                var affectedRows = _itemRejectPaymentRepository.Insert("USP_I_M_ItemRejectPayment", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Delete(MItemRejectPayment model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Id", model.Id);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("ItemCode", model.ItemCode);
                parameters.Add("PaymentType", model.PaymentType); 
                var affectedRows = _itemRejectPaymentRepository.Update("USP_D_M_ItemRejectPayment", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string CompanyCode, string ItemCode, string Status)
        {
            GenericResult result = new GenericResult();
            try
            { 
                var parameters = new DynamicParameters(); 
                parameters.Add("CompanyCode",  CompanyCode);
                parameters.Add("ItemCode", ItemCode); 
                parameters.Add("Status", Status); 
                var data = _itemRejectPaymentRepository.GetAllAsync("USP_S_M_ItemRejectPayment", parameters, commandType: CommandType.StoredProcedure);
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

    
        public async Task<GenericResult> Update(MItemRejectPayment model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters(); 
                parameters.Add("Id", model.Id);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("ItemCode", model.ItemCode);
                parameters.Add("PaymentType", model.PaymentType);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                var affectedRows = _itemRejectPaymentRepository.Update("USP_U_M_ItemRejectPayment", parameters, commandType: CommandType.StoredProcedure);
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
