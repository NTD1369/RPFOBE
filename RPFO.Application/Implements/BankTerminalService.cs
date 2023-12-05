
using AutoMapper;
using Dapper;
using DevExpress.CodeParser;
using Microsoft.Extensions.Configuration;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Models;
using RPFO.Data.OMSModels;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class BankTerminalService : IBankTerminalService
    {
        private readonly IGenericRepository<MBankTerminal> _bankTerminalRepository;

        private string PAYOO_URL = string.Empty;
        private string PAYOO_MERCHANT_NAME = string.Empty;
        private string PAYOO_CREDENTIAL = string.Empty;

        //private string LogPath = $"{DriveInfo.GetDrives().LastOrDefault()}RPFO.API.Log\\";

        //private readonly IMapper _mapper;
        public BankTerminalService(IGenericRepository<MBankTerminal> bankTerminalRepository, IConfiguration config/*, IMapper mapper, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _bankTerminalRepository = bankTerminalRepository;
            //_mapper = mapper;

            this.PAYOO_URL = Utilities.Helpers.Encryptor.DecryptString(config.GetSection("AppSettings:Payoo_Url").Value, Utilities.Constants.AppConstants.TEXT_PHRASE);
            this.PAYOO_MERCHANT_NAME = Utilities.Helpers.Encryptor.DecryptString(config.GetSection("AppSettings:Payoo_MerchantName").Value, Utilities.Constants.AppConstants.TEXT_PHRASE);
            this.PAYOO_CREDENTIAL = Utilities.Helpers.Encryptor.DecryptString(config.GetSection("AppSettings:Payoo_Credential").Value, Utilities.Constants.AppConstants.TEXT_PHRASE);
        }
        //public async Task<GenericResult> Import(DataImport model)
        //{
        //    GenericResult result = new GenericResult();
        //    List<UOMResultViewModel> resultlist = new List<UOMResultViewModel>();
        //    try
        //    {
        //        foreach (var item in model.Uom)
        //        {
        //            item.CreatedBy = model.CreatedBy;
        //            item.CompanyCode = model.CompanyCode;
        //            var itemResult = await Create(item);
        //            //if (itemResult.Success == false)
        //            //{
        //                UOMResultViewModel itemRs = new UOMResultViewModel();
        //                itemRs = _mapper.Map<UOMResultViewModel>(item);
        //                itemRs.Success = itemResult.Success;
        //                itemRs.Message = itemResult.Message;
        //            resultlist.Add(itemRs);
        //            //}
        //        }
        //        result.Success = true;
        //        result.Data = resultlist;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = ex.Message;
        //        //result.Data = failedlist;
        //    } 
        //    return result;
        //}

        public async Task<bool> checkExist(string CompanyCode, string CounterId, string PaymentMethod)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("CounterId", CounterId);
            parameters.Add("PaymentMethod", PaymentMethod);
            //parameters.Add("TerminalId", TerminalId); 
            parameters.Add("Id", null);
            var affectedRows = await _bankTerminalRepository.GetAsync("USP_S_M_BankTerminal", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MBankTerminal model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Id", Guid.NewGuid());
                parameters.Add("CounterId", model.CounterId);
                parameters.Add("PaymentMethod", model.PaymentMethod);
                parameters.Add("TerminalIdDefault", model.TerminalIdDefault);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                //var affectedRows = _bankTerminalRepository.Update("USP_U_M_BankTerminal", parameters, commandType: CommandType.StoredProcedure);
                var exist = await checkExist(model.CompanyCode, model.CounterId, model.PaymentMethod);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.CounterId + " " + model.TerminalIdDefault + " existed.";
                    return result;
                }
                var affectedRows = _bankTerminalRepository.Insert("USP_I_M_BankTerminal", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Delete(MBankTerminal Code)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {

                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("CounterId", "");
                parameters.Add("PaymentMethod", "");
                parameters.Add("Id", "");
                var data = await _bankTerminalRepository.GetAllAsync("USP_S_M_BankTerminal", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetByCounter(string CompanyCode, string StoreId, string CounterId)
        {
            GenericResult result = new GenericResult();
            try
            {

                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("CounterId", CounterId);
                parameters.Add("StoreId", StoreId);

                var data = await _bankTerminalRepository.GetAllAsync("USP_S_BankTerminalByCounter", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetByCode(string CompanyCode, string Code)
        {
            GenericResult result = new GenericResult();
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("CounterId", "");
                parameters.Add("PaymentMethod", "");
                parameters.Add("Id", Code);

                var data = await _bankTerminalRepository.GetAsync($"USP_S_M_BankTerminal", null, commandType: CommandType.Text);
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

        public async Task<GenericResult> Update(MBankTerminal model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                var exist = await checkExist(model.CompanyCode, model.CounterId, model.PaymentMethod);
                if (exist == false)
                {
                    result = await Create(model);
                }
                else
                {

                    parameters.Add("CompanyCode", model.CompanyCode);
                    parameters.Add("Id", model.Id);
                    parameters.Add("CounterId", model.CounterId);
                    parameters.Add("PaymentMethod", model.PaymentMethod);
                    parameters.Add("TerminalIdDefault", model.TerminalIdDefault);
                    parameters.Add("CustomF1", model.CustomF1);
                    parameters.Add("CustomF2", model.CustomF2);
                    parameters.Add("CustomF3", model.CustomF3);
                    parameters.Add("CustomF4", model.CustomF4);
                    parameters.Add("CustomF5", model.CustomF5);
                    parameters.Add("ModifiedBy", model.ModifiedBy);
                    parameters.Add("Status", model.Status);
                    var affectedRows = _bankTerminalRepository.Update("USP_U_M_BankTerminal", parameters, commandType: CommandType.StoredProcedure);
                    result.Success = true;
                }

                //result.Message = key;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }


        #region Bank Terminak Device (ECR)

        //private System.Threading.AutoResetEvent autoReset = new System.Threading.AutoResetEvent(false);
        //private string dataPortBuffer = string.Empty;
        //private string dataPortBufferSum = string.Empty;
        //private string dataPortListening = string.Empty;
        //private bool IsListening = false;

        public TerminalDataModel SendPaymentToTerminal(string type, string bankName, string portName, double amount, string invoiceNo, int timeOut, string orderId, out string message)
        {
            //  type = 1: Sales
            //  type = 5: Void
            TerminalDataModel dataModel = new TerminalDataModel();
            message = "";

            if (bankName == "CIMBBANK" || bankName == "CIMB")
            {
                if (type == "1" || string.IsNullOrEmpty(type))
                {
                    dataModel = SendPaymentToTerminal_ForCIMB(portName, CIMB_TransCode.Purchase, amount, "", timeOut, out message);
                }
                else if (type == "5")
                {
                    amount = Math.Abs(amount);
                    dataModel = SendPaymentToTerminal_ForCIMB(portName, CIMB_TransCode.Void, amount, invoiceNo, timeOut, out message);
                }

                if (dataModel != null && dataModel.StatusCode != "00" && string.IsNullOrEmpty(message))
                {
                    message += dataModel.ResponseText;
                }
            }
            else if (bankName == "PUBLICBANK" || bankName == "PBB" || bankName == "MBB" || bankName == "MAYBANK")
            {
                if (type == "1" || string.IsNullOrEmpty(type))
                {
                    dataModel = SendPaymentToTerminal_ForPBB(portName, PBB_CommandCode.SaleTransCode, amount, invoiceNo, timeOut, out message);
                }
                else if (type == "5")
                {
                    amount = Math.Abs(amount);
                    dataModel = SendPaymentToTerminal_ForPBB(portName, PBB_CommandCode.VoidTransCode, amount, invoiceNo, timeOut, out message);
                }
                else if (type == "9")
                {
                    amount = Math.Abs(amount);
                    dataModel = SendPaymentToTerminal_ForPBB(portName, PBB_CommandCode.SettlementTransCode, amount, invoiceNo, timeOut, out message);
                }
            }
            else
            {
                message = "Can't find this bank's terminal.";
            }

            string typeName = "Other";
            if (type == "1")
            {
                typeName = "Sale";
            }
            else if (type == "5")
            {
                typeName = "Void";
            }

            string refId = orderId;
            if (string.IsNullOrEmpty(orderId))
            {
                refId = string.IsNullOrEmpty(dataModel.InvoiceNumber) ? invoiceNo : dataModel.InvoiceNumber;
            }

            string logData = $"Type: {type}\nBankName: {bankName}\nPortName: {portName}\nAmount: {amount}\nInvoiceNo: {invoiceNo}";
            Utilities.Helpers.LogUtils.WriteLogData("C:\\RPFO.API.Log\\", "BankTerminal", $"Payment_{typeName}_{bankName}_{refId}", logData + "\n\n" + dataModel.ToJson() + "\n\n" + dataModel.TransactionLog + "\n\n" + message);

            return dataModel;
        }

        private TerminalDataModel SendPaymentToTerminal_ForCIMB(string portName, string transCode, double amount, string invoiceNo, int timeOut, out string message)
        {
            message = "";
            TerminalDataModel dataModel = new TerminalDataModel();
            //string responseData = "";
            string checkStr = "";

            AutoResetEvent autoReset = new AutoResetEvent(false);
            string dataPortBuffer = string.Empty;
            string dataPortBufferSum = string.Empty;
            string dataPortListening = string.Empty;
            bool IsListening = false;
            DateTime beginTime = DateTime.Now;
            DateTime endTime = DateTime.Now;
            string transactionLog = "";

            try
            {
                if (timeOut <= 0)
                {
                    timeOut = 60;
                }

                if (transCode == CIMB_TransCode.Purchase && amount <= 0) //  Purchase
                {
                    message = "Cannot found amount payment.";
                    return dataModel;
                }
                else if (transCode == CIMB_TransCode.Void && string.IsNullOrEmpty(invoiceNo)) //  void
                {
                    message = "Cannot found InvoiceNo.";
                    return dataModel;
                }

                byte[] request = BuildMessageToTerminal_ForCIMB(transCode, amount.ToString("0000000000.00").Replace(".", ""), invoiceNo);

                //dataPortBufferSum = string.Empty;
                //dataPortBuffer = string.Empty;
                //dataPortListening = string.Empty;

                SerialPort _serialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One)
                {
                    DtrEnable = true,
                    RtsEnable = true,
                    //.ReadTimeout = 3000;
                    WriteTimeout = 10000
                };
                _serialPort.Open();
                //_serialPort.DataReceived += SerialPort_DataReceived;
                _serialPort.DataReceived += (sender, e) =>
                {
                    if (sender is SerialPort serialPort)
                    {
                        int bytes = serialPort.BytesToRead;
                        // Create a byte array buffer to hold the incoming data
                        byte[] buffer = new byte[bytes];
                        // Read the data from the port and store it in our buffer
                        serialPort.Read(buffer, 0, bytes);

                        // Show the user the incoming data in hex format
                        string hexString = buffer.ByteArrayToHexString();
                        dataPortBuffer = hexString;
                        if (IsListening)// && !dataPortBuffer.StartsWith(ECRCode.ENQ))
                        {
                            dataPortListening += hexString.Replace(" ", "");

                            if (!string.IsNullOrEmpty(dataPortListening) && dataPortListening.Length >= 6)
                            {
                                int.TryParse(dataPortListening.Substring(2, 4), out int lenght);
                                if (dataPortListening.Length >= 2 + (lenght * ECRCode.CharByte) + 2 + 2)
                                {
                                    transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tReceived Hex: {hexString} \n";
                                    IsListening = false;
                                }
                            }
                        }
                        //else if (hexString.StartsWith(ECRCode.ENQ))
                        //{
                        //    byte[] request = ECRCode.ACK.HexStringToByteArray();
                        //    serialPort.Write(request, 0, request.Length);
                        //}

                        dataPortBufferSum += hexString + "| ";
                    }

                    if (!IsListening)
                    {
                        autoReset.Set();
                    }
                };

                _serialPort.Write(request, 0, request.Length);
                transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tWrite Command: {request.ByteArrayToHexString()} \n";

                checkStr += $"A1-";
                bool res1 = autoReset.WaitOne(5000);

                if (res1)
                {
                    checkStr += $"A20-";
                }
                else
                {
                    checkStr += $"A21-";
                }

                if (string.IsNullOrEmpty(dataPortBuffer))
                {
                    checkStr += $"A3-";
                    _serialPort.Write(request, 0, request.Length);
                    transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tRe-Write Command: {request.ByteArrayToHexString()} \n";
                    bool res2 = autoReset.WaitOne(5000);

                    if (res2)
                    {
                        checkStr += $"A40-";
                    }
                    else
                    {
                        checkStr += $"A41-";
                    }

                    if (string.IsNullOrEmpty(dataPortBuffer))
                    {
                        endTime = DateTime.Now;
                        message = "Cannot connect terminal device.";
                        dataModel.ResponseMessage = dataPortBufferSum;
                        dataModel.DataChecking = checkStr;
                        dataModel.DataListening = dataPortListening;

                        dataModel.BeginTime = beginTime;
                        dataModel.EndTime = endTime;
                        dataModel.TransactionLog = transactionLog;
                        return dataModel;
                    }
                }

                //responseData += dataPortBuffer + ",";
                checkStr += $"B5-";
                if (dataPortBuffer.Trim() == ECRCode.ACK || dataPortBuffer.Trim().StartsWith(ECRCode.ACK))
                {
                    checkStr += $"B6-";
                    //_serialPort.DataReceived -= SerialPort_DataReceived;
                    //_serialPort.DataReceived += SerialPort_DataListening;
                    transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tRead ACK: {dataPortBuffer} \n";
                    IsListening = true;
                    bool waitOne = autoReset.WaitOne(timeOut * 1000);

                    if (waitOne)
                    {
                        checkStr += $"B70-";
                    }
                    else
                    {
                        checkStr += $"B71-";
                    }

                    if (string.IsNullOrEmpty(dataPortListening))
                    {
                        endTime = DateTime.Now;
                        message += "Unable to receive data, device timeout.";
                        //message += "Disconnected device.";
                    }
                    else
                    {
                        checkStr += $"B72-";
                        endTime = DateTime.Now;
                        transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tRead Hex: {dataPortListening} \n";
                        TerminalMessageModel messageModel = ReadMessageFromTerminal_ForCIMB(dataPortListening, out string msg1);
                        if (!string.IsNullOrEmpty(msg1))
                        {
                            message += msg1;
                        }
                        else
                        {
                            byte[] response = ECRCode.ACK.HexStringToByteArray();
                            _serialPort.Write(response, 0, response.Length);

                            transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tWrite ACK: {response.ByteArrayToHexString()} \n";
                            MappingFromTerminalMessage(messageModel, ref dataModel);
                            //dataModel.ResponseMessage = "ACK:" + responseData;
                            dataModel.ExtendObject = messageModel;
                        }
                    }
                }
                else
                {
                    checkStr += $"B8-";
                    endTime = DateTime.Now;
                    transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tRead Hex:: {dataPortBufferSum} \n";
                    TerminalMessageModel messageModel = ReadMessageFromTerminal_ForCIMB(dataPortBufferSum.Replace("|", ""), out string msg1);
                    if (!string.IsNullOrEmpty(msg1))
                    {
                        message += msg1;
                    }
                    else
                    {
                        MappingFromTerminalMessage(messageModel, ref dataModel);
                        dataModel.ResponseMessage = "Received data";
                        dataModel.ExtendObject = messageModel;
                    }
                }

                //responseData += dataPortBuffer;

                _serialPort.Close();
                autoReset.Reset();
            }
            catch (Exception ex)
            {
                endTime = DateTime.Now;
                if (ex.Message.Contains($"Could not find file '{portName}'"))
                {
                    message = "Exception: Disconnected device.";
                }
                else
                {
                    message = "Exception: " + ex.Message;
                }
            }

            IsListening = false;

            dataModel.ResponseMessage = dataPortBufferSum;
            dataModel.DataChecking = checkStr;
            dataModel.DataListening = dataPortListening;

            dataModel.BeginTime = beginTime;
            dataModel.EndTime = endTime;
            dataModel.TransactionLog = transactionLog;

            return dataModel;
        }

        private TerminalDataModel SendPaymentToTerminal_ForPBB(string portName, string transCode, double amount, string invoiceNo, int timeOut, out string message)
        {
            message = "";
            TerminalDataModel dataModel = new TerminalDataModel();
            //string responseData = "";
            string checkStr = "";

            AutoResetEvent autoReset = new AutoResetEvent(false);
            string dataPortBuffer = string.Empty;
            string dataPortBufferSum = string.Empty;
            string dataPortListening = string.Empty;
            bool IsListening = false;
            DateTime beginTime = DateTime.Now;
            DateTime endTime = DateTime.Now;
            string transactionLog = "";

            try
            {
                if (timeOut <= 0)
                {
                    timeOut = 60;
                }

                if (transCode == PBB_CommandCode.SaleTransCode && amount <= 0)
                {
                    message = "Cannot found amount payment.";
                    return dataModel;
                }
                else if (transCode == PBB_CommandCode.VoidTransCode && string.IsNullOrEmpty(invoiceNo))
                {
                    message = "Cannot found InvoiceNo.";
                    return dataModel;
                }

                //string requestCheck = BuildMessageToTerminal_ForPBB(transCode, amount.ToString("0000000000.00").Replace(".", ""), invoiceNo);

                //dataPortBufferSum = string.Empty;
                //dataPortBuffer = string.Empty;
                //dataPortListening = string.Empty;

                SerialPort _serialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One)
                {
                    DtrEnable = true,
                    RtsEnable = true,
                    //ReadTimeout = 3000;
                    WriteTimeout = 10000
                };

                _serialPort.Open();
                //_serialPort.DataReceived += SerialPort_DataReceived;
                _serialPort.DataReceived += (sender, e) =>
                {
                    if (sender is SerialPort serialPort)
                    {
                        int bytes = serialPort.BytesToRead;
                        // Create a byte array buffer to hold the incoming data
                        byte[] buffer = new byte[bytes];
                        // Read the data from the port and store it in our buffer
                        serialPort.Read(buffer, 0, bytes);

                        // Show the user the incoming data in hex format
                        string hexString = buffer.ByteArrayToHexString().Replace(" ", "");
                        dataPortBuffer = hexString;
                        if (IsListening && !dataPortBuffer.StartsWith(ECRCode.ENQ))
                        {
                            dataPortListening += hexString.Replace(" ", "");

                            if (dataPortListening.Length > ((1 + 4 + 2) * ECRCode.CharByte) && dataPortListening.Substring(dataPortListening.Length - 4, 2) == ECRCode.ETX)
                            {
                                transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tReceived EXT: {dataPortListening.Substring(dataPortListening.Length - 4, 2)} \n";

                                string checksum = ComputeLRC_CIMB(dataPortListening.Substring(2, dataPortListening.Length - 4)).ToString("X2");
                                if (checksum == dataPortListening.Substring(dataPortListening.Length - 2, 2))
                                {
                                    transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tReceived LRC: {hexString} \n";
                                    byte[] request = ECRCode.ACK.HexStringToByteArray();
                                    serialPort.Write(request, 0, request.Length);
                                    transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tWrite ACK: {request.ByteArrayToHexString()} \n";
                                    IsListening = false;
                                }
                            }

                            //if (!string.IsNullOrEmpty(dataPortListening) && dataPortListening.Length >= 6)
                            //{
                            //    int.TryParse(dataPortListening.Substring(2, 4), out int lenght);
                            //    if (dataPortListening.Length >= 2 + (lenght * ECRCode.CharByte) + 2 + 2)
                            //    {
                            //        IsListening = false;
                            //    }
                            //}
                        }
                        else if (hexString.StartsWith(ECRCode.ENQ))
                        {
                            transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tReceived ENQ: {hexString} \n";
                            byte[] request = ECRCode.ACK.HexStringToByteArray();
                            serialPort.Write(request, 0, request.Length);
                            transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tWrite ACK End: {request.ByteArrayToHexString()} \n";
                        }
                        else if (hexString.StartsWith(ECRCode.EOT))
                        {
                            transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tReceived EOT: {hexString} \n";
                            IsListening = false;
                        }
                        else if (hexString.StartsWith(ECRCode.ACK))
                        {
                            transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tReceived ACK: {hexString} \n";
                        }

                        dataPortBufferSum += hexString + "| ";
                    }

                    if (!IsListening)
                    {
                        autoReset.Set();
                    }
                };

                byte[] check = ECRCode.ENQ.HexStringToByteArray();
                _serialPort.Write(check, 0, check.Length);
                transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tWrite ENQ: {check.ByteArrayToHexString()} \n";

                checkStr += $"A1-";
                bool res1 = autoReset.WaitOne(3000);

                if (res1)
                {
                    checkStr += $"A20-";
                }
                else
                {
                    checkStr += $"A21-";
                }

                if (string.IsNullOrEmpty(dataPortBuffer)
                    || (!string.IsNullOrEmpty(dataPortBuffer) && (dataPortBuffer.Trim() == ECRCode.NAK || dataPortBuffer.Trim().StartsWith(ECRCode.NAK))))
                {
                    transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tRead NAK: {dataPortBuffer} \n";
                    checkStr += $"A3-";
                    _serialPort.Write(check, 0, check.Length);
                    transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tRe-Write ENQ: {check.ByteArrayToHexString()} \n";

                    bool res2 = autoReset.WaitOne(3000);

                    if (res2)
                    {
                        checkStr += $"A40-";
                    }
                    else
                    {
                        checkStr += $"A41-";
                    }

                    if (string.IsNullOrEmpty(dataPortBuffer))
                    {
                        endTime = DateTime.Now;
                        message = "Cannot connect terminal device.";

                        dataModel.ResponseMessage = dataPortBufferSum;
                        dataModel.DataChecking = checkStr;
                        dataModel.DataListening = dataPortListening;

                        dataModel.BeginTime = beginTime;
                        dataModel.EndTime = endTime;
                        dataModel.TransactionLog = transactionLog;

                        return dataModel;
                    }
                }

                //responseData += dataPortBuffer + ",";
                checkStr += $"B5-";
                if (dataPortBuffer.Trim() == ECRCode.ACK || dataPortBuffer.Trim().StartsWith(ECRCode.ACK))
                {
                    transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tRead ACK: {dataPortBuffer} \n";
                    checkStr += $"B61-";
                    dataPortBuffer = string.Empty;

                    string request = BuildMessageToTerminal_ForPBB(transCode, amount.ToString("0000000000.00").Replace(".", ""), invoiceNo);
                    _serialPort.Write(request);
                    transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tWrite Command: {request.Replace("\u0002", "<STX>").Replace("\u0003", "<ETX>")} \n";
                    bool res2 = autoReset.WaitOne(3000);

                    if (res2)
                    {
                        checkStr += $"B70-";
                    }
                    else
                    {
                        checkStr += $"B71-";
                    }

                    if (!string.IsNullOrEmpty(dataPortBuffer) && (dataPortBuffer.Trim() == ECRCode.NAK || dataPortBuffer.Trim().StartsWith(ECRCode.NAK)))
                    {
                        transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tRead NAK:: {dataPortBuffer} \n";
                        checkStr += $"B62-";
                        dataPortBuffer = string.Empty;
                        //_serialPort.Write(request, 0, request.Length);
                        _serialPort.Write(request);
                        transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tWrite Command:: {request} \n";
                        bool res3 = autoReset.WaitOne(3000);

                        if (res3)
                        {
                            checkStr += $"B72-";
                        }
                        else
                        {
                            checkStr += $"B73-";
                        }
                    }

                    if (dataPortBuffer.Trim() == ECRCode.ACK || dataPortBuffer.Trim().StartsWith(ECRCode.ACK))
                    {
                        checkStr += $"B8-";
                        transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tRead ACK:: {dataPortBuffer} \n";
                        IsListening = true;
                        bool waitOne = autoReset.WaitOne(timeOut * 1000);

                        if (waitOne)
                        {
                            checkStr += $"B90-";
                        }
                        else
                        {
                            checkStr += $"B91-";
                        }

                        if (string.IsNullOrEmpty(dataPortListening))
                        {
                            endTime = DateTime.Now;
                            transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tRead Data timeout \n";
                            message += "Unable to receive data, device timeout.";
                            //message += "Disconnected device.";
                        }
                        else
                        {
                            checkStr += $"C1-";
                            endTime = DateTime.Now;
                            transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tRead Hex: {dataPortListening} \n";
                            dataModel = ReadMessageFromTerminal_ForPBB(dataPortListening, out string msg1);
                            if (!string.IsNullOrEmpty(msg1))
                            {
                                message += msg1;
                            }
                            //else
                            //{
                            //    dataModel.ResponseMessage = "ACK:" + responseData;
                            //}
                        }
                    }
                }
                else
                {
                    checkStr += $"B51-";
                    endTime = DateTime.Now;
                    transactionLog += $"{DateTime.Now:HH:mm:ss.fff}\tRead Hex: {dataPortBuffer} \n";
                    dataModel = ReadMessageFromTerminal_ForPBB(dataPortBuffer, out string msg1);
                    if (!string.IsNullOrEmpty(msg1))
                    {
                        message += msg1;
                    }
                    else
                    {
                        dataModel.ResponseMessage = "Received data";
                    }
                }

                //responseData += dataPortBuffer;

                _serialPort.Close();
                autoReset.Reset();
            }
            catch (Exception ex)
            {
                endTime = DateTime.Now;
                if (ex.Message.Contains($"Could not find file '{portName}'"))
                {
                    message = "Exception: Disconnected device.";
                }
                else
                {
                    message = "Exception: " + ex.Message;
                }
            }

            IsListening = false;

            //dataModel.ResponseMessage1 = responseData + ";" + dataPortBufferSum;
            dataModel.ResponseMessage = dataPortBufferSum;
            dataModel.DataChecking = checkStr;
            dataModel.DataListening = dataPortListening;

            dataModel.BeginTime = beginTime;
            dataModel.EndTime = endTime;
            dataModel.TransactionLog = transactionLog;

            return dataModel;
        }

        private byte[] BuildMessageToTerminal_ForCIMB(string transCode, string amount, string invoiceNo)
        {
            // set model
            TerminalMessageModel terminalMessage = new TerminalMessageModel();
            terminalMessage.MessageData.TransportHeader.TransportHeaderType = "60";
            terminalMessage.MessageData.TransportHeader.TransportDestination = "0000";
            terminalMessage.MessageData.TransportHeader.TransportSource = "0000";

            terminalMessage.MessageData.PresentationHeader.FormatVersion = "1";
            terminalMessage.MessageData.PresentationHeader.ResponseIndicator = "0";
            terminalMessage.MessageData.PresentationHeader.TransactionCode = transCode;   //20: Purchase; 26: Void
            terminalMessage.MessageData.PresentationHeader.ResponseCode = "00";   //Approved
            terminalMessage.MessageData.PresentationHeader.MoreIndicator = "0";
            terminalMessage.MessageData.PresentationHeader.FieldSeparator = "1C";

            if (transCode == CIMB_TransCode.Purchase)
            {
                TerminalMessageFieldData messageFieldData = new TerminalMessageFieldData
                {
                    FieldType = "40",   //  Amount, Transaction
                    Data = amount,
                    FieldSeparator = "1C"
                };

                terminalMessage.MessageData.FieldDatas.Add(messageFieldData);
            }
            else if (transCode == CIMB_TransCode.Void)
            {
                TerminalMessageFieldData messageFieldData = new TerminalMessageFieldData
                {
                    FieldType = "65",   //  Invoice Number 
                    Data = invoiceNo,
                    FieldSeparator = "1C"
                };

                terminalMessage.MessageData.FieldDatas.Add(messageFieldData);
            }

            //  buile hex string

            List<string> hexCodeData = new List<string>();
            //List<string> strCodeData = new List<string>();
            //strCodeData.Add(terminalMessage.MessageData.TransportHeader.TransportHeaderType);
            //strCodeData.Add(terminalMessage.MessageData.TransportHeader.TransportDestination);
            //strCodeData.Add(terminalMessage.MessageData.TransportHeader.TransportSource);

            hexCodeData.Add(terminalMessage.MessageData.TransportHeader.TransportHeaderType.ToHexString());
            hexCodeData.Add(terminalMessage.MessageData.TransportHeader.TransportDestination.ToHexString());
            hexCodeData.Add(terminalMessage.MessageData.TransportHeader.TransportSource.ToHexString());

            //strCodeData.Add(terminalMessage.MessageData.PresentationHeader.FormatVersion);
            //strCodeData.Add(terminalMessage.MessageData.PresentationHeader.ResponseIndicator);
            //strCodeData.Add(terminalMessage.MessageData.PresentationHeader.TransactionCode);
            //strCodeData.Add(terminalMessage.MessageData.PresentationHeader.ResponseCode);
            //strCodeData.Add(terminalMessage.MessageData.PresentationHeader.MoreIndicator);

            hexCodeData.Add(terminalMessage.MessageData.PresentationHeader.FormatVersion.ToHexString());
            hexCodeData.Add(terminalMessage.MessageData.PresentationHeader.ResponseIndicator.ToHexString());
            hexCodeData.Add(terminalMessage.MessageData.PresentationHeader.TransactionCode.ToHexString());
            hexCodeData.Add(terminalMessage.MessageData.PresentationHeader.ResponseCode.ToHexString());
            hexCodeData.Add(terminalMessage.MessageData.PresentationHeader.MoreIndicator.ToHexString());
            hexCodeData.Add(terminalMessage.MessageData.PresentationHeader.FieldSeparator);

            foreach (TerminalMessageFieldData fieldData in terminalMessage.MessageData.FieldDatas)
            {
                hexCodeData.Add(fieldData.FieldType.ToHexString());
                string strData = fieldData.Data.ToHexString();
                fieldData.LLLL = strData.Length / ECRCode.CharByte;
                hexCodeData.Add(fieldData.LLLL.ToString("0000"));
                hexCodeData.Add(strData);
                hexCodeData.Add(fieldData.FieldSeparator);

                //strCodeData.Add(fieldData.FieldType);
                //strCodeData.Add(fieldData.LLLL.ToString("0000"));
                //strCodeData.Add(fieldData.Data);
                //strCodeData.Add(fieldData.FieldSeparator);
            }

            string hexMessageData = string.Join("", hexCodeData);
            terminalMessage.LLLL = hexMessageData.Length / ECRCode.CharByte;
            //terminalMessage.LRC = ComputeLRC_CIMB($"{terminalMessage.LLLL:0000}{string.Join("", strCodeData)}{ECRCode.ETX}").ToString("X2");
            terminalMessage.LRC = ComputeLRC_CIMB($"{terminalMessage.LLLL:0000}{string.Join("", hexCodeData)}{ECRCode.ETX}").ToString("X2");
            //terminalMessage.LRC = "14";
            string hexString = $"{ECRCode.STX}{terminalMessage.LLLL:0000}{hexMessageData}{ECRCode.ETX}{terminalMessage.LRC}";

            return hexString.HexStringToByteArray();
        }

        private void MappingFromTerminalMessage(TerminalMessageModel messageModel, ref TerminalDataModel terminalData)
        {
            if (terminalData == null || messageModel == null || messageModel.MessageData == null || messageModel.MessageData.FieldDatas == null || messageModel.MessageData.FieldDatas.Count == 0)
            {
                return;
            }

            terminalData.StatusCode = messageModel.MessageData.PresentationHeader.ResponseCode;

            foreach (var item in messageModel.MessageData.FieldDatas)
            {
                switch (item.FieldType)
                {
                    case "01":
                        terminalData.ApprovalCode = item.Data.Trim();
                        break;

                    case "02":
                        terminalData.ResponseText = item.Data.Trim();
                        break;

                    case "65":
                        terminalData.InvoiceNumber = item.Data.Trim();
                        break;

                    case "D0":
                        terminalData.MerchantName = item.Data.Trim();
                        break;

                    case "16":
                        terminalData.TerminalID = item.Data.Trim();
                        break;

                    case "D1":
                        terminalData.MerchantNumber = item.Data.Trim();
                        break;

                    case "D2":
                        terminalData.CardIssuerName = item.Data.Trim();
                        break;

                    case "30":
                        terminalData.CardNumber = item.Data.Trim();
                        break;

                    case "31":
                        terminalData.ExpiryDate = item.Data.Trim();
                        break;

                    case "50":
                        terminalData.BatchNumber = item.Data.Trim();
                        break;

                    case "03":
                        terminalData.TransactionDate = item.Data.Trim();
                        break;

                    case "04":
                        terminalData.TransactionTime = item.Data.Trim();
                        break;

                    case "D3":
                        terminalData.RetrievalRefNo = item.Data.Trim();
                        break;

                    case "D4":
                        terminalData.CardIssuerID = item.Data.Trim();
                        break;

                    case "D5":
                        terminalData.CardHolderName = item.Data.Trim();
                        break;
                }
            }
        }

        private string BuildMessageToTerminal_ForPBB(string transCode, string amount, string invoiceNo)
        {
            string saleCommand = transCode; //  C200: Sales; C201: Void
            string hostNo = "00";
            //  00 - card and ewallet
            //  CP – for card payment
            //  QR – for QR payment
            string addData = "".PadRight(24, ' ');
            string messageContent = $"{saleCommand}{hostNo}{amount}{addData}";
            if (saleCommand == PBB_CommandCode.VoidTransCode)
            {
                messageContent = $"{saleCommand}{hostNo}{amount}{invoiceNo}{addData}";
            }
            else if (saleCommand == PBB_CommandCode.SettlementTransCode)
            {
                messageContent = $"{saleCommand}{hostNo}";
            }
            char lrc = Convert.ToChar(ComputeLRC(messageContent + "\x03"));

            //string transMessage = $"{ECRCode.STX}{messageContent}{ECRCode.ETX}{lrc}";
            string transMessage = $"\x02{messageContent}\x03{lrc}";

            return transMessage;
        }

        private int ComputeLRC_CIMB(string str)
        {
            int LRC = 0;
            int i = 0;
            while (i < str.Length)
            {
                LRC = LRC ^ Convert.ToByte(str.Substring(i, 2), 16);
                i += 2;
            }

            return LRC;
        }

        private int ComputeLRC(string str)
        {
            int LRC = 0;
            for (int i = 0; i < str.Length; i++)
                LRC = LRC ^ str[i];
            return LRC;
        }

        private TerminalMessageModel ReadMessageFromTerminal_ForCIMB(string messageHex, out string errMsg)
        {
            errMsg = "";
            //string messageStr = Encoding.ASCII.GetString(messageByte);
            string messageStr = messageHex.Replace(" ", "");
            int count = 0;
            int charByte = ECRCode.CharByte;

            TerminalMessageModel terminalMessage = new TerminalMessageModel();
            string firstStr = messageStr.Substring(count, Math.Min(charByte * 1, messageStr.Length - count));
            if (firstStr.ToLower() == ECRCode.ACK.ToLower())
            {
                messageStr = messageStr.Substring(firstStr.Length, Math.Max(0, messageStr.Length - firstStr.Length));
            }
            string STX = messageStr.Substring(count, Math.Min(charByte * 1, messageStr.Length - count));
            if (STX.ToLower() != ECRCode.STX.ToLower())
            {
                errMsg = "Message is not correct structure: " + messageStr;
                return terminalMessage;
            }
            count += STX.Length;
            string LLLL = messageStr.Substring(count, Math.Min(charByte * 2, messageStr.Length - count));
            int.TryParse(LLLL, out int msgLenght);
            if (msgLenght == 0)
            {
                errMsg = "The length of the message could not be found: " + messageStr;
                return terminalMessage;
            }

            terminalMessage.LLLL = msgLenght;
            count += LLLL.Length;
            string messageData = messageStr.Substring(count, Math.Min(charByte * msgLenght, messageStr.Length - count));
            count += messageData.Length;

            int countData = 0;
            //  Transport Header
            string transportHeader = messageData.Substring(countData, Math.Min(charByte * 2, messageData.Length - countData));
            countData += transportHeader.Length;
            terminalMessage.MessageData.TransportHeader.TransportHeaderType = transportHeader.HexToString();

            string transportDestination = messageData.Substring(countData, Math.Min(charByte * 4, messageData.Length - countData));
            countData += transportDestination.Length;
            terminalMessage.MessageData.TransportHeader.TransportDestination = transportDestination.HexToString();

            string transportSource = messageData.Substring(countData, Math.Min(charByte * 4, messageData.Length - countData));
            countData += transportSource.Length;
            terminalMessage.MessageData.TransportHeader.TransportSource = transportSource.HexToString();
            //

            //  Presentation Header
            string formatVersion = messageData.Substring(countData, Math.Min(charByte * 1, messageData.Length - countData));
            countData += formatVersion.Length;
            terminalMessage.MessageData.PresentationHeader.FormatVersion = formatVersion.HexToString();

            string responseIndicator = messageData.Substring(countData, Math.Min(charByte * 1, messageData.Length - countData));
            countData += responseIndicator.Length;
            terminalMessage.MessageData.PresentationHeader.ResponseIndicator = responseIndicator.HexToString();

            string transactionCode = messageData.Substring(countData, Math.Min(charByte * 2, messageData.Length - countData));
            countData += transactionCode.Length;
            terminalMessage.MessageData.PresentationHeader.TransactionCode = transactionCode.HexToString();

            string responseCode = messageData.Substring(countData, Math.Min(charByte * 2, messageData.Length - countData));
            countData += responseCode.Length;
            terminalMessage.MessageData.PresentationHeader.ResponseCode = responseCode.HexToString();

            string moreIndicator = messageData.Substring(countData, Math.Min(charByte * 1, messageData.Length - countData));
            countData += moreIndicator.Length;
            terminalMessage.MessageData.PresentationHeader.MoreIndicator = moreIndicator.HexToString();

            string fieldSeparator = messageData.Substring(countData, Math.Min(charByte * 1, messageData.Length - countData));
            if (fieldSeparator.ToLower() == ECRCode.FS.ToLower())
            {
                countData += fieldSeparator.Length;
                terminalMessage.MessageData.PresentationHeader.FieldSeparator = fieldSeparator;
            }
            //

            while (countData < messageData.Length)
            {
                TerminalMessageFieldData fieldData = new TerminalMessageFieldData();

                // Field Data Format
                string dataFieldType = messageData.Substring(countData, Math.Min(charByte * 2, messageData.Length - countData));
                countData += dataFieldType.Length;
                fieldData.FieldType = dataFieldType.HexToString();

                string dataLLLL = messageData.Substring(countData, Math.Min(charByte * 2, messageData.Length - countData));
                countData += dataLLLL.Length;
                int.TryParse(dataLLLL, out int dataLenght);
                fieldData.LLLL = dataLenght;

                string dataContent = messageData.Substring(countData, Math.Min(charByte * dataLenght, messageData.Length - countData));
                countData += dataContent.Length;
                fieldData.Data = dataContent.HexToString();

                string dataFieldSeparator = messageData.Substring(countData, Math.Min(charByte * 1, messageData.Length - countData));
                if (dataFieldSeparator == ECRCode.FS)
                {
                    countData += dataFieldSeparator.Length;
                    fieldData.FieldSeparator = dataFieldSeparator;
                }
                //

                terminalMessage.MessageData.FieldDatas.Add(fieldData);
            }

            string EXT = messageStr.Substring(count, Math.Min(2, messageStr.Length - count));
            if (EXT == ECRCode.ETX)
            {
                count += EXT.Length;

                string LRC = messageStr.Substring(count, Math.Min(2, messageStr.Length - count));
                terminalMessage.LRC = LRC;
            }

            return terminalMessage;
        }

        private TerminalDataModel ReadMessageFromTerminal_ForPBB(string messageHex, out string errMsg)
        {
            errMsg = "";
            TerminalDataModel terminalData = new TerminalDataModel();
            string messageStr = messageHex.Replace(" ", "");
            int count = 0;

            string STX = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 1, messageStr.Length - count));
            if (STX.ToLower() != ECRCode.STX.ToLower())
            {
                errMsg = "Message is not correct structure: " + messageStr;
                return terminalData;
            }
            count += STX.Length;
            string hexResponse = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 4, messageStr.Length - count));
            count += hexResponse.Length;
            terminalData.ResponseText = hexResponse.HexToString();

            switch (terminalData.ResponseText)
            {
                case PBB_CommandCode.SaleResponseCodeCard:  // CARD
                    ReadMessageFromTerminal_ForPBB_CARD_Sales(messageStr, count, ref terminalData);
                    if (string.IsNullOrEmpty(terminalData.CardIssuerID) && !string.IsNullOrEmpty(terminalData.AlipayTxnID))
                    {
                        terminalData.CardIssuerID = "ALIPAY";
                    }
                    break;

                case PBB_CommandCode.SaleResponseCodeWallet:  // GHL MAH Wallet pay
                    ReadMessageFromTerminal_ForPBB_WalletPay_Sales(messageStr, count, ref terminalData);
                    break;

                case PBB_CommandCode.SaleResponseCodeQRPay:   // MBB QR pay
                    ReadMessageFromTerminal_ForPBB_QRPay_Sales(messageStr, count, ref terminalData);
                    break;

                case PBB_CommandCode.VoidResponseCodeCard:  // CARD
                    ReadMessageFromTerminal_ForPBB_CARD_Void(messageStr, count, ref terminalData);
                    break;

                case PBB_CommandCode.SettlementResponseCode:  // Settlement
                    ReadMessageFromTerminal_ForPBB_Settlement(messageStr, count, ref terminalData);
                    break;
            }

            return terminalData;
        }

        private void ReadMessageFromTerminal_ForPBB_CARD_Sales(string messageStr, int count, ref TerminalDataModel terminalData)
        {
            string hexCardNumber = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 19, messageStr.Length - count));
            count += hexCardNumber.Length;

            string hexExpiryDate = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 4, messageStr.Length - count));
            count += hexExpiryDate.Length;

            string hexStatusCode = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 2, messageStr.Length - count));
            count += hexStatusCode.Length;

            string hexApprovalCode = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 6, messageStr.Length - count));
            count += hexApprovalCode.Length;

            string hexRRN = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 12, messageStr.Length - count));
            count += hexRRN.Length;

            string hexTransTrace = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 6, messageStr.Length - count));
            count += hexTransTrace.Length;

            string hexBatchNumber = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 6, messageStr.Length - count));
            count += hexBatchNumber.Length;

            string hexHostNo = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 2, messageStr.Length - count));
            count += hexHostNo.Length;

            string hexTerminalId = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 8, messageStr.Length - count));
            count += hexTerminalId.Length;

            string hexMerchantId = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 15, messageStr.Length - count));
            count += hexMerchantId.Length;

            string hexAID = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 14, messageStr.Length - count));
            count += hexAID.Length;

            string hexTC = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 16, messageStr.Length - count));
            count += hexTC.Length;

            string hexCardHolderName = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 26, messageStr.Length - count));
            count += hexCardHolderName.Length;

            string hexCardType = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 2, messageStr.Length - count));
            count += hexCardType.Length;

            terminalData.CardNumber = hexCardNumber.HexToString().Trim();
            terminalData.ExpiryDate = hexExpiryDate.HexToString().Trim();
            terminalData.StatusCode = hexStatusCode.HexToString().Trim();
            terminalData.ApprovalCode = hexApprovalCode.HexToString().Trim();
            terminalData.RetrievalRefNo = hexRRN.HexToString().Trim();
            terminalData.InvoiceNumber = hexTransTrace.HexToString().Trim();
            terminalData.BatchNumber = hexBatchNumber.HexToString().Trim();
            terminalData.HostNo = hexHostNo.HexToString().Trim();
            terminalData.TerminalID = hexTerminalId.HexToString().Trim();
            terminalData.MerchantNumber = hexMerchantId.HexToString().Trim();
            terminalData.ApplicationID = hexAID.HexToString().Trim();
            terminalData.TransactionCryptogram = hexTC.HexToString().Trim();
            terminalData.CardHolderName = hexCardHolderName.HexToString().Trim();
            terminalData.CardIssuerID = hexCardType.HexToString().Trim();

            string etx = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 1, messageStr.Length - count));
            if (etx != ECRCode.ETX)
            {
                string hexPrtnrTxnID = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 32, messageStr.Length - count));
                count += hexPrtnrTxnID.Length;

                string hexApayTxnID = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 64, messageStr.Length - count));
                count += hexApayTxnID.Length;

                string hexCustomerID = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 32, messageStr.Length - count));
                count += hexCustomerID.Length;

                terminalData.PartnerTransactionID = hexPrtnrTxnID.HexToString().Trim();
                terminalData.AlipayTxnID = hexApayTxnID.HexToString().Trim();
                terminalData.CustomerID = hexCustomerID.HexToString().Trim();
            }

            count += etx.Length;

            string lrc = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 1, messageStr.Length - count));
            count += lrc.Length;

            string nextMessage = messageStr.Substring(count, messageStr.Length - count);

            if (!string.IsNullOrEmpty(nextMessage) && terminalData.StatusCode != "00")
            {
                count = 0;
                TerminalDataModel terminalData1 = new TerminalDataModel();
                string STX = nextMessage.Substring(count, Math.Min(ECRCode.CharByte * 1, nextMessage.Length - count));
                if (STX.ToLower() != ECRCode.STX.ToLower())
                {
                    return;
                }
                count += STX.Length;
                string hexResponse = nextMessage.Substring(count, Math.Min(ECRCode.CharByte * 4, nextMessage.Length - count));
                count += hexResponse.Length;
                terminalData1.ResponseText = hexResponse.HexToString();

                if (terminalData1.ResponseText == PBB_CommandCode.SaleResponseCodeCard)
                {
                    ReadMessageFromTerminal_ForPBB_CARD_Sales(nextMessage, count, ref terminalData1);
                    if (terminalData1.StatusCode == "00")
                    {
                        terminalData = terminalData1;
                    }
                }
            }
        }

        private void ReadMessageFromTerminal_ForPBB_WalletPay_Sales(string messageStr, int count, ref TerminalDataModel terminalData)
        {
            string hexStatusCode = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 2, messageStr.Length - count));
            count += hexStatusCode.Length;

            string hexTransTrace = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 6, messageStr.Length - count));
            count += hexTransTrace.Length;

            string hexBatchNumber = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 6, messageStr.Length - count));
            count += hexBatchNumber.Length;

            string hexHostNo = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 2, messageStr.Length - count));
            count += hexHostNo.Length;

            string hexTerminalId = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 8, messageStr.Length - count));
            count += hexTerminalId.Length;

            string hexMerchantId = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 15, messageStr.Length - count));
            count += hexMerchantId.Length;

            ////
            //string hexCardNumber = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 19, messageStr.Length - count));
            //count += hexCardNumber.Length;

            //string hexExpiryDate = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 4, messageStr.Length - count));
            //count += hexExpiryDate.Length;

            //string hexApprovalCode = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 6, messageStr.Length - count));
            //count += hexApprovalCode.Length;

            //string hexRRN = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 12, messageStr.Length - count));
            //count += hexRRN.Length;

            //string hexAID = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 14, messageStr.Length - count));
            //count += hexAID.Length;

            //string hexTC = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 16, messageStr.Length - count));
            //count += hexTC.Length;

            //string hexCardHolderName = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 26, messageStr.Length - count));
            //count += hexCardHolderName.Length;

            //string hexCardType = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 2, messageStr.Length - count));
            //count += hexCardType.Length;

            //terminalData.CardNumber = hexCardNumber.HexToString().Trim();
            //terminalData.ExpiryDate = hexExpiryDate.HexToString().Trim();
            terminalData.StatusCode = hexStatusCode.HexToString().Trim();
            //terminalData.ApprovalCode = hexApprovalCode.HexToString().Trim();
            //terminalData.RetrievalRefNo = hexRRN.HexToString().Trim();
            terminalData.InvoiceNumber = hexTransTrace.HexToString().Trim();
            terminalData.BatchNumber = hexBatchNumber.HexToString().Trim();
            terminalData.HostNo = hexHostNo.HexToString().Trim();
            terminalData.TerminalID = hexTerminalId.HexToString().Trim();
            terminalData.MerchantNumber = hexMerchantId.HexToString().Trim();
            //terminalData.ApplicationID = hexAID.HexToString().Trim();
            //terminalData.TransactionCryptogram = hexTC.HexToString().Trim();
            //terminalData.CardHolderName = hexCardHolderName.HexToString().Trim();
            //terminalData.CardIssuerID = hexCardType.HexToString().Trim();

            string etx = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 1, messageStr.Length - count));
            if (etx != ECRCode.ETX)
            {
                string hexMAHTxnID = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 40, messageStr.Length - count));
                count += hexMAHTxnID.Length;

                string hexAppTxnID = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 40, messageStr.Length - count));
                count += hexAppTxnID.Length;

                string hexPrductBrand = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 40, messageStr.Length - count));
                count += hexPrductBrand.Length;

                string hexTotalRMB = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 16, messageStr.Length - count));
                count += hexTotalRMB.Length;

                string hexRate = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 16, messageStr.Length - count));
                count += hexRate.Length;

                terminalData.PartnerTransactionID = hexMAHTxnID.HexToString().Trim();
                terminalData.ApplicationID = hexAppTxnID.HexToString().Trim();
                terminalData.CardIssuerName = hexPrductBrand.HexToString().Trim();
                terminalData.CardIssuerID = hexPrductBrand.HexToString().Trim();
                terminalData.ExchangeRate = hexRate.HexToString().Trim();

                string amount = hexTotalRMB.HexToString().Trim();
                double.TryParse(amount, out double totalAmount);
                terminalData.TotalAmount = (totalAmount / 100).ToString();
            }

            count += etx.Length;

            string lrc = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 1, messageStr.Length - count));
            count += lrc.Length;

            string nextMessage = messageStr.Substring(count, messageStr.Length - count);
            if (!string.IsNullOrEmpty(nextMessage) && terminalData.StatusCode != "00")
            {
                count = 0;
                TerminalDataModel terminalData1 = new TerminalDataModel();
                string STX = nextMessage.Substring(count, Math.Min(ECRCode.CharByte * 1, nextMessage.Length - count));
                if (STX.ToLower() != ECRCode.STX.ToLower())
                {
                    return;
                }
                count += STX.Length;
                string hexResponse = nextMessage.Substring(count, Math.Min(ECRCode.CharByte * 4, nextMessage.Length - count));
                count += hexResponse.Length;
                terminalData1.ResponseText = hexResponse.HexToString();

                if (terminalData1.ResponseText == PBB_CommandCode.SaleResponseCodeWallet)
                {
                    ReadMessageFromTerminal_ForPBB_WalletPay_Sales(nextMessage, count, ref terminalData1);
                    if (terminalData1.StatusCode == "00")
                    {
                        terminalData = terminalData1;
                    }
                }
            }
        }

        private void ReadMessageFromTerminal_ForPBB_QRPay_Sales(string messageStr, int count, ref TerminalDataModel terminalData)
        {
            string hexStatusCode = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 2, messageStr.Length - count));
            count += hexStatusCode.Length;

            string hexStatusDesc = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 32, messageStr.Length - count));
            count += hexStatusDesc.Length;

            string hexTransTrace = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 6, messageStr.Length - count));
            count += hexTransTrace.Length;

            string hexBatchNumber = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 6, messageStr.Length - count));
            count += hexBatchNumber.Length;

            string hexHostNo = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 2, messageStr.Length - count));
            count += hexHostNo.Length;

            string hexTerminalId = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 8, messageStr.Length - count));
            count += hexTerminalId.Length;

            string hexMerchantId = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 15, messageStr.Length - count));
            count += hexMerchantId.Length;

            ////
            //string hexCardNumber = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 19, messageStr.Length - count));
            //count += hexCardNumber.Length;

            //string hexExpiryDate = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 4, messageStr.Length - count));
            //count += hexExpiryDate.Length;


            //string hexApprovalCode = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 6, messageStr.Length - count));
            //count += hexApprovalCode.Length;

            //string hexRRN = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 12, messageStr.Length - count));
            //count += hexRRN.Length;

            //string hexAID = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 14, messageStr.Length - count));
            //count += hexAID.Length;

            //string hexTC = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 16, messageStr.Length - count));
            //count += hexTC.Length;

            //string hexCardHolderName = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 26, messageStr.Length - count));
            //count += hexCardHolderName.Length;

            //string hexCardType = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 2, messageStr.Length - count));
            //count += hexCardType.Length;

            terminalData.StatusCode = hexStatusCode.HexToString().Trim();
            terminalData.ResponseText = hexStatusDesc.HexToString().Trim();
            //terminalData.CardNumber = hexCardNumber.HexToString().Trim();
            //terminalData.ExpiryDate = hexExpiryDate.HexToString().Trim();
            //terminalData.ApprovalCode = hexApprovalCode.HexToString().Trim();
            //terminalData.RetrievalRefNo = hexRRN.HexToString().Trim();
            terminalData.InvoiceNumber = hexTransTrace.HexToString().Trim();
            terminalData.BatchNumber = hexBatchNumber.HexToString().Trim();
            terminalData.HostNo = hexHostNo.HexToString().Trim();
            terminalData.TerminalID = hexTerminalId.HexToString().Trim();
            terminalData.MerchantNumber = hexMerchantId.HexToString().Trim();
            //terminalData.ApplicationID = hexAID.HexToString().Trim();
            //terminalData.TransactionCryptogram = hexTC.HexToString().Trim();
            //terminalData.CardHolderName = hexCardHolderName.HexToString().Trim();
            //terminalData.CardIssuerID = hexCardType.HexToString().Trim();

            string etx = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 1, messageStr.Length - count));
            if (etx != ECRCode.ETX)
            {
                string hexAppID = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 8, messageStr.Length - count));
                count += hexAppID.Length;

                string hexCurrency = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 3, messageStr.Length - count));
                count += hexCurrency.Length;

                string hexTotalAmt = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 12, messageStr.Length - count));
                count += hexTotalAmt.Length;

                string hexCashFeeType = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 3, messageStr.Length - count));
                count += hexCashFeeType.Length;

                string hexCashFeeValue = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 12, messageStr.Length - count));
                count += hexCashFeeValue.Length;

                string hexOrderNo = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 14, messageStr.Length - count));
                count += hexOrderNo.Length;

                string hexLLLLTransId = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 4, messageStr.Length - count));
                count += hexLLLLTransId.Length;
                int.TryParse(hexLLLLTransId.HexToString(), out int lenghtTransId);
                string hexMMBTransID = messageStr.Substring(count, Math.Min(ECRCode.CharByte * lenghtTransId, messageStr.Length - count));
                count += hexMMBTransID.Length;

                string hexEndTime = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 14, messageStr.Length - count));
                count += hexEndTime.Length;

                string hexUserAcount = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 128, messageStr.Length - count));
                count += hexUserAcount.Length;

                string hexCardTypeDesc = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 100, messageStr.Length - count));
                count += hexCardTypeDesc.Length;

                terminalData.ApplicationID = hexAppID.HexToString().Trim();
                terminalData.Currency = hexCurrency.HexToString().Trim();
                terminalData.FeeType = hexCashFeeType.HexToString().Trim();
                terminalData.FeeValue = hexCashFeeValue.HexToString().Trim();
                terminalData.InvoiceNumber = hexOrderNo.HexToString().Trim();
                terminalData.PartnerTransactionID = hexMMBTransID.HexToString().Trim();
                terminalData.ExpiryDate = hexEndTime.HexToString().Trim();
                terminalData.CustomerID = hexUserAcount.HexToString().Trim();
                terminalData.CardIssuerName = hexCardTypeDesc.HexToString().Trim();
                terminalData.CardIssuerID = hexAppID.HexToString().Trim();

                string amount = hexTotalAmt.HexToString().Trim();
                double.TryParse(amount, out double totalAmount);
                terminalData.TotalAmount = (totalAmount / 100).ToString();
            }

            count += etx.Length;

            string lrc = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 1, messageStr.Length - count));
            count += lrc.Length;

            string nextMessage = messageStr.Substring(count, messageStr.Length - count);

            if (!string.IsNullOrEmpty(nextMessage) && terminalData.StatusCode != "00")
            {
                count = 0;
                TerminalDataModel terminalData1 = new TerminalDataModel();
                string STX = nextMessage.Substring(count, Math.Min(ECRCode.CharByte * 1, nextMessage.Length - count));
                if (STX.ToLower() != ECRCode.STX.ToLower())
                {
                    return;
                }
                count += STX.Length;
                string hexResponse = nextMessage.Substring(count, Math.Min(ECRCode.CharByte * 4, nextMessage.Length - count));
                count += hexResponse.Length;
                terminalData1.ResponseText = hexResponse.HexToString();

                if (terminalData1.ResponseText == PBB_CommandCode.SaleResponseCodeQRPay)
                {
                    ReadMessageFromTerminal_ForPBB_CARD_Sales(nextMessage, count, ref terminalData1);
                    if (terminalData1.StatusCode == "00")
                    {
                        terminalData = terminalData1;
                    }
                }
            }
        }

        private void ReadMessageFromTerminal_ForPBB_CARD_Void(string messageStr, int count, ref TerminalDataModel terminalData)
        {
            string hexAmount = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 12, messageStr.Length - count));
            count += hexAmount.Length;

            string hexStatusCode = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 2, messageStr.Length - count));
            count += hexStatusCode.Length;

            string hexApprovalCode = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 6, messageStr.Length - count));
            count += hexApprovalCode.Length;

            string hexRRN = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 12, messageStr.Length - count));
            count += hexRRN.Length;

            string hexTransTrace = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 6, messageStr.Length - count));
            count += hexTransTrace.Length;

            string hexBatchNumber = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 6, messageStr.Length - count));
            count += hexBatchNumber.Length;

            string hexHostNo = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 2, messageStr.Length - count));
            count += hexHostNo.Length;

            string amount = hexAmount.HexToString().Trim();
            double.TryParse(amount, out double totalAmount);
            terminalData.TotalAmount = (totalAmount / 100).ToString();
            terminalData.StatusCode = hexStatusCode.HexToString().Trim();
            terminalData.ApprovalCode = hexApprovalCode.HexToString().Trim();
            terminalData.RetrievalRefNo = hexRRN.HexToString().Trim();
            terminalData.InvoiceNumber = hexTransTrace.HexToString().Trim();
            terminalData.BatchNumber = hexBatchNumber.HexToString().Trim();
            terminalData.HostNo = hexHostNo.HexToString().Trim();


            string etx = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 1, messageStr.Length - count));
            if (etx != ECRCode.ETX)
            {
                string hexPrtnrTxnID = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 32, messageStr.Length - count));
                count += hexPrtnrTxnID.Length;

                string hexApayTxnID = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 64, messageStr.Length - count));
                count += hexApayTxnID.Length;

                string hexCustomerID = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 32, messageStr.Length - count));
                count += hexCustomerID.Length;

                terminalData.PartnerTransactionID = hexPrtnrTxnID.HexToString().Trim();
                terminalData.AlipayTxnID = hexApayTxnID.HexToString().Trim();
                terminalData.CustomerID = hexCustomerID.HexToString().Trim();
            }

            count += etx.Length;

            string lrc = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 1, messageStr.Length - count));
            count += lrc.Length;

            string nextMessage = messageStr.Substring(count, messageStr.Length - count);

            if (!string.IsNullOrEmpty(nextMessage) && terminalData.StatusCode != "00")
            {
                count = 0;
                TerminalDataModel terminalData1 = new TerminalDataModel();
                string STX = nextMessage.Substring(count, Math.Min(ECRCode.CharByte * 1, nextMessage.Length - count));
                if (STX.ToLower() != ECRCode.STX.ToLower())
                {
                    return;
                }
                count += STX.Length;
                string hexResponse = nextMessage.Substring(count, Math.Min(ECRCode.CharByte * 4, nextMessage.Length - count));
                count += hexResponse.Length;
                terminalData1.ResponseText = hexResponse.HexToString();

                if (terminalData1.ResponseText == PBB_CommandCode.VoidResponseCodeCard)
                {
                    ReadMessageFromTerminal_ForPBB_CARD_Void(nextMessage, count, ref terminalData1);
                    if (terminalData1.StatusCode == "00")
                    {
                        terminalData = terminalData1;
                    }
                }
            }
        }

        private void ReadMessageFromTerminal_ForPBB_Settlement(string messageStr, int count, ref TerminalDataModel terminalData)
        {
            string hexHostNo = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 2, messageStr.Length - count));
            count += hexHostNo.Length;

            string hexStatusCode = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 2, messageStr.Length - count));
            count += hexStatusCode.Length;

            string hexBatchNumber = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 6, messageStr.Length - count));
            count += hexBatchNumber.Length;

            string hexBatchCount = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 3, messageStr.Length - count));
            count += hexBatchCount.Length;

            string hexBatchAmount = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 12, messageStr.Length - count));
            count += hexBatchAmount.Length;

            terminalData.StatusCode = hexStatusCode.HexToString().Trim();
            terminalData.BatchNumber = hexBatchNumber.HexToString().Trim();
            terminalData.BatchCount = hexBatchCount.HexToString().Trim();
            terminalData.TotalAmount = hexBatchAmount.HexToString().Trim();
            terminalData.HostNo = hexHostNo.HexToString().Trim();

            string etx = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 1, messageStr.Length - count));
            count += etx.Length;

            string lrc = messageStr.Substring(count, Math.Min(ECRCode.CharByte * 1, messageStr.Length - count));
            count += lrc.Length;

            string nextMessage = messageStr.Substring(count, messageStr.Length - count);

            if (!string.IsNullOrEmpty(nextMessage) && terminalData.StatusCode != "00")
            {
                count = 0;
                TerminalDataModel terminalData1 = new TerminalDataModel();
                string STX = nextMessage.Substring(count, Math.Min(ECRCode.CharByte * 1, nextMessage.Length - count));
                if (STX.ToLower() != ECRCode.STX.ToLower())
                {
                    return;
                }
                count += STX.Length;
                string hexResponse = nextMessage.Substring(count, Math.Min(ECRCode.CharByte * 4, nextMessage.Length - count));
                count += hexResponse.Length;
                terminalData1.ResponseText = hexResponse.HexToString();

                if (terminalData1.ResponseText == PBB_CommandCode.SettlementResponseCode)
                {
                    ReadMessageFromTerminal_ForPBB_CARD_Void(nextMessage, count, ref terminalData1);
                    if (terminalData1.StatusCode == "00")
                    {
                        terminalData = terminalData1;
                    }
                }
            }
        }

        public TerminalDataModel TestReadData(string type, string bankName, string portName, double amount, string invoiceNo, int timeOut, out string message)
        {
            TerminalDataModel dataModel = new TerminalDataModel();
            message = "";
            if (bankName == "CIMBBANK" || bankName == "CIMB")
            {
                if (type == "1" || string.IsNullOrEmpty(type))
                {
                    //string data = "02 03 25 36 30 30 30 30 30 30 30 30 30 31 31 32 30 30 30 30 1C 30 32 00 40 41 50 50 52 4F 56 41 4C 20 20 20 20 20 20 31 37 38 35 37 30 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 1C 30 31 00 06 31 37 38 35 37 30 1C 36 35 00 06 30 30 30 30 39 32 1C 44 30 00 69 4D 45 52 43 48 41 4E 54 20 31 20 20 20 20 20 20 20 20 20 20 20 20 20 4F 54 20 31 2D 33 41 20 47 52 4F 55 4E 44 20 46 4C 4F 4F 52 20 20 20 57 4F 4E 47 20 4B 57 4F 4B 20 43 4F 4D 4D 45 52 43 49 41 4C 20 43 45 1C 31 36 00 08 35 36 34 30 32 33 33 34 1C 44 31 00 15 30 30 30 30 30 31 39 33 30 34 30 30 30 30 37 1C 44 32 00 10 4D 79 44 65 62 69 74 20 20 20 1C 33 30 00 16 35 35 30 39 38 39 2A 2A 2A 2A 2A 2A 31 38 38 35 1C 33 31 00 04 58 58 58 58 1C 35 30 00 06 30 30 30 31 30 32 1C 30 33 00 06 32 32 30 32 31 31 1C 30 34 00 06 31 35 31 32 33 31 1C 44 33 00 12 32 30 34 32 31 32 32 39 30 31 33 39 1C 44 34 00 02 30 39 1C 44 35 00 26 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 1C 03 4B ";
                    //string data = "02032536303030303030303030313132303030301C30320040415050524F56414C20202020202031333233333020202020202020202020202020202020202020201C303100063133323333301C363500063030303133371C443000694D45524348414E542031202020202020202020202020204F5420312D33412047524F554E4420464C4F4F52202020574F4E47204B574F4B20434F4D4D45524349414C2043451C3136000835363430323333341C443100153030303030313933303430303030371C443200104D7944656269742020";

                    string data = "0602029436303030303030303030313132303030301C303100065230383831341C33300016353532313135585858585858353033391C4434000230351C443200104D6173746572202020201C33310004585858581C353000063030303030381C303300063232303930391C303400063130313730321C3136000835363430323339351C443100153030303030313130303130333630351C363500063030303034341C443300123030303031313030303035351C30320040415050524F56414C202020202020523038383134202020202020";

                    var terminalMessage = ReadMessageFromTerminal_ForCIMB(data, out message);
                    MappingFromTerminalMessage(terminalMessage, ref dataModel);
                    dataModel.ExtendObject = terminalMessage;
                }
                else if (type == "5")
                {
                    //dataModel = SendPaymentToTerminal_ForCIMB(portName, CIMB_TransCode.Void, amount, invoiceNo, timeOut, out message);
                    message = "Xin lỗi. Chúng tôi đã cố gắng hết sức nhưng không thành công.";
                }
            }
            else if (bankName == "PUBLICBANK" || bankName == "PBB")
            {
                if (type == "1" || string.IsNullOrEmpty(type))
                {
                    //string data = "02523230303531393630332A2A2A2A2A2A383138302020205858585830305059544553543131313732353232303232333030303030333030303030323035343030343137363136363031323438353834202020202041303030303030363135303030313834303335463837304233444441433420202020202020202020202020202020202020202020202020203038030A02523230303531393630332A2A2A2A2A2A383138302020205858585830305059544553543131313732353232303232333030303030333030303030323035343030343137363136363031323438353834202020202041303030303030363135303030313834303335463837304233444441433420202020202020202020202020202020202020202020202020203038030A02523230303531393630332A2A2A2A2A2A383138302020205858585830305059544553543131313732353232303232333030303030333030303030323035343030343137363136363031323438353834202020202041303030303030363135303030313834303335463837304233444441433420202020202020202020202020202020202020202020202020203038030A04";

                    //string data = "025232303130303030303030303437393030305059544553543135323835313232303332323030303033333030303030343035037F";

                    //  QR PBB
                    //string data = "02473230303030303030303031303030303031303631303030303535313632303137302020202020202020204550413030303131393735342020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020544E4757414C4C45542020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200343";

                    ////  QR MBB
                    //string data = "025132303030305355434345535320202020202020202020202020202020202020202020202020303030303034303030303031303531363634333637333030303032373036303130333236344D423030303030314D59523030303030303030303130304D59523030303030303030303130303039575143393451384432385355303032304D42313131313131323737363038373032393254323032313036323231303437323030202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020204D4242204341534120202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200359";

                    //  QR PBB Alipay
                    string data = "025232303032383934333858585858585858583031383120202020203030202020202020202020202020202020202020303030303238303030303031303631303033313531313838353034353036313320202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202031303030303434353536353820202020202020202020202020202020202020203230323230383036323230303134393738313134323834313039373520202020202020202020202020202020202020202020202020202020202020202020202032303838353232313035373937383133202020202020202020202020202020200369";

                    dataModel = ReadMessageFromTerminal_ForPBB(data, out message);
                }
                else if (type == "5")
                {
                    string data = "02523230313030303030303030303133303030505954455354313131393131323230323233303030303033303030303032303503700252323031303030303030303030313330303050595445535431313139313132323032323330303030303330303030303230350370025232303130303030303030303031333030305059544553543131313931313232303232333030303030333030303030323035037004";

                    dataModel = ReadMessageFromTerminal_ForPBB(data, out message);
                }
            }
            else
            {
                message = "Can't find this bank's terminal.";
            }

            //dataModel = ReadMessageFromTerminal_ForPBB(data, out message);

            //var terminalMessage = ReadMessageFromTerminal_ForCIMB(data, out message);
            //MappingFromTerminalMessage(terminalMessage, ref dataModel);
            //dataModel.ExtendObject = terminalMessage;

            return dataModel;
        }

        public struct CIMB_TransCode
        {
            public const string Purchase = "20";
            public const string Void = "26";
        }

        public struct PBB_CommandCode
        {
            public const string SaleTransCode = "C200";
            public const string SaleResponseCodeCard = "R200";
            public const string SaleResponseCodeWallet = "G200";
            public const string SaleResponseCodeQRPay = "Q200";

            public const string VoidTransCode = "C201";
            public const string VoidResponseCodeCard = "R201";

            public const string SettlementTransCode = "C500";
            public const string SettlementResponseCode = "R500";
        }

        public bool CheckConnectBankTerminal(string bankName, string portName, out string message)
        {
            message = "";
            bool result = false;
            message = "";
            byte[] request = new byte[0];
            if (bankName == "CIMBBANK" || bankName == "CIMB")
            {
                string strData = $"0000{ECRCode.ETX}";
                string lrc = ComputeLRC_CIMB(strData).ToString("X2");
                string sendData = $"{ECRCode.STX}0000{ECRCode.ETX}{lrc}";
                request = sendData.HexStringToByteArray();
            }
            else if (bankName == "PUBLICBANK" || bankName == "PBB" || bankName == "MAYBANK" || bankName == "MBB")
            {
                request = ECRCode.ENQ.HexStringToByteArray();
            }
            else
            {
                message = "Can't find this bank's terminal.";
                return false;
            }

            string hexStringRecevied = string.Empty;
            try
            {
                AutoResetEvent autoResetCheck = new AutoResetEvent(false);

                SerialPort _serialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One)
                {
                    DtrEnable = true,
                    RtsEnable = true,
                    //ReadTimeout = 3000;
                    WriteTimeout = 10000
                };
                _serialPort.Open();
                _serialPort.DataReceived += (sender, e) =>
                {
                    if (sender is SerialPort serialPort)
                    {
                        int bytes = serialPort.BytesToRead;
                        byte[] buffer = new byte[bytes];
                        serialPort.Read(buffer, 0, bytes);

                        hexStringRecevied = buffer.ByteArrayToHexString();
                    }
                    autoResetCheck.Set();
                };

                _serialPort.Write(request, 0, request.Length);
                bool res1 = autoResetCheck.WaitOne(5000);
                if (string.IsNullOrEmpty(hexStringRecevied) || (hexStringRecevied.Trim() == ECRCode.NAK || hexStringRecevied.Trim().StartsWith(ECRCode.NAK)))
                {
                    _serialPort.Write(request, 0, request.Length);
                    bool res2 = autoResetCheck.WaitOne(5000);
                }

                if (!string.IsNullOrEmpty(hexStringRecevied) && (hexStringRecevied.Trim() == ECRCode.ACK || hexStringRecevied.Trim().StartsWith(ECRCode.ACK)))
                {
                    message = "Connect terminal device success." + "; " + hexStringRecevied;
                    result = true;
                }
                else
                {
                    message = "Cannot connect terminal device." + "; " + hexStringRecevied;
                    result = false;
                }

                _serialPort.Close();
                autoResetCheck.Reset();
            }
            catch (Exception ex)
            {
                message = "Exception: " + ex.Message + "; " + hexStringRecevied;
                result = false;
            }

            return result;
        }

        #endregion

        #region Payoo

        public (string, string) AssignNewRSAKey(int keySize, bool isPem = false)
        {
            if (keySize <= 0)
            {
                keySize = 1024;
            }

            RSA rsa = new RSACryptoServiceProvider(keySize);
            string publicKey;

            string privateKey;

            if (!isPem)
            {
                string publicPrivateKeyXML = rsa.ToXmlString(true);
                string publicOnlyKeyXML = rsa.ToXmlString(false);

                privateKey = publicPrivateKeyXML;
                publicKey = publicOnlyKeyXML;
            }
            else
            {
                string publicPrivateKeyPem = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
                string publicOnlyKeyPem = Convert.ToBase64String(rsa.ExportRSAPublicKey());

                privateKey = publicPrivateKeyPem;
                publicKey = publicOnlyKeyPem;
            }

            return (privateKey, publicKey);
        }

        private string CallServicePayoo(object request, string apiName, out bool isValid)
        {
            isValid = false;
            try
            {
                if (ServicePointManager.ServerCertificateValidationCallback == null)
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate
                    {
                        return true;
                    };
                }

                ServicePointManager.Expect100Continue = false;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(this.PAYOO_URL + apiName);
                webRequest.Method = WebRequestMethods.Http.Post;
                webRequest.ContentType = "text/plain";
                webRequest.Timeout = 1000000;
                webRequest.Headers = new WebHeaderCollection();
                webRequest.Headers["MerchantName"] = this.PAYOO_MERCHANT_NAME;
                webRequest.Headers["Credential"] = Utilities.Extensions.HashCode.GenSHA512(this.PAYOO_CREDENTIAL, true); // ký tự in thường
                webRequest.Headers["RequestTime"] = DateTime.Now.ToString("yyyyMMddHHmmss");    //yyyyMMddHHmmss(chuẩn 24h).
                string jsonRequest = request.ToJsonIgnoreNull();
                webRequest.Headers["Signature"] = RSACrypto.GetInstance().Sign(jsonRequest);

                using (StreamWriter writer = new StreamWriter(webRequest.GetRequestStream()))
                {
                    writer.Write(jsonRequest);
                }
                string result = string.Empty;

                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    string responseSignature = webResponse.Headers["Signature"];
                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                        string data = string.Format("{0}|{1}", jsonRequest, result);
                        isValid = RSACrypto.GetInstance().VerifySign(data, responseSignature);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PayooResponseModel CreateOrderPayoo(PayooDataModel requestModel, out string message)
        {
            string res = CallServicePayoo(requestModel, "CreateOrder", out bool isValid);
            if (!isValid)
            {
                PayooResponseModel result = new PayooResponseModel();
                result.ReturnCode = -7;
                message = "Signature validation failed. The results returned are not reliable.";
                return result;
            }

            PayooResponseModel response = res.JsonToModel<PayooResponseModel>();
            if (response.ReturnCode != 0)
            {
                message = $"Order creation failed. Error: {response.ReturnCode}, Desc: {response.Description}";
                return response;
            }

            message = $"Order '{requestModel.OrderCode}' was created successfully.";
            if (response.ResponseData != null)
            {
                response.ResponseData = response.ResponseData.ToString();
            }

            return response;
        }

        public PayooDataModel GetOrderPayoo(string orderCode, out string message)
        {
            dynamic order = new System.Dynamic.ExpandoObject();
            ((IDictionary<String, object>)order).Add("OrderCode", orderCode);

            PayooDataModel payooData = new PayooDataModel();
            string res = CallServicePayoo(order, "GetOrder", out bool isValid);
            if (!isValid)
            {
                message = "Signature validation failed. The results returned are not reliable.";
                return null;
            }

            PayooResponseModel response = res.JsonToModel<PayooResponseModel>();
            if (response.ReturnCode != 0)
            {
                message = $"Retrieving order information failed. Error: {response.ReturnCode}, Desc: {response.Description}";
                return null;
            }

            message = "Successfully retrieved order information.";
            if (response.ResponseData != null)
            {
                payooData = response.ResponseData.ToString().JsonToModel<PayooDataModel>();
            }

            return payooData;
        }

        #endregion
    }
}
