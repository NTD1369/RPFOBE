
using Microsoft.AspNetCore.Mvc;
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IEmployeeTimekeepingService
    {
        Task<GenericResult> GetEmployeeInfor(string access_token, string code);
    }
}
