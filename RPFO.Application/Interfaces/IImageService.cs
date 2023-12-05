
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
    public interface IImageService
    {
      
        Task<List<MImage>> GetImage(string companyCode, string type, string Code, string Phone); 
        Task<GenericResult> Create(MImage model);
        Task<GenericResult> Update(MImage model);
        //Task<GenericResult> Import(DataImport model);
    }
}
