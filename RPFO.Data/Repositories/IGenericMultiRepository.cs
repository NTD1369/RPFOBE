using Dapper; 
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Data.Repositories
{
    public interface IGenericMultiRepository<T, TEnum> where T : class where TEnum : struct, IComparable, IFormattable, IConvertible
    {
        IDbConnection GetConnection();
        T Get(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        Task<T> GetAsync(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        Task<List<T>> GetAllAsync(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        List<T> GetAll(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        int Execute(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        T Insert(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        T Update(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);


    }
}
