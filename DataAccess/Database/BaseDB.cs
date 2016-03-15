using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;
using AutoMapper;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Collections;
using System.Diagnostics;

namespace DataAccess
{
    public abstract class BaseDB : HelperBase
    {
        protected abstract string connectionString { get; }
        protected OracleTransaction oracleTransaction = null;
        private Hashtable paramCache = new Hashtable();

        public int BatchInsert(string cmdText, Dictionary<string, object> commandParameters)
        {
            int arrayBindCount = -1;
            foreach (var item in commandParameters)
            {
                if (!(item.Value is Array))
                    throw new ArgumentException();
                else
                {
                    Array value = (Array)item.Value;
                    if (arrayBindCount == -1)
                        arrayBindCount = value.Length;
                    else if (arrayBindCount != value.Length)
                        throw new ArgumentException("数组参数长度不一致");
                }
            }
            Dictionary<string, object> tmpParam = new Dictionary<string, object>();
            foreach (var item in commandParameters)
            {
                Array ary = (Array)item.Value;
                Object obj = ary.GetValue(0);
                if (obj == null)
                {
                    if (ary.GetType() == typeof(decimal?[]))
                    {
                        obj = 0m;
                    }
                    else if (ary.GetType() == typeof(DateTime?[]))
                    {
                        obj = DateTime.MinValue;
                    }
                }
                tmpParam.Add(item.Key, obj);
            }

            OracleParameter[] sqlParams = Dp2Op(tmpParam);
            foreach (var item in sqlParams)
                item.Value = commandParameters[item.ParameterName];
            if (oracleTransaction == null)
            {
                return OracleHelper.ExecuteNonQuery(connectionString, CommandType.Text, cmdText, arrayBindCount, sqlParams);
            }
            else
            {
                return OracleHelper.ExecuteNonQuery(oracleTransaction, CommandType.Text, cmdText, arrayBindCount, sqlParams);
            }

        }

        public int ExecuteNonQuery(string cmdText, Dictionary<string, object> commandParameters)
        {
            OracleParameter[] sqlParams = Dp2Op(commandParameters);
            return OracleHelper.ExecuteNonQuery(connectionString, CommandType.Text, cmdText, sqlParams);
        }


        public int ExecuteNonQueryProc(string storedProcedureName, Dictionary<string, object> commandParameters)
        {
            return ExecuteNonQueryProc(this.GetOwner(), storedProcedureName, commandParameters);
        }

        /// <summary>
        /// Executes the non query proc.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public int ExecuteNonQueryProc(string owner, string storedProcedureName, Dictionary<string, object> commandParameters)
        {
            Dictionary<string, string> tmpIpMappingOp;
            //OracleParameter[] sqlParams = Dp2Op(owner, storedProcedureName, commandParameters, out tmpIpMappingOp);
            OracleParameter[] sqlParams = OracleHelperParameterCache.GetSpParameterSet(connectionString, storedProcedureName);
            AssignParameterValues(sqlParams, commandParameters, out tmpIpMappingOp);
            if (sqlParams != null)
            {
                int result = OracleHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, owner + "." + storedProcedureName, sqlParams);
                SetOutParamValue(commandParameters, tmpIpMappingOp, sqlParams);
                return result;
            }
            else
                return -1;
        }


        public int ExecuteNonQueryProcTran(string storedProcedureName, Dictionary<string, object> commandParameters)
        {
            return ExecuteNonQueryProcTran(this.GetOwner(), storedProcedureName, commandParameters);
        }

        public int ExecuteNonQueryProcTran(string owner, string storedProcedureName, Dictionary<string, object> commandParameters)
        {
            Dictionary<string, string> tmpIpMappingOp;
            //OracleParameter[] sqlParams = Dp2Op(owner, storedProcedureName, commandParameters, out tmpIpMappingOp);           
            OracleParameter[] sqlParams = OracleHelperParameterCache.GetSpParameterSet(connectionString, storedProcedureName);
            AssignParameterValues(sqlParams, commandParameters, out tmpIpMappingOp);
            //事务OUT值获取
            if (sqlParams != null)
            {
                int result = OracleHelper.ExecuteNonQuery(oracleTransaction, CommandType.StoredProcedure, owner + "." + storedProcedureName, sqlParams); ;
                SetOutParamValue(commandParameters, tmpIpMappingOp, sqlParams);
                return result;
            }
            else
                return -1;
            //return OracleHelper.ExecuteNonQuery(oracleTransaction, CommandType.StoredProcedure, owner + "." + storedProcedureName, sqlParams);;
        }

