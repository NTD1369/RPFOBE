
using AutoMapper;
using Dapper;
using DevExpress.Charts.Native;
using DevExpress.Printing.Utils.DocumentStoring;
using DevExpress.Xpo.Helpers;
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
using static iTextSharp.tool.xml.html.table.TableRowElement;

namespace RPFO.Application.Implements
{
    public class TableInforService : ITableInforService
    {
        private readonly IGenericRepository<MTableInfor> _tableRepository;
        private readonly IGenericRepository<MTablePlace> _tablePlaceRepository;
        private readonly IGenericRepository<MPlaceInfor> _placeRepository;

        private readonly IMapper _mapper;
        public TableInforService(IGenericRepository<MTableInfor> tableRepository,
            IGenericRepository<MTablePlace> tablePlaceRepository,
             IGenericRepository<MPlaceInfor> placeRepository,
        IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _tableRepository = tableRepository;
            _tablePlaceRepository = tablePlaceRepository;
            _placeRepository = placeRepository;
            _mapper = mapper;
        }

        public async Task<GenericResult> Create(MTableInfor model)
        {
            GenericResult result = new GenericResult();
            if (string.IsNullOrEmpty(model.TableName))
            {
                result.Success = false;
                result.Message = $"Table name is required";

                return await Task.FromResult(result);
            }

            var isTableInfor = await _tableRepository.GetAsync($"select * from M_TableInfor with (nolock) where CompanyCode = '{model.CompanyCode}' and  TableName='{model.TableName.ToUpper()}' and StoreId='{model.StoreId}'", null, commandType: CommandType.Text);
            if (isTableInfor != null)
            {
                result.Success = false;
                result.Message = $"{isTableInfor.TableName} already exists in system";
            }
            else
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", model.CompanyCode);
                    parameters.Add("StoreId", model.StoreId);
                    parameters.Add("TableName", model.TableName.ToUpper());
                    parameters.Add("Description", model.Description);
                    parameters.Add("Height", model.Height);
                    parameters.Add("Width", model.Width);
                    parameters.Add("Longs", model.Longs);
                    parameters.Add("Slot", model.Slot);
                    parameters.Add("Remark", model.Remark);
                    parameters.Add("CustomField1", model.CustomField1);
                    parameters.Add("CustomField2", model.CustomField2);
                    parameters.Add("CustomField3", model.CustomField3);
                    parameters.Add("CustomField4", model.CustomField4);
                    parameters.Add("CustomField5", model.CustomField5);
                    parameters.Add("CustomField4", model.CustomField4);
                    parameters.Add("CreatedBy", model.CreatedBy);
                    parameters.Add("Status", model.Status);
                    parameters.Add("DonViDoDai", model.DonViDoDai);

                    var affectedRows = _tableRepository.Insert("USP_I_M_TableInfor", parameters, commandType: CommandType.StoredProcedure);
                    var data = await _tableRepository.GetAsync($"select * from M_TableInfor with (nolock) where CompanyCode = '{model.CompanyCode}' and  TableName='{model.TableName.ToUpper()}' and StoreId='{model.StoreId}'", null, commandType: CommandType.Text);

                    result.Data = data;
                    result.Success = true;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }

            return await Task.FromResult(result);
        }

