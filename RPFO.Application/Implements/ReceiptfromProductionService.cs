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
    public class ReceiptfromProductionService : IReceiptfromProductionService
    {
        private readonly IGenericRepository<TReceiptfromProductionHeader> _ReceiptfromProductionHeaderRepository;
        private readonly IGenericRepository<TReceiptfromProductionLineSerial> _ReceiptfromProductionLineSerialRepository;
        private readonly IGenericRepository<TReceiptfromProductionLine> _ReceiptfromProductionLineRepository;
        IMapper _mapper;

        public ReceiptfromProductionService(IGenericRepository<TReceiptfromProductionHeader> receiptfromProductionHeaderRepository, IGenericRepository<TReceiptfromProductionLineSerial> receiptfromProductionLineSerialRepository, IGenericRepository<TReceiptfromProductionLine> receiptfromProductionLineRepository, IMapper mapper = null)
        {
            _ReceiptfromProductionHeaderRepository = receiptfromProductionHeaderRepository;
            _ReceiptfromProductionLineSerialRepository = receiptfromProductionLineSerialRepository;
            _ReceiptfromProductionLineRepository = receiptfromProductionLineRepository;
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

                var data = await _ReceiptfromProductionHeaderRepository.GetAllAsync($"USP_GetReceiptfromProduction", parameters, commandType: CommandType.StoredProcedure);
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
                ReceiptfromProductionViewModel order = new ReceiptfromProductionViewModel();

                TReceiptfromProductionHeader header = await _ReceiptfromProductionHeaderRepository.GetAsync($"select * from T_ReceiptfromProductionHeader with (nolock) where invtId='{id}' and CompanyCode= '{CompanyCode}' and StoreId= '{StoreId}'", null, commandType: CommandType.Text);

                string queryLine = $"select t1.*  from T_ReceiptfromProductionLine t1 with(nolock)  where t1.invtId = '{id}' and t1.CompanyCode = '{CompanyCode}'";
                string queryLineSerial = $"select t1.*   from T_ReceiptfromProductionLineSerial t1 with(nolock)   where t1.invtId = '{id}' and t1.CompanyCode = '{CompanyCode}'";

                //List<TPurchaseOrderLine> lines = await _poLineRepository.GetAllAsync(, null, commandType: CommandType.Text);

                //List<TPurchaseOrderPayment> payments = await _popaymentLineRepository.GetAllAsync(queryPayment, null, commandType: CommandType.Text);

                //var customer = await _customerRepository.GetAsync($"select * from M_Customer with (nolock) where CustomerId ='{header.CusId}'", null, commandType: CommandType.Text);

                var head = _mapper.Map<ReceiptfromProductionViewModel>(header);
                using (IDbConnection db = _ReceiptfromProductionHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        var lines = db.Query<TReceiptfromProductionLine>(queryLine, null, commandType: CommandType.Text);
                        var serialLines = db.Query<TReceiptfromProductionLineSerial>(queryLineSerial, null, commandType: CommandType.Text);
                        foreach (var line in lines)
                        {
                            line.SerialLines = serialLines.Where(x => x.ItemCode == line.ItemCode && x.UomCode == line.UomCode).ToList();
                        }
                        order = _mapper.Map<ReceiptfromProductionViewModel>(header);
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
    }
}
