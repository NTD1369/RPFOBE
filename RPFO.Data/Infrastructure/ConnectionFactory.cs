using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RPFO.Utilities.Constants;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Text;

namespace RPFO.Data.Infrastructure
{
    public class ConnectionFactory
    {
        private readonly IConfiguration _config;
        //private readonly
        //ConnectionFactory _connection;
        public ConnectionFactory(IConfiguration config)
        {
            //, ConnectionFactory connection
            _config = config;
            //_connection = connection;
        }
        //public IDbConnection GetConnection
        // {
        //     get
        //     {
        //         DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);

        //         string connectionString = _config.GetConnectionString("DefaultConnection"); //ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //         var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
        //         var conn = factory.CreateConnection();
        //         conn.ConnectionString = connectionString;
        //         conn.Open();
        //         return conn;
        //     }
        // }

        public IDbConnection GetConnection(GConnection connection)
        {
        
            string connectionString="";// = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("DefaultConnection"), AppConstants.TEXT_PHRASE);
            string centralConnectionStr = "";
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
            var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            var conn = factory.CreateConnection();
            if (!string.IsNullOrEmpty(_config.GetConnectionString("CentralConnection")))
            { 
                centralConnectionStr = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("CentralConnection"), AppConstants.TEXT_PHRASE);
            }    
            try
            {
              
               
              
                //string connectionString;// = _config.GetConnectionString("DefaultConnection"); //ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                switch (connection)
                {
                    case GConnection.SapConnection:
                        connectionString = _config.GetConnectionString("SAPConnection");
                        break;

                    case GConnection.HanaConection:
                        if (IntPtr.Size == 8)
                        {
                            // Do 64-bit stuff
                            connectionString = "DRIVER={HDBODBC};";
                        }
                        else
                        {
                            // Do 32-bit
                            connectionString = "DRIVER={HDBODBC32};";
                        }
                        string hanaConstr = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("SAPHanaConnection"), AppConstants.TEXT_PHRASE);
                        connectionString += hanaConstr;
                        break;

                    case GConnection.MwiConnection:
                        connectionString = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("MwiConnection"), AppConstants.TEXT_PHRASE);
                        break;
                    case GConnection.CentralConnection:
                        connectionString = centralConnectionStr;
                        break;
                    case GConnection.ShiftConnection:

                        try
                        {
                            var connect = _config.GetConnectionString("ShiftConnection");
                            if (!string.IsNullOrEmpty(connect))
                            {
                                connectionString = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("ShiftConnection"), AppConstants.TEXT_PHRASE);
                            }
                            else
                            {
                                connectionString = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("DefaultConnection"), AppConstants.TEXT_PHRASE);

                            }

                        }
                        catch (Exception ex)
                        {
                            connectionString = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("DefaultConnection"), AppConstants.TEXT_PHRASE);

                        }

                        break;
                    case GConnection.EndDateConnection:

                        try
                        {
                            var connect = _config.GetConnectionString("EndDateConnection");
                            if (!string.IsNullOrEmpty(connect))
                            {
                                connectionString = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("EndDateConnection"), AppConstants.TEXT_PHRASE);
                            }
                            else
                            {
                                connectionString = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("DefaultConnection"), AppConstants.TEXT_PHRASE);

                            }

                        }
                        catch (Exception ex)
                        {
                            connectionString = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("DefaultConnection"), AppConstants.TEXT_PHRASE);

                        }

                        break;
                    case GConnection.ReportConnection:

                        try
                        {
                            var connect = _config.GetConnectionString("ReportConnection");
                            if (!string.IsNullOrEmpty(connect))
                            {
                                connectionString = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("ReportConnection"), AppConstants.TEXT_PHRASE);
                            }
                            else
                            {
                                connectionString = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("DefaultConnection"), AppConstants.TEXT_PHRASE);

                            }

                        }
                        catch (Exception ex)
                        {
                            connectionString = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("DefaultConnection"), AppConstants.TEXT_PHRASE);

                        }

                        break;
                    default:
                        string defaultConstr = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("DefaultConnection"), AppConstants.TEXT_PHRASE);
                        connectionString = defaultConstr;
                        break;
                }
                conn.ConnectionString = connectionString;
                conn.Open();
               
            }
            catch (Exception ex)
            {
               
                if (!string.IsNullOrEmpty(centralConnectionStr))
                { 
                    conn.ConnectionString = centralConnectionStr;
                    conn.Open();
                }
                else
                {
                    throw ex;
                }
              
            }
            return conn;

        }
        public IDbConnection GetConnectionCustom(string connectionStr)
        {

            string connectionString = connectionStr;// = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("DefaultConnection"), AppConstants.TEXT_PHRASE);
            string centralConnectionStr = "";
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
            var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            var conn = factory.CreateConnection();
           
            try
            { 
                conn.ConnectionString = connectionString;
                conn.Open();

            }
            catch (Exception ex)
            {

                if (!string.IsNullOrEmpty(centralConnectionStr))
                {
                    conn.ConnectionString = centralConnectionStr;
                    conn.Open();
                }
                else
                {
                    throw ex;
                }

            }
            return conn;

        }
    }

    public enum GConnection
    {
        Default = 0,
        SapConnection = 1,
        HanaConection = 2,
        MwiConnection = 3,
        ReportConnection = 4,
        ShiftConnection = 5,
        EndDateConnection = 6,
        CentralConnection = 7,
    }
}
