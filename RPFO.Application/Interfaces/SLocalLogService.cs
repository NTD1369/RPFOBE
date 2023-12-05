
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
    public interface ILocalLogService
    {
        Task<GenericResult> GetAll( string DbType, string User, string CompanyCode, string StoreId, string Type, DateTime? FromDate, DateTime? ToDate );
        Task<GenericResult> GetPagedList(string DbType, UserParams userParams);   
        Task<GenericResult> Create(SLog model, string DbType);
        Task<GenericResult> Update(SLog model, string DbType);
        //Task<GenericResult> Import(DataImport model);
    }
}
