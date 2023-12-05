using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace RPFO.Data.Helpers
{
    public class TableValueParameter : SqlMapper.ICustomQueryParameter
    {
        private DataTable _datatable;
        private string _typeName;
        public TableValueParameter(DataTable dataTable)
        {
            _datatable = dataTable;
        }
        public void AddParameter(IDbCommand command, string name)
        {
            var parameter = (SqlParameter)command.CreateParameter();
            parameter.ParameterName = name;
            parameter.SqlDbType = SqlDbType.Structured;
            parameter.Value = _datatable;
            parameter.TypeName = _datatable.TableName;
            command.Parameters.Add(parameter);
        }
    }
}
