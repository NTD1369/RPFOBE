using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IProductionOrderService
    {
        Task<GenericResult> GetById(string id);
        Task<GenericResult> GetOrderById(string id, string companycode, string storeId);
        Task<GenericResult> GetByType(string companycode, string storeId, string fromdate, string todate, string key, string status);
    }
}
