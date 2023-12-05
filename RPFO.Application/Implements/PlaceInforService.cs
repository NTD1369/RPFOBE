
using AutoMapper;
using Dapper;
using Newtonsoft.Json;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Utilities.Dtos;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class PlaceInforService : IPlaceInforService
    {
        private readonly IGenericRepository<MPlaceInfor> _placeRepository;
        private readonly IGenericRepository<MTableInfor> _mTableInforRepository;
        private readonly IGenericRepository<TSalesHeader> _saleHeaderRepository;

        private readonly IMapper _mapper;
        public PlaceInforService(
           IGenericRepository<MPlaceInfor> placeRepository,
           IGenericRepository<MTableInfor> mTableInforRepository,
           IGenericRepository<TSalesHeader> saleHeaderRepository,
            IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _placeRepository = placeRepository;
            _mTableInforRepository = mTableInforRepository;
            _saleHeaderRepository = saleHeaderRepository;
            _mapper = mapper;

        }

        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public async Task<MPlaceInfor> GetMPlaceInfor(string companyCode, string storeId, string placeName)
        {
            var queryPlaceInfo = $"select * from M_PlaceInfor where CompanyCode = '{companyCode}' and StoreId='{storeId}' and TableName = '{placeName}'";
            var mPlaceInfor = await _placeRepository.GetAsync(queryPlaceInfo, null, commandType: CommandType.Text);

            return await Task.FromResult(mPlaceInfor);
        }

        public async Task<GenericResult> Create(MPlaceInfor model)
        {
            GenericResult result = new GenericResult();

            if (string.IsNullOrEmpty(model.PlaceName))
            {
                result.Message = $"Place name is required";
                result.Success = false;
                return await Task.FromResult(result);
            }

            if (string.IsNullOrEmpty(model?.Slot.ToString()))
            {
                result.Message = $"Seat is required";
                result.Success = false;
                return await Task.FromResult(result);
            }

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("PlaceName", model.PlaceName.ToUpper());
                parameters.Add("Description", model.Description);
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
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status ?? "A");
                parameters.Add("Type", model.Type);
                parameters.Add("UrlImage", model.UrlImage);
                parameters.Add("AssignMap", model.AssignMap);
                var affectedRows = _placeRepository.Insert("USP_I_M_PlaceInfor", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return await Task.FromResult(result);
        }

        public async Task<GenericResult> Delete(MPlaceInfor model)
        {
            GenericResult result = new GenericResult();

            var t_SalesHeaders = _saleHeaderRepository.Get($"select * from T_SalesHeader where  Status= 'C' and IsCanceled= 'N' and CompanyCode= N'{model.CompanyCode}' and StoreId=N'{model.StoreId}' and CustomF1=N'{model.PlaceId}' and CustomF4=N'NoPaymentOfTable'", null, commandType: CommandType.Text);
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
                var affectedRows = _placeRepository.Execute("USP_D_M_PlaceInfor", parameters, commandType: CommandType.StoredProcedure);

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

        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, string Keyword)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                parameters.Add("PlaceId", "");
                parameters.Add("Keyword", Keyword);
                var data = await _placeRepository.GetAllAsync($"USP_S_M_PlaceInfor", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string PlaceId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                parameters.Add("PlaceId", PlaceId);
                parameters.Add("Keyword", "");
                var data = _placeRepository.Get($"USP_S_M_PlaceInfor", parameters, commandType: CommandType.StoredProcedure);
                if (data != null && !string.IsNullOrEmpty(data.UrlImage))
                {
                    var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    string filename = PlaceId;

                    string fName = PlaceId;
                    string folderName = "images/places/";
                    string folder = Path.Combine(rootPath, folderName);
                    string path = Path.Combine(folder);
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    string urlFileName = path + @"\" + filename;

                    Image image = Base64ToImage(data.UrlImage);
                    image.Save(urlFileName + "." + image.RawFormat.ToString());

                    data.UrlImageSave = folderName + @"\" + filename + "." + image.RawFormat.ToString();

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

        public async Task<GenericResult> Update(MPlaceInfor model)
        {
            GenericResult result = new GenericResult();

            if (string.IsNullOrEmpty(model.PlaceName))
            {
                result.Message = $"Place name is required";
                result.Success = false;
                return await Task.FromResult(result);
            }

            if (string.IsNullOrEmpty(model?.Slot.ToString()))
            {
                result.Message = $"Seat is required";
                result.Success = false;
                return await Task.FromResult(result);
            }

            if (!string.IsNullOrEmpty(model.AssignMap))
            {
                AssignMapModel assignMap = JsonConvert.DeserializeObject<AssignMapModel>(model.AssignMap);
                if (assignMap != null && assignMap.Shapes != null && assignMap.Shapes.Any())
                {
                    var tableExists = assignMap.Shapes.GroupBy(item => item.Text).Where(group => group.Count() > 1).FirstOrDefault();
                    if (tableExists != null)
                    {
                        result.Message = $"Please check for duplicate tables of ({tableExists.Key})";
                        result.Success = false;
                        return await Task.FromResult(result);
                    }

                    var t_SalesHeaders = await _saleHeaderRepository.GetAllAsync($"select * from T_SalesHeader where  Status= 'C' and IsCanceled= 'N' and CompanyCode= N'{model.CompanyCode}' and StoreId=N'{model.StoreId}' and CustomF1=N'{model.PlaceId}' and CustomF4=N'NoPaymentOfTable'", null, commandType: CommandType.Text);
                    if (t_SalesHeaders != null && t_SalesHeaders.Any())
                    {
                        var tableNames = assignMap.Shapes.Where(xx => xx.Text.Length > 0).Select(x => x.Text.Split("-")[0]);
                        var mTableInfors = await _mTableInforRepository.GetAllAsync($"select * from M_TableInfor where  CompanyCode= N'{model.CompanyCode}' and StoreId=N'{model.StoreId}' and Status=N'A'", null, commandType: CommandType.Text);
                        if (mTableInfors != null && mTableInfors.Any())
                        {
                            var tableIdByTableName = mTableInfors.Where(t => tableNames.Contains(t.TableName)).Select(x => x.TableId.ToString());
                            var orderInTables = t_SalesHeaders.Where(x => !tableIdByTableName.Contains(x.ContractNo));
                            if (orderInTables.Any())
                            {
                                result.Message = $"Please check for tables that have orders";
                                result.Success = false;
                                return await Task.FromResult(result);
                            }
                        }
                    }
                }
                else
                {
                    var t_SalesHeaders = await _saleHeaderRepository.GetAllAsync($"select * from T_SalesHeader where  Status= 'C' and IsCanceled= 'N' and CompanyCode= N'{model.CompanyCode}' and StoreId=N'{model.StoreId}' and CustomF1=N'{model.PlaceId}' and CustomF4=N'NoPaymentOfTable'", null, commandType: CommandType.Text);
                    if (t_SalesHeaders != null && t_SalesHeaders.Any())
                    {
                        result.Message = $"Please check for tables that have orders";
                        result.Success = false;
                        return await Task.FromResult(result);
                    }
                }
            }

            try
            {
                //var delRs =  Delete(model).Result;
                //if(delRs.Success)
                //{
                //    result = Create(model).Result;
                //}   
                //else
                //{
                //    result.Success = false;
                //    result.Message = " Update Failed. B/c Transanction can't not completed";
                //}
                //result = Create(model);

                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("PlaceId", model.PlaceId);
                parameters.Add("PlaceName", model.PlaceName);
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
                parameters.Add("IsDefault", model.IsDefault);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status ?? "A");
                parameters.Add("Type", model.Type);
                //Base64Decode(
                parameters.Add("UrlImage", model.UrlImage);
                parameters.Add("AssignMap", model.AssignMap);
                var affectedRows = _placeRepository.Insert("USP_U_M_PlaceInfor", parameters, commandType: CommandType.StoredProcedure);
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

        public Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

        private Image SaveByteArrayAsImage(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);

            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }
            //string exten =;

            //if (!File.Exists(fullOutputPath+ "." + image.RawFormat.ToString()))
            //{
            //    image.Save(fullOutputPath, image.RawFormat);
            //}    


            return image;
        }
    }
}
