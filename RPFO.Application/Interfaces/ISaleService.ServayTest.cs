
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public partial interface ISaleService
    {
        string RunGRABTest(SaleViewModel model);
        Task<List<TSalesHeader>> GetUSP_S_T_SalesHeaderByOrderIdAsync(SaleViewModel model);
        Task<string> CheckOrderDataTest(string CompanyCode, string StoreId, string TransId, decimal? TotalAmount, decimal? LinesCount, decimal? QuantitySum);
        Task<string> GetGetRoundingPaymentDifByDefCurStoreAsync(SaleViewModel model);
    }
}
