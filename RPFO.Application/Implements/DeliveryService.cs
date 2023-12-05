using AutoMapper;
using Dapper;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class DeliveryService : IDeliveryService
    {
        //khai báo
        private readonly IGenericRepository<T_DeliveryHeader> _DeliveryHeaderlRepository;
        private readonly IGenericRepository<T_DeliveryLineSerial> _DeliveryLineSerialRepository;
        private readonly IGenericRepository<T_DeliveryLine> _DeliveryLineRepository;
        //private readonly IGenericRepository<TShippingDivisionLine> _ShippingDivisionLineRepository;
        IMapper _mapper;

        private readonly ICommonService _commonService;
        string ServiceName = "T_Delivery";
        List<string> TableNameList = new List<string>();

        //khởi tạo
        public DeliveryService(IGenericRepository<T_DeliveryLine> deliveryLineRepository, 
            //ICommonService commonService,
            //GenericRepository<TShippingDivisionLine> shippingDivisionLineRepository,
            IGenericRepository<T_DeliveryLineSerial> deliveryLineSerialRepository, IGenericRepository<T_DeliveryHeader> deliveryHeaderlRepository, IMapper mapper = null)
        {

            _DeliveryLineRepository = deliveryLineRepository;
            //_ShippingDivisionLineRepository = shippingDivisionLineRepository;
            _DeliveryLineSerialRepository = deliveryLineSerialRepository;
            _DeliveryHeaderlRepository = deliveryHeaderlRepository;
            _mapper = mapper;
            //TableNameList.Add(ServiceName + "Line"); 
            //_commonService.InitService(ServiceName, TableNameList);
        }
       

        public async Task<GenericResult> GetById(string id)
        {
            throw new NotImplementedException();

        }
      

        public async Task<GenericResult> GetOrderById(string id, string CompanyCode, string StoreId)
        {

            GenericResult result = new GenericResult();
            try
            {
                DeliveryModel order = new DeliveryModel();

                T_DeliveryHeader header = await _DeliveryHeaderlRepository.GetAsync($"select * from T_DeliveryHeader with (nolock) where PurchaseId='{id}' and CompanyCode= '{CompanyCode}' and StoreId= '{StoreId}'", null, commandType: CommandType.Text);

                string queryLine = $"select t1.*  from T_DeliveryLine t1 with(nolock)  where t1.PurchaseId = '{id}' and t1.CompanyCode = '{CompanyCode}'";
                string queryLineSerial = $"select t1.*   from T_DeliveryLineSerial t1 with(nolock)   where t1.PurchaseId = '{id}' and t1.CompanyCode = '{CompanyCode}'";

                //List<TPurchaseOrderLine> lines = await _poLineRepository.GetAllAsync(, null, commandType: CommandType.Text);

                //List<TPurchaseOrderPayment> payments = await _popaymentLineRepository.GetAllAsync(queryPayment, null, commandType: CommandType.Text);

                //var customer = await _customerRepository.GetAsync($"select * from M_Customer with (nolock) where CustomerId ='{header.CusId}'", null, commandType: CommandType.Text);

                var head = _mapper.Map<DeliveryModel>(header);
                using (IDbConnection db = _DeliveryHeaderlRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        var lines = db.Query<T_DeliveryLine>(queryLine, null, commandType: CommandType.Text);
                        var serialLines = db.Query<T_DeliveryLineSerial>(queryLineSerial, null, commandType: CommandType.Text);
                        foreach (var line in lines)
                        {
                            line.SerialLines = serialLines.Where(x => x.ItemCode == line.ItemCode && x.UOMCode == line.UOMCode).ToList();
                        }
                        order = _mapper.Map<DeliveryModel>(header);
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

                var data = await _DeliveryHeaderlRepository.GetAllAsync($"USP_GetDelivery", parameters, commandType: CommandType.StoredProcedure);
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
