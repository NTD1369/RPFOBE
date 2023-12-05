using AutoMapper;
using Dapper;
using RPFO.Application.Helpers;
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
    public class DeliveryReturnService : IDeliveryReturnService
    {
        private readonly IGenericRepository<TReturnHeader> _TDeliverReturnHeaderRepository;
        private readonly IGenericRepository<TReturnLine> _TDeliveryReturnLineRepository;
        private readonly IGenericRepository<TReturnLineSerial> _TDeliveryReturnLineSerialRepository;
        IMapper _mapper;

        public DeliveryReturnService(IGenericRepository<TReturnLine> deliveryReturnLineRepository, IGenericRepository<TReturnLineSerial> deliveryReturnLineSerialRepository, IGenericRepository<TReturnHeader> deliveryReturnHeaderlRepository, IMapper mapper = null)
        {

            _TDeliveryReturnLineRepository = deliveryReturnLineRepository;
            _TDeliveryReturnLineSerialRepository = deliveryReturnLineSerialRepository;
            _TDeliverReturnHeaderRepository = deliveryReturnHeaderlRepository;
            _mapper = mapper;
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

                var data = await _TDeliverReturnHeaderRepository.GetAllAsync($"USP_GetReturn", parameters, commandType: CommandType.StoredProcedure);
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
        public async Task<GenericResult> GetOrderById(string id, string CompanyCode, string StoreId)
        {

            GenericResult result = new GenericResult();
            try
            {
                GReturnDeliveryViewModel order = new GReturnDeliveryViewModel();

                TReturnHeader header = await _TDeliverReturnHeaderRepository.GetAsync($"select * from T_ReturnHeader with (nolock) where PurchaseId='{id}' and CompanyCode= '{CompanyCode}' and StoreId= '{StoreId}'", null, commandType: CommandType.Text);

                string queryLine = $"select t1.*  from T_ReturnLine t1 with(nolock)  where t1.PurchaseId = '{id}' and t1.CompanyCode = '{CompanyCode}'";
                string queryLineSerial = $"select t1.*   from T_ReturnLineSerial t1 with(nolock)   where t1.PurchaseId = '{id}' and t1.CompanyCode = '{CompanyCode}'";

                //List<TPurchaseOrderLine> lines = await _poLineRepository.GetAllAsync(, null, commandType: CommandType.Text);

                //List<TPurchaseOrderPayment> payments = await _popaymentLineRepository.GetAllAsync(queryPayment, null, commandType: CommandType.Text);

                //var customer = await _customerRepository.GetAsync($"select * from M_Customer with (nolock) where CustomerId ='{header.CusId}'", null, commandType: CommandType.Text);

                var head = _mapper.Map<GReturnDeliveryViewModel>(header);
                using (IDbConnection db = _TDeliverReturnHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        var lines = db.Query<TReturnLine>(queryLine, null, commandType: CommandType.Text);
                        var serialLines = db.Query<TReturnLineSerial>(queryLineSerial, null, commandType: CommandType.Text);
                        foreach (var line in lines)
                        {
                            line.SerialLines = serialLines.Where(x => x.ItemCode == line.ItemCode && x.UOMCode == line.UOMCode).ToList();
                        }
                        order = _mapper.Map<GReturnDeliveryViewModel>(header);
                        order.Lines = lines.ToList();
                        order.SerialLines = serialLines.ToList();
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







        public Task<GenericResult> Create(GReturnDeliveryViewModel model)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResult> Delete(string companycode, string storeId, string Code)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResult> GetAll(string CompanyCode)
        {
            throw new NotImplementedException();
        }

        public Task<TReturnHeader> GetById(string companycode, string storeId, string Id)
        {
            throw new NotImplementedException();
        }



        public Task<List<TReturnLine>> GetLinesById(string companycode, string storeId, string Id)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNewOrderCode(string companyCode, string storeId)
        {
            throw new NotImplementedException();
        }

 

        public Task<PagedList<TReturnHeader>> GetPagedList(UserParams userParams)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResult> Update(TReturnHeader model)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResult> UpdateStatus(GReturnDeliveryViewModel model)
        {
            throw new NotImplementedException();
        }

        Task<TGoodsReturnheader> IDeliveryReturnService.GetById(string companycode, string storeId, string Id)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResult> Update(TGoodsReturnheader model)
        {
            throw new NotImplementedException();
        }

        Task<List<TGoodsReturnline>> IDeliveryReturnService.GetLinesById(string companycode, string storeId, string Id)
        {
            throw new NotImplementedException();
        }
    }
}
