
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
    public interface IToDoListService
    {
         
        Task<GenericResult> GetAll(string Id, string Code, string Name, string Description, string Content, string Remark, string Status
            , DateTime? FromDate, DateTime? ToDate, string CreatedBy, DateTime? CreatedOn);
        Task<GenericResult> GetById(string Id);
        Task<GenericResult> Create(SToDoList model);
        Task<GenericResult> Update(SToDoList model);
        Task<GenericResult> Delete(SToDoList Code);  
    }
}
