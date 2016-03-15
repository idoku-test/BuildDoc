using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace DataAccess
{
    /// <summary>
    /// SqlServer database operations class.
    /// </summary>
    public partial class SqlServerHelper:HelperBase 
    {
        string connection_str;
        public SqlServerHelper()
        {
            connection_str = default_connection_str;
            Connection = new SqlConnection(connection_str);
            Command = Connection.CreateCommand();
        }

        public SqlServerHelper(int ConnectionStringsIndex)
        {
            connection_str = ConfigurationManager.ConnectionStrings[ConnectionStringsIndex].ConnectionString;
            Connection = new SqlConnection(connection_str);
            Command = Connection.CreateCommand();
        }

        public SqlServerHelper(string ConnectionString)
        {
            connection_str = ConnectionString;
            Connection = new SqlConnection(connection_str);
            Command = Connection.CreateCommand();
        }

        public override void Open()
        {
            base.Open();
        }

        public SqlParameter AddParameter(string ParameterName, SqlDbType type, object value)
        {
            return AddParameter(ParameterName, type, value, ParameterDirection.Input);
        }

        public SqlParameter AddParameter(string ParameterName, SqlDbType type, object value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter(ParameterName, type);
            param.Value = value;
            param.Direction = direction;
            Command.Parameters.Add(param);
            return param;
        }

        public SqlParameter AddParameter(string ParameterName, SqlDbType type, int size, object value)
        {
            return AddParameter(ParameterName, type, size, value, ParameterDirection.Input);
        }

        public SqlParameter AddParameter(string ParameterName, SqlDbType type, int size, object value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter(ParameterName, type, size);
            param.Direction = direction;
            param.Value = value;
            Command.Parameters.Add(param);
            return param;
        }

        public void AddRangeParameters(SqlParameter[] parameters)
        {
            Command.Parameters.AddRange(parameters);
        }
    }
}
