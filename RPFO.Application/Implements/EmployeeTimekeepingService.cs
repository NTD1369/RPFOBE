
using AutoMapper;
using Dapper;
using Newtonsoft.Json;
using RPFO.API.Helpers;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class EmployeeTimekeepingService : IEmployeeTimekeepingService
    {
        private readonly IGenericRepository<TEmployeeTimekeeping> _denoRepository;

        private readonly IMapper _mapper;

        private readonly ClientHelper _con = new ClientHelper();

        public EmployeeTimekeepingService(IGenericRepository<TEmployeeTimekeeping> denoRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _denoRepository = denoRepository;
            _mapper = mapper;

        }

        public async Task<GenericResult> GetEmployeeInfor(string access_token, string code)
        {
            GenericResult result = new GenericResult();
            try
            {
                _con.conect();
                var linkapi = _con._client.GetStringAsync("Timekeeping/GetEmployeeInfor?access_token=" + access_token + "&code=" + code);
                PersonnelProfileViewModel profile = new PersonnelProfileViewModel();
                profile = JsonConvert.DeserializeObject<PersonnelProfileViewModel>(linkapi.Result);
                if (profile.Data != null)
                {
                    var checkInOutEmployee = CheckInOutEmployee(code);
                    if (checkInOutEmployee == null)
                    {
                        var insertEmployeeTimekeeping = InsertEmployeeTimekeeping(profile.Data.Data);
                    }
                    result.Data = profile.Data.Data;
                    result.Success = true;
                    result.Message = "Successfully";
                    return result;
                }

                result.Data = null;
                result.Success = false;
                result.Message = "Error";
            
                return result;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        private TEmployeeTimekeeping CheckInOutEmployee(string code)
        {
            var sql = $"SELECT * FROM T_EmployeeTimekeeping WHERE Code = '{code}'";

            TEmployeeTimekeeping res = null;
            using (IDbConnection db = _denoRepository.GetConnection())
            {
                res = db.Query<TEmployeeTimekeeping>(sql).FirstOrDefault();
            }

            return res;
        }

        private TEmployeeTimekeeping InsertEmployeeTimekeeping(PersonnelProfileDetail model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Id", model.Id);
            parameters.Add("Code", model.Code);
            parameters.Add("Name", model.Name);
            parameters.Add("Position", model.Position_id);
            parameters.Add("Address", model.Place_current);
            parameters.Add("Birthday", model.Birthday);
            parameters.Add("Mobile", model.Mobile);
            parameters.Add("Email", model.Email);

            var affectedRows = _denoRepository.Insert("USP_I_T_EmployeeTimekeeping", parameters, commandType: CommandType.StoredProcedure);

            return affectedRows;
        }

    }

}
