using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IPlacePrintService
    {
        Task<GenericResult> GetAll(string CompanyCode, string StoreId);
        Task<GenericResult> ViewItemByItemGroup(string CompanyCode, string StoreId,string ItemGroup,string Status);
        Task<GenericResult> GetListItemGroup(string CompanyCode, string StoreId);
        Task<GenericResult> Create(TPlacePrint model);
        Task<GenericResult> Update(TPlacePrint model);
        Task<GenericResult> Delete(int PrintId);       
    }
}
