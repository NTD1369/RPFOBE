using Dapper;
using RPFO.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Data.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        IDbConnection GetConnection(GConnection gConnection = default); 
        IDbConnection GetConnectionCustom(string customStr); 
        string GetScalar(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure, GConnection gConnection = default); 
        T Get(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure, GConnection gConnection = default);
        Task<T> GetAsync(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure, GConnection gConnection = default);
        Task<List<T>> GetAllAsync(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure, GConnection gConnection = default);
        List<T> GetAll(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure, GConnection gConnection = default);
        int Execute(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure, GConnection gConnection = default);
        T Insert(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure, GConnection gConnection = default);
        T Update(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure, GConnection gConnection = default);


    }
}