        public async Task<GenericResult> Update(MTableInfor model)
        {
            GenericResult result = new GenericResult();
            if (string.IsNullOrEmpty(model.TableName))
            {
                result.Message = $"Table name is required";
                result.Success = false;
                return await Task.FromResult(result);
            }

            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("TableId", model.TableId);
                parameters.Add("TableName", model.TableName.ToUpper());
                parameters.Add("Description", model.Description);
                parameters.Add("Height", model.Height);
                parameters.Add("Width", model.Width);
                parameters.Add("Longs", model.Longs);
                parameters.Add("Slot", model.Slot);
                parameters.Add("Remark", model.Remark);
                parameters.Add("CustomField1", model.CustomField1);
                parameters.Add("CustomField2", model.CustomField2);
                parameters.Add("CustomField3", model.CustomField3);
                parameters.Add("CustomField4", model.CustomField4);
                parameters.Add("CustomField5", model.CustomField5);
                parameters.Add("CustomField4", model.CustomField4);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                parameters.Add("DonViDoDai", model.DonViDoDai);
                var affectedRows = _tableRepository.Insert("USP_U_M_TableInfor", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return await Task.FromResult(result);
        }

        public async Task<GenericResult> Delete(MTableInfor model)
        {
            GenericResult result = new GenericResult();
            var m_TablePlaceQuery = $"select * from M_TablePlace where CompanyCode='{model.CompanyCode}' and StoreId='{model.StoreId}' and TableId='{model.TableId}'";
            var m_TablePlace = await _tablePlaceRepository.GetAllAsync(m_TablePlaceQuery, null, commandType: CommandType.Text);

            if (m_TablePlace != null && m_TablePlace.Any())
            {
                result.Message = $"Table is activated using in place !!!";
                result.Success = false;
                return await Task.FromResult(result);
            }
            else
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", model.CompanyCode);
                    parameters.Add("StoreId", model.StoreId);
                    parameters.Add("TableId", model.TableId);
                    var affectedRows = _tableRepository.Execute("USP_D_M_TableInfor", parameters, commandType: CommandType.StoredProcedure);
                    result.Success = true;
                    //result.Message = key;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }

            return await Task.FromResult(result);
        }

        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, string Keyword)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                parameters.Add("TableId", "");
                parameters.Add("Keyword", Keyword);

                var execTableInfor = await _tableRepository.GetAllAsync($"USP_S_M_TableInfor", parameters, commandType: CommandType.StoredProcedure);

                result.Success = true;
                result.Data = execTableInfor;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string TableId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                parameters.Add("TableId", TableId);
                parameters.Add("Keyword", "");
                var data = _tableRepository.Get($"USP_S_M_TableInfor", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return await Task.FromResult(result);
        }

        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<TableReseachViewModel> resultlist = new List<TableReseachViewModel>();
            try
            {
                foreach (var item in model.TableInfor)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);

                    TableReseachViewModel itemRs = new TableReseachViewModel();
                    itemRs = _mapper.Map<TableReseachViewModel>(item);
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

        public async Task<TablePlaceViewModel> GetTableAndPlaceById(string companyCode, string store, string tableId, string placeId)
        {
            var result = new TablePlaceViewModel();
            using (IDbConnection db = _tablePlaceRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    try
                    {
                        string tablePrintQuery = $"select Distinct t2.TableName,t3.PlaceName from M_TablePlace t1 with (nolock) " +
                            $"join M_TableInfor t2 with (nolock) on t1.TableId = t2.TableId " +
                            $"join M_PlaceInfor t3 with (nolock) on t3.PlaceId = t1.PlaceId " +
                            $"where t1.PlaceId = '{placeId}' and t1.TableId = '{tableId}' and t1.CompanyCode = '{companyCode}' and t1.StoreId = '{store}' ";
                        result = (await db.QueryAsync<TablePlaceViewModel>(tablePrintQuery, null, commandType: CommandType.Text, commandTimeout: 3600)).FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
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
            }

            return await Task.FromResult(result);
        }

        public async Task<string> GetTableId(string companyCode, string storeId, string tableName)
        {
            var queryTableInfo = $"select top 1 TableId from M_TableInfor where CompanyCode = '{companyCode}' and StoreId='{storeId}' and TableName = '{tableName}'";
            string tableId = _tableRepository.GetScalar(queryTableInfo, null, commandType: CommandType.Text);

            return await Task.FromResult(tableId);
        }

        public async Task<MTableInfor> GetMTableInfor(string companyCode, string storeId, string tableName)
        {
            var queryTableInfo = $"select * from M_TableInfor where CompanyCode = '{companyCode}' and StoreId='{storeId}' and TableName = '{tableName}'";
            var m_TableInfor = await _tableRepository.GetAsync(queryTableInfo, null, commandType: CommandType.Text);
            return await Task.FromResult(m_TableInfor);
        }
    }
}
