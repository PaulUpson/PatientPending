using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;

namespace PatientPending.Core {
    /* Most of this is borrowed from Rob Conery's Massive but excludes 
     * all the 1 to 1 entity/object conventions since that is not required*/
    public abstract class SqlActuator {
        private readonly DbProviderFactory _factory;
        private readonly string _connectionString;
        protected SqlActuator(string connectionStringName) {
            if (connectionStringName == "")
                connectionStringName = ConfigurationManager.ConnectionStrings[0].Name;
            var providerName = "System.Data.SqlClient";
            if (ConfigurationManager.ConnectionStrings[connectionStringName] != null) {
                if (!string.IsNullOrEmpty(ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName))
                    providerName = ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;
            } else {
                throw new InvalidOperationException("Can't find a connection string with the name '" + connectionStringName + "'");
            }
            _factory = DbProviderFactories.GetFactory(providerName);
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        protected IEnumerable<dynamic> Fetch(string sql, params object[] args) {
            return Query(sql, args).ToList<dynamic>();
        }

        protected IEnumerable<dynamic> Query(string sql, params object[] args) {
            using (var conn = OpenConnection()) {
                var rdr = CreateCommand(sql, conn, args).ExecuteReader(CommandBehavior.CloseConnection);
                while (rdr.Read()) {
                    var e = new ExpandoObject();
                    var d = e as IDictionary<string, object>;
                    for (var i = 0; i < rdr.FieldCount; i++)
                        d.Add(rdr.GetName(i), rdr[i]);
                    yield return e;
                }
            }
        }

        protected object Scalar(string sql, params object[] args) {
            object result = null;
            using (var conn = OpenConnection()) {
                result = CreateCommand(sql, conn, args).ExecuteScalar();
            }
            return result;
        }

        protected int Execute(IEnumerable<DbCommand> commands) {
            var result = 0;
            using (var conn = OpenConnection()) {
                using (var tx = conn.BeginTransaction()) {
                    foreach (var cmd in commands) {
                        cmd.Connection = conn;
                        cmd.Transaction = tx;
                        result += cmd.ExecuteNonQuery();
                    }
                    tx.Commit();
                }
            }
            return result;
        }

        protected DbCommand CreateCommand(string sql, DbConnection conn, params object[] args) {
            DbCommand result = null;
            result = _factory.CreateCommand();
            result.Connection = conn;
            result.CommandText = sql;
            if (args.Length > 0)
                result.AddParams(args);
            return result;
        }

        private DbConnection OpenConnection() {
            var conn = _factory.CreateConnection();
            conn.ConnectionString = _connectionString;
            conn.Open();
            return conn;
        }
    }

    public static class ObjectExtensions {
        public static void AddParams(this DbCommand cmd, object[] args) {
            foreach (var item in args) {
                AddParam(cmd, item);
            }
        }
        private static void AddParam(this DbCommand cmd, object item) {
            var p = cmd.CreateParameter();
            p.ParameterName = string.Format("@{0}", cmd.Parameters.Count);
            if (item == null) {
                p.Value = DBNull.Value;
            } else {
                if (item.GetType() == typeof(Guid)) {
                    p.Value = item.ToString();
                    p.DbType = DbType.String;
                    p.Size = 4000;
                } else if (item.GetType() == typeof(ExpandoObject)) {
                    var d = (IDictionary<string, object>)item;
                    p.Value = d.Values.FirstOrDefault();
                } else {
                    p.Value = item;
                }
                if (item.GetType() == typeof(string))
                    p.Size = 4000;
            }
            cmd.Parameters.Add(p);
        }
    }
}