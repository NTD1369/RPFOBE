
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IPermissionService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<PagedList<MPermission>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string CompanyCode, string Code);
        //Task<GenericResult> GetByUser(string User); 
        Task<GenericResult> Create(MPermission model);
        Task<GenericResult> Update(MPermission model);
        Task<GenericResult> Delete(string Code); 
        Task<GenericResult> GetPermissionByFunction(string Function); 
        GenericResult CopyFromRole(string CompanyCode, string FrRole, string ToRole, string By);
        Task<GenericResult> GetPermissionByRole(string Role);
        Task<GenericResult> GetControlPermissionListByFunction(string Function, string Role);
        Task<GenericResult> GetHeaderPermission();
        Task<GenericResult> GetHeaderFunctionPermission();

        Task<GenericResult> UpdateFunctionFermission(MPermission model);

        Task<GenericResult> GetFunctionPermissionByUser(string UserName);
        Task<GenericResult> UpdateListFunctionFermission(List<MPermission> list);
        Task<GenericResult> CheckFunctionByUserName(string CompanyCode, string User, string Function, string ControlId, string Permission);
        Task<GenericResult>  CheckApproveFunctionByUser(string CompanyCode, string User, string Password, string CustomCode, string Function, string ControlId, string Permission);
    }
}
