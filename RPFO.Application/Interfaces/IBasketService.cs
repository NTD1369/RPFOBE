
using RPFO.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IBasketService
    {
        Task<BasketViewModel> GetBasketAsync(string basketId);
        Task<BasketViewModel> UpdateBasketAsync(BasketViewModel basket);
        Task<bool> DeleteBasketAsync(string basketId);
       
    }
}