        public int ExecuteNonQueryTran(string cmdText, Dictionary<string, object> commandParameters)
        {
            OracleParameter[] sqlParams = Dp2Op(commandParameters);
            return OracleHelper.ExecuteNonQuery(oracleTransaction, CommandType.Text, cmdText, sqlParams);
        }

        public object ExecuteScalar(string cmdText, Dictionary<string, object> commandParameters)
        {
            OracleParameter[] sqlParams = Dp2Op(commandParameters);
            return OracleHelper.ExecuteScalar(connectionString, CommandType.Text, cmdText, sqlParams);
        }

        public object ExecuteScalarProc(string storedProcedureName, Dictionary<string, object> commandParameters)
        {
            return ExecuteScalarProc(this.GetOwner(), storedProcedureName, commandParameters);
        }

        public object ExecuteScalarProc(string owner, string storedProcedureName, Dictionary<string, object> commandParameters)
        {
            Dictionary<string, string> tmpIpMappingOp;
            //OracleParameter[] sqlParams = Dp2Op(owner, storedProcedureName, commandParameters, out tmpIpMappingOp);
            OracleParameter[] sqlParams = OracleHelperParameterCache.GetSpParameterSet(connectionString, storedProcedureName);
            AssignParameterValues(sqlParams, commandParameters, out tmpIpMappingOp);
            if (sqlParams != null)
            {
                object result = OracleHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, storedProcedureName, sqlParams);
                SetOutParamValue(commandParameters, tmpIpMappingOp, sqlParams);
                return result;
            }
            else
                return null;
        }

        public List<T> ExecuteList<T>(string cmdText, Dictionary<string, object> commandParameters)
        {
            OracleParameter[] sqlParams = Dp2Op(commandParameters);
            OracleDataReader odr = OracleHelper.ExecuteReader(connectionString, CommandType.Text, cmdText, sqlParams);
            var result = Mapper.DynamicMap<IDataReader, List<T>>(odr);
            odr.Close();
            return result;
        }

        public DataTable ExecuteDataTable(string cmdText, Dictionary<string, object> commandParameters)
        {
            OracleParameter[] sqlParams = Dp2Op(commandParameters);
            return OracleHelper.ReadTable(connectionString, CommandType.Text, cmdText, sqlParams);
        }

        public List<T> ExecuteListProc<T>(string storedProcedureName, params object[] parameterValues)
        {
            OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connectionString, storedProcedureName);
            AssignParameterValues(commandParameters, parameterValues);
            OracleDataReader odr = OracleHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            var result = Mapper.DynamicMap<IDataReader, List<T>>(odr);
            odr.Close();
            return result;
        }

        public DataTable ExecuteDataTableProc(string storedProcedureName, params object[] parameterValues)
        {
            OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connectionString, storedProcedureName);
            AssignParameterValues(commandParameters, parameterValues);
            DataTable dt = OracleHelper.ReadTable(connectionString, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            return dt;
        }

        public DataTable ExecuteDataTableProc(string storedProcedureName, Dictionary<string, object> commandParameters)
        {
            Dictionary<string, string> tmpIpMappingOp;
            //OracleParameter[] sqlParams = Dp2Op(this.GetOwner(), storedProcedureName, commandParameters, out tmpIpMappingOp);
            OracleParameter[] sqlParams = OracleHelperParameterCache.GetSpParameterSet(connectionString, storedProcedureName);
            AssignParameterValues(sqlParams, commandParameters, out tmpIpMappingOp);
            DataTable dt = OracleHelper.ReadTable(connectionString, CommandType.StoredProcedure, storedProcedureName, sqlParams);
            SetOutParamValue(commandParameters, tmpIpMappingOp, sqlParams);
            return dt;
        }
        public DataSet ReadDataSetProc(string storedProcedureName, Dictionary<string, object> commandParameters)
        {
            Dictionary<string, string> tmpIpMappingOp;
            OracleParameter[] sqlParams = OracleHelperParameterCache.GetSpParameterSet(connectionString, storedProcedureName);
            AssignParameterValues(sqlParams, commandParameters, out tmpIpMappingOp);
            DataSet ds = OracleHelper.ReadDataSet(connectionString, CommandType.StoredProcedure, storedProcedureName, sqlParams);
            SetOutParamValue(commandParameters, tmpIpMappingOp, sqlParams);
            return ds;
        }

        public List<T> ExecuteListProc<T>(string storedProcedureName, Dictionary<string, object> commandParameters)
        {
            return ExecuteListProc<T>(this.GetOwner(), storedProcedureName, commandParameters);
        }

        public List<T> ExecuteListProc<T>(string owner, string storedProcedureName, Dictionary<string, object> commandParameters)
        {
            Dictionary<string, string> tmpIpMappingOp;
            //OracleParameter[] sqlParams = Dp2Op(owner, storedProcedureName, commandParameters, out tmpIpMappingOp);
            OracleParameter[] sqlParams = OracleHelperParameterCache.GetSpParameterSet(connectionString, storedProcedureName);
            AssignParameterValues(sqlParams, commandParameters, out tmpIpMappingOp);
            if (sqlParams != null)
            {
                OracleDataReader odr = OracleHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, storedProcedureName, sqlParams);
                SetOutParamValue(commandParameters, tmpIpMappingOp, sqlParams);
                var result = Mapper.DynamicMap<IDataReader, List<T>>(odr);
                odr.Close();

                return result;
            }
            else
                return null;
        }

        public OracleConnection GetConnection()
        {
            return new OracleConnection(connectionString);
        }

        public void BeginTransation()
        {
            if (Connection == null)
            {
                Connection = GetConnection();
            }

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            oracleTransaction = (OracleTransaction)Connection.BeginTransaction();
            //return oracleTransaction;
        }

        public void Commit()
        {
            if (oracleTransaction != null)
                oracleTransaction.Commit();
        }

        public void Rollback()
        {
            if (oracleTransaction != null)
                oracleTransaction.Rollback();
        }

        public override void Close()
        {
            oracleTransaction = null;
            base.Close();
        }

        private OracleParameter[] Dp2Op(string owner, string storedProcedureName, Dictionary<string, object> commandParameters, out Dictionary<string, string> tmpIpMappingOp)
        {
            tmpIpMappingOp = new Dictionary<string, string>();
            if (commandParameters == null)
                return null;
            string package = string.Empty;
            string procName = string.Empty;
            string[] tmpArr = storedProcedureName.Split('.');
            if (tmpArr.Length == 1)
                procName = tmpArr[0];
            else if (tmpArr.Length == 2)
            {
                package = tmpArr[0];
                procName = tmpArr[1];
            }
            if (!string.IsNullOrEmpty(procName))
            {
                string cacheKey = string.Format("{0}.{1}", owner, storedProcedureName);
                object cacheObj = paramCache[cacheKey];
                List<ProcedureInfo> procedureInfoList;
                if (cacheObj != null)
                {
                    procedureInfoList = (List<ProcedureInfo>)cacheObj;
                }
                else
                {
                    procedureInfoList = new List<ProcedureInfo>();
                    string sql = @"SELECT t.object_name, t.argument_name, t.data_type, t.in_out, t.data_length, t.data_precision, t.data_scale FROM dba_arguments t WHERE t.owner=:owner AND t.package_name = :i_package_name AND t.object_name = :i_object_name ORDER BY t.position";
                    OracleParameter[] sqlParams = new OracleParameter[]{
                        new OracleParameter(":owner", OracleDbType.Varchar2, 50, owner.ToUpper(), ParameterDirection.Input),
                        new OracleParameter(":i_package_name", OracleDbType.Varchar2, 50, package.ToUpper(), ParameterDirection.Input),
                        new OracleParameter(":i_object_name", OracleDbType.Varchar2, 50, procName.ToUpper(), ParameterDirection.Input)
                    };
                    OracleDataReader odr = OracleHelper.ExecuteReader(connectionString, CommandType.Text, sql, sqlParams);
                    while (odr.Read())
                    {
                        int dataLength = 0;//dataPrecision = 0, dataScale = 0;
                        int.TryParse(odr["DATA_LENGTH"].ToString(), out dataLength);
                        ProcedureInfo pi = new ProcedureInfo();
                        pi.ArgumentName = odr["ARGUMENT_NAME"].ToString();
                        pi.DataType = odr["DATA_TYPE"].ToString().Replace(" ", "");
                        pi.DataLength = dataLength;
                        pi.InOut = odr["IN_OUT"].ToString();
                        procedureInfoList.Add(pi);
                    }
                    odr.Close();
                    paramCache.Add(cacheKey, procedureInfoList);
                }
                List<OracleParameter> buildSqlParams = new List<OracleParameter>();
                foreach (var item in commandParameters)
                {
                    tmpIpMappingOp.Add(item.Key.ToUpper(), item.Key);
                }
                foreach (ProcedureInfo pi in procedureInfoList)
                {
                    string argument_name = pi.ArgumentName;
                    object paramValue = null;
                    if (tmpIpMappingOp.ContainsKey(argument_name))
                    {
                        paramValue = commandParameters[tmpIpMappingOp[argument_name]];
                    }
                    string data_type = pi.DataType;
                    OracleType otTataType = (OracleType)Enum.Parse(typeof(OracleType), data_type);
                    int enumIndex = (int)otTataType;
                    string in_out = pi.InOut;
                    int dataLength = pi.DataLength;
                    //int.TryParse(odr["DATA_PRECISION"].ToString(), out dataPrecision);
                    //int.TryParse(odr["DATA_SCALE"].ToString(), out dataScale);
                    if (in_out.Equals("IN"))
                    {
                        switch (otTataType)
                        {
                            case OracleType.VARCHAR2:
                            case OracleType.NVARCHAR2:
                            case OracleType.CHAR:
                            case OracleType.NCHAR:
                                buildSqlParams.Add(new OracleParameter(argument_name, (OracleDbType)enumIndex, dataLength, paramValue, ParameterDirection.Input));
                                break;
                            default:
                                buildSqlParams.Add(new OracleParameter(argument_name, (OracleDbType)enumIndex, dataLength, paramValue, ParameterDirection.Input));
                                break;
                        }
                    }
                    else if (in_out.Equals("OUT"))
                    {
                        switch (otTataType)
                        {
                            case OracleType.VARCHAR2:
                            case OracleType.NVARCHAR2:
                            case OracleType.CHAR:
                            case OracleType.NCHAR:
                                buildSqlParams.Add(new OracleParameter(argument_name, (OracleDbType)enumIndex, 4000, string.Empty, ParameterDirection.Output));
                                break;
                            default:
                                buildSqlParams.Add(new OracleParameter(argument_name, (OracleDbType)enumIndex, ParameterDirection.Output));
                                break;
                        }
                    }
                }
                return buildSqlParams.ToArray();
            }
            else
                return null;
        }

        private static OracleParameter[] Dp2Op(Dictionary<string, object> commandParameters)
        {
            if (commandParameters == null)
                return null;
            OracleParameter[] parameters = new OracleParameter[commandParameters.Count];
            int i = 0;
            foreach (var item in commandParameters)
            {
                parameters[i] = new OracleParameter(item.Key, item.Value);
                i++;
            }
            return parameters;
        }


        /// <summary>
        /// This method assigns an array of values to an array of OracleParameters.
        /// </summary>
        /// <param name="commandParameters">array of OracleParameters to be assigned values</param>
        /// <param name="parameterValues">array of objects holding the values to be assigned</param>
        private static void AssignParameterValues(OracleParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                //do nothing if we get no data
                return;
            }

            // we must have the same number of values as we pave parameters to put them in
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }

            //iterate through the OracleParameters, assigning the values from the corresponding position in the 
            //value array
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                commandParameters[i].Value = parameterValues[i];
            }
        }

        private static void AssignParameterValues(OracleParameter[] commandParameters, Dictionary<string, object> parameterValues, out Dictionary<string, string> tmpIpMappingOp)
        {
            tmpIpMappingOp = new Dictionary<string, string>();
            if ((commandParameters == null) || (parameterValues == null))
            {
                //do nothing if we get no data
                return;
            }
            foreach (var item in parameterValues)
            {
                tmpIpMappingOp.Add(item.Key.ToUpper(), item.Key);
            }
            //iterate through the OracleParameters, assigning the values from the corresponding position in the 
            //value array
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                var p = commandParameters[i];
                if (tmpIpMappingOp.ContainsKey(p.ParameterName))
                {
                    p.Value = parameterValues[tmpIpMappingOp[p.ParameterName]];
                }
            }
        }

        private static void SetOutParamValue(Dictionary<string, object> commandParameters, Dictionary<string, string> tmpIpMappingOp, OracleParameter[] sqlParams)
        {
            foreach (var item in sqlParams)
            {
                if (item.Direction == ParameterDirection.Output)
                {
                    Type type = Type.GetType(GetTypeFullName(item.DbType));
                    object o = Convert.ChangeType(item.Value.ToString(), type);
                    if (tmpIpMappingOp.ContainsKey(item.ParameterName) && commandParameters.ContainsKey(tmpIpMappingOp[item.ParameterName]))
                        commandParameters[tmpIpMappingOp[item.ParameterName]] = o;
                    else
                        commandParameters.Add(item.ParameterName, o);
                }
            }
        }

        private static string GetTypeFullName(DbType dbType)
        {
            string result = string.Empty;
            switch (dbType)
            {
                case DbType.Boolean:
                    result = "System.Boolean";
                    break;
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                case DbType.Time:
                    result = "System.DateTime";
                    break;
                case DbType.SByte:
                case DbType.Xml:
                    result = "System.Object";
                    break;
                case DbType.VarNumeric:
                    result = "System.Decimal";
                    break;
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    result = "System.String";
                    break;
                default:
                    result = string.Format("System.{0}", Enum.GetName(typeof(DbType), dbType));
                    break;
            }
            return result;
        }

        public static Dictionary<string, object> DataRowDictionary(DataRow row)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (DataColumn column in row.Table.Columns)
            {
                if (row[column] != null && row[column].ToString() != "")
                {
                    dict.Add("I_" + column.ColumnName, row[column]);
                }
            }
            return dict;
        }

        public static Dictionary<string, object> EntityToDictionary(object ety)
        {
            Type type = ety.GetType();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (System.Reflection.PropertyInfo item in type.GetProperties())
            {
                dict.Add("I_" + item.Name, item.GetValue(ety, null));
            }
            return dict;
        }

        private string GetOwner()
        {
            string pattern = @"(?<=USER ID=)\w+(?=;)";
            if (Regex.IsMatch(this.connectionString, pattern))
            {
                return Regex.Match(this.connectionString, pattern).Value;
            }
            else
            {
                throw new ArgumentNullException("找不到USER ID");
            }
        }

        public enum OracleType
        {
            BFILE = 101,
            BLOB = 102,
            BYTE = 103,
            CHAR = 104,
            CLOB = 105,
            DATE = 106,
            NUMBER = 107,//DECIMAL
            DOUBLE = 108,
            LONG = 109,
            LONGRAW = 110,
            INT16 = 111,
            INT32 = 112,
            INT64 = 113,
            INTERVALDS = 114,
            INTERVALYM = 115,
            NCLOB = 116,
            NCHAR = 117,
            NVARCHAR2 = 119,
            RAW = 120,
            REFCURSOR = 121,
            SINGLE = 122,
            TIMESTAMP = 123,
            TIMESTAMPLTZ = 124,
            TIMESTAMPTZ = 125,
            VARCHAR2 = 126,
            XMLTYPE = 127,
            ARRAY = 128,
            OBJECT = 129,
            REF = 130,
            BINARYDOUBLE = 132,
            BINARYFLOAT = 133,
        }

        public struct ProcedureInfo
        {
            public string ArgumentName;
            public string DataType;
            public string InOut;
            public int DataLength;
            public string DataPrecision;
            public string DataScale;
        }
    }
}
