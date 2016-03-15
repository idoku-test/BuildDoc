using System;
using System.Data.OleDb;
using System.Reflection;
using System.IO;
using System.Data;

namespace DataAccess
{
    /// <summary>
    /// Microsoft Access database operations class.
    /// </summary>
    public partial class AccessHelper:HelperBase
    {
        string _accessFPath = get_defualt_dbpath();

        public virtual string AccessFPath
        {
            get
            {
                return _accessFPath;
            }
            set
            {
                _accessFPath = value;
            }
        }

        public AccessHelper()
        {
            this.Connection = new OleDbConnection();
            Command = Connection.CreateCommand();
        }

        public override void Open()
        {
            this.Connection.ConnectionString = "Provider=Microsoft.Jet.Oledb.4.0;data source=" + AccessFPath;
            base.Open();
        }

        public AccessHelper(string accessfpath)
        {
            this.AccessFPath = accessfpath;
            Open();
        }

        public OleDbParameter AddParameter(string ParameterName, OleDbType type, object value)
        {
            return AddParameter(ParameterName, type, value,ParameterDirection.Input);
        }

        public OleDbParameter AddParameter(string ParameterName, OleDbType type, object value, ParameterDirection direction)
        {
            OleDbParameter param = new OleDbParameter(ParameterName, type);
            param.Value = value;
            param.Direction = direction;
            Command.Parameters.Add(param);
            return param;
        }

        public OleDbParameter AddParameter(string ParameterName, OleDbType type, int size, object value)
        {
            return AddParameter(ParameterName, type, size, value, ParameterDirection.Input);
        }

        public OleDbParameter AddParameter(string ParameterName, OleDbType type, int size, object value, ParameterDirection direction)
        {
            OleDbParameter param = new OleDbParameter(ParameterName, type, size);
            param.Direction = direction;
            param.Value = value;
            Command.Parameters.Add(param);
            return param;
        }

        public void AddRangeParameters(OleDbParameter[] parameters)
        {
            Command.Parameters.AddRange(parameters);
        }

    
    }
}
