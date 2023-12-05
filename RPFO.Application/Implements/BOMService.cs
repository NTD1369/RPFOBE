
using AutoMapper;
using Dapper;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
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
    public class BOMService : IBOMService
    {
        private readonly IGenericRepository<MBomheader> _bomHeaderRepository;
        private readonly IGenericRepository<MBomline> _bomLineRepository;

        private readonly IMapper _mapper;
        public BOMService(IGenericRepository<MBomheader> bomHeaderRepository, IGenericRepository<MBomline> bomLineRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _bomHeaderRepository = bomHeaderRepository;
            _bomLineRepository = bomLineRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }
        public async Task<GenericResult> BOMImport(BOMDataImport model)
        {
            GenericResult result = new GenericResult();
            List<BOMResultViewModel> failedlist = new List<BOMResultViewModel>();
            try
            {
                foreach (var item in model.Data)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var storeId = model.StoreId;
                    result = await CreateBomV1(item, storeId);
                    if (result.Success == false)
                    {
                        BOMResultViewModel itemRs = new BOMResultViewModel();
                        itemRs = _mapper.Map<BOMResultViewModel>(item);
                        itemRs.Success = result.Success;
                        itemRs.Message = result.Message;
                        failedlist.Add(itemRs);
                    }
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                result.Data = failedlist;
            }
            result.Data = failedlist;
            return result;
        }
        public async Task<GenericResult> CreateBomV1(BOMViewModel model, string storeId)
        {
            GenericResult result = new GenericResult();
            try
            {

                using (IDbConnection db = _bomHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {
                                var checkparameters = new DynamicParameters();
                                checkparameters.Add("CompanyCode", model.CompanyCode);
                                checkparameters.Add("StoreId", storeId);
                                checkparameters.Add("ItemCode", model.ItemCode);
                                checkparameters.Add("UOMCode", model.UomCode);
                                checkparameters.Add("BarCode", "");
                                checkparameters.Add("Keyword", "");
                                checkparameters.Add("Merchandise", "");
                                checkparameters.Add("Type", "");

                                var items = db.Query("USP_CheckItemBom", checkparameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                if (items.Count() <= 0)
                                {
                                    tran.Rollback();
                                    result.Success = false;
                                    result.Message = "Item code not found.";
                                    return result;
                                }

                                var parameters = new DynamicParameters();
                                parameters.Add("ItemCode", model.ItemCode);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("ItemName", model.ItemName);
                                parameters.Add("Quantity", model.Quantity);
                                parameters.Add("UOMCode", model.UomCode);
                                parameters.Add("CreatedBy", model.CreatedBy);
                                parameters.Add("Status", model.Status);

                                var affectedRows = db.Execute("USP_I_M_BOMHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                string query = $"update  M_Item set IsBom = 1 where ItemCode = '{model.ItemCode}' and CompanyCode='{model.CompanyCode}'";
                                var updateAffectedRows = db.Execute(query, parameters, commandType: CommandType.Text, transaction: tran);

                                int stt = 0;
                                foreach (var line in model.Lines)
                                {
                                    stt++;
                                    parameters = new DynamicParameters();
                                    parameters.Add("BOMId", model.ItemCode);
                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("ItemCode", line.ItemCode);
                                    parameters.Add("ItemName", line.ItemName);
                                    parameters.Add("UOMCode", line.UomCode);
                                    parameters.Add("Quantity", line.Quantity);
                                    parameters.Add("IsOption", line.IsOption);
                                    parameters.Add("OptionGroup", line.OptionGroup);
                                    parameters.Add("TriggerStatus", null);
                                    parameters.Add("TriggerSystem", null);
                                    parameters.Add("CreatedBy", model.CreatedBy);
                                    parameters.Add("Status", line.Status);

                                    db.Execute("usp_I_M_BOMLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                }
                                result.Success = true;
                                //result.Message = key;
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
        public async Task<GenericResult> Create(BOMViewModel model)
        {
            GenericResult result = new GenericResult();
            try
            {
                using (IDbConnection db = _bomHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {
                                var checkparameters = new DynamicParameters();
                                checkparameters.Add("CompanyCode", model.CompanyCode);
                                checkparameters.Add("ItemCode", model.ItemCode);
                                checkparameters.Add("UOMCode", model.UomCode);
                                checkparameters.Add("BarCode", "");
                                var items = db.Query("USP_GetItem", checkparameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                if (items.Count() <= 0)
                                {
                                    tran.Rollback();
                                    result.Success = false;
                                    result.Message = "Item code not found.";
                                    return result;
                                }
                                var parameters = new DynamicParameters();
                                parameters.Add("ItemCode", model.ItemCode);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("ItemName", model.ItemName);
                                parameters.Add("Quantity", model.Quantity);
                                parameters.Add("UOMCode", model.UomCode);
                                parameters.Add("CreatedBy", model.CreatedBy);
                                parameters.Add("Status", model.Status);

                                var affectedRows = db.Execute("USP_I_M_BOMHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                int stt = 0;
                                foreach (var line in model.Lines)
                                {
                                    stt++;
                                    parameters = new DynamicParameters();
                                    parameters.Add("BOMId", model.ItemCode);
                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("ItemCode", line.ItemCode);
                                    parameters.Add("ItemName", line.ItemName);
                                    parameters.Add("UOMCode", line.UomCode);
                                    parameters.Add("Quantity", line.Quantity);
                                    parameters.Add("IsOption", line.IsOption);
                                    parameters.Add("OptionGroup", line.OptionGroup);
                                    parameters.Add("CreatedBy", line.CreatedBy);
                                    parameters.Add("Status", line.Status);

                                    db.Execute("usp_I_M_BOMLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                }
                                result.Success = true;
                                //result.Message = key;
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
        public async Task<GenericResult> Delete(string CompanyCode, string ItemCode)
        {

            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ItemCode", ItemCode);
                parameters.Add("CompanyCode", CompanyCode);
                _bomLineRepository.Execute("USP_D_M_BOMHeader", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> DeleteLine(string Id, string CompanyCode, string BomId)
        {

            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Id", Id);
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("BOMId", BomId);
                _bomLineRepository.Execute("USP_D_M_BOMLine", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                return result;
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
                List<BOMViewModel> list = new List<BOMViewModel>();
                var listdata = await _bomHeaderRepository.GetAllAsync($"select * from M_BOMHeader with (nolock) where CompanyCode =N'{CompanyCode}'", null, commandType: CommandType.Text);
                list = _mapper.Map<List<BOMViewModel>>(listdata);
                result.Success = true;
                result.Data = list;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> GetByItemCode(string CompanyCode, string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                BOMViewModel BOM = new BOMViewModel();

                MBomheader header = await _bomHeaderRepository.GetAsync($"select * from M_BOMHeader with (nolock) where CompanyCode =N'{CompanyCode}' and ItemCode='{Code}'", null, commandType: CommandType.Text);

                List<MBomline> lines = await _bomLineRepository.GetAllAsync($"select * from M_BOMLine with (nolock) where CompanyCode =N'{CompanyCode}' and BOMId='{Code}'", null, commandType: CommandType.Text);

                if (header == null)
                {
                    result.Success = true;
                    result.Data = null;
                    return result;
                }

                BOM = _mapper.Map<BOMViewModel>(header);
                BOM.Lines = lines;

                result.Success = true;
                result.Data = BOM;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<PagedList<BOMViewModel>> GetPagedList(UserParams userParams)
        {
            try
            {
                List<BOMViewModel> bomViewModels = new List<BOMViewModel>();

                var mBOMHeader = await _bomHeaderRepository.GetAllAsync($"select * from M_BOMHeader with (nolock)", null, commandType: CommandType.Text);
                bomViewModels = _mapper.Map<List<BOMViewModel>>(mBOMHeader);
                foreach (var bomViewModel in bomViewModels)
                {
                    var mBOMLine = await _bomLineRepository.GetAllAsync($"select * from M_BOMLine where BomId='{bomViewModel.ItemCode}'", null, commandType: CommandType.Text);
                    bomViewModel.Lines.AddRange(mBOMLine);
                }

                return await PagedList<BOMViewModel>.Create(bomViewModels, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<GenericResult> CreateLine(MBomline line)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("BOMId", line.Bomid);
                parameters.Add("CompanyCode", line.CompanyCode);
                parameters.Add("ItemCode", line.ItemCode);
                parameters.Add("ItemName", line.ItemName);
                parameters.Add("UOMCode", line.UomCode);
                parameters.Add("Quantity", line.Quantity);
                parameters.Add("IsOption", line.IsOption);
                parameters.Add("OptionGroup", line.OptionGroup);
                parameters.Add("CreatedBy", line.CreatedBy);
                parameters.Add("Status", line.Status);
                parameters.Add("TriggerStatus", line.TriggerStatus);
                parameters.Add("TriggerSystem", line.TriggerSystem);
                _bomLineRepository.Insert("usp_I_M_BOMLine", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> UpdateLine(MBomline line)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Id", line.Id);
                parameters.Add("BOMId", line.Bomid);
                parameters.Add("CompanyCode", line.CompanyCode);
                parameters.Add("ItemCode", line.ItemCode);
                parameters.Add("ItemName", line.ItemName);
                parameters.Add("UOMCode", line.UomCode);
                parameters.Add("Quantity", line.Quantity);
                parameters.Add("IsOption", line.IsOption);
                parameters.Add("OptionGroup", line.OptionGroup);
                parameters.Add("ModifiedBy", line.ModifiedBy);
                parameters.Add("Status", line.Status);
                parameters.Add("TriggerStatus", line.TriggerStatus);
                parameters.Add("TriggerSystem", line.TriggerSystem);

                _bomLineRepository.Update("usp_U_M_BOMLine", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> Update(BOMViewModel model)
        {
            GenericResult result = new GenericResult();
            try
            {

                using (IDbConnection db = _bomHeaderRepository.GetConnection())
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
                                parameters.Add("ItemCode", model.ItemCode);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("ItemName", model.ItemName);
                                parameters.Add("Quantity", model.Quantity);
                                parameters.Add("UOMCode", model.UomCode);
                                parameters.Add("CreatedBy", model.CreatedBy);
                                parameters.Add("Status", model.Status);

                                var affectedRows = db.Execute("USP_U_M_BOMHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                int stt = 0;
                                foreach (var line in model.Lines)
                                {
                                    stt++;
                                    parameters = new DynamicParameters();
                                    parameters.Add("BOMId", model.ItemCode);
                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("ItemCode", line.ItemCode);
                                    parameters.Add("ItemName", line.ItemName);
                                    parameters.Add("UOMCode", line.UomCode);
                                    parameters.Add("Quantity", line.Quantity);
                                    parameters.Add("IsOption", line.IsOption);
                                    parameters.Add("OptionGroup", line.OptionGroup);
                                    parameters.Add("CreatedBy", line.CreatedBy);
                                    parameters.Add("Status", line.Status);
                                    db.Execute("usp_U_M_BOMLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                }
                                result.Success = true;
                                //result.Message = key;
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
    }
}
