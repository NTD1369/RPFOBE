//using AutoMapper.Internal;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text; 
using RPFO.Data.Infrastructure;
using System.Threading.Tasks;
using RPFO.Utilities.Dtos;

namespace RPFO.Data.Repositories
{
    public class GenericRepository<T, TEnum> : IDisposable, IGenericMultiRepository<T, TEnum> where T : class where TEnum : struct, IComparable, IFormattable, IConvertible
    {
        //private readonly IConfiguration _config; IConfiguration config
        AppConnections _connection;
        TEnum _connect;
        public GenericRepository(AppConnections connection, TEnum connect)
        {
            //, ConnectionFactory connection
            //_config = config;
            _connection = connection;
            _connect = connect;
        }
        public IDbConnection GetConnection()
        {
            //var xxx =  _connect.

            //IConfigurationRoot configuration = new ConfigurationBuilder()
            //       .SetBasePath(Directory.GetCurrentDirectory())
            //       .AddJsonFile("appsettings.json")
            //       .Build();
            //var connectionString = _config.GetConnectionString("DefaultConnection");
            //optionsBuilder.UseSqlServer(connectionString);
            //return new SqlConnection(connectionString);
            return _connection.GetConnection;
        }
        public void Dispose()
        {
            GC.Collect();
        }

        public int Execute(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using (IDbConnection db = GetConnection())
            {
                return db.Execute(sp, parms, commandType: commandType);
            }
        }

        public T Get(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using (IDbConnection db = GetConnection())
            {
                return db.Query<T>(sp, parms, commandType: commandType).FirstOrDefault();
            }
        }
        public async Task<T> GetAsync(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using (IDbConnection db = GetConnection())
            {
                var models = await db.QueryAsync<T>(sp, parms, commandType: commandType);
                var model = models.FirstOrDefault();
                return model;
            }
        }

        public List<T> GetAll(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using (IDbConnection db = _connection.GetConnection)
            {
                return db.Query<T>(sp, parms, commandType: commandType).ToList();
            }
        }
        public async Task<List<T>> GetAllAsync(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using (IDbConnection db = GetConnection())
            {
                var models = await db.QueryAsync<T>(sp, parms, commandType: commandType);
                return models.ToList();
            }
        }

        //public class SingleQuery
        //{
        //    public string query { get; set; }
        //    public DynamicParameters parms { get; set; }

        //    public CommandType commandType { get; set; }
        //}
        //public class MultiQuery
        //{
        //    public List<SingleQuery> queries { get; set; } = new List<SingleQuery>();
        //}
        //public GenericResult InsertMulti(MultiQuery multiQuery)
        //{
        //    T result;
        //    using (IDbConnection db = GetConnection())
        //    {
        //        try
        //        {
        //            if (db.State == ConnectionState.Closed)
        //                db.Open();
        //            using (var tran = db.BeginTransaction())
        //            {
        //                foreach(SingleQuery singleQuery in multiQuery.queries)
        //                {
        //                    try
        //                    {
        //                        result = db.Query<T>(singleQuery.query, singleQuery.parms, commandType: singleQuery.commandType, transaction: tran).FirstOrDefault();
        //                        tran.Commit();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        tran.Rollback();
        //                        throw ex;
        //                    }
        //                }    
                      
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            if (db.State == ConnectionState.Open)
        //                db.Close();
        //        }
        //        return result;
        //    }
        //}
        public T Insert(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            T result;
            using (IDbConnection db = GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    using (var tran = db.BeginTransaction())
                    {
                        try
                        {
                            result = db.Query<T>(sp, parms, commandType: commandType, transaction: tran).FirstOrDefault();
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        }

        public T Update(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            T result;
            using (IDbConnection db = GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    using (var tran = db.BeginTransaction())
                    {
                        try
                        {
                            result = db.Query<T>(sp, parms, commandType: commandType, transaction: tran).FirstOrDefault();
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        }

        public string GetScalar(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using (IDbConnection db = GetConnection())
            {
                var value = db.ExecuteScalar<string>(sp, parms, commandType: commandType);

                return value;
            }
        }
    }
}
