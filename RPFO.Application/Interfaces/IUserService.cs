using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IUserService
    {
        Task<GenericResult> GetAllUsers(string CompanyCode, bool? getLicense);
        Task<GenericResult> GetByUsername(string CompanyCode, string UserName);
        Task<GenericResult> GetById(string CompanyCode, string Id);
        Task<GenericResult> Create(MUser model);
        Task<GenericResult> Update(MUser model);
        Task<GenericResult> Delete(string Id);
        Task<GenericResult> Login(string userName, string pass, string customCode);
        Task<GenericResult> GenQRCode(string CompanyCode, string UserName, string Password);
        Task<GenericResult> Import(DataImport model); 
        Task<GenericResult> LoginMwi(string userName, string pass);
        GenericResult SetLicenseForUser(string CompanyCode, string License, List<string> UserList);
        GenericResult RemoveLicenseForUser(string CompanyCode, string License, List<string> UserList);
        Task<GenericResult> UpdateLastStore(MUser model);
        Task<GenericResult> Get_TokenLicense(string CompanyCode, string License, string User);
    }
}
