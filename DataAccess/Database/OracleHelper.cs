using System;
using System.Collections.Generic;
using System.Text;
using Oracle.DataAccess.Client;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace DataAccess
{
    /// <summary>
    /// SqlServer database operations class.
    /// </summary>
    public partial class OracleHelper : HelperBase 
    {
        string connection_str;
        public OracleHelper()
        {
            connection_str = default_connection_str;
            Connection = new OracleConnection(connection_str);
            Command = Connection.CreateCommand();
        }

        public OracleHelper(int ConnectionStringsIndex)
        {
            connection_str = ConfigurationManager.ConnectionStrings[ConnectionStringsIndex].ConnectionString;
            Connection = new OracleConnection(connection_str);
            Command = Connection.CreateCommand();
        }

        public OracleHelper(string ConnectionString)
        {
            connection_str = ConnectionString;
            Connection = new OracleConnection(connection_str);
            Command = Connection.CreateCommand();
        }

        public override void Open()
        {
            base.Open();
        }

        public DbParameter AddParameter(string ParameterName, OracleDbType type, object value)
        {
            return AddParameter(ParameterName, type, value, ParameterDirection.Input);
        }

        public DbParameter AddParameter(string ParameterName, OracleDbType dbType, object value, ParameterDirection direction)
        {
            OracleParameter param = new OracleParameter(ParameterName, dbType);
            param.Value = value;
            param.Direction = direction;
            Command.Parameters.Add(param);
            return param;
        }

        public DbParameter AddParameter(string ParameterName, OracleDbType type, int size, object value)
        {
            return AddParameter(ParameterName, type, size, value, ParameterDirection.Input);
        }

        public DbParameter AddParameter(string ParameterName, OracleDbType dbType, int size, object value, ParameterDirection direction)
        {
            OracleParameter param = new OracleParameter(ParameterName, dbType, size);
            param.Direction = direction;
            param.Value = value;
            Command.Parameters.Add(param);
            return param;
        }

        private void AddRangeParameters(OracleParameter[] parameters)
        {
            Command.Parameters.AddRange(parameters);
        }
    }
}
