
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
    public class ItemUomService : IItemUomService
    {
        private readonly IGenericRepository<MItemUom> _itemUomRepository;
        private readonly IGenericRepository<MItem> _itemRepository;
        private readonly IGenericRepository<MUom> _uomRepository;
        private readonly IMapper _mapper;
        public ItemUomService(IGenericRepository<MItemUom> itemUomRepository, IGenericRepository<MItem> itemRepository, IGenericRepository<MUom> uomRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _itemUomRepository = itemUomRepository;
            _mapper = mapper; 
            _itemRepository = itemRepository;
            _uomRepository = uomRepository;

        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<IItemUomResultViewModel> resultlist = new List<IItemUomResultViewModel>();
            try
            {
                foreach (var item in model.ItemUom)
                {
                    //item.CreatedBy = model.CreatedBy;
                    //item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);
                  
                        IItemUomResultViewModel itemRs = new IItemUomResultViewModel();
                        itemRs = _mapper.Map<IItemUomResultViewModel>(item);
                        itemRs.Success = itemResult.Success;
                        itemRs.Message = itemResult.Message;
                        resultlist.Add(itemRs);
                     
                }
                result.Success = true; 
                result.Data = resultlist;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                //result.Data = failedlist;
            }
            
            return result;
        }
        public async Task<List<ItemViewModel>> GetItemInfor(string CompanyCode, string ItemCode, string UomCode, string BarCode, string Keyword, string Merchandise, string Type)
        {
            using (IDbConnection db = _itemRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode);
                    parameters.Add("ItemCode", string.IsNullOrEmpty(ItemCode) ? "" : ItemCode);
                    parameters.Add("UomCode", string.IsNullOrEmpty(UomCode) ? "" : UomCode);
                    parameters.Add("BarCode", string.IsNullOrEmpty(BarCode) ? "" : BarCode);
                    parameters.Add("Keyword", string.IsNullOrEmpty(Keyword) ? "" : Keyword);
                    parameters.Add("Merchandise", string.IsNullOrEmpty(Merchandise) ? "" : Merchandise);
                    parameters.Add("Type", string.IsNullOrEmpty(Type) ? "" : Type);
                    var dblist = await db.QueryAsync<ItemViewModel>($"USP_GetItemInfor", parameters, commandType: CommandType.StoredProcedure);
                   
                    db.Close();
                    return dblist.ToList();

                }
                catch (Exception ex)
                {
                    db.Close();
                    return null;
                }
            }

        }
        public async Task<bool> checkUomExist(string CompanyCode, string UomCode)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("UomCode", UomCode);
            parameters.Add("Status", "");
            var affectedRows = await _itemUomRepository.GetAsync("USP_S_M_UOM", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> checkExist(string ItemCode, string UomCode, string Barcode)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("ItemCode", ItemCode);
            parameters.Add("UomCode", UomCode);
            parameters.Add("Barcode", Barcode);
            parameters.Add("Status", "");
            var affectedRows = await _itemUomRepository.GetAsync("USP_S_M_ItemUOM", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MItemUom model)
        {
            GenericResult result = new GenericResult();

            try
            {
                var itemexist = await GetItemInfor("", model.ItemCode,"","","","","");
                if (itemexist == null || itemexist.Count <= 0)
                {
                    result.Success = false;
                    result.Message = model.ItemCode + " not existed.";
                    return result;
                }
                var uomexist = await checkUomExist("", model.UomCode);
                if (uomexist == false)
                {
                    result.Success = false;
                    result.Message =   model.UomCode + " not existed.";
                    return result;
                }

                var exist = await checkExist(model.ItemCode, model.UomCode, model.BarCode);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.ItemCode + " - " + model.UomCode + " existed.";
                    return result;
                }
                 
                var parameters = new DynamicParameters();
 

                parameters.Add("ItemCode", model.ItemCode);
                parameters.Add("UOMCode", model.UomCode);
                parameters.Add("Factor", model.Factor); 
                parameters.Add("BarCode", model.BarCode); 
                parameters.Add("QRCode", model.QrCode);  
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                 
                var affectedRows = _itemUomRepository.Insert("USP_I_M_ItemUOM", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _itemUomRepository.GetAllAsync($"select * from M_ItemUOM with (nolock) where CompanyCode = N'{CompanyCode}'", null, commandType: CommandType.Text);
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

        public async Task<GenericResult> GetByBarcode(string CompanyCode, string barCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _itemUomRepository.GetAsync($"select * from M_ItemUOM with (nolock) where CompanyCode = N'{CompanyCode}' and barCode='{barCode}'", null, commandType: CommandType.Text);
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

        public async Task<GenericResult> GetByCode(string CompanyCode, string itemCode, string uomCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _itemUomRepository.GetAsync($"select * from M_ItemUOM with (nolock) where CompanyCode = N'{CompanyCode}' and ItemCode = '{itemCode}' and UOMCode='{uomCode}'", null, commandType: CommandType.Text);
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

        public async Task<GenericResult> GetByItem(string CompanyCode, string ItemCode)
        {
            GenericResult result = new GenericResult();
            List<ItemUOMViewModel> lst = new List<ItemUOMViewModel>();
            using (IDbConnection db = _itemUomRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string query = $" select t1.* , t2. UOMName from M_ItemUOM t1 with(nolock) left join M_UOM t2 with(nolock) on t1.UomCode = t2.UomCode where t1.ItemCode = '{ItemCode}'";
                    var data = await db.QueryAsync<ItemUOMViewModel>(query, null);
                    lst = data.ToList();
                    db.Close();
                    result.Success = true;
                    result.Data = lst;
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                }
              
                
            }
            return result;
        }

        public async Task<PagedList<MItemUom>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _itemUomRepository.GetAllAsync($"select * from M_ItemUOM with (nolock)", null, commandType: CommandType.Text);
             
                return await PagedList<MItemUom>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        public async Task<GenericResult> Update(MItemUom model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("ItemCode", model.ItemCode);
                parameters.Add("UOMCode", model.UomCode);
                parameters.Add("Factor", model.Factor);
                parameters.Add("BarCode", model.BarCode);
                parameters.Add("QRCode", model.QrCode);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                var affectedRows = _itemUomRepository.Update("USP_U_M_ItemUOM", parameters, commandType: CommandType.StoredProcedure);
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
