using Dapper;
using RPFO.Application.Interfaces;
using RPFO.Data.EntitiesMWI;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class StoreWareHouseService : IStoreWareHouseService
    {
        private readonly IGenericRepository<M_StoreWarehouse> _StoreWareHouseRepository;
        private readonly IGenericRepository<MWarehouse> _warehouseRepository;

        public StoreWareHouseService(IGenericRepository<M_StoreWarehouse> storeWareHouseRepository, IGenericRepository<MWarehouse> warehouseRepository = null)
        {
            _StoreWareHouseRepository = storeWareHouseRepository;
            _warehouseRepository = warehouseRepository;
        }

        public async Task<GenericResult> Create(MStoreWarehouseModel model)
        {
            GenericResult result = new GenericResult();
            string query = $"select * from M_StoreWarehouse with (nolock)";
            var data = await _StoreWareHouseRepository.GetAllAsync(query, null, commandType: CommandType.Text);
            for (int i = 0; i < data.Count; i++)
            {
                if(data[i].WareHouseID ==model.MWareHouseID || data[i].WareHouseID == model.TWareHouseID)
                {
                    result.Message = result.Message +","+ data[i].WareHouseID;
                    //break;
                }
                if(model.OWareHouseID!=null)
                foreach(var item in model.OWareHouseID)
                {
                    if (data[i].WareHouseID == item)
                    {
                        result.Message = result.Message + ","  + data[i].WareHouseID;
                        //break;
                    }
                }
            }
            if(result.Message !=null)
            {
                result.Success = false;
                result.Message = result.Message.Substring(1) + " Already Exit ";
                return result;
            }
            try
            {

                using (IDbConnection db = _StoreWareHouseRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {
                                var parameters = new DynamicParameters();
                                parameters.Add("StoreID", model.StoreID);
                                parameters.Add("WareHouseID", model.MWareHouseID);
                                parameters.Add("MappingType", "Main");

                                db.Execute("USP_I_M_StoreWarehouse", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                 parameters = new DynamicParameters();
                                parameters.Add("StoreID", model.StoreID);
                                parameters.Add("WareHouseID", model.TWareHouseID);
                                parameters.Add("MappingType", "Transit");
                                db.Execute("USP_I_M_StoreWarehouse", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                if(model.OWareHouseID!=null)
                                foreach (var item in model.OWareHouseID)
                                {
                                    parameters = new DynamicParameters();
                                    parameters.Add("StoreID", model.StoreID);
                                    parameters.Add("WareHouseID",item);
                                    parameters.Add("MappingType", "Other");
                                    db.Execute("USP_I_M_StoreWarehouse", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                }

                                result.Success = true;
                                tran.Commit();

                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                throw ex;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;

        }

        public async Task<GenericResult> Delete(string StoreID)
        {
            GenericResult result = new GenericResult();
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("StoreID", StoreID, DbType.String);
                var affectedRows = _StoreWareHouseRepository.Execute("USP_D_M_StoreWarehouse", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetByStoreID(string storeid)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _StoreWareHouseRepository.GetAllAsync($"select * from M_StoreWarehouse with (nolock) where StoreID ='{storeid}'", null, commandType: CommandType.Text);
                MStoreWarehouseModel model = new MStoreWarehouseModel();
                model.OWareHouseID = new List<string>();
                foreach (var item in data)
                {
                    model.StoreID = item.StoreID;
                    if (item.MappingType == "Main")
                        model.MWareHouseID = item.WareHouseID;
                    if(item.MappingType == "Transit")
                        model.TWareHouseID = item.WareHouseID;
                    if (item.MappingType == "Other")
                    {
                        model.OWareHouseID.Add(item.WareHouseID);
                    }
                        
                }

                result.Success = true;
                result.Data = model;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;

        }
        public async Task<GenericResult> GetAll(string storeid)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _StoreWareHouseRepository.GetAllAsync($"select * from M_StoreWarehouse with (nolock) where StoreID ='{storeid}'", null, commandType: CommandType.Text);
                MStoreWarehouseModel model = new MStoreWarehouseModel();
                List<MStoreWarehouseModel> mStore = new List<MStoreWarehouseModel>();
                model.OWareHouseID = new List<string>();
 
                foreach (var item in data)
                {
                    model.StoreID = item.StoreID;
                    if (item.MappingType == "Main")
                        model.MWareHouseID = item.WareHouseID;
                    if (item.MappingType == "Transit")
                        model.TWareHouseID = item.WareHouseID;
                    if (item.MappingType == "Other")
                    {
                        model.OWareHouseID.Add(item.WareHouseID);
                    }
                   
                }
                if(data.Count>0)
                mStore.Add(model);
                result.Success = true;
                result.Data = mStore;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;

        }
        public async Task<GenericResult> Update(MStoreWarehouseModel model)
        {
            GenericResult result = new GenericResult();
            try
            {

                using (IDbConnection db = _StoreWareHouseRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {
                                var isDelete = await Delete(model.StoreID);
                                if(isDelete.Success)
                                {
                                    var isUpdate = await Create(model);
                                    if(!isUpdate.Success)
                                    {
                                        result.Success = false;
                                        result.Message = isUpdate.Message;
                                        tran.Rollback();
                                        return result;
                                    }
                                }
                                else
                                {
                                    result = isDelete;
                                    return result;
                                }
                                result.Success = true;
                                tran.Commit();

                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                throw ex;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> GetWhsByStore(string CompanyCode, string storeid)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", storeid);
                parameters.Add("Status", "");
                var data = await _warehouseRepository.GetAllAsync("USP_S_M_WhsbyStore", parameters, commandType: CommandType.StoredProcedure);
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
    }
}
