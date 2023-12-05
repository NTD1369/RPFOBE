using AutoMapper;
using Dapper;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class ProductionOrderService: IProductionOrderService
    {
        private readonly IGenericRepository<TProductionOrderHeader> _ProductionOrderHeaderlRepository;
        private readonly IGenericRepository<TProductionOrderLine> _ProductionOrderLineRepository;
        IMapper _mapper;


        public ProductionOrderService(IGenericRepository<TProductionOrderHeader> productionOrderHeaderlRepository, IGenericRepository<TProductionOrderLine> productionOrderLineRepository, IMapper mapper)
        {
            _ProductionOrderHeaderlRepository = productionOrderHeaderlRepository;
            _ProductionOrderLineRepository = productionOrderLineRepository;
            _mapper = mapper;
        }

     

        public Task<GenericResult> GetById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetByType(string companycode, string storeId, string fromdate, string todate, string key, string status)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", companycode);
                parameters.Add("storeId", storeId);
                parameters.Add("Status", status);
                parameters.Add("FrDate", fromdate);
                parameters.Add("ToDate", todate);
                parameters.Add("Keyword", key);
                //parameters.Add("ViewBy", ViewBy); 

                var data = await _ProductionOrderHeaderlRepository.GetAllAsync($"USP_GetProductionOrder", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetOrderById(string id, string companycode, string storeId)
        {
            GenericResult result = new GenericResult();
            try
            {
                ProductionOrderViewModel order = new ProductionOrderViewModel();

                TProductionOrderHeader header = await _ProductionOrderHeaderlRepository.GetAsync($"select * from T_ProductionOrderHeader with (nolock) where PurchaseId='{id}' and CompanyCode= '{companycode}' and StoreId= '{storeId}'", null, commandType: CommandType.Text);

                string queryLine = $"select t1.*  from T_ProductionOrderLine t1 with(nolock)  where t1.PurchaseId = '{id}' and t1.CompanyCode = '{companycode}'";
              

                //List<TPurchaseOrderLine> lines = await _poLineRepository.GetAllAsync(, null, commandType: CommandType.Text);

                //List<TPurchaseOrderPayment> payments = await _popaymentLineRepository.GetAllAsync(queryPayment, null, commandType: CommandType.Text);

                //var customer = await _customerRepository.GetAsync($"select * from M_Customer with (nolock) where CustomerId ='{header.CusId}'", null, commandType: CommandType.Text);

                var head = _mapper.Map<ProductionOrderViewModel>(header);
                using (IDbConnection db = _ProductionOrderHeaderlRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        var lines = db.Query<TProductionOrderLine>(queryLine, null, commandType: CommandType.Text);
                       
                        order = _mapper.Map<ProductionOrderViewModel>(header);
                        order.Lines = lines.ToList();
                       
                        //order.PromoLines = promoLines.ToList();
                        //order.Payments = payments;
                        //order.Customer = customer;
                        result.Data = order;
                        result.Success = true;

                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.Message = ex.Message;

                    }
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
