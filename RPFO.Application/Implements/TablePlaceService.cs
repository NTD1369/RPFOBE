
using AutoMapper;
using Dapper;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Utilities.Dtos;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class TablePlaceService : ITablePlaceService
    {
        private readonly IGenericRepository<MTablePlace> _tableRepository;
        private readonly IGenericRepository<TSalesHeader> _saleHeaderRepository;
        private readonly IMapper _mapper;
        public TablePlaceService(IGenericRepository<MTablePlace> tableRepository,
            IGenericRepository<TSalesHeader> saleHeaderRepository,
            IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _tableRepository = tableRepository;
            _saleHeaderRepository = saleHeaderRepository;
            _mapper = mapper;
        }

        public async Task<GenericResult> Create(MTablePlace model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("TableId", model.TableId);
                parameters.Add("PlaceId", model.PlaceId);
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
                parameters.Add("DonViDoDai", model.DonViDoDai);

                parameters.Add("CustomField4", model.CustomField4);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                parameters.Add("Type", model.Type);




                parameters.Add("UrlImage", model.UrlImage);
                //var exist = await checkExist(model.CompanyCode, model.UomCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.UomCode + " existed.";
                //    return result;
                //}
                var affectedRows = _tableRepository.Insert("USP_I_M_TablePlace", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Delete(MTablePlace model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("TableId", model.TableId);
                var affectedRows = _tableRepository.Execute("USP_D_M_TablePlace", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, string ShiftId, string PlaceId, string Keyword, string IsSetup, string IsDesign)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                parameters.Add("PlaceId", PlaceId);
                parameters.Add("ShiftId", ShiftId);
                parameters.Add("TableId", "");
                parameters.Add("IsSetup", IsSetup);
                parameters.Add("IsDesign", IsDesign);

                var data = await _tableRepository.GetAllAsync($"USP_S_M_TablePlace", parameters, commandType: CommandType.StoredProcedure);

                if (IsSetup == "N")
                {
                    var tableNotInGrp = data.Where(x => (x.OrderCustomF2 == null ? "" : x.OrderCustomF2) == "").ToList();
                    var tableInGrp = data.Where(x => (x.OrderCustomF2 == null ? "" : x.OrderCustomF2) != "").ToList();

                    foreach (var table in tableNotInGrp)
                    {
                        //string GrpS = table.TableId;
                        string Query = $" select top 1 TransId from T_SalesHeader with (nolock) " +
                          $" where CompanyCode = N'{CompanyCode}'  and StoreId = N'{StoreId}' and ContractNo = N'{table.TableId}' and CustomF1 = N'{table.PlaceId}' and SalesType = 'Retail' and isnull(Status,'') = 'H' and isnull(IsCanceled,'')  = 'N'";


                        string TransId = _tableRepository.GetScalar(Query, null, commandType: CommandType.Text);
                        if (!string.IsNullOrEmpty(TransId))
                        {
                            table.TransId = TransId;

                        }
                    }
                    foreach (var table in tableInGrp)
                    {
                        //string GrpS = table.TableId;
                        string Query = $" select top 1 TransId from T_SalesHeader with (nolock) " +
                          $" where CompanyCode = N'{CompanyCode}'  and StoreId = N'{StoreId}' and ContractNo = N'{table.TableId}' and CustomF1 = N'{table.PlaceId}' and SalesType = 'Retail' and isnull(Status,'') = 'H' and isnull(IsCanceled,'')  = 'N'";


                        string TransId = _tableRepository.GetScalar(Query, null, commandType: CommandType.Text);
                        if (!string.IsNullOrEmpty(TransId))
                        {
                            //table.TransId = TransId;
                            var tableIngroup = data.Where(x => x.OrderCustomF2 == table.OrderCustomF2 && string.IsNullOrEmpty(x.TransId));
                            foreach (var tableX in tableIngroup)
                            {
                                tableX.TransId = TransId;
                            }
                        }
                    }
                }

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

        public async Task<GenericResult> GetAllTableNoActiveInPlace(string CompanyCode, string StoreId, string ShiftId, string PlaceId, string Keyword)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                parameters.Add("PlaceId", PlaceId);
                parameters.Add("ShiftId", ShiftId);
                parameters.Add("TableId", "");
                parameters.Add("IsSetup", "Y");
                parameters.Add("IsDesign", "Y");

                var data = await _tableRepository.GetAllAsync($"USP_S_M_TablePlace", parameters, commandType: CommandType.StoredProcedure);

                var parametersTable = new DynamicParameters();
                parametersTable.Add("CompanyCode", CompanyCode);
                parametersTable.Add("StoreId", StoreId);
                parametersTable.Add("PlaceId", PlaceId);
                var qTableActive = await _tableRepository.GetAllAsync($"USP_S_M_TablePlaceNoActive", parametersTable, commandType: CommandType.StoredProcedure);
                var tableUseOrNotUser = qTableActive.Select(x => x.TableName);

                result.Success = true;
                result.Data = data.Where(c => tableUseOrNotUser.Contains(c.TableName));
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> ApplyTableInPlace(string companyCode, string storeId, string placeId, string tableId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", companyCode);
                parameters.Add("StoreId", storeId);
                parameters.Add("PlaceId", placeId);
                parameters.Add("TableId", tableId);
                parameters.Add("CreatedBy", new DateTime());

                var data = _tableRepository.Execute($"USP_Apply_TablePlace", parameters, commandType: CommandType.StoredProcedure);

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

        public async Task<GenericResult> Apply(MTablePlace model)
        {
            GenericResult result = new GenericResult();

            var t_SalesHeaders = _saleHeaderRepository.Get($"select * from T_SalesHeader where ContractNo= N'{model.TableId}' and Status= 'C' and IsCanceled= 'N' and CompanyCode= N'{model.CompanyCode}' and StoreId=N'{model.StoreId}' and CustomF1=N'{model.PlaceId}' and CustomF4=N'NoPaymentOfTable'", null, commandType: CommandType.Text);
            if (t_SalesHeaders != null)
            {
                result.Message = $"Please check for tables that have orders";
                result.Success = false;
                return await Task.FromResult(result);
            }

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("PlaceId", model.PlaceId);
                parameters.Add("TableId", model.TableId);
                parameters.Add("CreatedBy", model.CreatedBy);

                var data = _tableRepository.Execute($"USP_Apply_TablePlace", parameters, commandType: CommandType.StoredProcedure);

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

        public async Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string ShiftId, string PlaceId, string TableId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                parameters.Add("ShiftId", ShiftId);
                parameters.Add("TableId", TableId);
                parameters.Add("PlaceId", PlaceId);
                var data = _tableRepository.Get($"USP_S_M_TablePlace", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Update(MTablePlace model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("TableId", model.TableId);
                parameters.Add("PlaceId", model.PlaceId);
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
                parameters.Add("DonViDoDai", model.DonViDoDai);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                parameters.Add("Type", model.Type);
                parameters.Add("UrlImage", model.UrlImage);

                var affectedRows = _tableRepository.Insert("USP_U_M_TablePlace", parameters, commandType: CommandType.StoredProcedure);
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
