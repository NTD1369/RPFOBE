using Dapper;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    //Create class use to run unit test.
    public partial class SaleService : ISaleService
    {
        public string RunGRABTest(SaleViewModel model)
        {
            if (string.IsNullOrEmpty(model.TransId) && model.DataSource == "GRAB")
            {
                var Ecomparameters = new DynamicParameters();
                Ecomparameters.Add("CompanyCode", model.CompanyCode);
                Ecomparameters.Add("OMSId", model.OMSId);
                string socheck = _saleHeaderRepository.GetScalar("USP_GetTransIdByOMSId", Ecomparameters, commandType: CommandType.StoredProcedure);
                if (!string.IsNullOrEmpty(socheck))
                {
                    model.TransId = socheck;
                }
            }
            return model.TransId;
        }

        public async Task<List<TSalesHeader>> GetUSP_S_T_SalesHeaderByOrderIdAsync(SaleViewModel model)
        {
            var OrderIdparameters = new DynamicParameters();
            OrderIdparameters.Add("CompanyCode", model.CompanyCode);
            OrderIdparameters.Add("StoreId", model.StoreId);
            OrderIdparameters.Add("OrderId", model.OrderId);

            var socheck = await _saleHeaderRepository.GetAllAsync("USP_S_T_SalesHeaderByOrderId", OrderIdparameters, commandType: CommandType.StoredProcedure);

            return await Task.FromResult(socheck);
        }

        public async Task<string> CheckOrderDataTest(string CompanyCode, string StoreId, string TransId, decimal? TotalAmount, decimal? LinesCount, decimal? QuantitySum)
        {
            string result = "";
            using (IDbConnection db = _saleHeaderRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string queryGetDatOrderCheck = $"select top 1 TotalAmount, Count(t2.LineId) SumLine, SUM(t2.Quantity) SumQty " +
                       $" from T_SalesHeader t1 left join T_SalesLine t2 on t1.CompanyCode = t2.CompanyCode and t1.StoreId = t2.StoreId and t1.TransId = t2.TransId " +
                       $" where t1.TransId = N'{TransId}' and t1.CompanyCode = N'{CompanyCode}' and t1.StoreId = N'{StoreId}' " +
                       $" group by t1.TotalAmount ";
                    var queryGetDatOrderCheckData = await db.QueryAsync<ResultCheck>(queryGetDatOrderCheck, null, commandType: CommandType.Text, commandTimeout: 3600);
                    //var queryGetDatOrderCheckData = await db.GetAsync(queryGetDatOrderCheck, null, commandType: CommandType.Text);
                    if (queryGetDatOrderCheckData != null)
                    {
                        var dataX = queryGetDatOrderCheckData as ResultCheck;
                        if (dataX != null && dataX.TotalAmount == TotalAmount && dataX.SumLine == LinesCount && dataX.SumQty == QuantitySum)
                        {
                            result = TransId;
                        }
                    }

                }
                catch (Exception ex)
                {
                    result = "";
                    //result.Data = failedlist;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }
            //result.Data = failedlist;
            return result;


        }

        public async Task<string> GetGetRoundingPaymentDifByDefCurStoreAsync(SaleViewModel model)
        {
            string currencyOff = _saleHeaderRepository.GetScalar($"select dbo.[fnc_GetRoundingPaymentDifByDefCurStore](N'{model.CompanyCode}','{model.StoreId}')", null, commandType: CommandType.Text);
            return await Task.FromResult(currencyOff);
        }
    }
}
